using Microsoft.OData.UriParser;
using Titanium.Web.Proxy.Http;

namespace MSGraphShield.Proxy.Validators
{
    internal class DefaultInputValidator
    {
        private readonly HttpWebClient _request;
        private readonly ODataUriParser _parser;

        public DefaultInputValidator(ODataUriParser parser, HttpWebClient request)
        {
            _parser = parser;
            _request = request;
        }

        public string Method => _request.Request.Method;
        public string RequestUri => _request.Request.RequestUriString;
        public bool HasBody => _request.Request.HasBody;
        public long? Top => _parser.ParseTop();
        public long? Skip => _parser.ParseSkip();
        public long? Index => _parser.ParseIndex();
        public bool? Count => _parser.ParseCount();
        public string SkipToken => _parser.ParseSkipToken();
        public string DeltaToken => _parser.ParseDeltaToken();
    }
}