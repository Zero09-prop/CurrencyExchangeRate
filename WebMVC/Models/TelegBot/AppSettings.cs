namespace WebMVC.Models.TelegBot
{
    public static class AppSettings
    {
        /// <summary>
        /// Настройки приложения бота. Его сайт расположения, имя и токен управления
        /// </summary>
        public static string Url { get; set; } = @"https://408c8e8d0cc2.ngrok.io/{0}";
        public static string Name { get; set; } = "Hop_hipBot";
        public static string Key { get; set; } = "1549046386:AAHJtCMVaCT-8O3D_P8VLxw6EKAr4P9JfSU";
    }
}
