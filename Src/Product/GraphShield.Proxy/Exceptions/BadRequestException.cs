using GraphShield.Proxy.Exceptions.Models;
using System.Collections.Generic;

namespace GraphShield.Proxy.Exceptions
{
    /// <summary>
    /// Represents an exception indicating a bad request.
    /// </summary>
    internal class BadRequestException : ProductException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BadRequestException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="extendedInfo">The extended information associated with the exception.</param>
        /// <param name="type">The type of the exception.</param>
        /// <param name="title">The title of the exception.</param>
        public BadRequestException(string message, IDictionary<string, object?>? extendedInfo = null, string? type = null, string? title = null)
            : base(message)
        {
            Status = 400;
            Extensions = extendedInfo ?? Extensions;
            Type = type ?? "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1";
            Title = title ?? "Bad Request";
        }
    }
}