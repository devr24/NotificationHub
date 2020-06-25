using System.Threading.Tasks;
using Cloud.Core.NotificationHub.Models.DTO;
using Cloud.Core.NotificationHub.Providers;
using Microsoft.AspNetCore.Mvc;

namespace Cloud.Core.NotificationHub.Controllers
{
    /// <summary>
    /// Class SmsController.
    /// Implements the <see cref="ControllerBase" />
    /// </summary>
    /// <seealso cref="ControllerBase" />
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class SmsController : ControllerBase
    {
        private readonly NamedInstanceFactory<ISmsProvider> _smsProviders;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmsController"/> class.
        /// </summary>
        /// <param name="smsProviders"></param>
        public SmsController(NamedInstanceFactory<ISmsProvider> smsProviders)
        {
            _smsProviders = smsProviders;
        }

        // POST: api/Notification
        /// <summary>
        /// Posts the specified value.
        /// </summary>
        /// <param name="sms">The value.</param>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateSms sms)
        { 
            var smsProvider = _smsProviders[sms.Provider.ToString()];

            await smsProvider.SendAsync(sms);

            return Ok();
        }
    }
}
