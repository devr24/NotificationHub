using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Cloud.Core.NotificationHub.Providers.Sms
{
    /// <summary>
    /// Class ClickatelProvider.
    /// Implements the <see cref="ISmsProvider" />
    /// </summary>
    /// <seealso cref="ISmsProvider" />
    public class ClickatelProvider : ISmsProvider
    {
        private readonly HttpClient _httpClient;
        private readonly AppSettings _settings;

        /// <summary>
        /// Gets or sets the name for the implementor of the INamedInstance interface.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        public ClickatelProvider(AppSettings settings)
        {
            _httpClient = new HttpClient()
            {
                BaseAddress = new System.Uri("https://platform.clickatell.com")
            };

            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "tviw6LTBRl6M7_ITTfzQrg==");
            _settings = settings;
        }

        /// <summary>
        /// Sends the specified message.
        /// </summary>
        /// <param name="sms">The message.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool Send(SmsMessage sms)
        {
            return SendAsync(sms).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Sends the asynchronous.
        /// </summary>
        /// <param name="sms">The message.</param>
        /// <returns>Task.</returns>
        public async Task<bool> SendAsync(SmsMessage sms)
        {
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(new { content = sms.FullContent, to = sms.To.ToArray() });

            var res = await _httpClient.PostAsync("/messages", new StringContent(jsonString, Encoding.UTF8, "application/json"));

            if (res.IsSuccessStatusCode == false)
            {
                throw new InvalidDataException(res.StatusCode.ToString());
            }

            return true;
        }
    }
}
