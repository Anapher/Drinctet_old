using System.IO;
using System.Xml;

namespace Drinctet.Core.Tests
{
    public static class XmlUtils
    {
        private static readonly XmlReaderSettings XmlReaderSettings =
            new XmlReaderSettings {ConformanceLevel = ConformanceLevel.Fragment};

        public static XmlReader CreateReader(string s) => XmlReader.Create(new StringReader(s), XmlReaderSettings);
    }
}