using Microsoft.Extensions.Options;

namespace WebMVC.Models
{
    public class IDP
    {
        private IOptions<IDP> _idp;
        public IDP(IOptions<IDP> idp)
        {
            _idp = idp;
        }
        public string UrlCbr { get; set; }
        public string UrlDadata { get; set; }
        public  string DadataToken { get; set; }
        public  string BotToken { get; set; }
        public  string BotName { get; set; }
        public  string BotUrl { get; set; }
    }
}
