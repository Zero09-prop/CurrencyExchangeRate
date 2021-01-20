
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebMVC.Models;

namespace WebMVC.HelperHtm
{
    public static class TableHelper
    {
        public static HtmlString CreateTable(this IHtmlHelper html, Cash cash)
        {
            string result = "<table class=\"table table-hover\"><thead>" +
                            "<tr><th>Название валюты</th><th>Курс валюты</th></tr></thead><tbody>";
            foreach (Currency item in cash.CashValues.Values)
            {
                result = $"{result}<tr></th><td>{item.Name}</td><td>{item.Value}</td></tr>";
            }
            result = $"{result}</tbody></table>";
            return new HtmlString(result);
        }
    }
}
