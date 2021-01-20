using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace WebMVC.Models
{
    public class IDP
    {
        public string UrlCbr { get; set; }
        public string UrlDadata { get; set; }
        public  string DadataToken { get; set; }
        public  string BotToken { get; set; }
        public  string BotName { get; set; }
        public  string BotUrl { get; set; }

        /*public IDP()
        {
            if(CountExempler < 1)
                CountExempler++;
            
        }

        private static int CountExempler = 0;
        private static IDP idp;

        public static IDP getInstance()
        {
            if(idp == null)
                return new IDP();
            return idp;
        }*/
    }
}
