using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Drinctet.Core.Cards.Base;
using Drinctet.Core.Logging;
using Drinctet.Core.Utilities;

namespace Drinctet.Core.Parsing
{
    public class TextContentParser
    {
        private static readonly ILog Logger = LogProvider.For<TextContentParser>();

        private bool? _isConditionalElement;

        public TextContentParser()
        {
            Result = new List<TextElement>();
        }

        public List<TextElement> Result { get; }

        public bool AddElement(XmlReader xmlReader)
        {
            switch (xmlReader.Name)
            {
                case "Case":
                    if (_isConditionalElement == false)
                    {
                        Logger.Error("Invalid 'Case' element found in content. The content must consist of either 'Case' or 'Text' elements only");
                        return false;
                    }

                    _isConditionalElement = true;
                    Result.Add(ParseTextElement(xmlReader));

                    return true;
                case "Text":
                    if (_isConditionalElement == true)
                    {
                        Logger.Error("Invalid 'Text' element found in content. The content must consist of either 'Case' or 'Text' elements only");
                        return false;
                    }
                    else if (_isConditionalElement == null)
                        Result.Add(new TextElement {Translations = new Dictionary<CultureInfo, string>(), Weight = 1});

                    _isConditionalElement = false;
                    ((IDictionary<CultureInfo, string>) Result[0].Translations).Add(ParseTranslation(xmlReader)
                        .ToKeyValuePair());

                    return true;
            }

            return false;
        }

        protected TextElement ParseTextElement(XmlReader reader)
        {
            var translationsDict = new Dictionary<CultureInfo, string>();
            var textElement = new TextElement {Translations = translationsDict};
            IDictionary<CultureInfo, string> translations = translationsDict;

            switch (reader.Name)
            {
                case "Case":
                    var weightAttribute = reader.GetAttribute("weight");
                    if (weightAttribute != null)
                    {
                        textElement.Weight = double.Parse(weightAttribute, CultureInfo.InvariantCulture);
                    }

                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            if (reader.Name == "Text")
                                translations.Add(ParseTranslation(reader).ToKeyValuePair());
                            else
                                break;
                        }
                        else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Case")
                            break;
                    }

                    break;
                case "Text":
                    do
                    {
                        translations.Add(ParseTranslation(reader).ToKeyValuePair());
                    } while (reader.Read() && reader.Name == "Text");

                    break;
            }

            return textElement;
        }

        protected (CultureInfo, string) ParseTranslation(XmlReader reader)
        {
            var language = reader.GetAttribute("lang");
            if (language == null)
                throw new ArgumentException("lang attribute is not available");

            var culture = CultureInfo.GetCultureInfo(language);
            var content = reader.ReadElementContentAsString();
            return (culture, content);
        }
    }
}