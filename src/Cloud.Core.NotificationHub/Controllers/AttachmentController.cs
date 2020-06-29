using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Cloud.Core.NotificationHub.Models.DTO;
using Cloud.Core.NotificationHub.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

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
        private readonly IConfiguration _configuration;

        private readonly string containerName = "attachments";

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailController"/> class.
        /// </summary>
        /// <param name="emailProviders">The email providers.</param>
        /// <param name="blobStorage"></param>
        /// <param name="configuration"></param>
        public AttachmentController(NamedInstanceFactory<IEmailProvider> emailProviders, IBlobStorage blobStorage, IConfiguration configuration)
        {
            _emailProviders = emailProviders;
            _blobStorage = blobStorage;
            _configuration = configuration;
        }

        // POST: api/attachment
        /// <summary>Send an email with attachments.</summary>
        /// <param name="email">The value.</param>
        //[HttpGet]
        //public async Task<IActionResult> Get([FromBody] CreateEmail email)
        //{
        //    var emailProvider = _emailProviders[email.Provider.ToString()];

        //    await emailProvider.SendAsync(email);

        //    return Ok();
        //}

        // POST: api/attachment
        /// <summary>Add an attachment into the notification hub that can be sent along with notifications.</summary>
        /// <param name="attachment">The attachment to upload.</param>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateAttachment attachment)
        {
            var extension = Path.GetExtension(attachment.Content.FileName);

            List<string> allowedExtensions = _configuration.GetValue<List<string>>("AllowedAttachmentTypes");

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest();
            }


            var identifier = Guid.NewGuid();

            var filePath = $"{containerName}/{identifier}";

            using (var uploadStream = attachment.ContentStream)
            {
                await _blobStorage.UploadBlob(filePath, uploadStream);
                uploadStream.Dispose();
            }

            return Ok(identifier);
        }
    }
}
