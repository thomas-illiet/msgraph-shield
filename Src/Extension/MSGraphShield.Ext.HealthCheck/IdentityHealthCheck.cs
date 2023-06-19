using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MSGraphShield.Ext.HealthCheck
{
    /// <summary>
    /// Provides extension methods for adding health checks related to the Graph Identity Server.
    /// </summary>
    public static class IdentityHealthCheck
    {
        /// <summary>
        /// Adds the graph identity server checks.
        /// </summary>
        /// <param name="builder">The health checks builder.</param>
        /// <param name="configuration">The configuration.</param>
        public static IHealthChecksBuilder AddIdentityCheck(this IHealthChecksBuilder builder, IConfiguration configuration)
        {
            var authority = configuration.GetValue<string>("AuthenticationConfiguration:Authority");
            if (string.IsNullOrEmpty(authority))
                throw new ArgumentException("Could not find a valid authority in the configuration.");

            builder.AddIdentityServer(
                idSvrUri: new Uri(authority),
                name: "Authentication",
                failureStatus: HealthStatus.Unhealthy);

            return builder;
        }
    }
}