using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using WebMVC.Models;
using WebMVC.Models.TelegBot;
using MediaTypeHeaderValue = System.Net.Http.Headers.MediaTypeHeaderValue;
using WebMVC.interfaces;

namespace WebMVC.Controllers
{

    public sealed class HomeController : Controller
    {
        private readonly ICash icash;
        private readonly IHome ihome;
        //Кеш сервера, хранящий информацию о запрошенных валютах в виде [Код валюты - Currency]
        //

        public Cash cash;

        //Клиент для скачивания файла с курсами валют с сайта центробанка
        //

        WebClient wc = new WebClient();

        //Сайт центробанка с которого берётся текущий курс
        //

        private string url = "https://www.cbr-xml-daily.ru/daily_json.js";

        //Путь по которому будет храниться текстовый файл с валютой
        //

        private string SavePath = @"wwwroot\";

        //контейнер для хранения промежуточных результатов с Dadata.ru
        //

        //public ConcurrentDictionary<string, string> dictValute;

        public HomeController(ICash _iCash, IHome ihome)
        {
            cash = Cash.GetInstance();
            icash = _iCash;
            this.ihome = ihome;
        }

        public IActionResult Index()
        {
            return View(cash);
        }

        /// <summary>
        /// Метод для для отправления запросов к сайту dadata.ru и создания autocomplete
        /// </summary>
        /// <param name="Prefix"></param>
        /// <returns>
        /// Возвращает Json с именами валют и их кодами 
        /// </returns>


        [HttpPost]
        public async Task<string> NameResponse(string Prefix)
        {
            return await ihome.NameResponse(Prefix);
        }

        /// <summary>
        /// Метод для работы с кэшем принимает код валюты и ссылку на флаг проверки наличия валюты
        /// </summary>
        /// <param name="ValuteCode"></param>
        /// <param name="isOk"></param>
        /// <returns>Возвращает словарь код валюты - курс</returns>
        /// <summary>
        /// Метод для работы с данными из формы
        /// на вход получает имя валюты,сделанное при помощи autocomplete
        /// </summary>
        /// <param name="Prefix"></param>
        /// <returns>Выдаёт представление с курсом(DataResponse) или с ошибкой(ErrorCurrency)</returns>

        [HttpPost]
        public IActionResult DataResponse(string Prefix)
        {
            //Проверяем чтобы не была отправлена пустая строка и введенный текст был действительно именем валюты
            //а не набором символов
            //так как при обращение к сайту Dadata у нас dictValute может заполниться только значениями с сайта

            if (Prefix == null || !Store.dictValute.ContainsKey(Prefix))
                return View("ErrorCurrency");

            //флаг, говорящий о том найдена ли запрошенная валюта
            //

            bool isOk = false;

            //получаем код валюты
            //

            string ValuteCode = Store.dictValute[Prefix];

            //По коду обращаемся к кэшу
            //

            ConcurrentDictionary<string, double> dictionary = icash.CashWork(ValuteCode, ref isOk);

            //Если валюта не найдена возвращаем ошибку
            //

            if (!isOk)
                return View("ErrorCurrency");

            //В противном случае достаём имя и курс из dictionary
            //так как мы ищем по коду, то в dictionary всегда будет единственное значение
            //

            foreach (var element in dictionary.Keys)
            {
                ViewBag.CurName = element;
                ViewBag.CurCourse = dictionary[element];
            }
            return View();

        }


        /// <summary>
        /// Метод для очистки кеша
        /// </summary>
        /// <returns>Возвращает страницу с поиском</returns>

        public IActionResult CashClean()
        {
            cash.CashValues.Clear();
            return View("Index", cash);
        }

        /// <summary>
        /// Метод для обновления всех данных из кеша
        /// </summary>
        /// <returns>Возвращает страницу с поиском</returns>

        public IActionResult CashNew()
        {
            icash.CashNew();
            return View("Index", cash);
        }


        /// <summary>
        /// Метод для принятия сообщений от бота
        /// </summary>
        /// <param name="update"></param>
        /// <returns>Возращает запрошенный курс валют или ошибку</returns>
    }
}
