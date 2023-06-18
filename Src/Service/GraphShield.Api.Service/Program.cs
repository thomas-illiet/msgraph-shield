using GraphShield.Data;
using GraphShield.Data.Configuration.Models;
using GraphShield.Data.Shared.DbContexts;
using Serilog;

namespace GraphShield.Api.Service
{
    /// <summary>
    /// Class that contains the entrypoint for the Reverse Proxy sample app.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Entrypoint of the client.
        /// </summary>
        public static async Task Main(string[] args)
        {
            var configuration = GetConfiguration(args);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            try
            {
                var host = CreateHostBuilder(args).Build();
                await ApplyDbMigrationsAsync(configuration, host);
                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        /// Creates the host builder.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args);

            hostBuilder.ConfigureAppConfiguration((hostContext, configApp) =>
            {
                var env = hostContext.HostingEnvironment;
                configApp.AddJsonFile("serilog.json", optional: true, reloadOnChange: true);
                configApp.AddJsonFile($"serilog.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                if (env.IsDevelopment())
                    configApp.AddUserSecrets<Startup>(true);

                configApp.AddEnvironmentVariables();
                configApp.AddCommandLine(args);
            });

            hostBuilder.ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureKestrel(options => options.AddServerHeader = false);
                webBuilder.UseStartup<Startup>();
            });

            hostBuilder.UseSerilog((hostContext, loggerConfig) =>
            {
                loggerConfig
                    .ReadFrom.Configuration(hostContext.Configuration)
                    .Enrich.WithProperty("ApplicationName", hostContext.HostingEnvironment.ApplicationName);
            });

            return hostBuilder;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static IConfiguration GetConfiguration(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddJsonFile("serilog.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"serilog.{environment}.json", optional: true, reloadOnChange: true);

            configurationBuilder.AddCommandLine(args);
            configurationBuilder.AddEnvironmentVariables();

            return configurationBuilder.Build();
        }

        /// <summary>
        /// Applies the database migrations asynchronous.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="host">The host.</param>
        public static async Task<bool> ApplyDbMigrationsAsync(IConfiguration configuration, IHost host)
        {
            var databaseMigrationsConfiguration = configuration.GetSection(nameof(DatabaseMigrationsConfiguration))
                .Get<DatabaseMigrationsConfiguration>();

            return await DatabaseMigrationHelpers
                .ApplyDbMigrationsAsync<DataConfigDbContext, DataProtectionDbContext>(host, databaseMigrationsConfiguration);
        }
    }
}