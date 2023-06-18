using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GraphShield.Ext.HealthCheck
{
    /// <summary>
    /// Provides extension methods for adding health checks for the Graph API.
    /// </summary>
    public static class GraphApiHealthCheck
    {
        /// <summary>
        /// Adds the Graph API health checks.
        /// </summary>
        /// <param name="builder">The health checks builder.</param>
        /// <returns>The health checks builder.</returns>
        public static IHealthChecksBuilder AddGraphApi(this IHealthChecksBuilder builder)
        {
            // Add legacy endpoint
            builder.AddUrlGroup(
                uri: new System.Uri(uriString: "https://graph.microsoft.com/v1.0/"),
                name: "GraphApi",
                failureStatus: HealthStatus.Unhealthy);

            // Add beta endpoint
            builder.AddUrlGroup(
                uri: new System.Uri(uriString: "https://graph.microsoft.com/beta/"),
                name: "GraphApiBeta",
                failureStatus: HealthStatus.Unhealthy);

            return builder;
        }
    }
}