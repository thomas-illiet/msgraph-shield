using MSGraphShield.Proxy.Models;
using MSGraphShield.Proxy.Plumbings.Pipeline;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;

namespace MSGraphShield.Proxy.Plumbings
{
    internal class ProxyService : IHostedService
    {
        private readonly ProxyConfiguration _config;
        private readonly PipelineManagement _pipelineMgmt;
        private readonly ILogger _logger;

        private ProxyServer? _proxyServer;
        private ExplicitProxyEndPoint? _explicitEndPoint;


        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyService"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="pipelineMgmt">The pipeline MGMT.</param>
        /// <param name="logger">The logger.</param>
        public ProxyService(IOptions<ProxyConfiguration> config, PipelineManagement pipelineMgmt, ILogger<ProxyService> logger)
        {
            _config = config.Value;
            _pipelineMgmt = pipelineMgmt;
            _logger = logger;
        }

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("{status} proxy service", "Starting");

            await _pipelineMgmt.LoadPipeLines();

            _proxyServer = new ProxyServer();
            _proxyServer.BeforeRequest += BeforeRequest;
            _proxyServer.AfterResponse += AfterResponse;

            if (_config.ExternalProxy != null) { 
                _proxyServer.UpStreamHttpProxy = _config.ExternalProxy;
                _proxyServer.UpStreamHttpsProxy = _config.ExternalProxy;
            }

            _proxyServer.ConnectTimeOutSeconds = _config.ConnectTimeOutSeconds;
            _proxyServer.ConnectionTimeOutSeconds = _config.ConnectionTimeOutSeconds;

            _explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, _config.Port);
            _proxyServer.AddEndPoint(_explicitEndPoint);
            _proxyServer.Start();   

            foreach (var endPoint in _proxyServer.ProxyEndPoints)
                _logger.LogInformation("Listening on {IpAddress}:{Port}...", endPoint.IpAddress, endPoint.Port);
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("{status} proxy service", "Stopping");

            if (_proxyServer is not null)
            {
                _proxyServer.BeforeRequest -= BeforeRequest;
                _proxyServer.AfterResponse -= AfterResponse;
                _proxyServer.Stop();
            }

            return Task.CompletedTask;
        }

        private async Task AfterResponse(object sender, SessionEventArgs session)
            => await _pipelineMgmt.ExecuteAsync(session, nameof(IPipeline.AfterResponseAsync));

        private async Task BeforeRequest(object sender, SessionEventArgs session)
            => await _pipelineMgmt.ExecuteAsync(session, nameof(IPipeline.BeforeRequestAsync));
    }
}