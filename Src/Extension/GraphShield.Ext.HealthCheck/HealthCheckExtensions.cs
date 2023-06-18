using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace GraphShield.Ext.HealthCheck
{


    public static class HealthCheckExtensions
    {
        /// <summary>
        /// Adds the health checks service.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        public static void AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddGraphApi()
                .AddDatabaseChecks(services, configuration)
                .AddIdentityCheck(configuration);
        }

        /// <summary>
        /// Enable health check routing.
        /// </summary>
        /// <param name="endpoints">The endpoints.</param>
        /// <returns>Updated endpoint route builder</returns>
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