namespace GraphShield.Data.Model.Abstracts
{
    /// <summary>
    /// This interface is implemented by entities and represents a generic entity with an identifier.
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier of the entity.
        /// </summary>
        Guid Id { get; set; }
    }
}