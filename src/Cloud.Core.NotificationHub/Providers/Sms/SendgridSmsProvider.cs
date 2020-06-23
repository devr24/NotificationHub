using Cloud.Core.NotificationHub.Models;
using System.Threading.Tasks;

namespace Cloud.Core.NotificationHub.Providers.Sms
{
    /// <summary>
    /// Class SendgridSmsProvider.
    /// Implements the <see cref="ISmsProvider" />
    /// </summary>
    /// <seealso cref="ISmsProvider" />
    public class SendgridSmsProvider : ISmsProvider
    {
        /// <summary>
        /// Gets or sets the name for the implementor of the INamedInstance interface.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Sends the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Send(SmsMessage message)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Sends the asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task SendAsync(SmsMessage message)
        {
            throw new System.NotImplementedException();
        }
    }
}
