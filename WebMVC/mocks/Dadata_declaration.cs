
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using WebMVC.interfaces;
using WebMVC.Models;

namespace WebMVC.mocks
{
    public class Dadata_declaration : IDadata
    {
        private readonly IOptions<IDP> _IDPs;

        public Dadata_declaration(IOptions<IDP> IDPs)
        {
            _IDPs = IDPs;
        }
        public async Task<string> DadataResponse(string Prefix)
        {
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), _IDPs.Value.UrlDadata))
                {
                    request.Headers.TryAddWithoutValidation("Accept", "application/json");
                    request.Headers.TryAddWithoutValidation("Authorization", "Token " + _IDPs.Value.DadataToken);
                    request.Content = new StringContent("{ \"query\": " + "\"" + Prefix + "\"" + " }");
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                    var response = await httpClient.SendAsync(request);
                    return response.Content.ReadAsStringAsync().Result;
                }
            }
        }
    }
}
