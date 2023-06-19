namespace MSGraphShield.Data.Model.Abstracts
{
    /// <summary>
    /// This interface is implemented by entities that require tracking of modification information,
    /// such as who and when it was last modified.
    /// The properties are automatically set when updating the Entity.
    /// </summary>
    public interface IModificationAuditable : IModificationTrackable
    {
        Guid? UpdatedBy { get; set; }
    }
}