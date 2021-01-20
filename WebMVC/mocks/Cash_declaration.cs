using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using WebMVC.interfaces;
using WebMVC.Models;
namespace WebMVC.mocks
{
    public class Cash_declaration : ICash
    {
        public Cash cash;
        private readonly IOptions<IDP> _IDPs;
        private readonly ICbr _cbr;
        public Cash_declaration(IOptions<IDP> IDPs, ICbr cbr)
        {
            _IDPs = IDPs;
            _cbr = cbr;
            cash = Cash.GetInstance();
        }
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

            var parsed = _cbr.CbrWork(_IDPs);

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
                    History.SendSearchHistory(parsed.Valute[key]);
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
            var parsed = _cbr.CbrWork(_IDPs);
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
