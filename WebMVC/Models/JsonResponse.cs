
namespace WebMVC.Models
{  
    /// <summary>
    /// Вариант ответа с сайта dadata
    /// </summary>
    public class JsonResponse
    {
        public Suggest[] Suggestions { get; set; }
    }

    public class Suggest{
        public string Value { get; set; }
        public string Unrestricted_value { get; set; }
        public Data Data { get; set; }
    }
    public class Data
    {
        public string Code { get; set; }
        public string Strcode { get; set; }
        public string Name { get; set;}
        public string Country { get; set; }
    }
}
