using GraphShield.Data.Model.Abstracts;
using GraphShield.Data.Model.Enums;

namespace GraphShield.Proxy.Plumbings.Data.Models
{
    public class ProfileData : IEntity
    {
        public Guid Id { get; set; }
        public Guid? CredentialId { get; set; }
        public AuditMode Audit { get; set; }
        public ICollection<RuleData> Rules { get; set; }
    }
}