using AutoMapper;
using MSGraphShield.Data.Model.Entities;
using MSGraphShield.Proxy.Plumbings.Data.Models;

namespace MSGraphShield.Proxy.Plumbings.Data.Profiles
{
    internal class ClientDataProfile : Profile
    {
        public ClientDataProfile()
        {
            CreateMap<ClientEntity, ClientData>()
                .ForMember(x => x.Id, cfg => cfg.MapFrom(y => y.Id));
        }
    }
}