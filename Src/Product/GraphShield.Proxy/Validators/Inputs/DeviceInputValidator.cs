using Microsoft.OData.UriParser;
using Titanium.Web.Proxy.Http;

namespace GraphShield.Proxy.Validators.Inputs
{
    [Validator(ValidatorType.Input, "microsoft.graph.device")]
    internal class DeviceInputValidator : DefaultInputValidator, IValidator
    {
        public DeviceInputValidator(ODataUriParser parser, HttpWebClient request)
            : base(parser, request) { }
    }
}