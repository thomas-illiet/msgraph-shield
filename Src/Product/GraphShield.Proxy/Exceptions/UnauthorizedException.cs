using GraphShield.Proxy.Exceptions.Models;
using System.Collections.Generic;

namespace GraphShield.Proxy.Exceptions
{
    /// <summary>
    /// Represents an exception indicating that the request is unauthorized.
    /// </summary>
    internal class UnauthorizedException : ProductException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnauthorizedException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="extendedInfo">The extended information associated with the exception.</param>
        /// <param name="type">The type of the exception.</param>
        /// <param name="title">The title of the exception.</param>
        public UnauthorizedException(string message, IDictionary<string, object?>? extendedInfo = null, string? type = null, string? title = null)
            : base(message)
        {
            Status = 401;
            Extensions = extendedInfo ?? Extensions;
            Type = type ?? "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1";
            Title = title ?? "Unauthorized";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnauthorizedException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="extendedInfo">The extended information associated with the exception.</param>
        /// <param name="type">The type of the exception.</param>
        /// <param name="title">The title of the exception.</param>
        public UnauthorizedException(Exception exception, string message, IDictionary<string, object?>? extendedInfo = null, string? type = null, string? title = null)
            : base(exception, message)
        {
            Status = 401;
            Extensions = extendedInfo ?? Extensions;
            Type = type ?? "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1";
            Title = title ?? "Unauthorized";
        }
    }
}