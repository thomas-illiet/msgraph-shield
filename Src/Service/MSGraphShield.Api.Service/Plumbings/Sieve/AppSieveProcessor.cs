using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;

namespace MSGraphShield.Api.Service.Plumbings.Sieve
{
    /// <summary>
    /// Custom Sieve processor for application-specific configuration.
    /// </summary>
    internal class AppSieveProcessor : SieveProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppSieveProcessor"/> class.
        /// </summary>
        /// <param name="options">The Sieve options.</param>
        public AppSieveProcessor(IOptions<SieveOptions> options)
            : base(options) { }

        /// <summary>
        /// Maps the properties for Sieve filtering.
        /// </summary>
        /// <param name="mapper">The SievePropertyMapper instance.</param>
        /// <returns>The configured SievePropertyMapper.</returns>
        protected override SievePropertyMapper MapProperties(SievePropertyMapper mapper)
        {
            return mapper.ApplyConfigurationsFromAssembly(typeof(AppSieveProcessor).Assembly);
        }
    }
}