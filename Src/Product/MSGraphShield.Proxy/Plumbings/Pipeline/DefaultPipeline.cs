using System.Threading.Tasks;
using Titanium.Web.Proxy.EventArguments;

namespace MSGraphShield.Proxy.Plumbings.Pipeline
{
    /// <summary>
    /// Default implementation of the pipeline.
    /// </summary>
    public class DefaultPipeline : IPipeline
    {
        /// <summary>
        /// Initializes the pipeline.
        /// </summary>
        public virtual Task InitializeAsync() => Task.CompletedTask;

        /// <summary>
        /// Executes tasks before sending a request.
        /// </summary>
        /// <param name="session">The session information.</param>
        public virtual Task BeforeRequestAsync(SessionEventArgs session) => Task.CompletedTask;

        /// <summary>
        /// Executes tasks after receiving a response.
        /// </summary>
        /// <param name="session">The session information.</param>
        public virtual Task AfterResponseAsync(SessionEventArgs session) => Task.CompletedTask;
    }
}