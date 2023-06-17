namespace GraphShield.Data.Model.Abstracts
{
    /// <summary>
    /// An entity can implement this interface if it requires tracking of the modification time,
    /// represented by the property <see cref="UpdatedUtc"/>.
    /// The <see cref="UpdatedUtc"/> property is automatically set when updating the Entity.
    /// </summary>
    public interface IModificationTrackable
    {
        DateTimeOffset? UpdatedUtc { get; set; }
    }
}