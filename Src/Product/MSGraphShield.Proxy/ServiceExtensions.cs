using MSGraphShield.Data;
using MSGraphShield.Data.Shared.Constants;
using MSGraphShield.Data.Shared.DbContexts;
using MSGraphShield.Proxy.Events;
using MSGraphShield.Proxy.Models;
using MSGraphShield.Proxy.Pipelines.AuthValidator;
using MSGraphShield.Proxy.Pipelines.ClientValidator;
using MSGraphShield.Proxy.Pipelines.Diagnostic;
using MSGraphShield.Proxy.Pipelines.HostValidator;
using MSGraphShield.Proxy.Pipelines.ProfileValidator;
using MSGraphShield.Proxy.Pipelines.TokenTransform;
using MSGraphShield.Proxy.Plumbings.Cache;
using MSGraphShield.Proxy.Plumbings.Data;
using MSGraphShield.Proxy.Plumbings.Data.Interfaces;
using MSGraphShield.Proxy.Plumbings.Graph;
using MSGraphShield.Proxy.Plumbings.Pipeline;
using MSGraphShield.Proxy.Validators;
using MSGraphShield.Proxy.Validators.Inputs;
using MassTransit;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace MSGraphShield.Proxy
{
    public static class ServiceExtensions
    {
        public static void AddMSGraphShield(this IServiceCollection services, IConfiguration configuration)
        {
            // Add database and data protection services.
            // For more information: https://learn.microsoft.com/en-us/aspnet/core/security/data-protection
            services.RegisterDbContexts<DataConfigDbContext, DataProtectionDbContext>(configuration);
            services.AddDataProtection()
                .SetApplicationName(DataProtectionConsts.ApplicationName)
                .PersistKeysToDbContext<DataProtectionDbContext>();

            // Register configurations
            services.Configure<ClientAuthentication>(options =>
                configuration.GetSection(nameof(ClientAuthentication)).Bind(options));
            services.Configure<ProxyConfiguration>(options =>
                configuration.GetSection(nameof(ProxyConfiguration)).Bind(options));

            //
            // For more information: https://masstransit.io/
            services.AddMassTransit(configuration);

            //
            services.AddMemoryCache();
            services.TryAddSingleton<IInternalCache, InternalCache>();

            // Enable internal data services to proxy to the database.
            services.AddAutoMapper(Assembly.GetAssembly(typeof(ServiceExtensions)));
            services.TryAddSingleton<ICredentialDataService, CredentialDataService>();
            services.TryAddSingleton<IProfileDataService, ProfileDataService>();
            services.TryAddSingleton<IClientDataService, ClientDataService>();

            // Pipeline registration
            services.AddSingleton<PipelineManagement>();
            services.TryAddSingleton<IDiagnosticPipeline, DiagnosticPipeline>();
            services.TryAddSingleton<IAuthValidatorPipeline, AuthValidatorPipeline>();
            services.TryAddSingleton<IClientValidatorPipeline, ClientValidatorPipeline>();
            services.TryAddSingleton<IHostValidatorPipeline, HostValidatorPipeline>();
            services.TryAddSingleton<IProfileValidatorPipeline, ProfileValidatorPipeline>();
            services.TryAddSingleton<ITokenTransformPipeline, TokenTransformPipeline>();

            services.AddSingleton<GraphRequestParser>();

            //
            services.AddSingleton<ValidatorManagement>();

            //
            services.AddLogging();

            //
            services.AddHostedService<Plumbings.ProxyService>();
        }

        private static void AddMassTransit(this IServiceCollection services, IConfiguration configuration)
        {
            var instanceConfig = new InstanceConfiguration();
            configuration.GetSection(nameof(InstanceConfiguration)).Bind(instanceConfig);
            services.AddMassTransit(x =>
            {
                x.AddConsumer<ClientRegistrationDefinition>();
                x.AddConsumer<ClientActivityDefinition>();

                if (instanceConfig.Type == InstanceConfiguration.ClusterType.Standalone)
                {
                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.ConfigureEndpoints(context);
                    });
                }
                else
                {
                    x.UsingGrpc((context, cfg) =>
                    {
                        cfg.Host(h =>
                        {
                            h.Host = instanceConfig.BusAddress;
                            h.Port = instanceConfig.BusPort;
                            foreach (var busPeer in instanceConfig.BusPeers)
                                h.AddServer(busPeer);
                        });
                        cfg.ConfigureEndpoints(context);
                    });
                }
            });
        }
    }
}
