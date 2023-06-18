namespace GraphShield.Proxy.Models
{
    /// <summary>
    /// Represents client authentication configuration.
    /// </summary>
    internal class ClientAuthentication
    {
        /// <summary>
        /// Represents the validation parameters for the client.
        /// </summary>
        public class ClientValidation
        {
            /// <summary>
            /// Gets or sets the valid issuers.
            /// </summary>
            public IEnumerable<string> ValidIssuers { get; set; }

            /// <summary>
            /// Gets or sets the valid audiences.
            /// </summary>
            public IEnumerable<string> ValidAudiences { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether to validate the audience.
            /// </summary>
            public bool ValidateAudience { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether to allow expired tokens.
            /// </summary>
            public bool AllowExpired { get; set; }
        }

        /// <summary>
        /// Gets or sets the claim identifier used for client authentication.
        /// </summary>
        public string ClaimIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the authority used for client authentication.
        /// </summary>
        public string? Authority { get; set; }

        /// <summary>
        /// Gets or sets the client validation parameters.
        /// </summary>
        public ClientValidation Validations { get; set; } = new ClientValidation();
    }
}