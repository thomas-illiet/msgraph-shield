using AutoMapper;
using GraphShield.Data.Model.Entities;
using GraphShield.Proxy.Plumbings.Data.Models;

namespace GraphShield.Proxy.Plumbings.Data.Profiles
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