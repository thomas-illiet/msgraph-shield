namespace GraphShield.Data.Model.Abstracts
{
    /// <summary>
    /// This interface is used to standardize soft deleting entities. Soft-deleted entities are not actually deleted from the database,
    /// but are marked as IsDeleted = true. They are typically not retrieved for the client.
    /// </summary>
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
    }
}