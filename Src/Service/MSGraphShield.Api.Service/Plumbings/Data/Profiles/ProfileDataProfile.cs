using AutoMapper;
using MSGraphShield.Api.Service.Plumbings.Data.Models;
using MSGraphShield.Data.Model.Entities;

namespace MSGraphShield.Proxy.Plumbings.Data.Profiles
{
    /// <summary>
    /// Profile for mapping the ProfileEntity.
    /// </summary>
    internal class ProfileDataProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileDataProfile"/> class.
        /// </summary>
        public ProfileDataProfile()
        {
            CreateMap<ProfileEntity, ProfileDto>();
        }
    }
}