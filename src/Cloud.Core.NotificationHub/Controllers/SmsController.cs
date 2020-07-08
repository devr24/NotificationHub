using System.Collections.Generic;
using System.Threading.Tasks;
using Cloud.Core.Notification.Events;
using Cloud.Core.NotificationHub.Models;
using Cloud.Core.NotificationHub.Models.DTO;
using Cloud.Core.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Cloud.Core.NotificationHub.Controllers
{
    /// <summary>
    /// Send Sms messages synchronously and asynchronously.
    /// Implements the <see cref="ControllerBase" />
    /// </summary>
    /// <seealso cref="ControllerBase" />
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/notification/[controller]")]
    [Produces("application/json")]
    public class SmsController : ControllerBase
    {
        private readonly NamedInstanceFactory<ISmsProvider> _smsProviders;
        private readonly IReactiveMessenger _messenger;
        private readonly IBlobStorage _blobStorage;
        private readonly AppSettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmsController" /> class.
        /// </summary>
        /// <param name="smsProviders">Configured sms providers.</param>
        /// <param name="messengers">EDA messengers list.</param>
        /// <param name="blobStorage">Blob storage.</param>
        /// <param name="settings">Application settings.</param>
        public SmsController(NamedInstanceFactory<ISmsProvider> smsProviders, NamedInstanceFactory<IReactiveMessenger> messengers, IBlobStorage blobStorage, AppSettings settings)
        {
            _smsProviders = smsProviders;
            _messenger = messengers["notification"];
            _blobStorage = blobStorage;
            _settings = settings;
        }

        /// <summary>Send an sms with attachments sychronously.</summary>
        /// <param name="sms">The sms to send.</param>
        /// <returns>Async Task IActionResult.</returns>
        [HttpPost]
        [SwaggerResponse(200, "Sms sent")]
        [SwaggerResponse(400, "Invalid create sms request", typeof(ApiErrorResult))]
        [RequestFormLimits(MultipartBodyLengthLimit = AppSettings.RequestSizeBytesLimit)] // 5mb limit
        public async Task<IActionResult> CreateSms([FromForm] CreateSms sms)
        {
            // If the model state is invalid (i.e. required fields are missing), then return bad request.
            if (!ModelState.IsValid)
                return BadRequest(new ApiErrorResult(ModelState));

            // Default the provider if not set.
            if (sms.Provider.IsNullOrDefault())
                sms.Provider = _settings.DefaultSmsProvider as SmsProviders?;

            if (!_smsProviders.TryGetValue(sms.Provider.ToString(), out var smsProvider))
            {
                ModelState.AddModelError("Provider", $"{sms.Provider.Value} has no implementation");
                return BadRequest(new ApiErrorResult(ModelState));
            }

            // Validate the attachments, if invalid type or exceeds max allowed size.
            foreach (var attachment in sms.Attachments)
            {
                var fileExt = attachment.GetExtension();
                if (!_settings.AllowedAttachmentTypesList.Contains(fileExt))
                {
                    ModelState.AddModelError("Extension", $"{fileExt} is not in list of valid extensions");
                    return BadRequest(new ApiErrorResult(ModelState));
                }

                if (attachment.Length > AppSettings.IndividualFileSizeBytesLimit)
                {
                    ModelState.AddModelError("MaxFileSizeExceeded", $"{attachment.FileName} size of {attachment.Length} exceeds the max allowed size of {AppSettings.IndividualFileSizeBytesLimit}");
                    return BadRequest(new ApiErrorResult(ModelState));
                }
            }

            // Send sms using the requested provider.
            await smsProvider.SendAsync(sms);
            return Ok();
        }

        /// <summary>Send an sms with attachments sychronously.</summary>
        /// <param name="sms">The sms to queue for sending.</param>
        /// <returns>Async Task IActionResult.</returns>
        [HttpPost("async")]
        [SwaggerResponse(202, "Sms queued for delivery")]
        [SwaggerResponse(400, "Invalid create sms request", typeof(ApiErrorResult))]
        [RequestFormLimits(MultipartBodyLengthLimit = AppSettings.RequestSizeBytesLimit)] // 5mb limit
        public async Task<IActionResult> CreateSmsAsync([FromBody] SmsEvent sms)
        {
            // TODO: REPLACE WITH FLUENT VALIDATION AND CREATE SMS VALIDATOR.
            // If the model state is invalid (i.e. required fields are missing), then return bad request.
            if (!ModelState.IsValid)
                return BadRequest(new ApiErrorResult(ModelState));

            // Default the provider if not set.
            if (sms.SmsProvider.IsNullOrDefault())
                sms.SmsProvider = _settings.DefaultSmsProvider.ToString();

            if (!_smsProviders.TryGetValue(sms.SmsProvider.ToString(), out _))
            {
                ModelState.AddModelError("Provider", $"{sms.SmsProvider} has no implementation");
                return BadRequest(new ApiErrorResult(ModelState));
            }

            foreach (var attId in sms.AttachmentIds)
            {
                var filePath = $"{_settings.AttachmentContainerName}/{attId}";
                if (await _blobStorage.Exists(filePath) == false)
                {
                    ModelState.AddModelError("AttachmentId", $"Attachment with id {attId} was not found");
                    return NotFound(new ApiErrorResult(ModelState));
                }
            }

            // Raise the Sms queue event.
            SmsEvent @event = sms;

            await _messenger.Send(@event, new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("type", "sms") });

            // Creation accepted, sms will be sent via messaging queue.
            return Accepted();
        }
    }
}
