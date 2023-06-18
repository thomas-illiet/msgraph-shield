using GraphShield.Api.Service.Plumbings.Data;
using GraphShield.Api.Service.Plumbings.Data.Models;
using GraphShield.Api.Service.Plumbings.Exceptions;
using GraphShield.Api.Service.Plumbings.Sieve;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace GraphShield.Api.Service.Controllers
{
    /// <summary>
    /// Controller for managing clients.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("/v{version:apiVersion}/clients")]
    [Produces("application/json")]
    public class ClientsController : ControllerBase
    {
        private readonly ClientDataService _clientData;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientsController"/> class.
        /// </summary>
        /// <param name="clientData">The client data service for accessing and manipulating clients.</param>
        public ClientsController(ClientDataService clientData)
        {
            _clientData = clientData ?? throw new ArgumentNullException(nameof(clientData));
        }

        /// <summary>
        /// Retrieves a paged list of clients.
        /// </summary>
        /// <param name="request">The SieveModel containing filtering and sorting parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpGet]
        [SwaggerOperation(Tags = new[] { "🐤 Client Management" })]
        [ProducesResponseType(typeof(PagedResponse<ClientDto>), 200)]
        public async Task<IActionResult> ListClientAsync([FromQuery] SieveModel request, CancellationToken cancellationToken)
        {
            var entities = await _clientData.ListAsync(request, cancellationToken);
            return Ok(entities);
        }

        /// <summary>
        /// Retrieves a client by its identifier.
        /// </summary>
        /// <param name="clientId">The identifier of the client to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpGet("{clientId:Guid}")]
        [SwaggerOperation(Tags = new[] { "🐤 Client Management" })]
        [ProducesResponseType(typeof(ClientDto), 200)]
        [ProducesResponseType(typeof(NotFoundDto), 404)]
        public async Task<IActionResult> GetClientAsync(Guid clientId, CancellationToken cancellationToken)
        {
            var entity = await _clientData.GetAsync(clientId, cancellationToken);
            if (entity == null)
                throw new NotFoundException("The requested client was not found in the system.");

            return Ok(entity);
        }


        /// <summary>
        /// Deletes a client by its identifier.
        /// </summary>
        /// <param name="clientId">The identifier of the client to delete.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpDelete("{clientId:Guid}")]
        [SwaggerOperation(Tags = new[] { "🐤 Client Management" })]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(NotFoundDto), 404)]
        public async Task<IActionResult> DeleteClientAsync(Guid clientId, CancellationToken cancellationToken)
        {
            await _clientData.DeleteAsync(clientId, cancellationToken);
            return NoContent();
        }
    }
}