namespace GraphShield.Api.Service.Models.Configuration
{
    /// <summary>
    /// Represents the configuration for Swagger.
    /// </summary>
    public class SwaggerConfiguration
    {
        /// <summary>
        /// Gets or sets the prefix for Swagger routes.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Gets or sets the authorization URL for Swagger.
        /// </summary>
        public Uri AuthorizationUrl { get; set; }

        /// <summary>
        /// Gets or sets the token URL for Swagger.
        /// </summary>
        public Uri TokenUrl { get; set; }

        /// <summary>
        /// Gets or sets the scopes for Swagger.
        /// </summary>
        public Dictionary<string, string> Scopes { get; set; }

        /// <summary>
        /// Gets or sets the client ID for Swagger.
        /// </summary>
        public string ClientId { get; set; }
    }
}