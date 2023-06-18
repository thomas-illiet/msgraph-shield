using AutoMapper;
using AutoMapper.QueryableExtensions;
using GraphShield.Api.Service.Plumbings.Data.Models;
using GraphShield.Api.Service.Plumbings.Exceptions;
using GraphShield.Api.Service.Plumbings.Sieve;
using GraphShield.Data.Model.Entities;
using GraphShield.Data.Shared.DbContexts;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;
using System.Data;

namespace GraphShield.Api.Service.Plumbings.Data
{
    /// <summary>
    /// Service for accessing and manipulating profile data.
    /// </summary>
    public class ProfileDataService
    {
        private readonly DataConfigDbContext _dataContext;
        private readonly ISieveProcessor _sieve;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileDataService"/> class.
        /// </summary>
        /// <param name="dataContext">The data context for accessing the database.</param>
        /// <param name="sieve">The Sieve processor for filtering and sorting data.</param>
        /// <param name="mapper">The AutoMapper instance for object mapping.</param>
        public ProfileDataService(DataConfigDbContext dataContext, ISieveProcessor sieve, IMapper mapper)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _sieve = sieve ?? throw new ArgumentNullException(nameof(sieve));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Retrieves a paged list of profiles based on the provided SieveModel.
        /// </summary>
        /// <param name="request">The SieveModel containing filtering and sorting parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<PagedResponse<ProfileDto>> ListAsync(SieveModel request, CancellationToken cancellationToken)
        {
            var query = _dataContext.Set<ProfileEntity>().AsNoTracking();
            return await query.ToPagedAsync<ProfileEntity, ProfileDto>(_sieve, _mapper, request, cancellationToken);
        }

        /// <summary>
        /// Retrieves a profile by its identifier.
        /// </summary>
        /// <param name="profileId">The identifier of the profile to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<ProfileDto> GetAsync(Guid profileId, CancellationToken cancellationToken)
        {
            var query = _dataContext.Set<ProfileEntity>().AsNoTracking();
            var entity = await query.Where(x => x.Id == profileId)
                .ProjectTo<ProfileDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            if (entity == null)
                throw new NotFoundException("The requested profile was not found in the system");

            return entity;
        }

        /// <summary>
        /// Deletes a profile by its identifier.
        /// </summary>
        /// <param name="profileId">The identifier of the profile to delete.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task DeleteAsync(Guid profileId, CancellationToken cancellationToken)
        {
            var entity = await _dataContext.Set<ProfileEntity>().FirstOrDefaultAsync(x => x.Id == profileId, cancellationToken);
            if (entity == null)
                throw new NotFoundException("The requested profile was not found in the system.");

            _dataContext.Remove(entity);
            await _dataContext.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves a paged list of rules associated with a specific profile.
        /// </summary>
        /// <param name="profileId">The identifier of the profile.</param>
        /// <param name="request">The model representing the filtering and pagination options.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<PagedResponse<RuleDto>> ListRuleAsync(Guid profileId, SieveModel request, CancellationToken cancellationToken)
        {
            var query = _dataContext.Set<RuleEntity>().AsNoTracking().Where(x => x.ProfileId == profileId);
            return await query.ToPagedAsync<RuleEntity, RuleDto>(_sieve, _mapper, request, cancellationToken);
        }

        /// <summary>
        /// Retrieves a rule by its identifier within a specific profile.
        /// </summary>
        /// <param name="profileId">The identifier of the profile containing the rule.</param>
        /// <param name="ruleId">The identifier of the rule to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<RuleDto> GetRuleAsync(Guid profileId, Guid ruleId, CancellationToken cancellationToken)
        {
            var entity = await _dataContext.Set<RuleEntity>().AsNoTracking()
                .Where(x => x.Id == ruleId && x.ProfileId == profileId)
                .ProjectTo<RuleDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);

            if (entity == null)
                throw new NotFoundException("The requested rule was not found in the system");

            return entity;
        }

        /// <summary>
        /// Adds a new rule to the specified profile.
        /// </summary>
        /// <param name="profileId">The ID of the profile to add the rule to.</param>
        /// <param name="request">The RuleRequest object containing the details of the rule.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<RuleDto> AddRuleAsync(Guid profileId, RuleRequest request, CancellationToken cancellationToken)
        {
            var profileExists = await _dataContext.Set<ProfileEntity>().AnyAsync(x => x.Id == profileId, cancellationToken);
            if (!profileExists)
                throw new NotFoundException("The requested profile was not found in the system");

            var entity = _mapper.Map<RuleEntity>(request);
            entity.ProfileId = profileId;

            await _dataContext.AddAsync(entity, cancellationToken);
            await _dataContext.SaveChangesAsync(cancellationToken);

            return _mapper.Map<RuleDto>(entity);
        }

        /// <summary>
        /// Updates a rule within a specific profile.
        /// </summary>
        /// <param name="profileId">The identifier of the profile containing the rule.</param>
        /// <param name="ruleId">The identifier of the rule to update.</param>
        /// <param name="request">The updated RuleRequest object.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<RuleDto> UpdateRuleAsync(Guid profileId, Guid ruleId, RuleRequest request, CancellationToken cancellationToken)
        {
            var entity = await _dataContext.Set<RuleEntity>()
                .FirstOrDefaultAsync(x => x.Id == ruleId && x.ProfileId == profileId, cancellationToken);
            if (entity == null)
                throw new NotFoundException("The requested rule was not found in the system.");

            _mapper.Map(request, entity);

            _dataContext.Update(entity);
            await _dataContext.SaveChangesAsync(cancellationToken);

            return _mapper.Map<RuleDto>(entity);
        }

        /// <summary>
        /// Sets the request rule content for a specific profile and rule.
        /// </summary>
        /// <param name="profileId">The identifier of the profile containing the rule.</param>
        /// <param name="ruleId">The identifier of the rule.</param>
        /// <param name="content">The RuleContent to set.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task SetRequestRuleAsync(Guid profileId, Guid ruleId, RuleContent content, CancellationToken cancellationToken)
        {
            var entity = await _dataContext.Set<RuleEntity>()
                .FirstOrDefaultAsync(x => x.Id == ruleId && x.ProfileId == profileId, cancellationToken);
            if (entity == null)
                throw new NotFoundException("The requested rule was not found in the system.");

            entity.Request = content;
            await _dataContext.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Sets the remote rule content for a specific rule within a profile.
        /// </summary>
        /// <param name="profileId">The identifier of the profile containing the rule.</param>
        /// <param name="ruleId">The identifier of the rule to update.</param>
        /// <param name="content">The remote rule content.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task SetRemoteRuleAsync(Guid profileId, Guid ruleId, RuleContent content, CancellationToken cancellationToken)
        {
            var entity = await _dataContext.Set<RuleEntity>()
                .FirstOrDefaultAsync(x => x.Id == ruleId && x.ProfileId == profileId, cancellationToken);
            if (entity == null)
                throw new NotFoundException("The requested rule was not found in the system.");

            entity.Remote = content;
            await _dataContext.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Deletes a rule from a specific profile.
        /// </summary>
        /// <param name="profileId">The identifier of the profile containing the rule.</param>
        /// <param name="ruleId">The identifier of the rule to delete.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task DeleteRuleAsync(Guid profileId, Guid ruleId, CancellationToken cancellationToken)
        {
            var entity = await _dataContext.Set<RuleEntity>()
                .FirstOrDefaultAsync(x => x.Id == ruleId && x.ProfileId == profileId, cancellationToken);
            if (entity == null)
                throw new NotFoundException("The requested rule was not found in the system.");

            _dataContext.Remove(entity);
            await _dataContext.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Deletes the remote rule content for a specific profile and rule.
        /// </summary>
        /// <param name="profileId">The identifier of the profile containing the rule.</param>
        /// <param name="ruleId">The identifier of the rule.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task DeleteRemoteRuleAsync(Guid profileId, Guid ruleId, CancellationToken cancellationToken)
        {
            var entity = await _dataContext.Set<RuleEntity>()
                .FirstOrDefaultAsync(x => x.Id == ruleId && x.ProfileId == profileId, cancellationToken);
            if (entity == null)
                throw new NotFoundException("The requested rule was not found in the system.");

            entity.Remote = null;
            await _dataContext.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Deletes the request rule content for a specific profile and rule.
        /// </summary>
        /// <param name="profileId">The identifier of the profile containing the rule.</param>
        /// <param name="ruleId">The identifier of the rule.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task DeleteRequestRuleAsync(Guid profileId, Guid ruleId, CancellationToken cancellationToken)
        {
            var entity = await _dataContext.Set<RuleEntity>()
                .FirstOrDefaultAsync(x => x.Id == ruleId && x.ProfileId == profileId, cancellationToken);
            if (entity == null)
                throw new NotFoundException("The requested rule was not found in the system.");

            entity.Request = null;
            await _dataContext.SaveChangesAsync(cancellationToken);
        }
    }
}