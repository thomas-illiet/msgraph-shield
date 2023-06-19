using AutoMapper;
using MSGraphShield.Data.Model.Entities;
using MSGraphShield.Data.Shared.Constants;
using MSGraphShield.Proxy.Plumbings.Cache;
using MSGraphShield.Proxy.Plumbings.Data.Interfaces;
using MSGraphShield.Proxy.Plumbings.Data.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MSGraphShield.Proxy.Plumbings.Data
{
    internal class CredentialDataService : DefaultDataService<CredentialEntity, CredentialData>, ICredentialDataService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<CredentialDataService> _logger;

        public CredentialDataService(
            IServiceScopeFactory scopeFactory, IInternalCache cache,
            IMapper mapper, ILogger<CredentialDataService> logger)
            : base(scopeFactory, cache, mapper, logger)
        {
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task CustomizeEntityAsync(CredentialData entity, CancellationToken cancellationToken = default)
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var dbProtector = scope.ServiceProvider.GetRequiredService<IDataProtectionProvider>();
            var protector = dbProtector.CreateProtector(DataProtectionConsts.DefaultPurpose);

            try
            {
                entity.Secret = protector.Unprotect(entity.Secret);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to unprotects a piece of protected data for the following credential: {credentialId}", entity.Id);
            }
        }
    }
}