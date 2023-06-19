using FluentValidation;
using FluentValidation.AspNetCore;
using MSGraphShield.Api.Service.Plumbings.Authentication;
using MSGraphShield.Api.Service.Plumbings.Data;
using MSGraphShield.Api.Service.Plumbings.Sieve;
using MSGraphShield.Api.Service.Plumbings.Swagger;
using MSGraphShield.Data;
using MSGraphShield.Data.Shared.Constants;
using MSGraphShield.Data.Shared.DbContexts;
using Microsoft.AspNetCore.DataProtection;
using System.Reflection;
using System.Text.Json.Serialization;
using MSGraphShield.Ext.HealthCheck;

namespace MSGraphShield.Api.Service
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            // Add authentication
            services.AddJwtAuthentication(_configuration);

            // Add swagger generator
            services.AddSwagger(_configuration);

            // Add service for controllers.
            var controllers = services.AddControllers();
            controllers.AddJsonOptions(opts => opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            controllers.AddMvcOptions(opts => opts.SuppressAsyncSuffixInActionNames = false);
            services.AddValidatorsFromAssemblyContaining<Startup>();
            services.AddFluentValidationAutoValidation();
            services.AddAutoMapper(Assembly.GetAssembly(typeof(Startup)));
            services.AddSieveService();

            // Add DbContexts
            services.RegisterDbContexts<DataConfigDbContext, DataProtectionDbContext>(_configuration);
            services.AddDataProtection()
                .SetApplicationName(DataProtectionConsts.ApplicationName)
                .PersistKeysToDbContext<DataProtectionDbContext>();

            services.AddHealthChecks(_configuration);

            services.AddTransient<ClientDataService>();
            services.AddTransient<ProfileDataService>();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Checks if the current host environment name is development.
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            // Enables routing capabilities
            app.UseRouting();

            // Add wagger implementation
            app.UseSwaggerUI(_configuration);

            // Enable authentication and authorization
            app.UseAuthentication();
            app.UseAuthorization();

            // Endpoint Configuration
            app.UseEndpoints(endpoints =>
            {
                endpoints.UseHealthChecks();
                endpoints.MapControllers();
            });
        }
    }
}