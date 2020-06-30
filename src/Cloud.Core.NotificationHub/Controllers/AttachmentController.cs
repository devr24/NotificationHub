using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Cloud.Core.NotificationHub.Models.DTO;
using Cloud.Core.NotificationHub.Providers;
using Cloud.Core.Web;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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
        private readonly IBlobStorage _blobStorage;
        private readonly AppSettings _settings;

        private readonly string ContainerName = "attachments";

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailController"/> class.
        /// </summary>
        /// <param name="emailProviders">The email providers.</param>
        /// <param name="blobStorage"></param>
        /// <param name="settings"></param>
        public AttachmentController(NamedInstanceFactory<IEmailProvider> emailProviders, IBlobStorage blobStorage, AppSettings settings)
        {
            _emailProviders = emailProviders;
            _blobStorage = blobStorage;
            _settings = settings;
        }

        /// <summary>Retrieve attachment from storage.</summary>
        /// <param name="id">Id to attachment.</param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(404, "Attachment not found", typeof(ApiErrorResult))]
        [SwaggerResponse(200, "Attachment", typeof(FileStreamResult))]
        public async Task<IActionResult> GetAttachment([FromRoute]Guid id)
        {
            const string mimeType = "application/octet-stream";
            var filePath = $"{ContainerName}/{id}";

            // Return not found result if the blob does not exist.
            var exists = await _blobStorage.Exists(filePath);
            if (!exists)
            {
                return NotFound();
            }

            // Fetch the blob metadata and return the filestream result with the blob.
            var blobData = await _blobStorage.GetBlob(filePath, true);
            var fileName = blobData.Metadata["Name"];

            return new FileStreamResult(await _blobStorage.DownloadBlob(filePath), mimeType) { FileDownloadName = fileName };
        }

        /// <summary>Add an attachment into the notification hub that can be sent along with notifications.</summary>
        /// <param name="createAttachment">The attachment to upload.</param>
        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 5242880)] // 5mb limit
        [SwaggerResponse(400, "Bad request", typeof(ApiErrorResult))]
        [SwaggerResponse(201, "Attachment uploaded", typeof(Guid))]
        public async Task<IActionResult> UploadAttachment([FromForm] CreateAttachment createAttachment)
        {
            // If the model state is invalid (i.e. required fields are missing), then return bad request.
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiErrorResult(ModelState));
            }

            var fileExt = Path.GetExtension(createAttachment.File.FileName).Replace(".", "");
            var fileName = Path.GetFileNameWithoutExtension(createAttachment.File.FileName);
            
            // If the extension for the file is not on the allowed list, return bad request.
            if (!_settings.AllowedAttachmentTypes.ContainsEquivalent(fileExt))
            {
                ModelState.AddModelError("Extension", $"{fileExt} is not in list of valid extensions");
                return BadRequest(new ApiErrorResult(ModelState));
            }
            
            var identifier = Guid.NewGuid();
            var filePath = $"{ContainerName}/{identifier}";

            // Upload to storage.
            using (var uploadStream = createAttachment.File.OpenReadStream())
            {
                await _blobStorage.UploadBlob(filePath, uploadStream, new Dictionary<string, string> {
                    { "Name", fileName }
                });
                uploadStream.Dispose();
            }

            // Return the created response.
            return CreatedAtAction(nameof(GetAttachment), new
            {
                id = identifier,
                version = "1"
            });
        }
    }
}
