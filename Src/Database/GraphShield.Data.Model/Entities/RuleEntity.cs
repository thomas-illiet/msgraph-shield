﻿using GraphShield.Data.Model.Abstracts;
using GraphShield.Data.Model.Enums;
using System;

namespace GraphShield.Data.Model.Entities
{
    /// <summary>
    /// Represents a rule entity.
    /// </summary>
    public class RuleEntity : ICreationTrackable, IModificationTrackable
    {
        #region Data

        /// <summary>
        /// Gets or sets the unique identifier of the rule.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the profile identifier associated with the rule.
        /// </summary>
        public Guid ProfileId { get; set; }

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
        /// Gets or sets the pattern of the rule.
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

        #region Navigation

        /// <summary>
        /// Gets or sets the profile associated with the rule.
        /// </summary>
        public ProfileEntity Profile { get; set; }

        #endregion Navigation

        #region Metadata

        /// <summary>
        /// Gets or sets the creation timestamp of the rule.
        /// </summary>
        public DateTimeOffset CreatedUtc { get; set; }

        /// <summary>
        /// Gets or sets the last modification timestamp of the rule.
        /// </summary>
        public DateTimeOffset? UpdatedUtc { get; set; }

        #endregion Metadata
    }
}