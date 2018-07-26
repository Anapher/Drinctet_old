using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Drinctet.Core.Cards.Base;
using Drinctet.Harvester.Logging;

namespace Drinctet.Harvester.Csw.PersonalGirl
{
    [HarvesterName("ConversationStartersWorld.PersonalGirlQuestions", Short = "PersonalGirlQuestions")]
    public class CswPersonalGirlQuestionsHarvester : SingleCardTranslatableHarvester<CswPersonalGirlQuestionsHarvester>
    {
        private const string SourceUrl = "https://conversationstartersworld.com/personal-questions-to-ask-a-girl/";
        private static readonly ILog Logger = LogProvider.For<CswPersonalGirlQuestionsHarvester>();

        public override string CardName { get; } = "QuestionCard";
        public override int SourceId { get; } = (int)SourceIds.CswPersonalGirlQuestionsHarvester;

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

        protected override void WriteElements(XmlWriter xmlWriter)
        {
            base.WriteElements(xmlWriter);
            xmlWriter.WriteStartElement("QuestionCard.targetPlayer");
            xmlWriter.WriteStartElement("Player");
            xmlWriter.WriteAttributeString("gender", "f");
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
        }
    }
}