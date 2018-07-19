using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Drinctet.Harvester.Bevil
{
    public class BevilHarvester : IHarvester
    {
        private readonly string _dataJsFile;
        private readonly Regex _objectRegex = new Regex(@"{id:\d+,question:""(?<question>(.*?))"",alclvl:(?<alcLvl>(\d+)),gender:""(?<gender>(.*?))""}");

        public BevilHarvester(string dataJsFile)
        {
            _dataJsFile = dataJsFile;
        }

        public Task Harvest(XmlWriter xmlWriter, IArtifactDirectory directory, HttpClient httpClient)
        {
            ParseDares(xmlWriter);
            ParseTruths(xmlWriter);
            ParseBadLuck(xmlWriter);

            return Task.CompletedTask;
        }
        
        private void ParseTruths(XmlWriter xmlWriter)
        {
            var segment = Regex.Match(_dataJsFile, @"case wahrheit:.*?break;", RegexOptions.Singleline).Value;
            foreach (var dataObj in ParseData(segment))
            {
                WriteData(xmlWriter, dataObj, "QuestionCard", "Truth");
            }
        }

        private void ParseDares(XmlWriter xmlWriter)
        {
            var segment = Regex.Match(_dataJsFile, @"case pfl:.*?break;", RegexOptions.Singleline).Value;
            foreach (var dataObj in ParseData(segment))
            {
                if (dataObj.Question.Contains("#pantomime"))
                    continue;

                WriteData(xmlWriter, dataObj, "TaskCard", "Dare");
            }
        }

        private void ParseBadLuck(XmlWriter xmlWriter)
        {
            var segment = Regex.Match(_dataJsFile, @"case badLuck:.*?break;", RegexOptions.Singleline).Value;
            foreach (var dataObj in ParseData(segment))
            {
                WriteData(xmlWriter, dataObj, "TaskCard", "Dare");
            }
        }

        private void WriteData(XmlWriter xmlWriter, DataObj dataObj, string cardName, string category = null)
        {
            xmlWriter.WriteStartElement(cardName);
            xmlWriter.WriteAttributeString("willPower", dataObj.WillPower.ToString());

            if (category != null)
                xmlWriter.WriteAttributeString("category", category);

            if (dataObj.Gender != null)
            {
                xmlWriter.WriteStartElement("TaskCard.targetPlayer");
                xmlWriter.WriteStartElement("Player");
                xmlWriter.WriteAttributeString("gender", dataObj.Gender);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteStartElement("Text");
            xmlWriter.WriteAttributeString("lang", "de");
            xmlWriter.WriteString(dataObj.Question);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
        }

        private class DataObj
        {
            public string Question { get; set; }
            public int WillPower { get; set; }
            public string Gender { get; set; }
        }

        private IEnumerable<DataObj> ParseData(string segment)
        {
            foreach (Match match in _objectRegex.Matches(segment))
            {
                var question = match.Groups["question"].Value;
                question = question.Replace("#player", "[Player1]");

                var gender = match.Groups["gender"].Value;
                if (!string.IsNullOrEmpty(gender))
                    gender = gender == "male" ? "m" : "f";
                else gender = null;

                yield return new DataObj
                {
                    Question = question,
                    WillPower = int.Parse(match.Groups["alcLvl"].Value) * 2,
                    Gender = gender
                };
            }
        }
    }
}