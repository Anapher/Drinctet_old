using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Drinctet.Core.Cards.Base;
using Drinctet.Harvester.Logging;

namespace Drinctet.Harvester.Csw.TruthOrDare
{
    [HarvesterName("ConversationStartersWorld.TruthOrDareTruth", Short = "TruthOrDareTruth")]
    public class CswTruthOrDareTruthHarvester : SingleCardTranslatableHarvester<CswTruthOrDareTruthHarvester>
    {
        private const string SourceUrl = "https://conversationstartersworld.com/truth-dare-questions/";
        private static readonly ILog Logger = LogProvider.For<CswTruthOrDareTruthHarvester>();

        public override string CardName { get; } = "QuestionCard";

        protected override async Task<(string language, List<string> texts)> GetOriginalTexts(HttpClient httpClient)
        {
            Logger.Info("Start http request to site {siteUrl}", SourceUrl);

            var response = await httpClient.GetAsync(SourceUrl);
            Logger.Info("Http request completed");
            response.EnsureSuccessStatusCode();

            var source = await response.Content.ReadAsStringAsync();

            var truthHalf = source.Split("id=\"dares\"")[0].Split("id=\"questions\"")[1];

            var regex = new Regex(@"<p>(?<question>(.+?))</p>");
            return ("en", regex.Matches(truthHalf).Select(x => x.Groups["question"].Value).ToList());
        }

        protected override void WriteAttributes(XmlWriter xmlWriter, string originalText)
        {
            base.WriteAttributes(xmlWriter, originalText);
            xmlWriter.WriteAttributeString("source", "ConversationStartersWorld");
            xmlWriter.WriteAttributeString("willPower", "6");
        }
    }
}