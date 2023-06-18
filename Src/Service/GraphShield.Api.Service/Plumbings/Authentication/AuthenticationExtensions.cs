using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace GraphShield.Api.Service.Plumbings.Authentication
{
    public static class AuthenticationExtensions
    {
        /// <summary>
        /// Adds the JWT authentication.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // Get authentication configurations.
            var authConfiguration = new AuthenticationConfiguration();
            configuration.GetSection(nameof(AuthenticationConfiguration)).Bind(authConfiguration);

            // Configure authentication.
            services.AddAuthentication(x => x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = authConfiguration.Authority;
                    options.RequireHttpsMetadata = authConfiguration.RequireHttpsMetadata;
                    options.Audience = authConfiguration.Audience;

                    // Add issuer validation is defined
                    if (!string.IsNullOrEmpty(authConfiguration.Issuer))
                    {
                        options.TokenValidationParameters.ValidIssuer = authConfiguration.Issuer;
                        options.TokenValidationParameters.ValidateIssuer = true;
                    }
                }
            );
        }
    }
}