using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using WebMVC.interfaces;
using WebMVC.Models;

namespace WebMVC.mocks
{
    public class Home : IHome
    {
        private readonly IOptions<IDP> _IDPs;
        public Home(IOptions<IDP> IDPs)
        {
            _IDPs = IDPs;
        }
        [HttpPost]
        public async Task<string> NameResponse(string Prefix)
        {
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), _IDPs.Value.UrlDadata))
                {
                    request.Headers.TryAddWithoutValidation("Accept", "application/json");
                    request.Headers.TryAddWithoutValidation("Authorization", "Token "+_IDPs.Value.DadataToken);
                    request.Content = new StringContent("{ \"query\": " + "\"" + Prefix + "\"" + " }");
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                    var response = await httpClient.SendAsync(request);

                    //Считали Json в строку и разбили на сущности
                    //

                    string content = response.Content.ReadAsStringAsync().Result;
                    var parsed = JsonConvert.DeserializeObject<JsonResponse>(content);


                    List<Dadata> list = new List<Dadata>();

                    //словарь будет хранить пары имя валюты - код
                    //временное хранилище
                    //

                    Store.dictValute = new ConcurrentDictionary<string, string>();
                    foreach (var str in parsed.Suggestions)
                    {
                        Store.dictValute[str.Value] = str.Data.Strcode;
                        list.Add(new Dadata() { Name = str.Value, StrCode = str.Data.Strcode });
                    }
                    return JsonConvert.SerializeObject(list);

                }
            }

        }
    }

}
