using AutoMapper;
using AutoMapper.QueryableExtensions;
using GraphShield.Data.Model.Abstracts;
using GraphShield.Data.Shared.DbContexts;
using GraphShield.Proxy.Plumbings.Cache;
using GraphShield.Proxy.Plumbings.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GraphShield.Proxy.Plumbings.Data
{
    internal class DefaultDataService<TEntity, TEntityDto> : DefaultDataService<TEntity>, IDefaultDataService<TEntity, TEntityDto>
        where TEntity : class, IEntity
        where TEntityDto : class, IEntity
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IInternalCache _cache;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDataService{TEntity, TEntityDto}"/> class.
        /// </summary>
        /// <param name="scopeFactory">The scope factory.</param>
        /// <param name="cache">The memory cache.</param>
        /// <param name="mapper">The entity mapper.</param>
        /// <param name="logger">The logger.</param>
        public DefaultDataService(
            IServiceScopeFactory scopeFactory, IInternalCache cache,
            IMapper mapper, ILogger logger) : base(cache, logger)
        {
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TEntityDto?> GetAsync(Guid entityId, CancellationToken cancellationToken = default)
        {
            if (!_cache.TryGetValue(GetCacheKey(entityId), out TEntityDto? entity))
                entity = await SyncAsync(entityId, cancellationToken);
            return entity;
        }

        public async Task<TEntityDto?> SyncAsync(Guid entityId, CancellationToken cancellationToken = default)
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataConfigDbContext>();

            var entity = await dbContext
                .Set<TEntity>()
                .AsNoTracking()
                .Where(x => x.Id == entityId)
                .ProjectTo<TEntityDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);

            if (entity == null)
                return entity;

            // Some entities require customization.
            await CustomizeEntityAsync(entity, cancellationToken);

            _cache.Set(GetCacheKey(entityId), entity, TimeSpan.FromHours(1));
            _logger.LogDebug("Following entity has been added to the cache: {entityId}", entityId);
            return entity;
        }

        public virtual Task CustomizeEntityAsync(TEntityDto entity, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }


    internal class DefaultDataService<TEntity> : IDefaultDataService<TEntity>
        where TEntity : class, IEntity
    {
        private readonly IInternalCache _cache;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDataService{TEntity}"/> class.
        /// </summary>
        /// <param name="cache">The memory cache.</param>
        /// <param name="logger">The logger.</param>
        public DefaultDataService(IInternalCache cache, ILogger logger)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task RemoveAsync(Guid entityId)
        {
            _cache.Remove(GetCacheKey(entityId));
            _logger.LogDebug("Following entity has been removed from the cache: {entityId}", entityId);
            return Task.CompletedTask;
        }

        protected static int GetCacheKey(string entityId)
            => (typeof(TEntity).Name + entityId).GetHashCode();

        protected static int GetCacheKey(Guid entityId)
            => (typeof(TEntity).Name + entityId).GetHashCode();
    }
}