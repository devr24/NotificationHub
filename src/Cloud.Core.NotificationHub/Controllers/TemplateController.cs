using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloud.Core.Extensions;
using Cloud.Core.NotificationHub.Models;
using Cloud.Core.Template.HtmlMapper;
using Cloud.Core.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.Annotations;

namespace Cloud.Core.NotificationHub.Controllers
{
    /// <summary>
    /// [DRAFT] Move to Document Engine API
    /// Template controller for getting and uploading templates into the system.
    /// Implements the <see cref="ControllerBase" />
    /// </summary>
    /// <seealso cref="ControllerBase" />
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class TemplateController : ControllerBase
    {
        private readonly ITemplateMapper _templateMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailController"/> class.
        /// </summary>
        /// <param name="templateMapper">Template mapper.</param>
        /// <param name="settings">Application settings.</param>
        public TemplateController(ITemplateMapper templateMapper)
        {
            _templateMapper = templateMapper;
        }

        /// <summary>Retrieve template from storage.</summary>
        /// <param name="id">Id of template to retrieve.</param>
        /// <returns>Template.</returns>
        [HttpGet("{id}")]
        [SwaggerResponse(404, "Template not found", typeof(ApiErrorResult))]
        [SwaggerResponse(200, "Template", typeof(FileStreamResult))]
        public async Task<IActionResult> GetTemplate(Guid id)
        {
            var template = await _templateMapper.GetTemplateContent(id.ToString()) as HtmlTemplateResult;

            if (template == null || template.TemplateFound == false)
            {
                return NotFound();
            }
            return Ok(template);
        }

        /// <summary>
        /// Gets all templates.
        /// </summary>
        /// <returns>IActionResult.</returns>
        [HttpGet]
        [SwaggerResponse(200, "Templates", typeof(List<ITemplateResult>))]
        public async Task<IActionResult> GetTemplates()
        {
            //TODO: Implement this.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete a specific template.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>IActionResult.</returns>
        [HttpDelete("{id}")]
        [SwaggerResponse(404, "Template not found", typeof(ApiErrorResult))]
        [SwaggerResponse(200, "Template", typeof(void))]
        public async Task<IActionResult> DeleteTemplate([FromRoute]Guid id)
        {
            var template = await _templateMapper.GetTemplateContent(id.ToString()) as HtmlTemplateResult;

            if (template == null || template.TemplateFound == false)
            {
                return NotFound();
            }
            return Ok(template);
        }

        /// <summary>Add the template content.</summary>
        /// <param name="createTemplate">The template content to upload.</param>
        /// <returns>File template identifier.</returns>
        /// <example>"&lt;html&gt;&lt;body&gt;&lt;h1&gt;{{TITLE}}&lt;/h1&gt;&lt;/body&gt;&lt;/html&gt;"</example>
        [HttpPost(Name = "UploadTemplateV1")]
        [RequestFormLimits(MultipartBodyLengthLimit = AppSettings.IndividualFileSizeBytesLimit)] // 1mb limit
        [SwaggerResponse(400, "Invalid upload template request", typeof(ApiErrorResult))]
        [SwaggerResponse(201, "Template uploaded", typeof(Guid))]
        public async Task<IActionResult> UploadTemplate([FromBody] CreateTemplate createTemplate)
        {
            // If the model state is invalid (i.e. required fields are missing), then return bad request.
            if (createTemplate == null || createTemplate.TemplateContent.IsNullOrEmpty())
            {
                ModelState.AddModelError("Template", "Template content is required");
                return BadRequest(new ApiErrorResult(ModelState));
            }

            var result = await _templateMapper.CreateTemplateContent(createTemplate.TemplateContent);

            Guid.TryParse(result.TemplateId, out var id);

            return CreatedAtAction(nameof(GetTemplate), new { id, version = "1" }, result);
        }

        /// <summary>
        /// Updates a specific template.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="template">The template.</param>
        /// <returns>IActionResult.</returns>
        [HttpPut("{id}")]
        [RequestFormLimits(MultipartBodyLengthLimit = AppSettings.IndividualFileSizeBytesLimit)] // 1mb limit
        [SwaggerResponse(400, "Invalid upload template request", typeof(ApiErrorResult))]
        [SwaggerResponse(200, "Template uploaded", typeof(ITemplateResult))]
        public async Task<IActionResult> UpdateTemplate([FromRoute]Guid id, [FromBody] UpdateTemplate template)
        {
            //TODO: Implement.
            throw new NotImplementedException();
        }
    }
}
