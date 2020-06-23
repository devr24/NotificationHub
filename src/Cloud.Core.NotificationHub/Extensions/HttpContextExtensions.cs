namespace Microsoft.AspNetCore.Http
{
    using Cloud.Core.NotificationHub;
    using System.Linq;

    /// <summary>
    /// Class HttpContextExtensions.
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>Gets the requested locale.</summary>
        /// <param name="context">The context.</param>
        /// <returns>ProviderCultureResult.</returns>
        public static string GetRequestLocale(this HttpContext context)
        {
            var userLangs = context.Request.Headers["Accept-Language"].ToString();
            var firstLang = userLangs.Split(',').FirstOrDefault();
            var defaultLang = string.IsNullOrEmpty(firstLang) ? AppSettings.SupportedCultures.First() : firstLang;
            return defaultLang;
        }
    }
}
