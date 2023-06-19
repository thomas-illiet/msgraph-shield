namespace MSGraphShield.Data.Model.Enums
{
    /// <summary>
    /// Represents the HTTP method of a rule.
    /// </summary>
    public enum RuleMethod
    {
        /// <summary>
        /// HTTP GET method.
        /// </summary>
        Get = 1,

        /// <summary>
        /// HTTP POST method.
        /// </summary>
        Post = 2,

        /// <summary>
        /// HTTP PATCH method.
        /// </summary>
        Patch = 3,

        /// <summary>
        /// HTTP DELETE method.
        /// </summary>
        Delete = 4
    }
}