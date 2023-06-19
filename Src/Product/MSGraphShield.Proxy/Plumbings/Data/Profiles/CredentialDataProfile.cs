using AutoMapper;
using MSGraphShield.Data.Model.Entities;
using MSGraphShield.Proxy.Plumbings.Data.Models;

namespace MSGraphShield.Proxy.Plumbings.Data.Profiles
{
    internal class CredentialDataProfile : Profile
    {
        public CredentialDataProfile()
        {
            CreateMap<CredentialEntity, CredentialData>();
        }
    }
}