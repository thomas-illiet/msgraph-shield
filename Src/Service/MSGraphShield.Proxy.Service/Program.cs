using Serilog;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MSGraphShield.Proxy.Service
{
    public class Program
    {
        public static void Main(string[] args)
            => CreateHostBuilder(args).Build().Run();

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            // Initialize host builder.
            var builder = Host.CreateDefaultBuilder(args);

            // Add serilog implementation.
            builder.UseSerilog((hostingContext, services, loggerConfiguration) => loggerConfiguration.ReadFrom
                .Configuration(hostingContext.Configuration)
                
                .Enrich.FromLogContext());

            // Configure the application configuration.
            builder.ConfigureAppConfiguration((hostContext, config) =>
            {
                // Retrieve the name of the environment.
                var aspnetcore = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                var dotnetcore = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
                var environmentName = string.IsNullOrEmpty(aspnetcore) ? dotnetcore : aspnetcore;
                if (string.IsNullOrEmpty(environmentName))
                    environmentName = "Production";

                // Define the source configuration.
                config.Sources.Clear();
                config.SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!);
                config.AddJsonFile("appsettings.json", optional: false);
                config.AddJsonFile($"appsettings.{environmentName}.json", optional: true);
                config.AddEnvironmentVariables();
                config.AddCommandLine(args);
            });

            builder.ConfigureServices((hostContext, services) =>
            {
                services.AddMSGraphShield(hostContext.Configuration);
            });

            // Sets the host lifetime.
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                builder.UseWindowsService();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                builder.UseSystemd();

            return builder;
        }
    }
}