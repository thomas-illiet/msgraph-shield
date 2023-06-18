using Microsoft.Extensions.Logging;
using System.Reflection;
using GraphShield.Proxy.Extensions;
using Titanium.Web.Proxy.EventArguments;
using static GraphShield.Proxy.Plumbings.Pipeline.PipelineManagement;

namespace GraphShield.Proxy.Plumbings.Pipeline
{
    /// <summary>
    /// Manages the handling and execution of pipelines.
    /// </summary>
    internal class PipelineManagement 
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PipelineManagement> _logger;

        private readonly List<IPipeline> _instances = new();
        private readonly Dictionary<string, List<PipelineSteps>> _pipelineSteps = new();
        public record PipelineSteps(uint Position, PipelineCategory Category, Func<SessionEventArgs, Task> MethodFunc);

        /// <summary>
        /// Initializes a new instance of the PipelineManagement class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="logger">The logger.</param>
        public PipelineManagement(IServiceProvider serviceProvider, ILogger<PipelineManagement> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// Runs a set of specified pipelines.
        /// </summary>
        /// <param name="session">The <see cref="SessionEventArgs"/> instance containing the event data.</param>
        /// <param name="setName">Name of the specified pipelines.</param>
        public async Task ExecuteAsync(SessionEventArgs session, string setName)
        {
            await session.HandleExceptionAsync(async () =>
            {
                foreach (var pipelineStep in _pipelineSteps[setName])
                {
                    var category = session.GetData<PipelineCategory>(nameof(PipelineCategory));
                    if (pipelineStep.Category == PipelineCategory.Everything || pipelineStep.Category == category)
                    {
                        //_logger.LogDebug("Handling - {pipeline} - {type}", setName,
                        //    pipelineStep.MethodFunc.Method.DeclaringType);
                        await pipelineStep.MethodFunc.Invoke(session);
                    }
                }
            }, _logger);
        }


        /// <summary>
        /// Loads the pipelines.
        /// </summary>
        public async Task LoadPipeLines()
        {
            // Initialize pipeline variables
            _pipelineSteps.Add(nameof(IPipeline.AfterResponseAsync), new List<PipelineSteps>());
            _pipelineSteps.Add(nameof(IPipeline.BeforeRequestAsync), new List<PipelineSteps>());

            // Create all pipeline instances.
            foreach (var pipeType in GetPipelineInformations())
            {
                var instance = _serviceProvider.GetService(pipeType) as IPipeline;
                await instance!.InitializeAsync();
                _instances.Add(instance);
            }

            // Register all pipeline steps
            foreach (var instance in _instances)
            {
                AddPipelineStep(instance, instance.BeforeRequestAsync);
                AddPipelineStep(instance, instance.AfterResponseAsync);
            }

            _pipelineSteps[nameof(IPipeline.AfterResponseAsync)] =
                _pipelineSteps[nameof(IPipeline.AfterResponseAsync)].OrderBy(x => x.Position).ToList();
            _pipelineSteps[nameof(IPipeline.BeforeRequestAsync)] =
                _pipelineSteps[nameof(IPipeline.BeforeRequestAsync)].OrderBy(x => x.Position).ToList();

            foreach (var pipelineStep in _pipelineSteps)
            {
                foreach (var step in pipelineStep.Value)
                {
                    _logger.LogDebug($"{pipelineStep.Key} - {step.Category} - {step.Position} - {step.MethodFunc.Method.DeclaringType}");
                }
            }
        }

        private void AddPipelineStep(IPipeline instance, Func<SessionEventArgs, Task> methodFunc)
        {
            var method = instance.GetType().GetMethods()
                .First(methodInfo => methodInfo.Name == methodFunc.Method.Name);
            var attribute = method.GetCustomAttributes<PipelineAttribute>(false).FirstOrDefault();
            if (attribute != null)
            {
                _pipelineSteps[methodFunc.Method.Name].Add(new PipelineSteps(attribute.Position, attribute.Category, methodFunc));
            }
        }

        private IEnumerable<TypeInfo> GetPipelineInformations()
        {
            foreach (var typeInfo in GetType().Assembly.DefinedTypes)
            {
                if (typeInfo.ImplementedInterfaces.Contains(typeof(IPipeline)) && typeInfo.IsInterface)
                {
                    yield return typeInfo;
                }
            }
        }
    }
}