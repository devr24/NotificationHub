namespace Cloud.Core.NotificationHub.Models
{
    /// <summary>Email provider enumeration.</summary>
    public enum EmailProviders
    {
        /// <summary>The SMTP relay provider.</summary>
        Smtp = 1,
        /// <summary>Sendgrid provider.</summary>
        Sendgrid = 2
    }

    /// <summary>Sms provider enumeration.</summary>
    public enum SmsProviders
    {
        /// <summary>Clickatel provider.</summary>
        Clickatel = 1,
        /// <summary>Sendgrid provider.</summary>
        Sendgrid = 2,
        /// <summary>Textlocal provider,</summary>
        Textlocal = 3
    }
}
