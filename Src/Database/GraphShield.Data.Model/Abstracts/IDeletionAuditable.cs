namespace GraphShield.Data.Model.Abstracts
{
    /// <summary>
    /// This interface is implemented by entities that want to store deletion information, including the identifier of the user who performed the deletion and the deletion timestamp.
    /// </summary>
    public interface IDeletionAuditable : IDeletionTrackable
    {
        /// <summary>
        /// Gets or sets the identifier of the user who performed the deletion.
        /// </summary>
        Guid? DeletedBy { get; set; }
    }
}