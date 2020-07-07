using System;
using System.Reflection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Cloud.Core.NotificationHub
{
    /// <summary> Main application class.</summary>
    public static class Program
    {
        /// <summary>Defines the entry point of the application.</summary>
        public static void Main(string[] args)
        {
            try
            {
                // Build the web host.
                var host = CreateWebHostBuilder(args).Build();

                // Run the web host.
                host.Run();
            }
            catch (Exception e)
            {
                // Catch startup errors.
                Console.WriteLine($"Problem occured during startup of {Assembly.GetExecutingAssembly().GetName().Name}");
                Console.WriteLine(e);
                throw;
            }
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(config => {
                    // Import default configurations (env vars, command line args, appSettings.json etc).
                    config.UseDefaultConfigs();

                    config.AddKeyVaultSecrets(config.GetValue<string>("KeyVaultInstanceName"),
                        "TenantId",
                        "SubscriptionId",
                        "InstrumentationKey",
                        "serviceBusConnection",
                        "storageConnection",
                        "SmtpServer",
                        "SmtpPort",
                        "SmtpUsername",
                        "SmtpPassword",
                        "BitlyApiKey",
                        "TextlocalApiKey",
                        "ClickatelApiKey");
                })
                .ConfigureLogging((context, logging) => {
                    
                    // Add logging configuration and loggers.
                    logging.AddConfiguration(context.Configuration)
                        .AddConsole()
                        .AddDebug()
                        .AddAppInsightsLogger();
                })
                .UseStartup<Startup>();
    }
}
