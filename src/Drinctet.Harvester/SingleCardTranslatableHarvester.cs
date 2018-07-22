using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using Drinctet.Harvester.Logging;

namespace Drinctet.Harvester
{
    public abstract class SingleCardTranslatableHarvester<THarvester> : TranslatableHarvester<THarvester>
        where THarvester : IHarvester
    {
        private static readonly ILog Logger = LogProvider.For<SingleCardTranslatableHarvester<THarvester>>();

        public abstract string CardName { get; }

        public override async Task Harvest(XmlWriter xmlWriter, IArtifactDirectory directory, HttpClient httpClient)
        {
            var originalTextsTask = GetOriginalTexts(httpClient);
            var name = HarvesterNameAttribute.GetShort(typeof(THarvester));

            var translations = GetTranslations(directory);

            var (originalLanguage, originalTexts) = await originalTextsTask;
            var counter = 0;

            using (var translatableWriter = directory.CreateTextFile($"{name}.translatable.txt"))
            {
                for (var i = 0; i < originalTexts.Count; i++)
                {
                    var originalText = HtmlUtilities.RemoveTags(originalTexts[i]);

                    translatableWriter.WriteLine(TransformForTranslation(originalText));
                    translatableWriter.WriteLine();

                    xmlWriter.WriteStartElement(CardName);
                    WriteAttributes(xmlWriter, originalText);
                    WriteElements(xmlWriter);

                    IEnumerable<(string lang, string text)> GetTexts()
                    {
                        yield return (originalLanguage, originalText);

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

                            yield return (translation.Key, translation.Value[counter]);
                        }
                    }

                    foreach (var (lang, text) in GetTexts())
                    {
                        xmlWriter.WriteStartElement("Text");
                        xmlWriter.WriteAttributeString("lang", lang);
                        xmlWriter.WriteString(text);
                        xmlWriter.WriteEndElement();
                    }

                    xmlWriter.WriteEndElement();

                    counter++;
                }
            }

            Logger.Info("{counter} {name} texts were created!", counter, name);
        }

        protected abstract Task<(string language, List<string> texts)> GetOriginalTexts(HttpClient httpClient);

        protected virtual string TransformForTranslation(string text) => text;

        protected virtual void WriteElements(XmlWriter xmlWriter)
        {
        }

        protected virtual void WriteAttributes(XmlWriter xmlWriter, string originalText)
        {
        }
    }
}