using System;

namespace GraphShield.Data.Model.Abstracts
{
    /// <summary>
    /// Represents an entity that stores creation information, including the creator's identifier and creation time.
    /// The creation time and creator user are automatically set when the entity is saved to the database.
    /// </summary>
    public interface ICreationAuditable : ICreationTrackable
    {
        /// <summary>
        /// Gets or sets the identifier of the user who created the entity.
        /// </summary>
        Guid CreatedBy { get; set; }
    }
}