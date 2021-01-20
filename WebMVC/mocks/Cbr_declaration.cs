using System.Net;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WebMVC.interfaces;
using WebMVC.Models;

namespace WebMVC.mocks
{
    public class Cbr_declaration : ICbr
    {
        private WebClient wc = new WebClient();
        public CbrResponse CbrWork(IOptions<IDP> _IDPs)
        {
            string json = wc.DownloadString(_IDPs.Value.UrlCbr);

            //разбиваем на сущности
            //
            return JsonConvert.DeserializeObject<CbrResponse>(json);
        }
    }
}
