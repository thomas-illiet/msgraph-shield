namespace GraphShield.Proxy.Plumbings.Data.Interfaces
{
    public interface IDefaultDataService<TEntity, TEntityDto> : IDefaultDataService<TEntity>
    {
        Task<TEntityDto?> GetAsync(Guid entityId, CancellationToken cancellationToken = default);

        Task<TEntityDto?> SyncAsync(Guid entityId, CancellationToken cancellationToken = default);
    }

    public interface IDefaultDataService<TEntity>
    {
        Task RemoveAsync(Guid entityId);
    }
}