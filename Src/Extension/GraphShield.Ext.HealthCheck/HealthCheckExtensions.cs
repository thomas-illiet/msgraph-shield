using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GraphShield.Ext.HealthCheck
{
    /// <summary>
    /// Provides extension methods for adding and configuring health checks.
    /// </summary>
    public static class HealthCheckExtensions
    {
        /// <summary>
        /// Adds the health checks service.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The configuration.</param>
        public static void AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddGraphApi()
                .AddDatabaseChecks(services, configuration)
                .AddIdentityCheck(configuration);
        }

        /// <summary>
        /// Enables health check routing.
        /// </summary>
        /// <param name="endpoints">The endpoint route builder.</param>
        /// <returns>The updated endpoint route builder.</returns>
        public static IEndpointRouteBuilder UseHealthChecks(this IEndpointRouteBuilder endpoints)
        {
            // Adds a health checks endpoint.
            // https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks
            endpoints.MapHealthChecks("/healthz", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
            });

            return endpoints;
        }
    }
}