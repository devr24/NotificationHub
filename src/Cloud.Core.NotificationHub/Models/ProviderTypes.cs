namespace Cloud.Core.NotificationHub.Models
{
    /// <summary>Email provider enumeration.</summary>
    public enum EmailProviders
    {
        /// <summary>The SMTP relay provider.</summary>
        SmtpProvider = 1,
        /// <summary>Sendgrid provider.</summary>
        SendgridProvider = 2,
        /// <summary>Dummy email provider for testing.</summary>
        DummyEmailProvider = 3
    }

    /// <summary>Sms provider enumeration.</summary>
    public enum SmsProviders
    {
        /// <summary>Clickatel provider.</summary>
        ClickatelProvider = 1,
        /// <summary>Sendgrid provider.</summary>
        SendgridProvider = 2,
        /// <summary>Textlocal provider.</summary>
        TextlocalProvider = 3,
        /// <summary>Dummy sms provider for testing.</summary>
        DummySmsProvider = 4
    }
}
