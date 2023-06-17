namespace GraphShield.Data.Model.Abstracts
{
    /// <summary>
    /// This interface is implemented by entities that require full auditing.
    /// It combines the functionality of all auditing interfaces: IFullTrackable,
    /// ICreationAuditable, IModificationAuditable, and IDeletionAuditable.
    /// The related properties are automatically set when saving, updating, or deleting Entity objects.
    /// </summary>
    public interface IFullAuditable : IFullTrackable, ICreationAuditable, IModificationAuditable, IDeletionAuditable
    {
    }
}