using GraphShield.Data.Model.Abstracts;
using GraphShield.Data.Model.Enums;
using System;
using System.Collections.Generic;

namespace GraphShield.Data.Model.Entities
{
    /// <summary>
    /// Represents a credential entity.
    /// </summary>
    public class CredentialEntity : IEntity, ICreationTrackable, IModificationTrackable
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
        /// Gets or sets the secret value of the credential.
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// Gets or sets the expiration timestamp in UTC for the credential.
        /// </summary>
        public DateTimeOffset? ExpireUtc { get; set; }

        #endregion Data

        #region Navigation

        /// <summary>
        /// Gets or sets the clients associated with this credential.
        /// </summary>
        public ICollection<ClientEntity>? Clients { get; set; }

        /// <summary>
        /// Gets or sets the profiles associated with this credential.
        /// </summary>
        public ICollection<ProfileEntity>? Profiles { get; set; }

        #endregion Navigation

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
