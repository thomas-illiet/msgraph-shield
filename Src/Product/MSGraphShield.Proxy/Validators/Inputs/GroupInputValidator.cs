using MSGraphShield.Data.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.UriParser;
using Titanium.Web.Proxy.Http;

namespace MSGraphShield.Proxy.Validators.Inputs
{
    [Validator(ValidatorType.Input, "microsoft.graph.group")]

    internal class GroupInputValidator : DefaultInputValidator, IValidator
    {
        public GroupInputValidator(ODataUriParser parser, HttpWebClient request)
            : base(parser, request) { }
    }
}