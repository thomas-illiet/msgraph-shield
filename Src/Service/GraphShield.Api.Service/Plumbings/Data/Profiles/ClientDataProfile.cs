using AutoMapper;
using GraphShield.Api.Service.Plumbings.Data.Models;
using GraphShield.Data.Model.Entities;

namespace GraphShield.Api.Service.Plumbings.Data.Profiles
{
    /// <summary>
    /// Profile for mapping the ClientEntity.
    /// </summary>
    internal class ClientDataProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientDataProfile"/> class.
        /// </summary>
        public ClientDataProfile()
        {
            CreateMap<ClientEntity, ClientDto>();
        }
    }
}