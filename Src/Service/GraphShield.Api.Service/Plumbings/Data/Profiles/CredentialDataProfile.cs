using AutoMapper;
using GraphShield.Api.Service.Plumbings.Data.Models;
using GraphShield.Data.Model.Entities;

namespace GraphShield.Proxy.Plumbings.Data.Profiles
{
    /// <summary>
    /// Profile for mapping the CredentialEntity.
    /// </summary>
    internal class CredentialDataProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CredentialDataProfile"/> class.
        /// </summary>
        public CredentialDataProfile()
        {
            CreateMap<CredentialEntity, CredentialDto>();
        }
    }
}