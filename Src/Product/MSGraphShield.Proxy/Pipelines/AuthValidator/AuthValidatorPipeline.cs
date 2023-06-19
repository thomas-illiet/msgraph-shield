using MSGraphShield.Proxy.Exceptions;
using MSGraphShield.Proxy.Extensions;
using MSGraphShield.Proxy.Models;
using MSGraphShield.Proxy.Plumbings.Pipeline;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Titanium.Web.Proxy.EventArguments;

namespace MSGraphShield.Proxy.Pipelines.AuthValidator
{
    /// <summary>
    /// Represents the authentication validation pipeline for Graph Shield Proxy.
    /// </summary>
    internal class AuthValidatorPipeline : DefaultPipeline, IAuthValidatorPipeline
    {
        private readonly ClientAuthentication _config;
        private readonly ILogger<AuthValidatorPipeline> _logger;

        private readonly ConfigurationManager<OpenIdConnectConfiguration> _manager;
        private readonly JwtSecurityTokenHandler _handler;
        private readonly TokenValidationParameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthValidatorPipeline"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="logger">The logger.</param>
        public AuthValidatorPipeline(IOptions<ClientAuthentication> config, ILogger<AuthValidatorPipeline> logger)
        {
            _config = config.Value ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (string.IsNullOrEmpty(_config.Authority))
                throw new ArgumentException("The 'Authority' property is null or empty in the configuration.", nameof(_config.Authority));
            if (string.IsNullOrEmpty(_config.ClaimIdentifier))
                throw new ArgumentException("The 'ClaimIdentifier' property is null or empty in the configuration.", nameof(_config.ClaimIdentifier));

            _manager = new ConfigurationManager<OpenIdConnectConfiguration>(
                metadataAddress: $"{_config.Authority}/.well-known/openid-configuration",
                configRetriever: new OpenIdConnectConfigurationRetriever());

            _parameters = new TokenValidationParameters
            {
                ValidIssuers = _config.Validations.ValidIssuers,
                ValidAudiences = _config.Validations.ValidAudiences,
                ValidateAudience = _config.Validations.ValidateAudience,
                ValidateLifetime = !_config.Validations.AllowExpired,
                ValidateIssuerSigningKey = true,
            };

            _handler = new JwtSecurityTokenHandler();
        }

        /// <inheritdoc/>
        public override async Task InitializeAsync()
        {
            _logger.LogInformation("Initializing pipeline: {fullName}", GetType().FullName);

            // Flag which indicates whether or not PII is shown in logs.
            if (Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") == "Development")
                Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

            _logger.LogDebug("Setting up configuration manager to control refreshing on authentication data");
            await _manager.GetConfigurationAsync();

            _logger.LogDebug("Configuration manager initialized successfully");
        }

        /// <inheritdoc/>
        [Pipeline(PipelineCategory.Graph, 3)]
        public override async Task BeforeRequestAsync(SessionEventArgs session)
        {
            var headerValue = session.HttpClient.Request.Headers.GetFirstHeader("Authorization");
            if (headerValue == null)
                throw new UnauthorizedException("No authorization token was found in the request headers.");

            var tokenValues = headerValue.Value.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            if (tokenValues is not ["Bearer", _])
                throw new UnauthorizedException("Invalid authorization header. The header should be in the format 'Bearer {token}'.");

            ClaimsPrincipal user;
            try
            {
                var config = await _manager.GetConfigurationAsync();
                _parameters.IssuerSigningKeys = config.SigningKeys;
                user = _handler.ValidateToken(tokenValues[1], _parameters, out _);
            }
            catch (SecurityTokenException ex)
            {
                throw new UnauthorizedException(ex, "Failed to validate the authorization token.");
            }
            catch (Exception ex)
            {
                throw new InternalServerException(ex, "An error occurred while processing the authorization token.");
            }

            var remoteIdentifierClaim = user.FindFirst(_config.ClaimIdentifier);
            if (remoteIdentifierClaim == null || string.IsNullOrEmpty(remoteIdentifierClaim.Value))
                throw new UnauthorizedException("No remote identifier was found in the authorization token.");

            session.InsertData("RemoteId", remoteIdentifierClaim.Value);
        }
    }
}