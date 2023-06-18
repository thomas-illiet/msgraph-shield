using GraphShield.Data.Model.Abstracts;
using GraphShield.Data.Model.Enums;

namespace GraphShield.Proxy.Plumbings.Data.Models
{
    public class CredentialData : IEntity
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid ClientId { get; set; }
        public CredentialType SecretType { get; set; }
        public string Secret { get; set; }
    }
}