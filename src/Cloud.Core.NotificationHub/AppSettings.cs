using System.Collections.Generic;

namespace Cloud.Core.NotificationHub
{
    /// <summary>
    /// Class App Settings.
    /// </summary>
    public class AppSettings
    {
        private string[] _allowedAttachmentTypes;

        /// <summary>Gets the supported cultures.</summary>
        /// <value>The supported cultures.</value>
        public static string[] SupportedCultures { get; } = new string[] { "en" };

        /// <summary>Gets the cultures.</summary>
        /// <value>The cultures.</value>
        public string[] Cultures { get { return SupportedCultures; } }

        /// <summary>Gets or sets the monitor frequency seconds.</summary>
        /// <value>The monitor frequency seconds.</value>
        public int MonitorFrequencySeconds { get; set; }

        /// <summary>Gets or sets the default email provider.</summary>
        /// <value>The default email provider.</value>
        public string DefaultEmailProvider { get; set; }

        /// <summary>Gets or sets the default SMS provider.</summary>
        /// <value>The default SMS provider.</value>
        public string DefaultSmsProvider { get; set; }

        /// <summary>
        /// Gets or sets the allowed file types for attachment
        /// </summary>
        public string[] AllowedAttachmentTypes { 
            get => _allowedAttachmentTypes == null ? new string[] { } : _allowedAttachmentTypes; 
            set => _allowedAttachmentTypes = value;
        }
    }
}
