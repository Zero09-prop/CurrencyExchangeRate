using System.IO;
using Newtonsoft.Json;

namespace WebMVC.Models
{
    public class History
    {
        public static void SendSearchHistory(Currency valute)
        {
            using (var sr = new StreamWriter(@"history_book\example.js", true))
            {
                sr.WriteLine(JsonConvert.SerializeObject(valute));
                sr.Close();
            }
        }
    }   
}
