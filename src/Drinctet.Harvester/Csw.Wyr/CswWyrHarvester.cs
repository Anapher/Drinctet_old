using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Drinctet.Harvester.Logging;

namespace Drinctet.Harvester.Csw.Wyr
{
    [HarvesterName("ConversationStartersWorld.Wyr")]
    public class CswWyrHarvester : SingleCardTranslatableHarvester<CswWyrHarvester>
    {
        private const string SourceUrl = "https://conversationstartersworld.com/would-you-rather-questions/";
        private static readonly ILog Logger = LogProvider.For<CswWyrHarvester>();

        public CswWyrHarvester()
        {
            TranslationTransformings = new Dictionary<string, Func<string, string>>
            {
                {"de", s => PatchTranslationRemoveStart(s, "Mike, würdest du lieber ")}
            };
        }

        public override string CardName { get; } = "WyrCard";
        public override int SourceId { get; } = (int)SourceIds.CswWyrHarvester;

        protected override async Task<(string language, List<string> texts)> GetOriginalTexts(HttpClient httpClient)
        {
            Logger.Info("Start http request to site {siteUrl}", SourceUrl);

            var response = await httpClient.GetAsync(SourceUrl);
            Logger.Info("Http request completed");
            response.EnsureSuccessStatusCode();

            var source = await response.Content.ReadAsStringAsync();
            var regex = new Regex(@"<p>Would you rather (?<text>([\w \?]+?))<\/p>");
            return ("en", regex.Matches(source).Select(x => x.Groups["text"].Value).ToList());
        }

        protected override string TransformForTranslation(string text) => "Mike, would you rather " + text;
    }
}