using MSGraphShield.Data.Model.Abstracts;
using System;
using System.Collections.Generic;

namespace MSGraphShield.Data.Model.Entities
{
    /// <summary>
    /// Represents a client entity that interacts with the application.
    /// </summary>
    public class ClientEntity : IEntity, ICreationTrackable, IModificationTrackable
    {
        #region Data

        /// <summary>
        /// Gets or sets the internal identifier of the client.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the client used to receive the request.
        /// </summary>
        public string RemoteId { get; set; } = null!;

        /// <summary>
        /// Gets or sets the display name of this client.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the credential used to send requests to the Microsoft Graph.
        /// </summary>
        public Guid? CredentialId { get; set; }

        /// <summary>
        /// Gets or sets the last time the client was seen.
        /// </summary>
        public DateTimeOffset LastSeenUtc { get; set; }

        /// <summary>
        /// Gets or sets the status of the client.
        /// </summary>
        public bool Status { get; set; }

        #endregion Data

        #region Navigation

        /// <summary>
        /// Gets or sets the credential used to send requests to the Microsoft Graph.
        /// </summary>
        public CredentialEntity? Credential { get; set; }

        /// <summary>
        /// Gets or sets the attached profiles for this client.
        /// </summary>
        public ICollection<ProfileEntity>? Profiles { get; set; }

        #endregion Navigation

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

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientEntity"/> class.
        /// </summary>
        public ClientEntity()
        {
            DisplayName = "Unknown";
        }
    }
}
