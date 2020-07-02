using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cloud.Core.NotificationHub.Models.DTO;
using Cloud.Core.NotificationHub.Models.Events;
using Cloud.Core.NotificationHub.Providers;
using Cloud.Core.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Cloud.Core.NotificationHub.Controllers
{
    /// <summary>
    /// Class SmsController.
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
        /// <param name="smsProviders">The SMS providers.</param>
        /// <param name="messengers">The messengers.</param>
        /// <param name="blobStorage">The BLOB storage.</param>
        /// <param name="settings">The settings.</param>
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
            // TODO: REPLACE WITH FLUENT VALIDATION AND CREATE SMS VALIDATOR.
            // If the model state is invalid (i.e. required fields are missing), then return bad request.
            if (!ModelState.IsValid)
            {
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
            var smsProvider = _smsProviders[sms.Provider.ToString()];
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
        public async Task<IActionResult> CreateSmsAsync([FromForm] CreateSms sms)
        {
            // TODO: REPLACE WITH FLUENT VALIDATION AND CREATE SMS VALIDATOR.
            // If the model state is invalid (i.e. required fields are missing), then return bad request.
            if (!ModelState.IsValid)
            {
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

            // Store each of the files in storage.
            var attachmentIds = new List<Guid>();
            foreach (var attachment in sms.Attachments)
            {
                var id = Guid.NewGuid();
                var filePath = $"{_settings.AttachmentContainerName}/{id}";

                // Upload to storage.
                using var uploadStream = attachment.OpenReadStream();
                await _blobStorage.UploadBlob(filePath, uploadStream, new Dictionary<string, string> {
                    { "name", attachment.FileName },
                    { "id", id.ToString() }
                });
                uploadStream.Dispose();
                attachmentIds.Add(id);
            }

            // Raise the Sms queue event.
            SmsEvent @event = sms;
            @event.AttachmentIds = attachmentIds;
            await _messenger.Send(@event, new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("type", "sms") });

            // Creation accepted, sms will be sent via messaging queue.
            return Accepted();
        }
    }
}
