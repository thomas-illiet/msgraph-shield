using MSGraphShield.Data.Model.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System.Net;
using System.Reflection;
using MSGraphShield.Proxy.Plumbings.RuleEngine;
using Titanium.Web.Proxy.Http;

namespace MSGraphShield.Proxy.Validators
{
    internal class ValidatorManagement
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ValidatorManagement> _logger;

        private readonly Dictionary<string, TypeInfo> _inputValidators = new();
        private readonly Dictionary<string, TypeInfo> _remoteValidators = new();

        public ValidatorManagement(IServiceProvider serviceProvider, ILogger<ValidatorManagement> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task InitializeAsync()
        {
            var validatorTypes = GetValidatorTypes();
            foreach (var validatorType in validatorTypes)
            {
                var attribute = validatorType.GetCustomAttribute<ValidatorAttribute>();
                if (attribute != null)
                {
                    if (attribute.ValidatorType == ValidatorType.Input)
                        _inputValidators.Add(attribute.EdmType, validatorType);
                    else
                        _remoteValidators.Add(attribute.EdmType, validatorType);

                    _logger.LogInformation("Add {validatorType} validator: {EdmType}", attribute.ValidatorType,
                        attribute.EdmType);
                }
            }
            return Task.CompletedTask;
        }

        public Task RequestValidationAsync(ODataUriParser parser, HttpWebClient request, RuleContent rule)
        {
            var pathCollection = parser.ParsePath();
            var entryType = pathCollection.FirstSegment.EdmType.AsElementType().FullTypeName();
            if (!_inputValidators.ContainsKey(entryType))
                throw new NotImplementedException($"Following entry is not implemented for request validation: {entryType}");

            var instanceType = _inputValidators[entryType];
            var validator = (IValidator)Activator.CreateInstance(instanceType, parser, request)!;


            var _compiledRule = new RuleFactory().CompileRule<IValidator>(rule.ToRule());
            if (_compiledRule(validator))
            {
                _logger.LogError($"Forbidden request from {request.Request.RequestUri}");
            }

            return Task.CompletedTask;
        }

        public Task RemoteValidationAsync(ODataUriParser parser, HttpWebClient request, RuleContent rule)
        {
            var pathCollection = parser.ParsePath();
            if (!_remoteValidators.ContainsKey(pathCollection.FirstSegment.EdmType.AsElementType().FullTypeName()))
                throw new Exception();

            return Task.CompletedTask;
        }

        private List<TypeInfo> GetValidatorTypes()
        {
            var interfaceType = typeof(IValidator);
            return GetType().Assembly.DefinedTypes
                .Where(typeInfo => typeInfo.IsClass && interfaceType.IsAssignableFrom(typeInfo))
                .Select(typeInfo => typeInfo).ToList();
        }
    }
}