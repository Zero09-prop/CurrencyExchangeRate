using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebMVC.interfaces;
using WebMVC.Models;

namespace WebMVC.mocks
{
    public class Home : IHome
    {
       
        private readonly IDadata _dadata;
        public Home(IDadata idadata)
        {
            _dadata = idadata;
        }
        [HttpPost]
        public async Task<string> NameResponse(string Prefix)
        {
                    string content = await _dadata.DadataResponse(Prefix);
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
    