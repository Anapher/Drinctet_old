using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Drinctet.Core.Fragments;
using Drinctet.Core.Parsing;

namespace Drinctet
{
    class Program
    {
        static void Main(string[] args)
        {
            var file = new FileInfo(@"..\..\..\..\..\cards\wyr.xml");
            var settings = new XmlReaderSettings {ConformanceLevel = ConformanceLevel.Fragment};

            using (var reader = XmlReader.Create(file.FullName, settings))
            {
                var cardsParser = new CardsParser();
                cardsParser.Parse(reader);
            }
        }
    }
}
