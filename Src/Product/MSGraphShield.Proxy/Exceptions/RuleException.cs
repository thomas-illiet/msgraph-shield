using MSGraphShield.Proxy.Exceptions.Models;
using System.Collections.Generic;

namespace MSGraphShield.Proxy.Exceptions
{
    /// <summary>
    /// Represents an exception indicating an internal rule error.
    /// </summary>
    internal class RuleException : ProductException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuleException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="extendedInfo">The extended information associated with the exception.</param>
        /// <param name="type">The type of the exception.</param>
        /// <param name="title">The title of the exception.</param>
        public RuleException(string message, IDictionary<string, object?>? extendedInfo = null, string? type = null, string? title = null)
            : base(message)
        {
            Status = 500;
            Extensions = extendedInfo ?? Extensions;
            Type = type ?? "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1";
            Title = title ?? "Internal Rule Error";
        }
    }
}