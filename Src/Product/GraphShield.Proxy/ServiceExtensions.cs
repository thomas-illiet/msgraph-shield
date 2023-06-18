using GraphShield.Data;
using GraphShield.Data.Shared.Constants;
using GraphShield.Data.Shared.DbContexts;
using GraphShield.Proxy.Events;
using GraphShield.Proxy.Models;
using GraphShield.Proxy.Pipelines.AuthValidator;
using GraphShield.Proxy.Pipelines.ClientValidator;
using GraphShield.Proxy.Pipelines.Diagnostic;
using GraphShield.Proxy.Pipelines.HostValidator;
using GraphShield.Proxy.Pipelines.ProfileValidator;
using GraphShield.Proxy.Pipelines.TokenTransform;
using GraphShield.Proxy.Plumbings.Cache;
using GraphShield.Proxy.Plumbings.Data;
using GraphShield.Proxy.Plumbings.Data.Interfaces;
using GraphShield.Proxy.Plumbings.Graph;
using GraphShield.Proxy.Plumbings.Pipeline;
using GraphShield.Proxy.Validators;
using GraphShield.Proxy.Validators.Inputs;
using MassTransit;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace GraphShield.Proxy
{
    public static class ServiceExtensions
    {
        public static void AddGraphShield(this IServiceCollection services, IConfiguration configuration)
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
