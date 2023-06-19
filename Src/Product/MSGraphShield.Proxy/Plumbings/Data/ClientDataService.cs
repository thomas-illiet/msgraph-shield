using AutoMapper;
using AutoMapper.QueryableExtensions;
using MSGraphShield.Data.Model.Abstracts;
using MSGraphShield.Data.Model.Entities;
using MSGraphShield.Data.Shared.DbContexts;
using MSGraphShield.Proxy.Plumbings.Cache;
using MSGraphShield.Proxy.Plumbings.Data.Interfaces;
using MSGraphShield.Proxy.Plumbings.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MSGraphShield.Proxy.Plumbings.Data
{
    internal class ClientDataService : DefaultDataService<ClientEntity, ClientData>, IClientDataService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IInternalCache _cache;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ClientDataService(
            IServiceScopeFactory scopeFactory, IInternalCache cache,
            IMapper mapper, ILogger<ClientDataService> logger)
            : base(scopeFactory, cache, mapper, logger)
        {
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ClientData?> GetAsync(string remoteId, CancellationToken cancellationToken = default)
        {
            if (!_cache.TryGetValue(GetCacheKey(remoteId), out ClientData? entity))
                entity = await SyncAsync(remoteId, cancellationToken);
            return entity;
        }

        public async Task<ClientData?> SyncAsync(string remoteId, CancellationToken cancellationToken = default)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataConfigDbContext>();

            var entity = await dbContext
                .Set<ClientEntity>()
                .AsNoTracking()
                .Where(x => x.RemoteId == remoteId)
                .ProjectTo<ClientData>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);

            if (entity == null)
                return entity;

            _cache.Set(GetCacheKey(remoteId), entity, TimeSpan.FromHours(1));
            _logger.LogDebug("Following entity has been added to the cache: {entityId}", remoteId);
            return entity;
        }

    }
}