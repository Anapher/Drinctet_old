using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Drinctet.Harvester.Logging;

namespace Drinctet.Harvester.Picolo
{
    public class PicoloHarvester : TranslatableHarvester<PicoloHarvester>
    {
        private static readonly ILog Logger = LogProvider.For<PicoloHarvester>();
        private readonly IEnumerable<string[]> _csvData;
        private static readonly string[] DummyNames = { "Vincent", "Nora", "Bursod", "Larny", "Annika", "Sven" };
        private static readonly int[] DummySips = { 71685, 32623, 51052, 16352, 93972 }; //numbers must be high to avoid confusing them with other numbers
        private static readonly Regex SocialMediaRegex = new Regex("\"[^%]+?\"");

        private readonly IReadOnlyDictionary<(string lang, int type), Func<string, string>>
            _typeSpecificTranslationTransformings;

        public PicoloHarvester(IEnumerable<string[]> csvData)
        {
            _csvData = csvData;

            TranslationTransformings = new Dictionary<string, Func<string, string>>
            {
                {"en", PatchString}
            };

            _typeSpecificTranslationTransformings =
                new Dictionary<(string lang, int type), Func<string, string>> { { ("en", 14), s => PatchTranslationRemoveStart(s, "Would you rather ") } };
        }

        public override Task Harvest(XmlWriter xmlWriter, IArtifactDirectory directory, HttpClient httpClient)
        {
            var translations = GetTranslations(directory);

            var counter = 0;

            var (nonDependent, dependent) = GroupRules(_csvData);
            using (var translatableWriter = directory.CreateTextFile("Picolo.translatable.txt"))
            {
                (string text, int type) Extract(string[] data)
                {
                    var text = data[2];
                    var type = int.Parse(data[3]);

                    text = text.Replace(" ", " ");
                    text = text.Replace("  ", " ").Trim();

                    translatableWriter.WriteLine(TransformForTranslation(text, type));
                    translatableWriter.WriteLine();

                    return (text, type);
                }

                IEnumerable<(string lang, string text)> GetTexts(string text, int type)
                {
                    yield return ("de", TransformText(text, type));

                    foreach (var translation in translations)
                    {
                        if (translation.Value.Count == counter - 1)
                        {
                            Logger.Error(
                                "Translation for language {lang} has not enough lines for the site. The site may have received an update.",
                                translation.Key);
                            continue;
                        }

                        if (translation.Value.Count <= counter)
                            continue;

                        var translatedText = translation.Value[counter];
                        if (_typeSpecificTranslationTransformings.TryGetValue((translation.Key, type),
                            out var typeSpecificTransforming))
                            translatedText = typeSpecificTransforming(translatedText);

                        if (translatedText == null)
                            translatedText = "[TODO]" + translation.Value[counter];

                        yield return (translation.Key, translatedText);
                    }
                }

                foreach (var data in nonDependent)
                {
                    var (text, type) = Extract(data);

                    void WriteCard(string cardName)
                    {
                        xmlWriter.WriteStartElement(cardName);
                        foreach (var (lang, translation) in GetTexts(text, type))
                        {
                            xmlWriter.WriteStartElement("Text");
                            xmlWriter.WriteAttributeString("lang", lang);
                            xmlWriter.WriteString(translation);
                            xmlWriter.WriteEndElement();
                        }

                        xmlWriter.WriteEndElement();
                    }

                    switch (type)
                    {
                        case 1: //Core
                            WriteCard("DrinkCard");
                            break;
                        case 4: //NoIdeaLoses
                            WriteCard("NoIdeaLosesCard");
                            break;
                        case 5: //Down
                            WriteCard("DownCard");
                            break;
                        case 14: //Would you rather
                            WriteCard("WyrCard");
                            break;
                        case 15: //SocialMedia
                            WriteCard("SocialMediaCard");
                            break;
                        case 23:
                            WriteCard("GroupGameCard");
                            break;
                        default:
                            Logger.Warn("The card {card} was skipped because no writer is assigned (type: {type}).", text, type);
                            break;
                    }

                    counter++;
                }

                foreach (var keyValuePair in dependent)
                {
                    var (startingCards, endingCards) = keyValuePair.Value;
                    var startingCard = startingCards.Single();

                    var (text, type) = Extract(startingCard);

                    if (!endingCards.Any())
                        throw new InvalidOperationException($"The card {text} has no ending card.");

                    void WriteMultiCard(string cardName)
                    {
                        xmlWriter.WriteStartElement(cardName);
                        foreach (var (lang, translation) in GetTexts(text, type))
                        {
                            xmlWriter.WriteStartElement("Text");
                            xmlWriter.WriteAttributeString("lang", lang);
                            xmlWriter.WriteString(translation);
                            xmlWriter.WriteEndElement();
                        }

                        xmlWriter.WriteStartElement($"{cardName}.followUp");

                        if (endingCards.Count == 1)
                        {
                            var endingCard = endingCards.Single();
                            foreach (var (lang, translation) in GetTexts(endingCard[2], type))
                            {
                                xmlWriter.WriteStartElement("Text");
                                xmlWriter.WriteAttributeString("lang", lang);
                                xmlWriter.WriteString(translation);
                                xmlWriter.WriteEndElement();
                            }
                        }
                        else
                        {
                            foreach (var endingCard in endingCards)
                            {
                                xmlWriter.WriteStartElement("Case");
                                foreach (var (lang, translation) in GetTexts(endingCard[2], type))
                                {
                                    xmlWriter.WriteStartElement("Text");
                                    xmlWriter.WriteAttributeString("lang", lang);
                                    xmlWriter.WriteString(translation);
                                    xmlWriter.WriteEndElement();
                                }
                                xmlWriter.WriteEndElement();
                            }
                        }

                        xmlWriter.WriteEndElement();
                        xmlWriter.WriteEndElement();
                    }

                    switch (type)
                    {
                        case 2: //Virus
                        case 3:
                            WriteMultiCard("VirusCard");
                            break;
                        case 1:
                            WriteMultiCard("DrinkCard");
                            break;
                        case 23:
                            WriteMultiCard("GroupGameCard");
                            break;
                        case 4:
                            WriteMultiCard("NoIdeaLosesCard");
                            break;
                        case 14:
                            WriteMultiCard("WyrCard");
                            break;
                        default:
                            Logger.Warn("The card {card} was skipped because no writer is assigned (type: {type}).", text, type);
                            break;
                    }
                }
            }

            Logger.Info("{counter} {name} texts were created!", counter, "Picolo");
            return Task.CompletedTask;
        }

        private static (IReadOnlyList<string[]> nonDependent,
            IReadOnlyDictionary<string, (List<string[]> beginning, List<string[]> ending)> dependent) GroupRules(
                IEnumerable<string[]> csvData)
        {
            var dependentCards =
                new Dictionary<string, (List<string[]> beginning, List<string[]> ending)>(StringComparer
                    .OrdinalIgnoreCase);
            var nonDependentCards = new List<string[]>();

            foreach (var csvEntry in csvData)
            {
                var rule = csvEntry[0];
                var isBeginning = true;

                if (string.IsNullOrWhiteSpace(rule))
                {
                    rule = csvEntry[1];
                    isBeginning = false;
                }

                if (string.IsNullOrWhiteSpace(rule))
                {
                    nonDependentCards.Add(csvEntry);
                    continue;
                }

                if (!dependentCards.TryGetValue(rule, out var ruleCards))
                    dependentCards.Add(rule, ruleCards = (new List<string[]>(), new List<string[]>()));

                if (isBeginning)
                    ruleCards.beginning.Add(csvEntry);
                else ruleCards.ending.Add(csvEntry);
            }

            return (nonDependentCards, dependentCards);
        }

        public static string TransformText(string text, int cardType)
        {
            if (cardType == 15)
                return string.Join(",", SocialMediaRegex.Matches(text).Select(x => x.Value.Trim('"')));

            text = ReplaceIncremental(text, "%s", "Player");
            text = ReplaceIncremental(text, "$ Schlucke", "sips");
            
            if (cardType == 14)
            {
                text = text.Substring(0, text.IndexOf("?") + 1);

                if (text.StartsWith("Lieber "))
                    text = text.Remove(0, "Lieber ".Length);
                else text = "[TODO:WYR]" + text;
            }
            
            return text;
        }

        public static string TransformForTranslation(string text, int cardType)
        {
            if (cardType == 15)
                return string.Join(",", SocialMediaRegex.Matches(text).Select(x => x.Value.Trim('"')));

            text = ReplaceWithArray(text, "$", DummySips.Select(x => x.ToString()).ToArray());
            text = ReplaceWithArray(text, "%s", DummyNames);

            if (cardType == 14 && text.StartsWith("Lieber "))
            {
                text = text.Remove(0, "Lieber ".Length);
                text = text.Substring(0, text.IndexOf("?") + 1);
                return "Würdest du lieber " + text; //No mike because german language is aware
            }

            return text;
        }

        private static string ReplaceIncremental(string source, string oldValue, string newValue)
        {
            if (source.Split(oldValue).Length > 2)
            {
                var counter = 1;
                while (true)
                {
                    var playerIx = source.IndexOf(oldValue, StringComparison.Ordinal);
                    if (playerIx == -1)
                        break;

                    var splitter = source.Split(oldValue, 2);
                    source = splitter[0] + $"[{newValue}{counter++}]" + splitter[1];
                }

                return source;
            }

            return source.Replace(oldValue, $"[{newValue}]", StringComparison.OrdinalIgnoreCase);
        }

        public static string ReplaceWithArray(string source, string oldValue, IReadOnlyList<string> newValues)
        {
            var counter = 0;
            while (true)
            {
                var playerIx = source.IndexOf(oldValue, StringComparison.Ordinal);
                if (playerIx == -1)
                    break;

                var splitter = source.Split(oldValue, 2);
                source = splitter[0] + newValues[counter++] + splitter[1];
            }

            return source;
        }

        private static string PatchString(string s)
        {
            s = s.Trim();

            //more than one player
            if (s.IndexOf(DummyNames[1], StringComparison.OrdinalIgnoreCase) > -1)
            {
                var counter = 1;
                foreach (var dummyName in DummyNames)
                {
                    s = s.Replace(dummyName, $"[Player{counter++}]", StringComparison.OrdinalIgnoreCase);
                }
            }
            else
            {
                s = s.Replace(DummyNames[0], "[Player]");
            }

            bool TryReplace(string oldValue, string newValue)
            {
                var oldLen = s.Length;
                s = s.Replace(oldValue, newValue);
                return oldLen > s.Length;
            }

            bool TryReplaceSips(string oldValue, string newValue)
            {
                if (TryReplace(oldValue + " sips", newValue))
                    return true;
                if (TryReplace(oldValue + " swigs", newValue))
                    return true;
                if (TryReplace(oldValue + " swallows", newValue))
                    return true;

                return false;
            }

            if (s.IndexOf(DummySips[1].ToString(), StringComparison.Ordinal) > -1) //second sip is also there
            {
                var counter = 1;
                foreach (var dummySip in DummySips.Select(x => x.ToString()))
                {
                    if (s.IndexOf(dummySip, StringComparison.Ordinal) == -1)
                        break;

                    var newVal = $"[sips{counter++}]";
                    if (!TryReplaceSips(dummySip, newVal))
                        s = "[TODO:Sips]" + s.Replace(dummySip, newVal);
                }
            }
            else if(s.IndexOf(DummySips[0].ToString(), StringComparison.Ordinal) > -1)
            {
                if (!TryReplaceSips(DummySips[0].ToString(), "[sips]"))
                    s = "[TODO:Sips]" + s.Replace(DummySips[0].ToString(), "[sips]");
            }

            return s;
        }
    }
}