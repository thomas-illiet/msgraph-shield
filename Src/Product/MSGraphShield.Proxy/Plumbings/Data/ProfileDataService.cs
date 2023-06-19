using AutoMapper;
using AutoMapper.QueryableExtensions;
using MSGraphShield.Data.Model.Entities;
using MSGraphShield.Data.Shared.DbContexts;
using MSGraphShield.Proxy.Plumbings.Cache;
using MSGraphShield.Proxy.Plumbings.Data.Interfaces;
using MSGraphShield.Proxy.Plumbings.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Titanium.Web.Proxy.Http;

namespace MSGraphShield.Proxy.Plumbings.Data
{
    internal class ProfileDataService : DefaultDataService<ProfileEntity, ProfileData>, IProfileDataService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IInternalCache _cache;
        private readonly IMapper _mapper;

        public ProfileDataService(
            IServiceScopeFactory scopeFactory, IInternalCache cache,
            IMapper mapper, ILogger<ProfileDataService> logger)
            : base(scopeFactory, cache, mapper, logger)
        {
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<ProfileData>> ListAsync(Guid clientId, CancellationToken cancellationToken = default)
        {
            if (_cache.TryGetValue(GetCacheKey(clientId), out List<ProfileData>? profiles))
                return profiles!;

            await using var scope = _scopeFactory.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataConfigDbContext>();

            profiles = await dbContext.Set<ClientProfileEntity>()
                .AsNoTracking()
                .Include(x => x.Profile)
                .ThenInclude(x => x.Rules)
                .Where(x => x.ClientId == clientId && x.Profile.IsActive == true)
                .Select(x => x.Profile)
                .ProjectTo<ProfileData>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            _cache.Set(GetCacheKey(clientId), profiles, TimeSpan.FromHours(1));

            return profiles;
        }


    }
}