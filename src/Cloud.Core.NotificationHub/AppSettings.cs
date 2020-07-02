using System.Collections.Generic;
using System.Linq;

namespace Cloud.Core.NotificationHub
{
    /// <summary>
    /// Class App Settings.
    /// </summary>
    public class AppSettings
    {
        private string _allowedTypes;

        /// <summary>Gets the supported cultures.</summary>
        /// <value>The supported cultures.</value>
        public static string[] SupportedCultures { get; } = new string[] { "en" };

        public static readonly string ContainerName = "attachments";

        public const long IndividualFileSizeBytesLimit = 1048576;

        public const long RequestSizeBytesLimit = 5242880;

        /// <summary>Gets the cultures.</summary>
        /// <value>The cultures.</value>
        public string[] Cultures { get { return SupportedCultures; } }

        public string AttachmentContainerName => ContainerName;

        /// <summary>Gets or sets the monitor frequency seconds.</summary>
        /// <value>The monitor frequency seconds.</value>
        public int MonitorFrequencySeconds { get; set; }

        /// <summary>Gets or sets the default email provider.</summary>
        /// <value>The default email provider.</value>
        public string DefaultEmailProvider { get; set; }

        /// <summary>Gets or sets the default SMS provider.</summary>
        /// <value>The default SMS provider.</value>
        public string DefaultSmsProvider { get; set; }

        public List<string> AllowedAttachmentTypesList { get; private set; }

        /// <summary>
        /// Gets or sets the allowed file types for attachment
        /// </summary>
        public string AllowedAttachmentTypes {
            get => _allowedTypes;
            set {
                _allowedTypes = value;
                AllowedAttachmentTypesList = value.IsNullOrDefault() ? new List<string>() : value.Split(',').ToList();
            }
        }
    }
}
