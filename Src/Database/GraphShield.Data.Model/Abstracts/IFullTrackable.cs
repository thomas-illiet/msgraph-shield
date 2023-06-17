namespace GraphShield.Data.Model.Abstracts
{
    /// <summary>
    /// This interface is implemented by entities that require full tracking of modification times.
    /// It combines the functionality of all tracking interfaces: ICreationTrackable,
    /// IModificationTrackable, and IDeletionTrackable.
    /// The related properties are automatically set when saving, updating, or deleting Entity objects.
    /// </summary>
    public interface IFullTrackable : ICreationTrackable, IModificationTrackable, IDeletionTrackable
    {
    }
}