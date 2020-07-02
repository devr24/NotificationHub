namespace Cloud.Core.NotificationHub.Extensions
{
    using Cloud.Core.NotificationHub.Providers;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>Class ServiceCollection extensions.</summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>Add the email provider.</summary>
        /// <typeparam name="T">Type of IEmailProvider.</typeparam>
        /// <param name="services">The service collection to add email provider to.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddEmailProvider<T>(this IServiceCollection services) where T: class, IEmailProvider
        {
            services.AddSingleton<IEmailProvider, T>();
            services.AddFactoryIfNotAdded<IEmailProvider>();
            return services;
        }

        /// <summary>Add the sms provider.</summary>
        /// <typeparam name="T">Type of ISmsProvider.</typeparam>
        /// <param name="services">The service collection to add sms provider to.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddSmsProvider<T>(this IServiceCollection services) where T : class, ISmsProvider
        {
            services.AddSingleton<ISmsProvider, T>();
            services.AddFactoryIfNotAdded<ISmsProvider>();
            return services;
        }
    }
}
