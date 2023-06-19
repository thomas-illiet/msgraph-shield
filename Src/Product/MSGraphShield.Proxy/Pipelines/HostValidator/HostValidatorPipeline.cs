using MSGraphShield.Proxy.Exceptions;
using MSGraphShield.Proxy.Extensions;
using MSGraphShield.Proxy.Models;
using MSGraphShield.Proxy.Plumbings.Pipeline;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using Titanium.Web.Proxy.EventArguments;

namespace MSGraphShield.Proxy.Pipelines.HostValidator
{
    public class HostValidatorPipeline : DefaultPipeline, IHostValidatorPipeline
    {
        private readonly ProxyConfiguration _config;
        private readonly ISet<Regex> _graphUris = new HashSet<Regex>();
        private readonly ISet<Regex> _defaultUris = new HashSet<Regex>();
        private readonly ILogger<HostValidatorPipeline> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="HostValidatorPipeline"/> class.
        /// </summary>
        /// <param name="config">The proxy configuration.</param>
        /// <param name="logger">The logger.</param>
        public HostValidatorPipeline(IOptions<ProxyConfiguration> config, ILogger<HostValidatorPipeline> logger)
        {
            _config = config?.Value ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Converts strings from the configuration to regular expressions.
        /// Extracts URLs from the configuration and converts them to regular expressions.
        /// </summary>
        public override Task InitializeAsync()
        {
            _logger.LogInformation("Initialize pipeline: {fullName}", GetType().FullName);

            ConvertToRegex(_config.GraphUrls, _graphUris);
            _logger.LogDebug("Graph pipeline expressions: {Expressions}", _graphUris);

            ConvertToRegex(_config.DefaultUrls, _defaultUris);
            _logger.LogDebug("Default pipeline expressions: {Expressions}", _defaultUris);

            return Task.CompletedTask;
        }

        [Pipeline(PipelineCategory.Everything, 2)]
        public override Task BeforeRequestAsync(SessionEventArgs session)
        {
            var requestUri = session.HttpClient.Request.RequestUri.AbsoluteUri;

            if (_graphUris.Any(h => h.IsMatch(requestUri)))
            {
                session.InsertData(nameof(PipelineCategory), PipelineCategory.Graph);
                return Task.CompletedTask;
            }

            if (_defaultUris.Any(h => h.IsMatch(requestUri)))
            {
                session.InsertData(nameof(PipelineCategory), PipelineCategory.Everything);
                return Task.CompletedTask;
            }

            throw new ForbiddenException(
                message: $"The following host is not allowed: {requestUri}",
                title: "Host is not allowed");
        }

        /// <summary>
        /// Converts a list of URIs into regular expressions.
        /// </summary>
        /// <param name="inputUris">The list of URIs.</param>
        /// <param name="outputRegexes">The output regular expressions.</param>
        private void ConvertToRegex(List<string> inputUris, ISet<Regex> outputRegexes)
        {
            foreach (var inputUri in inputUris)
            {
                var urlToWatchPattern = Regex.Unescape(inputUri).Replace(".*", "*");
                var urlToWatchRegexString = Regex.Escape(urlToWatchPattern).Replace("\\*", ".*");
                var urlRegex = new Regex($"^{urlToWatchRegexString}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

                outputRegexes.Add(urlRegex);
            }
        }
    }
}