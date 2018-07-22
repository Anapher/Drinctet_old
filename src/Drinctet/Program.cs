using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using Drinctet.Core;
using Drinctet.Presentation.Screen;

namespace Drinctet
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var status = new DrinctetStatus();
            status.Players.Add(new PlayerInfo(1, Gender.Male){Name = "Vincent"});
            status.Players.Add(new PlayerInfo(4, Gender.Male){Name = "Bursod"});
            status.Players.Add(new PlayerInfo(8, Gender.Female){Name = "Larny"});
            status.Players.Add(new PlayerInfo(9, Gender.Female){Name = "Britta"});

            status.Initialize();
            status.PlayerArrangements.Add(4, 9);
            var cards = GetCards();

            var manager = new ScreenGameManager(status, new Texts());
            while (true)
            {
                DisplaySlide(manager.Next(cards));
                Console.ReadKey();
            }

        }

        private static void DisplaySlide(ISlidePresenter slide)
        {
            Console.WriteLine(slide + " - " + slide.Title);
            Console.WriteLine(slide.Text);
            Console.WriteLine();
        }

        private static ICardsProvider GetCards()
        {
            var sourceFiles = new List<string> { @"F:\Projects\Drinctet\cards\common.wouldYouRather.xml" };
            sourceFiles.AddRange(
                new DirectoryInfo(@"F:\Projects\Drinctet\src\Drinctet.Harvester\bin\Debug\netcoreapp2.1")
                    .GetFiles("*.xml").Select(x => x.FullName));

            //var sourceFiles = new[] {"F:\\Projects\\Drinctet\\cards\\test.xml"};

            var settings = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment };
            var provider = new CardsProvider();

            var sw = Stopwatch.StartNew();
            foreach (var sourceFile in sourceFiles)
                using (var reader = XmlReader.Create(sourceFile, settings))
                {
                    provider.AddCards(reader);
                }

            Console.WriteLine($"Parsed {provider.Cards.Count} cards in {sw.ElapsedMilliseconds} ms");

            return provider;
        }

        private class Texts : ITextResource
        {
            public string LanguageKey { get; } = "de";
            public CultureInfo Culture { get; } = CultureInfo.GetCultureInfo("de");

            public string this[string index]
            {
                get
                {
                    switch (index)
                    {
                        case "OneSip":
                            return "einen Schluck";
                        case "Sips":
                            return "{0} Schlucke";
                        case "DownPresenter.Title":
                            return "Auf Ex'";
                        case "NeverEver.Title":
                            return "Ich habe noch nie...";
                        case "WouldYouRather.Title":
                            return "Würdest du lieber...";
                        case "WouldYouRather.Wyr1":
                            return "[Player], würdest du lieber {0}";
                        case "WouldYouRather.Wyr2":
                            return "An [Player1] und [Player2], würdet ihr lieber {0} Wenn ihr euch nicht einigen könnt, trinkt [sips].";
                        case "WouldYouRather.Wyr3":
                            return "An alle: Würdet ihr lieber {0} Stimmt alle gleichzeitig ab, die Verlierer trinken [sips].";
                        case "Question.Title":
                            return "Frage";
                        case "Question.Question1":
                            return "[Player], {0}";
                        case "Question.Question2":
                            return "Nachdem [Player1] und [Player2] [sips] getrunken haben: {0}";
                        case "Question.Question3":
                            return "An alle: {0}";
                    }

                    return "Placeholder";
                }
            }
        }
    }
}