namespace Cloud.Core.NotificationHub.Providers
{
    using Cloud.Core;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface IEmailProvider.
    /// Implements the <see cref="INamedInstance" />
    /// </summary>
    /// <seealso cref="INamedInstance" />
    public interface IEmailProvider : INamedInstance
    {
        /// <summary>Sends the specified email.</summary>
        /// <param name="mail">The email to send.</param>
        void Send(EmailMessage mail);

        /// <summary>Sends the email asynchronously.</summary>
        /// <param name="mail">The email to send.</param>
        /// <returns>Task.</returns>
        Task SendAsync(EmailMessage mail);
    }
}
