using FluentValidation;
using MSGraphShield.Api.Service.Plumbings.Data.Models;

namespace MSGraphShield.Api.Service.Plumbings.Data.Validators
{
    /// <summary>
    /// Validator for the RuleRequest model.
    /// </summary>
    public class RuleRequestValidator : AbstractValidator<RuleRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuleRequestValidator"/> class.
        /// </summary>
        public RuleRequestValidator()
        {
            RuleFor(x => x.Name).Length(4, 64);
            RuleFor(x => x.DisplayName).Length(0, 128);
            RuleFor(x => x.Method).NotEmpty();
            RuleFor(x => x.Pattern).Length(4, 256);
            RuleFor(x => x.Version).NotEmpty();
            RuleFor(x => x.Type).NotEmpty();
        }
    }
}