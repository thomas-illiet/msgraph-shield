using AutoMapper;
using GraphShield.Data.Model.Entities;
using GraphShield.Proxy.Plumbings.Data.Models;

namespace GraphShield.Proxy.Plumbings.Data.Profiles
{
    internal class ProfileDataProfile : Profile
    {
        public ProfileDataProfile()
        {
            CreateMap<ProfileEntity, ProfileData>();
        }
    }
}