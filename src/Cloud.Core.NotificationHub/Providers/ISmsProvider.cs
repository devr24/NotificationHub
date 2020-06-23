namespace Cloud.Core.NotificationHub.Providers
{
    using Cloud.Core;
    using Cloud.Core.NotificationHub.Models;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface ISmsProvider.
    /// Implements the <see cref="INamedInstance" />
    /// </summary>
    /// <seealso cref="INamedInstance" />
    public interface ISmsProvider : INamedInstance
    {
        /// <summary>Sends the specified message.</summary>
        /// <param name="message">The message to send.</param>
        void Send(SmsMessage message);

        /// <summary>Sends the message asynchronously.</summary>
        /// <param name="message">The message to send.</param>
        /// <returns>Task.</returns>
        Task SendAsync(SmsMessage message);
    }
}
