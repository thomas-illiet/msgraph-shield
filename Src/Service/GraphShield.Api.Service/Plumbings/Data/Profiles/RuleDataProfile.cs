using AutoMapper;
using GraphShield.Api.Service.Plumbings.Data.Models;
using GraphShield.Data.Model.Entities;

namespace GraphShield.Api.Service.Plumbings.Data.Profiles
{
    /// <summary>
    /// Profile for mapping the RuleEntity.
    /// </summary>
    internal class RuleDataProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuleDataProfile"/> class.
        /// </summary>
        public RuleDataProfile()
        {
            CreateMap<RuleEntity, RuleDto>();
            CreateMap<RuleRequest, RuleEntity>();
        }
    }
}