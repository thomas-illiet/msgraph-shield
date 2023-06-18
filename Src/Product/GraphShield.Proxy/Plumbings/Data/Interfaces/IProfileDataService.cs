using GraphShield.Data.Model.Entities;
using GraphShield.Proxy.Plumbings.Data.Models;

namespace GraphShield.Proxy.Plumbings.Data.Interfaces
{
    public interface IProfileDataService : IDefaultDataService<ProfileEntity, ProfileData>
    {
        Task<List<ProfileData>> ListAsync(Guid clientId, CancellationToken cancellationToken = default);
    }
}