using GraphShield.Proxy.Events;
using GraphShield.Proxy.Exceptions;
using GraphShield.Proxy.Extensions;
using GraphShield.Proxy.Plumbings.Cache;
using GraphShield.Proxy.Plumbings.Data.Interfaces;
using GraphShield.Proxy.Plumbings.Pipeline;
using MassTransit;
using Microsoft.Extensions.Logging;
using Titanium.Web.Proxy.EventArguments;

namespace GraphShield.Proxy.Pipelines.ClientValidator
{
    /// <summary>
    /// Pipeline responsible for validating the client before processing the request.
    /// </summary>
    internal class ClientValidatorPipeline : DefaultPipeline, IClientValidatorPipeline
    {
        private readonly IBus _bus;
        private readonly IInternalCache _cache;
        private readonly IClientDataService _clientService;
        private readonly ILogger<ClientValidatorPipeline> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientValidatorPipeline"/> class.
        /// </summary>
        /// <param name="bus">The message bus.</param>
        /// <param name="cache">The internal cache.</param>
        /// <param name="clientService">The client data service.</param>
        /// <param name="logger">The logger.</param>
        public ClientValidatorPipeline(IBus bus, IInternalCache cache, IClientDataService clientService, ILogger<ClientValidatorPipeline> logger)
        {
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _clientService = clientService ?? throw new ArgumentNullException(nameof(clientService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public override Task InitializeAsync()
        {
            _logger.LogInformation("Initialize pipeline: {fullName}", GetType().FullName);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        [Pipeline(PipelineCategory.Graph, 4)]
        public override async Task BeforeRequestAsync(SessionEventArgs session)
        {
            // Get the remote identifier in the current session.
            var remoteId = session.GetData<string>("RemoteId");
            if (string.IsNullOrEmpty(remoteId))
                throw new ArgumentException("The remote identifier is null or empty in the session.");

            // Get client or initialize client registration.
            var entity = await _clientService.GetAsync(remoteId);
            if (entity == null)
                await UserRegisterAsync(remoteId);

            if (!entity.Status)
                throw new ForbiddenException("The client is deactivated.");

            await UpdateActivityAsync(entity.Id);

            // Insert data to allow execution of the next pipeline.
            if (entity.CredentialId != null)
                session.InsertData("CredentialId", entity.CredentialId);
            session.InsertData("ClientId", entity.Id);
        }

        /// <summary>
        /// Initiates the user registration process asynchronously.
        /// </summary>
        /// <param name="remoteId">The remote identifier of the client.</param>
        private async Task UserRegisterAsync(string remoteId)
        {
            var cacheKey = $"Register:{remoteId}";
            if (_cache.TryGetValue(cacheKey, out bool _))
                throw new ForbiddenException("The client is already in the registration process.");

            _cache.Set(cacheKey, true, DateTimeOffset.Now.AddMinutes(1));
            await _bus.Publish(new ClientRegistration { RemoteId = remoteId });
            throw new ForbiddenException("The client has entered the registration process.");
        }

        /// <summary>
        /// Updates the activity of the client asynchronously.
        /// </summary>
        /// <param name="clientId">The identifier of the client.</param>
        private async Task UpdateActivityAsync(Guid clientId)
        {
            // To avoid a massive database update.
            var cacheKey = $"Activity:{clientId}";
            if (_cache.TryGetValue(cacheKey, out bool _))
                return;

            _cache.Set(cacheKey, true, DateTimeOffset.Now.AddMinutes(1));
            await _bus.Publish(new ClientActivity { ClientId = clientId });
        }
    }
}