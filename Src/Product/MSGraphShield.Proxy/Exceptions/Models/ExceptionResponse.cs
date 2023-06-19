using Titanium.Web.Proxy.Http;

namespace MSGraphShield.Proxy.Exceptions.Models
{
    internal class ExceptionResponse : Response
    {
        public ExceptionResponse(byte[] body) : base(body)
        {
            // Add additional functionality if needed
        }
    }
}