namespace GraphShield.Api.Service.Plumbings.Data.Models
{
    public class ClientDto
    {
        #region Data

        /// <summary>
        /// Gets or sets the internal identifier of the client.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the client used to receive the request.
        /// </summary>
        public string RemoteId { get; set; }

        /// <summary>
        /// Gets or sets the display name of this client.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the credential used to send requests to the Microsoft Graph.
        /// </summary>
        public CredentialDto? Credential { get; set; }

        /// <summary>
        /// Gets or sets the last time the client was seen.
        /// </summary>
        public DateTimeOffset LastSeenUtc { get; set; }

        /// <summary>
        /// Gets or sets the status of the client.
        /// </summary>
        public bool Status { get; set; }

        #endregion Data

        #region Metadata

        /// <summary>
        /// Gets or sets the creation timestamp in UTC for the client.
        /// </summary>
        public DateTimeOffset CreatedUtc { get; set; }

        /// <summary>
        /// Gets or sets the last modification timestamp in UTC for the client.
        /// </summary>
        public DateTimeOffset? UpdatedUtc { get; set; }

        #endregion Metadata
    }
}