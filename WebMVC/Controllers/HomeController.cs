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

namespace WebMVC.Controllers
{
    public class HomeController : Controller
    {
        //Кеш сервера, хранящий информацию о запрошенных валютах в виде [Код валюты - Currency]
        //

        public static Cash cash = new Cash();

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

        public static ConcurrentDictionary<string, string> dictValute;

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
            using(var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://suggestions.dadata.ru/suggestions/api/4_1/rs/suggest/currency"))
                {
                    request.Headers.TryAddWithoutValidation("Accept", "application/json");
                    request.Headers.TryAddWithoutValidation("Authorization", "Token 94dabe1e8342c21fdd9622be29514d4f0f99bbd8");
                    request.Content = new StringContent("{ \"query\": " + "\"" + Prefix + "\""+" }");
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                    var response = await httpClient.SendAsync(request);

                    //Считали Json в строку и разбили на сущности
                    //

                    string content = response.Content.ReadAsStringAsync().Result;
                    var parsed = JsonConvert.DeserializeObject<JsonResponse>(content);


                    List<Dadata> list = new List<Dadata>();

                    //словарь будет хранить пары имя валюты - код
                    //временное хранилище
                    //

                    dictValute =  new ConcurrentDictionary<string, string>(); 
                    foreach (var str in parsed.Suggestions)
                    {
                        dictValute[str.Value] = str.Data.Strcode;
                        list.Add(new Dadata(){Name=str.Value,StrCode = str.Data.Strcode});
                    }
                    return JsonConvert.SerializeObject(list);
                }
            }
        }

        /// <summary>
        /// Метод для работы с кэшем принимает код валюты и ссылку на флаг проверки наличия валюты
        /// </summary>
        /// <param name="ValuteCode"></param>
        /// <param name="isOk"></param>
        /// <returns>Возвращает словарь код валюты - курс</returns>
       
        private ConcurrentDictionary<string,double> CashWork(string ValuteCode,ref bool isOk)
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

            wc.DownloadFile(url, SavePath + "example.js");
            string json;

            //считываем файл
            //

            using (var sr = new StreamReader(SavePath + "example.js"))
            {
                json = sr.ReadToEnd();
            }
            
            //удаляем
            //

            System.IO.File.Delete(SavePath + "example.js");

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
            
            if (Prefix == null || !dictValute.ContainsKey(Prefix))
                return View("ErrorCurrency");

            //флаг, говорящий о том найдена ли запрошенная валюта
            //

            bool isOk = false;

            //получаем код валюты
            //

            string ValuteCode = dictValute[Prefix];

            //По коду обращаемся к кэшу
            //

            ConcurrentDictionary<string, double> dictionary = CashWork(ValuteCode, ref isOk);

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
            //Загружаем файл с валютами с сайта ЦБ
            //

            wc.DownloadFile(url, SavePath + "example.js");
            string json;

            //Считываем данные
            //

            using (var sr = new StreamReader(SavePath + "example.js"))
            {
                json = sr.ReadToEnd();
            }

            //Удаляем загруженный файл
            //

            System.IO.File.Delete(SavePath + "example.js");

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
            return View("Index",cash);
        }
        
        
        /// <summary>
        /// Метод для принятия сообщений от бота
        /// </summary>
        /// <param name="update"></param>
        /// <returns>Возращает запрошенный курс валют или ошибку</returns>
        
        [Route("api/bot")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] Update update)
        {
            //Инициализировали клиент
            //

            TelegramBotClient client = await Bot.GetBotClientAsync();
            if (update == null)
            {
                return NotFound("Error message");
            }

            //Взяли id сообщения и чата
            //

            var chatId = update.Message.Chat.Id;
            var messageId = update.Message.MessageId;

            //Вызвали функцию для работы с сайтом Dadata, для того чтобы она установила значения в словарь
            //dictValute. В последствии строка empty нам не понадобится
            //
            string empty = NameResponse(update.Message.Text).Result;
            string ValuteCode ="";

            //пробегаемя по всем ключам,которые могли выпасть при обращению к Dadata,
            //как правило это один ключ. Если отправленное боту сообщение содержит нечеткое выражение,
            // например "доллар", то в таком случае dictValute содержит несколько валют со словом доллар
            // но по условию задачи на вход пойдет либо код валюты,либо её полное имя

            foreach (var key in dictValute.Keys)
            {
                ValuteCode = dictValute[key];
            }

            //Проверка на наличие ошибок
            //

            bool isOk = false;

            //Обращаемя к кешу,а затем и к сайту ЦБ
            //

            ConcurrentDictionary<string, double> dictionary = CashWork(ValuteCode, ref isOk);

            //Если ошибка,то отправляем пользователю сообщение
            //

            if (!isOk)
            {
                await client.SendTextMessageAsync(chatId, "Ваша валюта не представлена на сайте ЦБ",
                    replyToMessageId: messageId);
                return Ok();
            }

            //В противном случае словарь dictionary содержит один экземпляр валюты
            //
            string Name = "";
            double Value = 0.0;
            foreach (var element in dictionary.Keys)
            {
                Name = element;
                Value = dictionary[element];
            }

            await client.SendTextMessageAsync(chatId, $"Курс вашей валюты {Name} равен {Value}",
                replyToMessageId: messageId);
            return Ok("its good and okey");
        }
    }
}
