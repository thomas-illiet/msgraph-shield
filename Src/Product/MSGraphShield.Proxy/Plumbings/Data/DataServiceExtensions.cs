using MSGraphShield.Proxy.Plumbings.Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MSGraphShield.Proxy.Plumbings.Data
{
    internal static class DataServiceExtensions
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services)
        {
            services.AddMemoryCache();

            services.TryAddSingleton<ICredentialDataService, CredentialDataService>();
            services.TryAddSingleton<IProfileDataService, ProfileDataService>();
            services.TryAddSingleton<IClientDataService, ClientDataService>();

            return services;
        }
    }
}