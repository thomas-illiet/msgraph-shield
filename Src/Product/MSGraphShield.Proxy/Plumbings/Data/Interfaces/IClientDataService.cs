using MSGraphShield.Data.Model.Entities;
using MSGraphShield.Proxy.Plumbings.Data.Models;

namespace MSGraphShield.Proxy.Plumbings.Data.Interfaces
{
    public interface IClientDataService : IDefaultDataService<ClientEntity, ClientData>
    {
        Task<ClientData?> GetAsync(string remoteId, CancellationToken cancellationToken = default);
    }
}