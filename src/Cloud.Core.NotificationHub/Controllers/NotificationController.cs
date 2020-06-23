using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace Cloud.Core.NotificationHub.Controllers
{
    /// <summary>
    /// Class Notification Controller.
    /// Implements the <see cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    /// </summary>
    /// <seealso cref="ControllerBase" />
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class NotificationController : ControllerBase
    {
        private readonly IStringLocalizer<NotificationController> _localizer;
        private readonly ILogger<NotificationController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="localizer">The localizer.</param>
        public NotificationController(ILogger<NotificationController> logger, IStringLocalizer<NotificationController> localizer)
        {
            _logger = logger;
            _localizer = localizer;
        }

        /// <summary>
        /// Create notification
        /// POST api/v1/values
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        /// <exception cref="InvalidOperationException">Somethings gone wrong!</exception>
        [HttpPost]
        [SwaggerResponse(200, "Example get result for v1", typeof(IEnumerable<string>))]
        public IEnumerable<string> Post()
        {
            _logger.LogInformation("Get all method called!");
            var defaultLocale = HttpContext.GetRequestLocale();

            return new[] { string.Format(_localizer["Example1"], "1"), _localizer["Example2"] };
        }
    }
}
