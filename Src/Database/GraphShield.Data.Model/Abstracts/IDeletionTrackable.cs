namespace GraphShield.Data.Model.Abstracts
{
    /// <summary>
    /// This interface is implemented by entities that want to store deletion information, including the deletion timestamp.
    /// </summary>
    public interface IDeletionTrackable : ISoftDeletable
    {
        /// <summary>
        /// Gets or sets the deletion timestamp of the entity.
        /// </summary>
        DateTimeOffset? DeletedUtc { get; set; }
    }
}