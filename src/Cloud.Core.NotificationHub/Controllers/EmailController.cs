using System.Threading.Tasks;
using Cloud.Core.NotificationHub.Models;
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
    public class EmailController : ControllerBase
    {
        private readonly NamedInstanceFactory<IEmailProvider> _emailProviders;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailController"/> class.
        /// </summary>
        /// <param name="emailProviders">The email providers.</param>
        public EmailController(NamedInstanceFactory<IEmailProvider> emailProviders)
        {
            _emailProviders = emailProviders;
        }

        // POST: api/Notification
        /// <summary>
        /// Posts the specified value.
        /// </summary>
        /// <param name="email">The value.</param>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateEmail email)
        {
            var emailProvider = _emailProviders[email.Provider.ToString()];

            await emailProvider.SendAsync(email);

            return Ok();
        }
    }
}
