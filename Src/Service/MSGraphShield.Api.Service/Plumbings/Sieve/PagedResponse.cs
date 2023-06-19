using System.Text.Json.Serialization;

namespace MSGraphShield.Api.Service.Plumbings.Sieve
{
    /// <summary>
    /// Represents a paged response containing a subset of items from a larger data source.
    /// </summary>
    /// <typeparam name="T">The type of items in the paged response.</typeparam>
    public class PagedResponse<T> where T : class
    {
        /// <summary>
        /// Gets or sets the results of the paged response.
        /// </summary>
        [JsonPropertyOrder(100)]
        public List<T> Results { get; set; }

        /// <summary>
        /// Gets or sets the current page number.
        /// </summary>
        [JsonPropertyOrder(101)]
        public int CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets the total number of pages.
        /// </summary>
        [JsonPropertyOrder(102)]
        public int PageCount { get; set; }

        /// <summary>
        /// Gets or sets the number of items per page.
        /// </summary>
        [JsonPropertyOrder(103)]
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the total number of items in the data source without pagination.
        /// </summary>
        [JsonPropertyOrder(104)]
        public long RawCount { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedResponse{T}"/> class.
        /// </summary>
        public PagedResponse()
        {
            Results = new List<T>();
        }
    }
}