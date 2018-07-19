using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Drinctet.Harvester.Logging;

namespace Drinctet.Harvester
{
    public abstract class TranslatableHarvester<THarvester> : IHarvester where THarvester : IHarvester
    {
        private static readonly ILog Logger = LogProvider.For<TranslatableHarvester<THarvester>>();
        protected IReadOnlyDictionary<string, Func<string, string>> TranslationTransformings;

        public abstract Task Harvest(XmlWriter xmlWriter, IArtifactDirectory directory, HttpClient httpClient);

        protected Dictionary<string, List<string>> GetTranslations(IArtifactDirectory directory)
        {
            var name = HarvesterNameAttribute.GetShort(typeof(THarvester));

            var translations = new Dictionary<string, List<string>>();
            foreach (var file in directory.GetFiles($"{name}.??.txt"))
            {
                Logger.Info("Read translation from file {filename}", Path.GetFileName(file));

                var language = Regex.Match(file, $@"{name}\.(?<name>(.+))\.txt$").Groups["name"].Value.ToLowerInvariant();
                using (var reader = directory.ReadTextFile(file))
                {
                    Func<string, string> transformationFunc = null;
                    if (TranslationTransformings?.TryGetValue(language, out transformationFunc) == false)
                        Logger.Warn("No translation transformation function found for {lang}", language);

                    translations.Add(language, ReadTranslation(reader, transformationFunc));
                }
            }

            return translations;
        }

        private static List<string> ReadTranslation(StreamReader streamReader, Func<string, string> patchSentence)
        {
            var result = new List<string>();
            var failedPatches = 0;

            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (patchSentence != null)
                {
                    var patchedLine = patchSentence(line);
                    if (patchedLine == null)
                    {
                        failedPatches++;
                        line = "[TODO]" + line;
                    }
                    else line = patchedLine;
                }

                result.Add(line);
            }

            if (failedPatches > 0)
                Logger.Warn("{failedPatches} patches failed", failedPatches);

            return result;
        }

        protected static string PatchTranslationRemoveStart(string line, string defaultBeginning)
        {
            var lineLength = line.Length;
            line = line.Replace(defaultBeginning, null);

            if (line.Length == lineLength)
            {
                Logger.Debug("The translation {line} could not be processed automatically.", line);
                return null;
            }

            return line;
        }
    }
}