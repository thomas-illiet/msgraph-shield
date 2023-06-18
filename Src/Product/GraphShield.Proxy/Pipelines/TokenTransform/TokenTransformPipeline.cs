using GraphShield.Data.Model.Enums;
using GraphShield.Proxy.Exceptions;
using GraphShield.Proxy.Extensions;
using GraphShield.Proxy.Plumbings.Cache;
using GraphShield.Proxy.Plumbings.Data.Interfaces;
using GraphShield.Proxy.Plumbings.Data.Models;
using GraphShield.Proxy.Plumbings.Pipeline;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Titanium.Web.Proxy.EventArguments;

namespace GraphShield.Proxy.Pipelines.TokenTransform
{
    internal class TokenTransformPipeline : DefaultPipeline, ITokenTransformPipeline
    {
        private readonly ICredentialDataService _credentialData;
        private readonly IInternalCache _credentialCache;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenTransformPipeline"/> class.
        /// </summary>
        /// <param name="credentialData">The service for accessing credential data.</param>
        /// <param name="credentialCache">The cache for storing access tokens.</param>
        /// <param name="logger">The logger.</param>
        public TokenTransformPipeline(
            ICredentialDataService credentialData, IInternalCache credentialCache,
            ILogger<TokenTransformPipeline> logger)
        {
            _credentialData = credentialData ?? throw new ArgumentNullException(nameof(credentialData));
            _credentialCache = credentialCache ?? throw new ArgumentNullException(nameof(credentialCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override Task InitializeAsync()
        {
            _logger.LogInformation("Initialize pipeline: {fullName}", GetType().FullName);
            return Task.CompletedTask;
        }

        [Pipeline(PipelineCategory.Graph, 6)]
        public override async Task BeforeRequestAsync(SessionEventArgs session)
        {
            // Get the credential identifier in the current session.
            var credentialId = session.GetData<Guid>("CredentialId");
            if (credentialId == default)
                throw new ArgumentException("Credential identifier is null or empty in the session");

            // Try retrieve token from memory cache.
            if (!_credentialCache.TryGetValue(credentialId, out string? accessToken))
            {
                var credential = await _credentialData.GetAsync(credentialId);
                if (credential == null)
                    throw new ForbiddenException("Request has no active credential");

                var result = await GenerateTokenAsync(credential);
                accessToken = result.AccessToken;

                _credentialCache.Set(
                    credentialId,
                    accessToken,
                    result.ExpiresOn.AddSeconds(-60));
            }

            // Validation of the access token before updating the request.
            if (string.IsNullOrEmpty(accessToken))
                throw new ArgumentException("Access token is null or empty");

            // Update request header with new authorization header.
            session.HttpClient.Request.Headers.RemoveHeader("Authorization");
            session.HttpClient.Request.Headers.AddHeader("Authorization", "bearer " + accessToken);
        }

        /// <summary>
        /// Generates a token for the current request.
        /// </summary>
        /// <param name="credential">The credential entity.</param>
        private async Task<AuthenticationResult> GenerateTokenAsync(CredentialData credential)
        {
            if (credential == null)
                throw new ArgumentException("Unable to find the credentials for the client");

            try
            {
                if (credential.SecretType == CredentialType.Certificate)
                    return await GenerateCertificateTokenAsync(credential);
                else
                    return await GeneratePlainTokenAsync(credential);
            }
            catch (Exception ex)
            {
                throw new InternalServerException(ex, "Unable to generate a token for the Microsoft Graph API");
            }
        }

        /// <summary>
        /// Generates token with certificate credential.
        /// </summary>
        /// <param name="credential">The credential entity.</param>
        private async Task<AuthenticationResult> GenerateCertificateTokenAsync(CredentialData credential)
        {
            throw new NotImplementedException("Token generation with certificate credential is not implemented");
        }

        /// <summary>
        /// Generates token with plain credential.
        /// </summary>
        /// <param name="credential">The credential entity.</param>
        private async Task<AuthenticationResult> GeneratePlainTokenAsync(CredentialData credential)
        {
            var client = ConfidentialClientApplicationBuilder.Create(credential.ClientId.ToString())
                .WithClientSecret(credential.Secret)
                .WithTenantId(credential.TenantId.ToString()).Build();

            var scopes = new List<string> { "https://graph.microsoft.com/.default" };
            return await client.AcquireTokenForClient(scopes).ExecuteAsync();
        }
    }
}