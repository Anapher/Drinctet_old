using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using Drinctet.Core;

namespace Drinctet
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var sourceFiles = new List<string> {@"F:\Projects\Drinctet\cards\common.wouldYouRather.xml"};
            sourceFiles.AddRange(
                new DirectoryInfo(@"F:\Projects\Drinctet\src\Drinctet.Harvester\bin\Debug\netcoreapp2.1")
                    .GetFiles("*.xml").Select(x => x.FullName));

            var settings = new XmlReaderSettings {ConformanceLevel = ConformanceLevel.Fragment};
            var provider = new CardsProvider();

            var sw = Stopwatch.StartNew();
            foreach (var sourceFile in sourceFiles)
                using (var reader = XmlReader.Create(sourceFile, settings))
                {
                    provider.AddCards(reader);
                }

            Console.WriteLine($"Parsed {provider.Cards.Count} cards in {sw.ElapsedMilliseconds} ms");
            Console.ReadKey();
        }
    }
}