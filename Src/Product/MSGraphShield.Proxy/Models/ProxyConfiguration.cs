using Titanium.Web.Proxy.Models;

namespace MSGraphShield.Proxy.Models
{
    /// <summary>
    /// Represents the configuration for the proxy.
    /// </summary>
    public class ProxyConfiguration
    {
        /// <summary>
        /// Gets or sets the port number for the proxy.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the external proxy configuration.
        /// </summary>
        public ExternalProxy? ExternalProxy { get; set; }

        /// <summary>
        /// Gets or sets the list of URLs to be intercepted for the Microsoft Graph API.
        /// </summary>
        public List<string> GraphUrls { get; set; }

        /// <summary>
        /// Gets or sets the list of default URLs to be intercepted.
        /// </summary>
        public List<string> DefaultUrls { get; set; }

        /// <summary>
        /// Gets or sets the timeout in seconds for server connections to wait for connection establishment.
        /// </summary>
        public int ConnectTimeOutSeconds { get; set; }

        /// <summary>
        /// Gets or sets the timeout in seconds for client/server connections to be kept alive when waiting for read/write to complete.
        /// </summary>
        public int ConnectionTimeOutSeconds { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyConfiguration"/> class.
        /// </summary>
        public ProxyConfiguration()
        {
            ConnectTimeOutSeconds = 30;
            ConnectionTimeOutSeconds = 30;
        }
    }
}