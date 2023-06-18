using GraphShield.Data.Model.Entities;
using GraphShield.Data.Shared.DbContexts;
using GraphShield.Proxy.Plumbings.Cache;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace GraphShield.Proxy.Events
{
    public record ClientActivity()
    {
        public Guid ClientId { get; set; }
    }

    internal class ClientActivityDefinition : IConsumer<ClientActivity>
    {
        private readonly IServiceScopeFactory _factory;
        private readonly IInternalCache _cache;

        public ClientActivityDefinition(IServiceScopeFactory factory, IInternalCache cache)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task Consume(ConsumeContext<ClientActivity> context)
        {
            if (_cache.TryGetValue(context.Message.ClientId, out bool _))
                return;

            await using var scope = _factory.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataConfigDbContext>();

            var record = new ClientEntity { Id = context.Message.ClientId, LastSeenUtc = DateTimeOffset.Now };
            dbContext.Attach(record);
            dbContext.Entry(record).Property(x => x.LastSeenUtc).IsModified = true;
            await dbContext.SaveChangesAsync(context.CancellationToken);

            _cache.Set(context.Message.ClientId, true, DateTimeOffset.Now.AddMinutes(1));
        }
    }
}