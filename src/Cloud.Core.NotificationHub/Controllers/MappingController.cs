using System;
using System.Threading.Tasks;
using Cloud.Core.Template.HtmlMapper;
using Cloud.Core.Web;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.Annotations;

namespace Cloud.Core.NotificationHub.Controllers
{
    /// <summary>
    /// [DRAFT] Move to Document Engine API
    /// Mapping controller for mapping content into pdf.
    /// Implements the <see cref="ControllerBase" />
    /// </summary>
    /// <seealso cref="ControllerBase" />
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class MappingController : ControllerBase
    {
        private const string DefaultContentType = "application/octet-stream";
        private const string PdfContentType = "application/pdf";
        private readonly ITemplateMapper _templateMapper;
        private readonly IBlobStorage _fileStorage;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailController"/> class.
        /// </summary>
        /// <param name="templateMapper">Template mapper.</param>
        /// <param name="fileStorage">Blob storage to store created files.</param>
        public MappingController(ITemplateMapper templateMapper, IBlobStorage fileStorage)
        {
            _templateMapper = templateMapper;
            _fileStorage = fileStorage;
        }

        /// <summary>Retrieve template from storage and map the model into the content.</summary>
        /// <param name="id">Id of template to retrieve.</param>
        /// <param name="model">The model to map.</param>
        /// <returns>Template.</returns>
        [HttpPost("{id}/pdf")]
        [SwaggerResponse(404, "Template not found", typeof(ApiErrorResult))]
        [SwaggerResponse(200, "Template", typeof(FileStreamResult))]
        public async Task<IActionResult> GetTemplateAsPdf([FromRoute] Guid id, [FromBody] dynamic model)
        {
            // Get the dynamically passed in model as a JToken.  This is used for the placeholder substitution.
            JToken outer = JToken.Parse(model.ToString());
            var templateResult = await _templateMapper.MapTemplateId(id.ToString(), outer);

            // If unsuccessful, return not found result.
            if (templateResult is HtmlTemplateResult res)
            {
                if (res.TemplateFound == false)
                    return NotFound(res);
            }

            // Otherwise, we generate the PDF stream and return.
            var pdfService = new HtmlToPdf.HtmlToPdfService();
            var pdfStream = pdfService.GetPdfStream(templateResult.TemplateContent);

            // Store the file (somewhere... TBC).
            await _fileStorage.UploadBlob($"documents/{id}.pdf", pdfStream);

            return new FileStreamResult(pdfStream, PdfContentType)
            {
                FileDownloadName = $"{id}.pdf"
            };
        }

    }
}
