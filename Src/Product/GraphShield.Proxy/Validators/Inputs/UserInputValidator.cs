using Microsoft.OData.UriParser;
using Titanium.Web.Proxy.Http;

namespace GraphShield.Proxy.Validators.Inputs
{
    [Validator(ValidatorType.Input, "microsoft.graph.user")]
    internal class UserInputValidator : DefaultInputValidator, IValidator
    {
        public UserInputValidator(ODataUriParser parser, HttpWebClient request)
            : base(parser, request) { }
    }
}