using GraphShield.Data.Model.Enums;

namespace GraphShield.Api.Service.Plumbings.Data.Models
{
    public class ProfileDto
    {
        #region Data

        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public CredentialDto? Credential { get; set; }
        public AuditMode Audit { get; set; }
        public bool IsProtected { get; set; }
        public bool IsActive { get; set; }

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