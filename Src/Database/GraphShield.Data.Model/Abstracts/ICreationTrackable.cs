using System;

namespace GraphShield.Data.Model.Abstracts
{
    /// <summary>
    /// Represents an entity that tracks its creation time.
    /// The <see cref="CreatedUtc" /> property is automatically set when the entity is saved to the database.
    /// </summary>
    public interface ICreationTrackable
    {
        /// <summary>
        /// Gets or sets the UTC time when the entity was created.
        /// </summary>
        DateTimeOffset CreatedUtc { get; set; }
    }
}