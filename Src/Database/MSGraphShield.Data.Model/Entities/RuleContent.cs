using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace MSGraphShield.Data.Model.Entities
{
    /// <summary>
    /// Represents the content of a rule.
    /// </summary>
    public class RuleContent
    {
        /// <summary>
        /// Gets or sets the member name associated with the rule content.
        /// </summary>
        public string MemberName { get; set; }

        /// <summary>
        /// Gets or sets the operator associated with the rule content.
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// Gets or sets the target value associated with the rule content.
        /// </summary>
        public string TargetValue { get; set; }

        /// <summary>
        /// Gets or sets the inputs associated with the rule content.
        /// </summary>
        public List<string> Inputs { get; set; }

        /// <summary>
        /// Gets or sets the rules associated with the rule content.
        /// </summary>
        public List<RuleContent> Rules { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the rule content should be negated.
        /// </summary>
        public bool Negate { get; set; }
    }
}