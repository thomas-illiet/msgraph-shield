﻿using GraphShield.Data.Model.Entities;
using GraphShield.Data.Model.Enums;

namespace GraphShield.Api.Service.Plumbings.Data.Models
{
    /// <summary>
    /// Represents a request for creating or updating a rule.
    /// </summary>
    public class RuleRequest
    {
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
        /// Gets or sets the remote content of the rule.
        /// </summary>
        public RuleContent? Remote { get; set; }

        /// <summary>
        /// Gets or sets the request content of the rule.
        /// </summary>
        public RuleContent? Request { get; set; }

        /// <summary>
        /// Gets or sets the type of the rule.
        /// </summary>
        public RuleType? Type { get; set; }

        /// <summary>
        /// Gets or sets the version of the rule.
        /// </summary>
        public string Version { get; set; }
    }
}