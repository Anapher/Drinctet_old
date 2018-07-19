using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Drinctet.Harvester.Logging;

namespace Drinctet.Harvester.Csw.TruthOrDare
{
    [HarvesterName("ConversationStartersWorld.TruthOrDareDare", Short = "TruthOrDareDare")]
    public class CswTruthOrDareDareHarvester : SingleCardTranslatableHarvester<CswTruthOrDareDareHarvester>
    {
        private const string SourceUrl = "https://conversationstartersworld.com/truth-dare-questions/";
        private static readonly ILog Logger = LogProvider.For<CswTruthOrDareDareHarvester>();

        public override string CardName { get; } = "TaskCard";

        protected override async Task<(string language, List<string> texts)> GetOriginalTexts(HttpClient httpClient)
        {
            Logger.Info("Start http request to site {siteUrl}", SourceUrl);

            var response = await httpClient.GetAsync(SourceUrl);
            Logger.Info("Http request completed");
            response.EnsureSuccessStatusCode();

            var source = await response.Content.ReadAsStringAsync();

            var daresHalf = source.Split("id=\"dares\"")[1].Split("<h2>")[0];

            var regex = new Regex(@"<p>(?<task>(.+?))</p>");
            return ("en", regex.Matches(daresHalf).Select(x => x.Groups["task"].Value).ToList());
        }

        protected override void WriteAttributes(XmlWriter xmlWriter, string originalText)
        {
            xmlWriter.WriteAttributeString("category", "Dare");
        }
    }
}