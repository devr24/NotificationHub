using System;
using System.Threading.Tasks;
using Cloud.Core.NotificationHub.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace Cloud.Core.NotificationHub.Controllers
{
    /// <summary>
    /// Class Notification Controller.
    /// Implements the <see cref="ControllerBase" />
    /// </summary>
    /// <seealso cref="ControllerBase" />
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/notification/[controller]")]
    [Produces("application/json")]
    public class PushController : ControllerBase
    {
        //private readonly IStringLocalizer<PushNotificationController> _localizer;
        private readonly ILogger<PushController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PushController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public PushController(ILogger<PushController> logger/*, IStringLocalizer<PushNotificationController> localizer*/)
        {
            _logger = logger;
            //_localizer = localizer;
        }

        /// <summary>
        /// Create push notification for all subscribers
        /// POST api/v1/values
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        /// <exception cref="InvalidOperationException">Somethings gone wrong!</exception>
        [HttpPost]
        [SwaggerResponse(200, "Example", typeof(string))]
        public async Task<IActionResult> Post([FromBody] CreatePush createPushNotification)
        {
            _logger.LogInformation($"Created new push notification for event {createPushNotification.EventName}");
            var defaultLocale = HttpContext.GetRequestLocale();
            return await Task.FromResult(Ok(defaultLocale));
        }

        /// <summary>
        /// Subscribe to a push notification - implemented using SignalR.
        /// POST api/v1/values
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        /// <exception cref="InvalidOperationException">Somethings gone wrong!</exception>
        [HttpPost("subscribe/{eventname}")]
        [SwaggerResponse(200, "Example")]
        public async Task<IActionResult> Post([FromRoute] string eventname)
        {
            _logger.LogInformation($"Subscribed to event {eventname}");

            return await Task.FromResult(Ok());
        }
    }
}
