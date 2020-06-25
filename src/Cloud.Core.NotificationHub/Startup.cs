using Cloud.Core.Messaging.AzureServiceBus.Models;
using Cloud.Core.NotificationHub.HostedServices;
using Cloud.Core.NotificationHub.Extensions;
using Cloud.Core.NotificationHub.Providers.Email;
using Cloud.Core.NotificationHub.Providers.Sms;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using blobConfig = Cloud.Core.Storage.AzureBlobStorage.Config;
using sbConfig = Cloud.Core.Messaging.AzureServiceBus.Config;

namespace Cloud.Core.NotificationHub
{
    /// <summary>Class Startup.</summary>
    public class Startup
    {
        private readonly ILogger<Startup> _logger;
        private readonly IConfiguration _configuration;
        private readonly double[] _appVersions = { 1.0, 2.0 };
        private AppSettings _appSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logger">The logger.</param>
        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>This method gets called by the runtime. Use this method to add services to the dependency root.</summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Setup application settings (options).
            _appSettings = _configuration.BindBaseSection<AppSettings>();
            services.AddSingleton(_appSettings);

            // Add two instances of the service bus client.
            services.AddServiceBusSingletonNamed<IReactiveMessenger>("email", new sbConfig.ConnectionConfig
            {
                ConnectionString = _configuration["serviceBusConnection"],
                Receiver = new ReceiverSetup
                {
                    EntityType = EntityType.Topic,
                    EntityName = "notification-hub",
                    EntitySubscriptionName = "email",
                    CreateEntityIfNotExists = true,
                }
            });
            services.AddServiceBusSingletonNamed<IReactiveMessenger>("sms", new sbConfig.ConnectionConfig
            {
                ConnectionString = _configuration["serviceBusConnection"],
                Receiver = new ReceiverSetup
                {
                    EntityType = EntityType.Topic,
                    EntityName = "notification-hub",
                    EntitySubscriptionName = "sms",
                    CreateEntityIfNotExists = true,
                }
            });

            // Add blob storage instance.
            services.AddBlobStorageSingleton(new blobConfig.ConnectionConfig
            {
                ConnectionString = _configuration["storageConnection"],
                CreateFolderIfNotExists = true
            });

            services.AddSingleton(new SmtpConfig { });

            services.AddSingleton<MonitorService>();
            services.AddHostedService<EmailService>();
            services.AddHostedService<SmsService>();

            //var rpo = Models.SmsProviders.GetProviders();

            // Add email providers.
            services.AddEmailProvider<SmtpProvider>()
                    .AddEmailProvider<SendgridProvider>()
                    .AddEmailProvider<DummyProvider>();

            // Add sms providers.
            services.AddSmsProvider<ClickatelProvider>()
                    .AddSmsProvider<SendgridSmsProvider>()
                    .AddSmsProvider<TextlocalProvider>();

            // Add Mvc 
            services.AddHealthChecks();
            services.AddControllers();
            services.AddSwaggerWithVersions(_appVersions, c => c.IncludeXmlComments("Cloud.Core.NotificationHub.xml"));
            services.AddVersionedApiExplorer(options => {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
            services.AddLocalization(o => o.ResourcesPath = "Resources");
            services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
        }

        /// <summary>Configures the specified application.</summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseUnhandledExceptionMiddleware();
            app.UseSwaggerWithVersion(_appVersions);
            app.UseLocalization(_appSettings.Cultures);
            app.UseHttpsRedirection();
            app.UseCors();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());

            // Log startup finished.
            _logger.LogInformation("Cloud.Core.NotificationHub web API startup finished.");
        }
    }
}
