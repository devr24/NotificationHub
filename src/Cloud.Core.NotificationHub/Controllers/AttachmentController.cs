using System.Threading.Tasks;
using Cloud.Core.NotificationHub.Models;
using Cloud.Core.NotificationHub.Models.DTO;
using Cloud.Core.NotificationHub.Providers;
using Microsoft.AspNetCore.Mvc;

namespace Cloud.Core.NotificationHub.Controllers
{
    /// <summary>
    /// Class EmailController.
    /// Implements the <see cref="ControllerBase" />
    /// </summary>
    /// <seealso cref="ControllerBase" />
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class AttachmentController : ControllerBase
    {
        private readonly NamedInstanceFactory<IEmailProvider> _emailProviders;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailController"/> class.
        /// </summary>
        /// <param name="emailProviders">The email providers.</param>
        public AttachmentController(NamedInstanceFactory<IEmailProvider> emailProviders)
        {
            _emailProviders = emailProviders;
        }

        // POST: api/email/attachment
        /// <summary>Send an email with attachments.</summary>
        /// <param name="email">The value.</param>
        [HttpPost]
        public async Task<IActionResult> Get([FromBody] CreateEmail email)
        {
            var emailProvider = _emailProviders[email.Provider.ToString()];

            await emailProvider.SendAsync(email);

            return Ok();
        }

        // POST: api/email/attachment
        /// <summary>Add an attachment into the notification hub that can be sent along with notifications.</summary>
        /// <param name="attachement">The attachment to upload.</param>
        [HttpPost]
        public async Task<IActionResult> Post(CreateAttachement attachement)
        {
            var identifier = new System.Guid();

            return Ok(identifier);
        }
    }
}
