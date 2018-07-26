using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Drinctet.Harvester.Logging;

namespace Drinctet.Harvester.Csw.NeverHaveIEver
{
    [HarvesterName("ConversationStartersWorld.NeverHaveIEver", Short = "NeverHaveIEver")]
    public class CswNeverHaveIEverHarvester : SingleCardTranslatableHarvester<CswNeverHaveIEverHarvester>
    {
        private const string SourceUrl = "https://conversationstartersworld.com/never-have-i-ever-questions/";
        private static readonly ILog Logger = LogProvider.For<CswNeverHaveIEverHarvester>();

        public CswNeverHaveIEverHarvester()
        {
            TranslationTransformings = new Dictionary<string, Func<string, string>>{{"de", s => PatchTranslationRemoveStart(s, "Ich habe noch nie ")}};
        }

        public override string CardName { get; } = "NeverEverCard";
        public override int SourceId { get; } = (int)SourceIds.CswNeverHaveIEverHarvester;

        protected override async Task<(string language, List<string> texts)> GetOriginalTexts(HttpClient httpClient)
        {
            Logger.Info("Start http request to site {siteUrl}", SourceUrl);

            var response = await httpClient.GetAsync(SourceUrl);
            Logger.Info("Http request completed");
            response.EnsureSuccessStatusCode();

            var source = await response.Content.ReadAsStringAsync();
            var regex = new Regex(@"<p>&#8230; (?<neverEver>(.+?))</p>");
            return ("en", regex.Matches(source).Select(x => x.Groups["neverEver"].Value).ToList());
        }

        protected override string TransformForTranslation(string text)
        {
            return "Never have I ever " + text;
        }
    }
}