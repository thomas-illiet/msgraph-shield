using GraphShield.Proxy.Exceptions.Models;
using System;
using System.Collections.Generic;

namespace GraphShield.Proxy.Exceptions
{
    /// <summary>
    /// Represents an exception indicating an internal server error.
    /// </summary>
    internal class InternalServerException : ProductException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InternalServerException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="extendedInfo">The extended information associated with the exception.</param>
        /// <param name="type">The type of the exception.</param>
        /// <param name="title">The title of the exception.</param>
        public InternalServerException(string message, IDictionary<string, object?>? extendedInfo = null, string? type = null, string? title = null)
            : base(message)
        {
            Status = 500;
            Extensions = extendedInfo ?? Extensions;
            Type = type ?? "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1";
            Title = title ?? "Internal Server Error";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalServerException"/> class.
        /// </summary>
        /// <param name="exception">The inner exception.</param>
        /// <param name="message">The error message.</param>
        /// <param name="extendedInfo">The extended information associated with the exception.</param>
        /// <param name="type">The type of the exception.</param>
        /// <param name="title">The title of the exception.</param>
        public InternalServerException(Exception exception, string message, IDictionary<string, object?>? extendedInfo = null, string? type = null, string? title = null)
            : base(exception, message)
        {
            Status = 500;
            Extensions = extendedInfo ?? Extensions;
            Type = type ?? "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1";
            Title = title ?? "Internal Server Error";
        }
    }
}
