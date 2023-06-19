using Sieve.Models;
using Sieve.Services;

namespace MSGraphShield.Api.Service.Plumbings.Sieve
{
    /// <summary>
    /// Provides extension methods to register Sieve services.
    /// </summary>
    internal static class ServiceExtensions
    {
        /// <summary>
        /// Registers Sieve services.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to register the services in.</param>
        /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddSieveService(this IServiceCollection services)
        {
            services.AddScoped<AppSieveProcessor>();
            services.AddScoped<ISieveProcessor, AppSieveProcessor>();

            services.Configure<SieveOptions>(x =>
            {
                x.CaseSensitive = false;
                x.ThrowExceptions = true;
                x.MaxPageSize = 1000;
                x.DefaultPageSize = 100;
                x.DisableNullableTypeExpressionForSorting = true;
            });

            return services;
        }
    }
}