using System.Collections.Generic;
using System.Linq;
using Cloud.Core.NotificationHub.Models;

namespace Cloud.Core.NotificationHub
{
    /// <summary>Application Settings.</summary>
    public class AppSettings
    {
        private string _allowedTypes;

        /// <summary>Gets the supported cultures.</summary>
        public static string[] SupportedCultures { get; } = { "en" };

        /// <summary>The container name.</summary>
        public const string ContainerName = "attachments";

        /// <summary>The individual file size bytes limit.</summary>
        public const long IndividualFileSizeBytesLimit = 1048576;

        /// <summary>The request size bytes limit.</summary>
        public const long RequestSizeBytesLimit = 5242880;

        /// <summary>Gets the cultures.</summary>
        public string[] Cultures { get { return SupportedCultures; } }

        /// <summary>Attachment container name.</summary>
        public string AttachmentContainerName => ContainerName;

        /// <summary>Gets or sets the monitor frequency seconds.</summary>
        public int MonitorFrequencySeconds { get; set; }

        /// <summary>Gets or sets the default email provider.</summary>
        public EmailProviders DefaultEmailProvider { get; set; }

        /// <summary>Gets or sets the default SMS provider.</summary>
        public SmsProviders DefaultSmsProvider { get; set; }

        /// <summary>Gets the allowed attachment types list.</summary>
        public List<string> AllowedAttachmentTypesList { get; private set; }

        /// <summary>Whether to use Sendgrids own templating functionality or our own.</summary>
        public bool UseSendgridsTemplating { get; set; }

        /// <summary>Gets or sets the allowed file types for attachment.</summary>
        public string AllowedAttachmentTypes {
            get => _allowedTypes;
            set {
                _allowedTypes = value;
                AllowedAttachmentTypesList = value.IsNullOrDefault() ? new List<string>() : value.Split(',').ToList();
            }
        }
    }
}
