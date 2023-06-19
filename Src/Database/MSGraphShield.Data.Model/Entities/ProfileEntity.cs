using MSGraphShield.Data.Model.Abstracts;
using MSGraphShield.Data.Model.Enums;
using System;
using System.Collections.Generic;

namespace MSGraphShield.Data.Model.Entities
{
    /// <summary>
    /// Represents a profile entity.
    /// </summary>
    public class ProfileEntity : IEntity, IProtectedEntity, ICreationTrackable, IModificationTrackable
    {
        #region Data

        /// <summary>
        /// Gets or sets the ID of the profile.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the display name of the profile.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the description of the profile.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the ID of the credential associated with the profile.
        /// </summary>
        public Guid? CredentialId { get; set; }

        /// <summary>
        /// Gets or sets the audit mode of the profile.
        /// </summary>
        public AuditMode Audit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the profile is protected.
        /// </summary>
        public bool IsProtected { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the profile is active.
        /// </summary>
        public bool IsActive { get; set; }

        #endregion Data

        #region Navigation

        /// <summary>
        /// Gets or sets the credential associated with the profile.
        /// </summary>
        public CredentialEntity? Credential { get; set; }

        /// <summary>
        /// Gets or sets the rules associated with the profile.
        /// </summary>
        public ICollection<RuleEntity> Rules { get; set; }

        /// <summary>
        /// Gets or sets the clients associated with the profile.
        /// </summary>
        public ICollection<ClientEntity> Clients { get; set; }

        #endregion Navigation

        #region Metadata

        /// <summary>
        /// Gets or sets the creation timestamp of the profile.
        /// </summary>
        public DateTimeOffset CreatedUtc { get; set; }

        /// <summary>
        /// Gets or sets the last modification timestamp of the profile.
        /// </summary>
        public DateTimeOffset? UpdatedUtc { get; set; }

        #endregion Metadata
    }
}
