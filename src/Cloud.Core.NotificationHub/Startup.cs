using System.Text.Json.Serialization;
using Cloud.Core.Links.Bitly;
using Cloud.Core.Messaging.AzureServiceBus.Models;
using Cloud.Core.Notification.Clickatel;
using Cloud.Core.Notification.Smtp;
using Cloud.Core.Notification.Textlocal;
using Cloud.Core.NotificationHub.HostedServices;
using Cloud.Core.NotificationHub.Providers;
using Cloud.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Cloud.Core.NotificationHub
{
    /// <summary>Class Startup.</summary>
    public class Startup
    {
        private readonly ILogger<Startup> _logger;
        private readonly IConfiguration _configuration;
        private readonly double[] _appVersions = { 1.0 };
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

            // Add instances of the event messenger client.
            services.AddServiceBusSingletonNamed<IReactiveMessenger>("email", _configuration["MessengerInstanceName"], _configuration["TenantId"], _configuration["SubscriptionId"],
                new ReceiverSetup
                {
                    EntityType = EntityType.Topic,
                    EntityName = "notification-hub",
                    EntitySubscriptionName = "email",
                    EntityFilter = new System.Collections.Generic.KeyValuePair<string, string>("EmailFilter", "type LIKE '%email%'"),
                    CreateEntityIfNotExists = true,
                });
            services.AddServiceBusSingletonNamed<IReactiveMessenger>("sms", _configuration["MessengerInstanceName"], _configuration["TenantId"], _configuration["SubscriptionId"],
                new ReceiverSetup
                {
                    EntityType = EntityType.Topic,
                    EntityName = "notification-hub",
                    EntitySubscriptionName = "sms",
                    EntityFilter = new System.Collections.Generic.KeyValuePair<string, string>("SmsFilter", "type LIKE '%sms%'"),
                    CreateEntityIfNotExists = true,
                });
            services.AddServiceBusSingletonNamed<IReactiveMessenger>("push", _configuration["MessengerInstanceName"],_configuration["TenantId"], _configuration["SubscriptionId"],
                new ReceiverSetup
                {
                    EntityType = EntityType.Topic,
                    EntityName = "notification-hub",
                    EntitySubscriptionName = "push",
                    EntityFilter = new System.Collections.Generic.KeyValuePair<string, string>("PushFilter", "type LIKE '%push%'"),
                    CreateEntityIfNotExists = true,
                });
            services.AddServiceBusSingletonNamed<IReactiveMessenger>("notification", _configuration["MessengerInstanceName"], _configuration["TenantId"], _configuration["SubscriptionId"],
                sender: new SenderSetup
                {
                    EntityName = "notification-hub",
                    EntityType = EntityType.Topic,
                    CreateEntityIfNotExists = true
                });

            // Add blob storage instance.
            services.AddBlobStorageSingleton(_configuration["StorageInstanceName"], _configuration["TenantId"], _configuration["SubscriptionId"]);

            // Background monitor service for logging.
            services.AddSingleton(new MonitorService(_appSettings.MonitorFrequencySeconds));

            // Url shortener service.
            services.AddSingleton<IUrlShortener>(new BitlyUrlService(new BitlyConfig { ApiKey = _configuration["BitlyApiKey"] }));

            // Add hosted services to pickup the events.
            services.AddHostedService<EmailService>();
            services.AddHostedService<SmsService>();

            // Add template mapper.
            services.AddHtmlTemplateMapper();

            // Add configs for providers.
            services.AddSingleton(new SmtpConfig { 
                SmtpServer = _configuration["SmtpServer"],
                SmtpPort = _configuration.GetValue<int>("SmtpPort"),
                SmtpUsername = _configuration["SmtpUsername"],
                SmtpPassword = _configuration["SmtpPassword"]
            });
            services.AddSingleton(new ClickatelConfig { ApiAuthorisationKey = _configuration["ClickatelApiKey"] });
            services.AddSingleton(new TextlocalConfig { ApiAuthorisationKey = _configuration["TextlocalApiKey"] });

            // Add email providers.
            services.AddEmailProvider<SmtpProvider>()
                    .AddEmailProvider<DummyEmailProvider>();

            // Add sms providers.
            services.AddSmsProvider<ClickatelProvider>()
                    .AddSmsProvider<TextlocalProvider>()
                    .AddSmsProvider<DummySmsProvider>();

            services.AddHealthChecks();
            services.AddControllers();
            services.AddSwaggerWithVersions(_appVersions, c => c.IncludeXmlComments("Cloud.Core.NotificationHub.xml"), v => {

                var description = new OpenApiInfo {
                    Title = "Notification Hub API",
                    Version = $"v{v}",
                    Description = "Send notifications of various types, synchronously or asynchronously.",
                    Contact = new OpenApiContact
                    {
                        Name = "Robert McCabe",
                        Email = "robert.mccabe@outlook.com"
                    }
                };

                return description;
            });
            services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddMvc().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.IgnoreNullValues = true;
             });
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
