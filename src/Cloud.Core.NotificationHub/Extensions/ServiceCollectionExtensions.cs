namespace Cloud.Core.NotificationHub.Extensions
{
    using Cloud.Core;
    using Cloud.Core.NotificationHub.Providers;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Linq;

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
            AddFactoryIfNotAdded<T>(services);
            return services;
        }

        /// <summary>Add the sms provider.</summary>
        /// <typeparam name="T">Type of ISmsProvider.</typeparam>
        /// <param name="services">The service collection to add sms provider to.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddSmsProvider<T>(this IServiceCollection services) where T : class, ISmsProvider
        {
            services.AddSingleton<ISmsProvider, T>();
            AddFactoryIfNotAdded<T>(services);
            return services;
        }

        /// <summary>
        /// Add the generic service factory from Cloud.Core for the IReactiveMessenger and IMessenger type.  This allows multiple named instances of the same instance.
        /// </summary>
        /// <param name="services">Service collection to extend.</param>
        private static void AddFactoryIfNotAdded<T>(IServiceCollection services) where T: INamedInstance
        {
            if (!services.ContainsService(typeof(NamedInstanceFactory<T>)))
            {
                // Service Factory doesn't exist, so we add it to ensure it's always available.
                services.AddSingleton<NamedInstanceFactory<T>>();
            }
        }

        /// <summary>
        /// Search through the service collection for a particular object type.
        /// </summary>
        /// <param name="services">IServiceCollection to check.</param>
        /// <param name="objectTypeToFind">Type of object to find.</param>
        /// <returns>Boolean true if service exists and false if not.</returns>
        public static bool ContainsService(this IServiceCollection services, Type objectTypeToFind)
        {
            return services.Any(x => x.ServiceType == objectTypeToFind);
        }
    }
}
