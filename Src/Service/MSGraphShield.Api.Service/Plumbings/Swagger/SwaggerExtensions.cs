using MSGraphShield.Api.Service.Models.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;

namespace MSGraphShield.Api.Service.Plumbings.Swagger
{
    /// <summary>
    /// Contains extension methods for configuring Swagger in the service.
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Adds Swagger generator and related configurations to the service.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The configuration.</param>
        public static void AddSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            // Get swagger configurations.
            var swaggerConfiguration = new SwaggerConfiguration();
            configuration.GetSection(nameof(SwaggerConfiguration)).Bind(swaggerConfiguration);

            services.AddApiVersioning(setup =>
            {
                setup.DefaultApiVersion = new ApiVersion(1, 0);
                setup.AssumeDefaultVersionWhenUnspecified = true;
                setup.ReportApiVersions = true;
            });

            services.AddVersionedApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = true;
            });

            services.AddSwaggerGen(c =>
            {
                // Define API documentation
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Graph Shield Management API",
                    Description = "Microsoft Graph Proxy with Advanced Authorization Systems.",
                    Contact = new OpenApiContact
                    {
                        Name = "Thomas ILLIET",
                        Email = "contact@thomas-illiet.fr",
                        Url = new Uri("https://github.com/thomas-illiet/")
                    }
                });

                // Define OAuth2 configuration
                c.AddSecurityDefinition("Oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Description = "Standard authorisation using the Oauth2 scheme.",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Scheme = "Bearer",
                    BearerFormat = "JWT",

                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = swaggerConfiguration.AuthorizationUrl,
                            TokenUrl = swaggerConfiguration.TokenUrl,
                            Scopes = swaggerConfiguration.Scopes,
                        }
                    }
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Oauth2"
                            }
                        },
                        new string[] {}
                    }
                });

                c.EnableAnnotations();
                c.TagActionsBy(x =>
                {
                    if (x.GroupName != null)
                        return new[] { x.GroupName };
                    if (x.ActionDescriptor is ControllerActionDescriptor descriptor)
                        return new[] { descriptor.ControllerName };
                    throw new InvalidOperationException("Unable to determine tag for api endpoint");
                });
                c.DocInclusionPredicate((_, __) => true);

                // Describe all parameters, regardless of how they appear in code, in camelCase.
                c.DescribeAllParametersInCamelCase();

                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
            });
        }

        /// <summary>
        /// Configures and registers the Swagger middleware.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="configuration">The configuration.</param>
        public static void UseSwaggerUI(this IApplicationBuilder app, IConfiguration configuration)
        {
            // Get swagger configurations.
            var swaggerConfiguration = new SwaggerConfiguration();
            configuration.GetSection(nameof(SwaggerConfiguration)).Bind(swaggerConfiguration);

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "/docs/{documentname}/swagger.json";
            });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                // Global configuration.
                c.EnableFilter();
                c.DocExpansion(DocExpansion.None);
                c.DisplayRequestDuration();
                c.RoutePrefix = "docs";

                // Set OAuth configuration.
                c.OAuthClientId(swaggerConfiguration.ClientId);
                c.OAuthScopes(swaggerConfiguration.Scopes?.Keys.ToArray());
                c.OAuthUsePkce();

                // Add swagger endpoint.
                c.SwaggerEndpoint($"{swaggerConfiguration.Prefix}/docs/v1/swagger.json", "Graph Shield - V1");
            });
        }
    }
}