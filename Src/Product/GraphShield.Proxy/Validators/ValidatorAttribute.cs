using GraphShield.Data.Model.Enums;

namespace GraphShield.Proxy.Validators
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