namespace GraphShield.Data.Model.Enums
{
    /// <summary>
    /// Represents the type of a rule.
    /// </summary>
    public enum RuleType
    {
        /// <summary>
        /// Dynamic rule type.
        /// </summary>
        Dynamic = 0,

        /// <summary>
        /// User rule type.
        /// </summary>
        User = 1,

        /// <summary>
        /// Device rule type.
        /// </summary>
        Device = 2,

        /// <summary>
        /// Group rule type.
        /// </summary>
        Group = 3
    }
}