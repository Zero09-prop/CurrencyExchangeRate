using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebMVC.interfaces;
using WebMVC.Models;

namespace WebMVC.mocks
{
    public class Home : IHome
    {
        [HttpPost]
        public async Task<string> NameResponse(string Prefix)
        {
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://suggestions.dadata.ru/suggestions/api/4_1/rs/suggest/currency"))
                {
                    request.Headers.TryAddWithoutValidation("Accept", "application/json");
                    request.Headers.TryAddWithoutValidation("Authorization", "Token 94dabe1e8342c21fdd9622be29514d4f0f99bbd8");
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
