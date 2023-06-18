﻿using System.Collections.Generic;

namespace GraphShield.Proxy.Exceptions.Models
{
    /// <summary>
    /// Represents an interface for product exceptions.
    /// </summary>
    internal interface IProductException
    {
        /// <summary>
        /// A URI reference that identifies the problem type.
        /// </summary>
        string? Type { get; set; }

        /// <summary>
        /// A short, human-readable summary of the problem type. It SHOULD NOT change from occurrence to occurrence
        /// of the problem, except for purposes of localization(e.g., using proactive content negotiation;
        /// see[RFC7231], Section 3.4).
        /// </summary>
        string? Title { get; set; }

        /// <summary>
        /// The HTTP status code([RFC7231], Section 6) generated by the origin server for this occurrence of the problem.
        /// </summary>
        int Status { get; set; }

        /// <summary>
        /// Gets the <see cref="IDictionary{TKey, TValue}"/> for extension members.
        /// <para>
        /// Problem type definitions MAY extend the problem details object with additional members. Extension members appear in the same namespace as
        /// other members of a problem type.
        /// </para>
        /// </summary>
        IDictionary<string, object?> Extensions { get; set; }

        /// <summary>
        /// Gets a message that describes the current exception.
        /// </summary>
        string Message { get; set; }
    }
}