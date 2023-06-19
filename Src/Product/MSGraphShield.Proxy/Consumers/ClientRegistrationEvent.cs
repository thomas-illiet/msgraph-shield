using MSGraphShield.Data.Model.Entities;
using MSGraphShield.Data.Shared.DbContexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MSGraphShield.Proxy.Events
{
    public record ClientRegistration()
    {
        public string RemoteId { get; set; }
    }

    internal class ClientRegistrationDefinition : IConsumer<ClientRegistration>
    {
        private readonly IServiceScopeFactory _factory;
        private readonly ILogger _logger;

        public ClientRegistrationDefinition(IServiceScopeFactory factory, ILogger<ClientRegistrationDefinition> logger)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<ClientRegistration> context)
        {
            await using var scope = _factory.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataConfigDbContext>();

            var validation = await dbContext.Set<ClientEntity>()
                .AsNoTracking()
                .Where(x => x.RemoteId == context.Message.RemoteId)
                .Select(x => x.Id)
                .FirstOrDefaultAsync(context.CancellationToken);
            if (validation != default)
            {
                _logger.LogWarning("Following client already exists: {clientId}", context.Message.RemoteId);
                return;
            }

            var record = new ClientEntity() { RemoteId = context.Message.RemoteId };
            await dbContext.AddAsync(record, context.CancellationToken);
            await dbContext.SaveChangesAsync(context.CancellationToken);
            _logger.LogInformation("Following client has been created: {clientId}", context.Message.RemoteId);
        }
    }
}