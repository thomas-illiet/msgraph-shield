using System;

namespace GraphShield.Data.Model.Entities
{
    /// <summary>
    /// Represents the relationship between a client and a profile.
    /// </summary>
    public class ClientProfileEntity
    {
        /// <summary>
        /// Gets or sets the identifier of the client.
        /// </summary>
        public Guid ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client associated with this relationship.
        /// </summary>
        public ClientEntity Client { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the profile.
        /// </summary>
        public Guid ProfileId { get; set; }

        /// <summary>
        /// Gets or sets the profile associated with this relationship.
        /// </summary>
        public ProfileEntity Profile { get; set; }
    }
}