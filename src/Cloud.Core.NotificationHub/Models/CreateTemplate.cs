namespace Cloud.Core.NotificationHub.Models
{
    /// <summary>
    /// Create Html Template.
    /// </summary>
    public class CreateTemplate
    {
        /// <summary>Html content of the template.</summary>
        /// <example>&lt;html&gt;&lt;body&gt;&lt;h1&gt;{{TITLE}}&lt;/h1&gt;&lt;/body&gt;&lt;/html&gt;</example>
        public string TemplateContent { get; set; }
    }
}
