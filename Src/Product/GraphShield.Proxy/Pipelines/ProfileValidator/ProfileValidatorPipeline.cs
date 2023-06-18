using GraphShield.Data.Model.Enums;
using GraphShield.Proxy.Exceptions;
using GraphShield.Proxy.Extensions;
using GraphShield.Proxy.Plumbings.Data.Interfaces;
using GraphShield.Proxy.Plumbings.Data.Models;
using GraphShield.Proxy.Plumbings.Graph;
using GraphShield.Proxy.Plumbings.Pipeline;
using GraphShield.Proxy.Validators;
using Microsoft.Extensions.Logging;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Http;

namespace GraphShield.Proxy.Pipelines.ProfileValidator
{
    internal class ProfileValidatorPipeline : DefaultPipeline, IProfileValidatorPipeline
    {
        private readonly IProfileDataService _profileData;
        private readonly GraphRequestParser _uriParser;
        private readonly ValidatorManagement _validatorMgmt;
        private readonly ILogger<ProfileValidatorPipeline> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileValidatorPipeline"/> class.
        /// </summary>
        /// <param name="profileData">The profile data.</param>
        /// <param name="uriParser">The URI parser.</param>
        /// <param name="validatorMgmt">The validator MGMT.</param>
        /// <param name="logger">The logger.</param>
        public ProfileValidatorPipeline(
            IProfileDataService profileData, GraphRequestParser uriParser,
            ValidatorManagement validatorMgmt, ILogger<ProfileValidatorPipeline> logger)
        {
            _profileData = profileData ?? throw new ArgumentNullException(nameof(profileData));
            _uriParser = uriParser ?? throw new ArgumentNullException(nameof(uriParser));
            _validatorMgmt = validatorMgmt ?? throw new ArgumentNullException(nameof(validatorMgmt));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task InitializeAsync()
        {
            _logger.LogInformation("Initialize pipeline: {fullName}", GetType().FullName);

            await _validatorMgmt.InitializeAsync();
            await _uriParser.InitializeAsync();
        }

        [Pipeline(PipelineCategory.Graph, 5)]
        public override async Task BeforeRequestAsync(SessionEventArgs session)
        {
            // Get the client identifier in the current session.
            // This section is optional, as the client identifier should always be defined.
            var clientId = session.GetData<Guid>("ClientId");
            if (clientId == default)
                throw new ArgumentException("Client identifier is null or empty in the session");

            // Gets user profiles from the database.
            var profiles = await _profileData.ListAsync(clientId);
            if (!profiles.Any())
                throw new ForbiddenException("Client has no active profile");

            // Retrieves the first rule corresponding to the user's request
            // From the list of user profiles.
            var matchSet = GetFirstMatch(profiles, session.HttpClient.Request);
            if (matchSet == null)
                throw new ForbiddenException("No rules match with current request");

            // Initialize OData uri analyzer if filtering has been defined.
            if (matchSet.Value.Rule.Request != null || matchSet.Value.Rule.Remote != null)
            {
                var parser = _uriParser.Parse(session.HttpClient.Request);

                // Execute request filtering if defined
                // This section is mainly used to restrict the use of OData functionality.
                if (matchSet.Value.Rule.Request != null)
                    await _validatorMgmt.RequestValidationAsync(parser, session.HttpClient, matchSet.Value.Rule.Request);

                // Execute remote entry filtering if defined
                if (matchSet.Value.Rule.Remote != null)
                    await _validatorMgmt.RemoteValidationAsync(parser, session.HttpClient, matchSet.Value.Rule.Remote);
            }

            // Insert data to allow execute next pipeline.
            if (matchSet.Value.Profile.CredentialId != null)
                session.InsertData("CredentialId", matchSet.Value.Profile.CredentialId);
        }

        private static (ProfileData Profile, RuleData Rule)? GetFirstMatch(List<ProfileData> profiles, Request request)
        {
            var requestUri = request.RequestUriString;
            var method = (RuleMethod)Enum.Parse(typeof(RuleMethod), request.Method, true);

            foreach (var profile in profiles)
            {
                var rule = profile.Rules.FirstOrDefault(x => x.Pattern.IsMatch(requestUri) && x.Method == method);
                if (rule != null)
                    return (profile, rule);
            }

            return null;
        }
    }
}