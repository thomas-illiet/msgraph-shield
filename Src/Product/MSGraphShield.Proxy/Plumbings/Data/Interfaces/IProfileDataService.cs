using MSGraphShield.Data.Model.Entities;
using MSGraphShield.Proxy.Plumbings.Data.Models;

namespace MSGraphShield.Proxy.Plumbings.Data.Interfaces
{
    public interface IProfileDataService : IDefaultDataService<ProfileEntity, ProfileData>
    {
        Task<List<ProfileData>> ListAsync(Guid clientId, CancellationToken cancellationToken = default);
    }
}