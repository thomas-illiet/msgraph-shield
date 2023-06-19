using MSGraphShield.Data.Model.Enums;

namespace MSGraphShield.Proxy.Validators
{
    internal class ValidatorAttribute : Attribute
    {
        public readonly ValidatorType ValidatorType;
        public readonly string EdmType;

        public ValidatorAttribute(ValidatorType validatorType, string edmType)
        {
            EdmType = edmType;
            ValidatorType = validatorType;
        }
    }
}