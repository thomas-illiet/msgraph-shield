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

namespace GraphShield.Api.Service.Plumbings.Data
{
    /// <summary>
    /// Service for accessing and manipulating client data.
    /// </summary>
    public class ClientDataService
    {
        private readonly DataConfigDbContext _dataContext;
        private readonly ISieveProcessor _sieve;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientDataService"/> class.
        /// </summary>
        /// <param name="dataContext">The data context for accessing the database.</param>
        /// <param name="sieve">The Sieve processor for filtering and sorting data.</param>
        /// <param name="mapper">The AutoMapper instance for object mapping.</param>
        public ClientDataService(DataConfigDbContext dataContext, ISieveProcessor sieve, IMapper mapper)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _sieve = sieve ?? throw new ArgumentNullException(nameof(sieve));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Retrieves a paged list of clients based on the provided SieveModel.
        /// </summary>
        /// <param name="request">The SieveModel containing filtering and sorting parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<PagedResponse<ClientDto>> ListAsync(SieveModel request, CancellationToken cancellationToken)
        {
            var query = _dataContext.Set<ClientEntity>().AsNoTracking();
            return await query.ToPagedAsync<ClientEntity, ClientDto>(_sieve, _mapper, request, cancellationToken);
        }

        /// <summary>
        /// Retrieves a client by its identifier.
        /// </summary>
        /// <param name="clientId">The identifier of the client to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<ClientDto?> GetAsync(Guid clientId, CancellationToken cancellationToken)
        {
            var query = _dataContext.Set<ClientEntity>().AsNoTracking();
            var entity = await query.Where(x => x.Id == clientId)
                .ProjectTo<ClientDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(cancellationToken);

            if (entity == null)
                throw new NotFoundException("The requested profile was not found in the system");

            return entity;
        }

        /// <summary>
        /// Deletes a client by its identifier.
        /// </summary>
        /// <param name="clientId">The identifier of the client to delete.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task DeleteAsync(Guid clientId, CancellationToken cancellationToken)
        {
            var entity = await _dataContext.Set<ClientEntity>().SingleOrDefaultAsync(x => x.Id == clientId, cancellationToken);
            if (entity == null)
                throw new NotFoundException("The requested client was not found in the system.");

            _dataContext.Remove(entity);
            await _dataContext.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves a paged list of profiles for a specific client.
        /// </summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="request">The SieveModel containing filtering and sorting parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>b
        public async Task<PagedResponse<ProfileDto>> ListProfileAsync(Guid clientId, SieveModel request, CancellationToken cancellationToken)
        {
            var query = _dataContext.Set<ProfileEntity>().AsNoTracking()
                .Include(x => x.Clients)
                .Where(x => x.Clients.Any(x => x.Id == clientId));
            return await query.ToPagedAsync<ProfileEntity, ProfileDto>(_sieve, _mapper, request, cancellationToken);
        }


        /// <summary>
        /// Retrieves a profile by its identifier for a specific client.
        /// </summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="profileId">The identifier of the profile to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<ProfileDto> GetProfileAsync(Guid clientId, Guid profileId, CancellationToken cancellationToken)
        {
            var client = await _dataContext.Set<ClientEntity>()
                .Include(x => x.Profiles)
                .SingleOrDefaultAsync(x => x.Id == clientId, cancellationToken);
            if (client == null)
                throw new NotFoundException("The requested client was not found in the system.");

            var profile = client.Profiles?.FirstOrDefault(x => x.Id == profileId);
            if (profile == null)
                throw new BadRequestException("The requested profile is not associated with the client.");

            return _mapper.Map<ProfileDto>(profile);
        }

        /// <summary>
        /// Adds a profile to a client.
        /// </summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="profileId">The identifier of the profile to add.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task AddProfileAsync(Guid clientId, Guid profileId, CancellationToken cancellationToken)
        {
            var client = await _dataContext.Set<ClientEntity>()
                .Include(x => x.Profiles)
                .SingleOrDefaultAsync(x => x.Id == clientId, cancellationToken);
            if (client == null)
                throw new NotFoundException("The requested client was not found in the system.");

            var profile = await _dataContext.Set<ProfileEntity>()
                .SingleOrDefaultAsync(x => x.Id == profileId, cancellationToken);
            if (profile == null)
                throw new NotFoundException("The requested profile was not found in the system.");

            if (client.Profiles?.Any(x => x.Id == profileId) == true)
                throw new ConflictException("The requested profile is already associated with the client.");

            client.Profiles!.Add(profile);
            await _dataContext.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Deletes a profile from a client.
        /// </summary>
        /// <param name="clientId">The identifier of the client.</param>
        /// <param name="profileId">The identifier of the profile to delete.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task DeleteProfileAsync(Guid clientId, Guid profileId, CancellationToken cancellationToken)
        {
            var client = await _dataContext.Set<ClientEntity>()
                .Include(x => x.Profiles)
                .SingleOrDefaultAsync(x => x.Id == clientId, cancellationToken);
            if (client == null)
                throw new NotFoundException("The requested client was not found in the system.");

            var profile = client.Profiles?.FirstOrDefault(x => x.Id == profileId);
            if (profile == null)
                throw new BadRequestException("The requested profile is not associated with the client.");

            client.Profiles!.Remove(profile);
            await _dataContext.SaveChangesAsync(cancellationToken);
        }
    }
}