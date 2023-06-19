using MSGraphShield.Data.Model.Entities;
using MSGraphShield.Data.Model.Enums;

namespace MSGraphShield.Api.Service.Plumbings.Data.Models
{
    public class RuleDto
    {
        #region Data

        /// <summary>
        /// Gets or sets the identifier of the rule.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the rule.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display name of the rule.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the method of the rule.
        /// </summary>
        public RuleMethod Method { get; set; }

        /// <summary>
        /// Gets or sets the pattern associated with the rule.
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// Gets or sets the type of the rule.
        /// </summary>
        public RuleType Type { get; set; }

        /// <summary>
        /// Gets or sets the remote content of the rule.
        /// </summary>
        public RuleContent? Remote { get; set; }

        /// <summary>
        /// Gets or sets the request content of the rule.
        /// </summary>
        public RuleContent? Request { get; set; }

        #endregion Data

        #region Metadata

        /// <summary>
        /// Gets or sets the version of the rule.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the creation date and time of the rule.
        /// </summary>
        public DateTimeOffset CreatedUtc { get; set; }

        /// <summary>
        /// Gets or sets the last updated date and time of the rule.
        /// </summary>
        public DateTimeOffset? UpdatedUtc { get; set; }

        #endregion Metadata
    }
}