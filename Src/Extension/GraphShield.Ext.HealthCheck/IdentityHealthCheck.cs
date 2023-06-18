using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
namespace GraphShield.Ext.HealthCheck
{


    public static class IdentityHealthCheck
    {
        /// <summary>
        /// Adds the graph identity server checks.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static IHealthChecksBuilder AddIdentityCheck(this IHealthChecksBuilder builder, IConfiguration configuration)
        {
            // Create server uri
            var tenantId = configuration.GetValue<Guid>("ClientAuthentication:TenantId");
            if (tenantId == Guid.Empty)
                throw new ArgumentException("Could not find a valid tenant identifier in the configuration.");

            // Add default endpoint
            builder.AddIdentityServer(
                idSvrUri: new Uri($"https://login.microsoftonline.com/{tenantId}/v2.0"),
                name: "GraphIdentity",
                failureStatus: HealthStatus.Unhealthy);

            return builder;
        }
    }
}