using MSGraphShield.Proxy.Exceptions.Models;
using System.Collections.Generic;

namespace MSGraphShield.Proxy.Exceptions
{
    /// <summary>
    /// Represents an exception indicating a forbidden access.
    /// </summary>
    internal class ForbiddenException : ProductException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForbiddenException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="extendedInfo">The extended information associated with the exception.</param>
        /// <param name="type">The type of the exception.</param>
        /// <param name="title">The title of the exception.</param>
        public ForbiddenException(string message, IDictionary<string, object?>? extendedInfo = null, string? type = null, string? title = null)
            : base(message)
        {
            Status = 403;
            Extensions = extendedInfo ?? Extensions;
            Type = type ?? "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3";
            Title = title ?? "Forbidden";
        }
    }
}