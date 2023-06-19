using AutoMapper;
using MSGraphShield.Data.Model.Entities;
using MSGraphShield.Proxy.Plumbings.Data.Models;

namespace MSGraphShield.Proxy.Plumbings.Data.Profiles
{
    internal class ProfileDataProfile : Profile
    {
        public ProfileDataProfile()
        {
            CreateMap<ProfileEntity, ProfileData>();
        }
    }
}