using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Drinctet.Harvester.Logging;

namespace Drinctet.Harvester.Csw.DeepTalk
{
    [HarvesterName("ConversationStartersWorld.DeepTalkQuestions", Short = "DeepTalkQuestions")]
    public class CswDeepTalkQuestionsHarvester : SingleCardTranslatableHarvester<CswDeepTalkQuestionsHarvester>
    {
        private const string SourceUrl = "https://conversationstartersworld.com/deep-conversation-topics/";
        private static readonly ILog Logger = LogProvider.For<CswDeepTalkQuestionsHarvester>();

        public override string CardName { get; } = "QuestionCard";

        protected override async Task<(string language, List<string> texts)> GetOriginalTexts(HttpClient httpClient)
        {
            Logger.Info("Start http request to site {siteUrl}", SourceUrl);

            var response = await httpClient.GetAsync(SourceUrl);
            Logger.Info("Http request completed");
            response.EnsureSuccessStatusCode();

            var source = await response.Content.ReadAsStringAsync();
            var regex = new Regex(@"<p>(?<question>(.+?))\?</p>");
            return ("en", regex.Matches(source).Select(x => x.Groups["question"].Value + "?").ToList());
        }

        protected override void WriteAttributes(XmlWriter xmlWriter, string originalText)
        {
            xmlWriter.WriteAttributeString("category", "DeepTalk");
        }
    }
}