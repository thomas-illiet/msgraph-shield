using MSGraphShield.Api.Service.Plumbings.Data;
using MSGraphShield.Api.Service.Plumbings.Data.Models;
using MSGraphShield.Api.Service.Plumbings.Exceptions;
using MSGraphShield.Api.Service.Plumbings.Sieve;
using MSGraphShield.Data.Model.Entities;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace MSGraphShield.Api.Service.Controllers
{
    /// <summary>
    /// Controller for managing profiles.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("/v{version:apiVersion}/profiles")]
    [Produces("application/json")]
    public class ProfilesController : ControllerBase
    {
        private readonly ProfileDataService _profileData;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfilesController"/> class.
        /// </summary>
        /// <param name="profileData">The profile data service for accessing and manipulating profiles.</param>
        public ProfilesController(ProfileDataService profileData)
        {
            _profileData = profileData ?? throw new ArgumentNullException(nameof(profileData));
        }

        /// <summary>
        /// Retrieves a paged list of profiles.
        /// </summary>
        /// <param name="request">The model containing filtering and sorting parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpGet]
        [SwaggerOperation(Tags = new[] { "📜 Profile Management" })]
        [ProducesResponseType(typeof(PagedResponse<ProfileDto>), 200)]
        public async Task<IActionResult> ListProfileAsync([FromQuery] SieveModel request, CancellationToken cancellationToken)
        {
            var entities = await _profileData.ListAsync(request, cancellationToken);
            return Ok(entities);
        }

        /// <summary>
        /// Retrieves a profile by its identifier.
        /// </summary>
        /// <param name="profileId">The identifier of the profile to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpGet("{profileId:Guid}")]
        [SwaggerOperation(Tags = new[] { "📜 Profile Management" })]
        [ProducesResponseType(typeof(ProfileDto), 200)]
        [ProducesResponseType(typeof(NotFoundDto), 404)]
        public async Task<IActionResult> GetProfileAsync(Guid profileId, CancellationToken cancellationToken)
        {
            var entity = await _profileData.GetAsync(profileId, cancellationToken);
            return Ok(entity);
        }

        /// <summary>
        /// Deletes a profile by its identifier.
        /// </summary>
        /// <param name="profileId">The identifier of the profile to delete.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpDelete("{profileId:Guid}")]
        [SwaggerOperation(Tags = new[] { "📜 Profile Management" })]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(NotFoundDto), 404)]
        public async Task<IActionResult> DeleteProfileAsync(Guid profileId, CancellationToken cancellationToken)
        {
            await _profileData.DeleteAsync(profileId, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Retrieves a list of rules associated with a specific profile.
        /// </summary>
        /// <param name="profileId">The identifier of the profile.</param>
        /// <param name="request">The model representing the filtering and pagination options.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpGet("{profileId:Guid}/rules")]
        [SwaggerOperation(Tags = new[] { "📜 Profile Management" })]
        [ProducesResponseType(typeof(PagedResponse<RuleDto>), 200)]
        public async Task<IActionResult> ListProfileRuleAsync(Guid profileId, [FromQuery] SieveModel request, CancellationToken cancellationToken)
        {
            var entities = await _profileData.ListRuleAsync(profileId, request, cancellationToken);
            return Ok(entities);
        }

        /// <summary>
        /// Adds a rule to a specific profile.
        /// </summary>
        /// <param name="profileId">The identifier of the profile.</param>
        /// <param name="request">The rule request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("{profileId:Guid}/rules")]
        [SwaggerOperation(Tags = new[] { "📜 Profile Management" })]
        [ProducesResponseType(typeof(PagedResponse<RuleDto>), 200)]
        public async Task<IActionResult> AddProfileRuleAsync(Guid profileId, RuleRequest request, CancellationToken cancellationToken)
        {
            var entities = await _profileData.AddRuleAsync(profileId, request, cancellationToken);
            return Ok(entities);
        }

        /// <summary>
        /// Retrieves a single rule from a specific profile.
        /// </summary>
        /// <param name="profileId">The identifier of the profile.</param>
        /// <param name="ruleId">The identifier of the rule to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpGet("{profileId:Guid}/rules/{ruleId:Guid}")]
        [SwaggerOperation(Tags = new[] { "📜 Profile Management" })]
        [ProducesResponseType(typeof(RuleDto), 200)]
        [ProducesResponseType(typeof(NotFoundDto), 404)]
        public async Task<IActionResult> GetProfileRuleAsync(Guid profileId, Guid ruleId, CancellationToken cancellationToken)
        {
            var entity = await _profileData.GetRuleAsync(profileId, ruleId, cancellationToken);
            return Ok(entity);
        }

        /// <summary>
        /// Updates a rule within a specific profile.
        /// </summary>
        /// <param name="profileId">The identifier of the profile.</param>
        /// <param name="ruleId">The identifier of the rule.</param>
        /// <param name="request">The updated rule request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPut("{profileId:Guid}/rules/{ruleId:Guid}")]
        [SwaggerOperation(Tags = new[] { "📜 Profile Management" })]
        [ProducesResponseType(typeof(RuleDto), 200)]
        [ProducesResponseType(typeof(NotFoundDto), 404)]
        public async Task<IActionResult> UpdateRuleAsync(Guid profileId, Guid ruleId, RuleRequest request, CancellationToken cancellationToken)
        {
            var entity = await _profileData.UpdateRuleAsync(profileId, ruleId, request, cancellationToken);
            return Ok(entity);
        }

        /// <summary>
        /// Updates the request configuration of a rule within a specific profile.
        /// </summary>
        /// <param name="profileId">The identifier of the profile.</param>
        /// <param name="ruleId">The identifier of the rule.</param>
        /// <param name="request">The updated rule request content.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPut("{profileId:Guid}/rules/{ruleId:Guid}/request")]
        [SwaggerOperation(Tags = new[] { "📜 Profile Management" })]
        [ProducesResponseType(202)]
        [ProducesResponseType(typeof(NotFoundDto), 404)]
        public async Task<IActionResult> UpdateRuleRequestAsync(Guid profileId, Guid ruleId, RuleContent request, CancellationToken cancellationToken)
        {
            await _profileData.SetRequestRuleAsync(profileId, ruleId, request, cancellationToken);
            return Accepted();
        }

        /// <summary>
        /// Deletes the request configuration of a rule within a specific profile.
        /// </summary>
        /// <param name="profileId">The identifier of the profile.</param>
        /// <param name="ruleId">The identifier of the rule you want to delete the request configuration.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpDelete("{profileId:Guid}/rules/{ruleId:Guid}/request")]
        [SwaggerOperation(Tags = new[] { "📜 Profile Management" })]
        [ProducesResponseType(202)]
        [ProducesResponseType(typeof(NotFoundDto), 404)]
        public async Task<IActionResult> UpdateRuleRequestAsync(Guid profileId, Guid ruleId, CancellationToken cancellationToken)
        {
            await _profileData.DeleteRequestRuleAsync(profileId, ruleId, cancellationToken);
            return Accepted();
        }

        /// <summary>
        /// Updates the remote configuration of a rule within a specific profile.
        /// </summary>
        /// <param name="profileId">The identifier of the profile.</param>
        /// <param name="ruleId">The identifier of the rule.</param>
        /// <param name="request">The updated rule remote content.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPut("{profileId:Guid}/rules/{ruleId:Guid}/remote")]
        [SwaggerOperation(Tags = new[] { "📜 Profile Management" })]
        [ProducesResponseType(202)]
        [ProducesResponseType(typeof(NotFoundDto), 404)]
        public async Task<IActionResult> UpdateRemoteRuleAsync(Guid profileId, Guid ruleId, RuleContent request, CancellationToken cancellationToken)
        {
            await _profileData.SetRemoteRuleAsync(profileId, ruleId, request, cancellationToken);
            return Accepted();
        }

        /// <summary>
        /// Deletes the remote configuration of a rule within a specific profile.
        /// </summary>
        /// <param name="profileId">The identifier of the profile.</param>
        /// <param name="ruleId">The identifier of the rule you want to delete the remote configuration.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpDelete("{profileId:Guid}/rules/{ruleId:Guid}/remote")]
        [SwaggerOperation(Tags = new[] { "📜 Profile Management" })]
        [ProducesResponseType(202)]
        [ProducesResponseType(typeof(NotFoundDto), 404)]
        public async Task<IActionResult> UpdateRemoteRuleAsync(Guid profileId, Guid ruleId, CancellationToken cancellationToken)
        {
            await _profileData.DeleteRemoteRuleAsync(profileId, ruleId, cancellationToken);
            return Accepted();
        }

        /// <summary>
        /// Deletes a rule from a specific profile.
        /// </summary>
        /// <param name="profileId">The identifier of the profile.</param>
        /// <param name="ruleId">The identifier of the rule to delete.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpDelete("{profileId:Guid}/rules/{ruleId:Guid}")]
        [SwaggerOperation(Tags = new[] { "📜 Profile Management" })]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(NotFoundDto), 404)]
        public async Task<IActionResult> DeleteProfileRuleAsync(Guid profileId, Guid ruleId, CancellationToken cancellationToken)
        {
            await _profileData.DeleteRuleAsync(profileId, ruleId, cancellationToken);
            return NoContent();
        }
    }
}