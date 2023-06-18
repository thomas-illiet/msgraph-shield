using Titanium.Web.Proxy.Http;

namespace GraphShield.Proxy.Exceptions.Models
{
    internal class ExceptionResponse : Response
    {
        public ExceptionResponse(byte[] body) : base(body)
        {
            // Add additional functionality if needed
        }
    }
}