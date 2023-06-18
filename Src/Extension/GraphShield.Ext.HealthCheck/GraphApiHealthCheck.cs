using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
namespace GraphShield.Ext.HealthCheck
{


    public static class GraphApiHealthCheck
    {
        /// <summary>
        /// Adds the graph api health checks.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public static IHealthChecksBuilder AddGraphApi(this IHealthChecksBuilder builder)
        {
            // Add legacy endpoint
            builder.AddUrlGroup(
                uri: new Uri(uriString: "https://graph.microsoft.com/v1.0/"),
                name: "GraphApi",
                failureStatus: HealthStatus.Unhealthy);

            // Add beta endpoint
            builder.AddUrlGroup(
                new Uri(uriString: "https://graph.microsoft.com/beta/"),
                "GraphApiBeta",
                HealthStatus.Unhealthy);

            return builder;
        }
    }
}