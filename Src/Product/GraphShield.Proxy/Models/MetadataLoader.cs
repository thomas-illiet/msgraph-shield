namespace GraphShield.Proxy.Models
{
    /// <summary>
    /// Represents a metadata loader for endpoints.
    /// </summary>
    internal class MetadataLoader
    {
        /// <summary>
        /// Gets or sets the list of endpoints.
        /// </summary>
        public List<string> Endpoints { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataLoader"/> class.
        /// </summary>
        public MetadataLoader()
        {
            Endpoints = new List<string>();
        }
    }
}