using GraphShield.Data.Model.Entities;
using GraphShield.Proxy.Plumbings.Data.Models;

namespace GraphShield.Proxy.Plumbings.Data.Interfaces
{
    public interface IClientDataService : IDefaultDataService<ClientEntity, ClientData>
    {
        Task<ClientData?> GetAsync(string remoteId, CancellationToken cancellationToken = default);
    }
}