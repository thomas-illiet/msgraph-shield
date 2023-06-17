namespace GraphShield.Data.Model.Enums
{
    /// <summary>
    /// Represents the mode of audit.
    /// </summary>
    public enum AuditMode
    {
        /// <summary>
        /// No auditing.
        /// </summary>
        None = 0,

        /// <summary>
        /// Audit successful operations.
        /// </summary>
        Success = 1,

        /// <summary>
        /// Audit failed operations.
        /// </summary>
        Failures = 2,

        /// <summary>
        /// Audit both successful and failed operations.
        /// </summary>
        Both = 3
    }
}