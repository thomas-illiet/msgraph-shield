namespace MSGraphShield.Data.Model.Abstracts
{
    /// <summary>
    /// This interface is implemented by entities that need to indicate whether they are protected or not.
    /// The property <see cref="IsProtected"/> represents the protection status of the entity.
    /// </summary>
    public interface IProtectedEntity
    {
        bool IsProtected { get; }
    }
}