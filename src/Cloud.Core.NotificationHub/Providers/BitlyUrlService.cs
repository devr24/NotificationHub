using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Cloud.Core.NotificationHub.Providers
{
    /// <summary>Bit.ly url shortener configuration class.</summary>
    public class BitlyConfig
    {
        /// <summary>The bit.ly API key.</summary>
        public string ApiKey { get; set; }
    }

    /// <summary>Bitly shorten result.
    /// Implements the <see cref="IUrlShortenResult" />
    /// </summary>
    /// <seealso cref="IUrlShortenResult" />
    public class BitlyShortenResult : IUrlShortenResult
    {
        /// <summary>Original source link to shorten.</summary>
        public Uri SourceLink { get; set; }

        /// <summary>Shortened version of the link.</summary>
        public Uri ShortLink { get; set; }
    }

    /// <summary>
    /// Bitly url shortener service.
    /// Implements the <see cref="IUrlShortener" />
    /// </summary>
    /// <seealso cref="IUrlShortener" />
    public class BitlyUrlService : IUrlShortener
    {
        private readonly BitlyConfig _config;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="BitlyUrlService"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public BitlyUrlService(BitlyConfig config)
        {
            _config = config;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {_config.ApiKey}");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BitlyUrlService" /> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="httpClient">The http client.</param>
        public BitlyUrlService(BitlyConfig config, HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {_config.ApiKey}");
        }

        /// <summary>Shorten the passed in link.</summary>
        /// <param name="original">Original link to shorten.</param>
        /// <returns>Task ILinkShortenResult with the short link.</returns>
        public async Task<IUrlShortenResult> ShortenLink(Uri original)
        {
            var jsonString = $"{{\"long_url\": \"{original.AbsoluteUri}\"}}";
            var res = await _httpClient.PostAsync("https://api-ssl.bitly.com/v4/shorten", new StringContent(jsonString, Encoding.UTF8, "application/json"));
            var content = await res.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<BitlyResponse>(content);

            return new BitlyShortenResult { 
                ShortLink = new Uri(obj.link),
                SourceLink = original
            };
        }
    }

    internal class BitlyResponse
    {
        public DateTime created_at { get; set; }
        public string id { get; set; }
        public string link { get; set; }
        public string long_url { get; set; }
        public bool archived { get; set; }
    }
}
