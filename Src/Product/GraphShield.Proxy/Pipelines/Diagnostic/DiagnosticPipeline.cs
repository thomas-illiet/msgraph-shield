using GraphShield.Proxy.Extensions;
using GraphShield.Proxy.Plumbings.Pipeline;
using Microsoft.Extensions.Logging;
using Titanium.Web.Proxy.EventArguments;

namespace GraphShield.Proxy.Pipelines.Diagnostic
{
    internal class DiagnosticPipeline : DefaultPipeline, IDiagnosticPipeline
    {
        private readonly ILogger<DiagnosticPipeline> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticPipeline"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public DiagnosticPipeline(ILogger<DiagnosticPipeline> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public override Task InitializeAsync()
        {
            _logger.LogInformation("Initialize pipeline: {fullName}", GetType().FullName);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        [Pipeline(PipelineCategory.Everything, uint.MinValue)]
        public override Task BeforeRequestAsync(SessionEventArgs session)
        {
            _logger.LogInformation("Request starting {HttpVersion} {Method} {Domain} {RequestUri}",
                "HTTP/" + session.HttpClient.Request.HttpVersion, session.HttpClient.Request.Method,
                session.HttpClient.Request.Host, session.HttpClient.Request.RequestUriString);

            // Add request id header to help Microsoft investigate errors, to each request to Microsoft Graph
            // add the client-request-id header with a unique GUID.
            // More info at https://aka.ms/graph/proxy/guidance/client-request-id
            if (session.IsGraphRequest() && !session.HttpClient.Request.Headers.HeaderExists("client-request-id"))
            {
                session.HttpClient.Request.Headers.AddHeader("client-request-id", session.ClientConnectionId.ToString());
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        [Pipeline(PipelineCategory.Everything, uint.MaxValue)]
        public override Task AfterResponseAsync(SessionEventArgs session)
        {
            _logger.LogInformation("Request finished {HttpVersion} {Method} {Domain} {RequestUri} {ContentType} {StatusCode} {Duration}ms",
                "HTTP/" + session.HttpClient.Request.HttpVersion, session.HttpClient.Request.Method,
                session.HttpClient.Request.Host, session.HttpClient.Request.RequestUriString,
                session.HttpClient.Response.ContentType, session.HttpClient.Response.StatusCode,
                (session.TimeLine.Last().Value - session.TimeLine.First().Value).TotalMilliseconds);

            return Task.CompletedTask;
        }
    }
}