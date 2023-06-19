using AutoMapper;
using MSGraphShield.Data.Model.Entities;
using MSGraphShield.Proxy.Plumbings.Data.Models;
using System.Text.RegularExpressions;

namespace MSGraphShield.Proxy.Plumbings.Data.Profiles
{
    internal class RuleDataProfile : Profile
    {
        public RuleDataProfile()
        {
            CreateMap<RuleEntity, RuleData>()
                .ForMember(
                    member => member.Pattern,
                    opt => opt.MapFrom(src => new Regex(src.Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase))
                );
        }
    }
}