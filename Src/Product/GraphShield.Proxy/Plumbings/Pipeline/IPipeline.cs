using System.Threading.Tasks;
using Titanium.Web.Proxy.EventArguments;

namespace GraphShield.Proxy.Plumbings.Pipeline
{
    /// <summary>
    /// Represents a pipeline for processing requests and responses.
    /// </summary>
    public interface IPipeline
    {
        /// <summary>
        /// Initializes the pipeline.
        /// </summary>
        /// <returns>A task that represents the initialization process.</returns>
        Task InitializeAsync();

        /// <summary>
        /// Executes tasks before sending a request.
        /// </summary>
        /// <param name="session">The session information.</param>
        /// <returns>A task that represents the before request process.</returns>
        Task BeforeRequestAsync(SessionEventArgs session);

        /// <summary>
        /// Executes tasks after receiving a response.
        /// </summary>
        /// <param name="session">The session information.</param>
        /// <returns>A task that represents the after response process.</returns>
        Task AfterResponseAsync(SessionEventArgs session);
    }
}