using System.Text.RegularExpressions;
using MSGraphShield.Data.Model.Entities;
using MSGraphShield.Data.Model.Enums;

namespace MSGraphShield.Proxy.Plumbings.Data.Models
{
    public class RuleData
    {
        public Guid Id { get; set; }
        public RuleMethod Method { get; set; }
        public Regex Pattern { get; set; }
        public RuleType Type { get; set; }
        public RuleContent? Remote { get; set; }
        public RuleContent? Request { get; set; }
    }
}