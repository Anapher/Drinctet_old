using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Drinctet.Core.Cards.Base;
using Drinctet.Harvester.Logging;

namespace Drinctet.Harvester.Csw.PersonalGuy
{
    [HarvesterName("ConversationStartersWorld.PersonalGuyQuestions", Short = "PersonalGuyQuestions")]
    public class CswPersonalGuyQuestionsHarvester : SingleCardTranslatableHarvester<CswPersonalGuyQuestionsHarvester>
    {
        private const string SourceUrl = "https://conversationstartersworld.com/personal-questions-to-ask-a-guy/";
        private static readonly ILog Logger = LogProvider.For<CswPersonalGuyQuestionsHarvester>();

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
            xmlWriter.WriteAttributeString("tags", CardTag.Personal.ToString());
            xmlWriter.WriteAttributeString("source", "ConversationStartersWorld");
        }

        protected override void WriteElements(XmlWriter xmlWriter)
        {
            base.WriteElements(xmlWriter);
            xmlWriter.WriteStartElement("QuestionCard.targetPlayer");
            xmlWriter.WriteStartElement("Player");
            xmlWriter.WriteAttributeString("gender", "m");
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
        }
    }
}