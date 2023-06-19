namespace MSGraphShield.Api.Service.Plumbings.Authentication
{
    /// <summary>
    /// Represents the configuration settings for authentication.
    /// </summary>
    public class AuthenticationConfiguration
    {
        /// <summary>
        /// Gets or sets the authority URL for the authentication server.
        /// </summary>
        public string Authority { get; set; }

        /// <summary>
        /// Gets or sets the audience of the authentication token.
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// Gets or sets the issuer of the authentication token.
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether HTTPS metadata is required.
        /// </summary>
        public bool RequireHttpsMetadata { get; set; } = true;
    }
}
