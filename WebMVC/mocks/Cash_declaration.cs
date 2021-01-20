using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Net;
using Microsoft.Extensions.Options;
using WebMVC.interfaces;
using WebMVC.Models;

namespace WebMVC.mocks
{
    public class Cash_declaration : ICash
    {
        public Cash cash;
        private readonly IOptions<IDP> _IDPs;
        public Cash_declaration(IOptions<IDP> IDPs)
        {
            _IDPs = IDPs;
            cash = Cash.GetInstance();
        }

        WebClient wc = new WebClient();
        public ConcurrentDictionary<string, double> CashWork(string ValuteCode, ref bool isOk)
        {
            //Проверка наличия в кеше валюты и подходит ли она по времени
            //

            if (cash.CashValues.ContainsKey(ValuteCode) &&
                DateTime.Now.Hour - cash.CashValues[ValuteCode].TimeResponse.Hour < 1)
            {

                //Валюта найдена возвращаем пару имя-курс
                //
                isOk = true;
                ConcurrentDictionary<string, double> dct = new ConcurrentDictionary<string, double>();
                dct[cash.CashValues[ValuteCode].Name] = cash.CashValues[ValuteCode].Value;
                return dct;
            }

            //Если валюты в кеше нет, то скачиваем файл со всеми валютами с сайта ЦБ
            //

            string json = wc.DownloadString(_IDPs.Value.UrlCbr);

            //разбиваем на сущности
            //

            var parsed = JsonConvert.DeserializeObject<CbrResponse>(json);

            //и пробегаясь по каждой сущности, проверяем совпадает ли её код с запрошенным кодом
            //
            foreach (var key in parsed.Valute.Keys)
            {
                if (key == ValuteCode)
                {
                    //если да, то добавляем её в кеш
                    //

                    isOk = true;
                    cash.CashValues[key] = parsed.Valute[key];
                    break;
                }
            }

            //если валюты нет, то ничего не возвращаем
            //

            if (!isOk)
            {
                return null;
            }

            //в противном случае просто возвращаем добавленную валюту прямо из кэша
            //

            ConcurrentDictionary<string, double> dct2 = new ConcurrentDictionary<string, double>
            {
                [cash.CashValues[ValuteCode].Name] = cash.CashValues[ValuteCode].Value
            };
            return dct2;
        }

        public void CashClean()
        {
            cash.CashValues.Clear();
        }
        
        public void CashNew()
        {
            //Загружаем файл с валютами с сайта ЦБ
            //
            string json = wc.DownloadString(_IDPs.Value.UrlCbr);

            //Превращаем текст в сущности
            //

            var parsed = JsonConvert.DeserializeObject<CbrResponse>(json);

            //Создаём временное хранилище
            //
            ConcurrentDictionary<string, Currency> temp = new ConcurrentDictionary<string, Currency>();

            //пробегаясь по всем данным из кеша,добавляем те же данные но обновленные во временное хранилище
            //

            foreach (var key in cash.CashValues.Keys)
            {
                temp[key] = parsed.Valute[key];
            }

            //Переопределяем кеш

            cash.CashValues = temp;
        }
    }
}
