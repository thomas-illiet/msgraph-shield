using GraphShield.Data.Model.Abstracts;

namespace GraphShield.Proxy.Plumbings.Data.Models
{
    public class ClientData : IEntity
    {
        public Guid Id { get; set; }
        public string RemoteId { get; set; }
        public Guid? CredentialId { get; set; }
        public bool Status { get; set; }
    }
}
