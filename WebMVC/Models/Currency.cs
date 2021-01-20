using System;

namespace WebMVC.Models
{
   
    /// <summary>
    /// Сущность из json ответа ЦБ
    /// </summary>
    public class Currency
    {
        public string ID { get; set; }
        public string NumCode { get; set; }
        public string CharCode { get; set; }
        public int Nominal { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public double Previos { get; set; }
        public DateTime TimeResponse = DateTime.Now;
    }
}
