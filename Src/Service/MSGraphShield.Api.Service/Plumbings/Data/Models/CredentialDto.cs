using MSGraphShield.Data.Model.Enums;

namespace MSGraphShield.Api.Service.Plumbings.Data.Models
{
    public class CredentialDto
    {
        #region Data

        /// <summary>
        /// Gets or sets the internal identifier of the credential.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the display name of the credential.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the description of the credential.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the tenant associated with the credential.
        /// </summary>
        public Guid TenantId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the client associated with the credential.
        /// </summary>
        public Guid ClientId { get; set; }

        /// <summary>
        /// Gets or sets the secret type of the credential.
        /// </summary>
        public CredentialType SecretType { get; set; }

        /// <summary>
        /// Gets or sets the expiration timestamp in UTC for the credential.
        /// </summary>
        public DateTimeOffset? ExpireUtc { get; set; }

        #endregion Data

        #region Metadata

        /// <summary>
        /// Gets or sets the creation timestamp in UTC for the credential.
        /// </summary>
        public DateTimeOffset CreatedUtc { get; set; }

        /// <summary>
        /// Gets or sets the last modification timestamp in UTC for the credential.
        /// </summary>
        public DateTimeOffset? UpdatedUtc { get; set; }

        #endregion Metadata
    }
}