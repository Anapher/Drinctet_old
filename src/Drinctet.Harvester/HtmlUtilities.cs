using System.Net;
using HtmlAgilityPack;

namespace Drinctet.Harvester
{
    public static class HtmlUtilities
    {
        public static string RemoveTags(string source)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(source);
            return WebUtility.HtmlDecode(htmlDoc.DocumentNode.InnerText);
        }
    }
}