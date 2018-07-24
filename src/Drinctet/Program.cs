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

            status.InitializePlayers();
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
            private readonly IReadOnlyDictionary<string, string> _translations = new Dictionary<string, string>
            {
                {"OneSip", "einen Schluck"},
                {"Sips", "{0} Schlucke"},
                {"DownPresenter.Title", "Auf Ex'"},
                {"NeverEver.Title", "Ich habe noch nie..."},
                {"WouldYouRather.Title", "Würdest du lieber..."},
                {"WouldYouRather.Wyr1", "[Player], würdest du lieber {0}"},
                {"WouldYouRather.Wyr2", "An [Player1] und [Player2], würdet ihr lieber {0} Wenn ihr euch nicht einigen könnt, trinkt [sips]."},
                {"WouldYouRather.Wyr3", "An alle: Würdet ihr lieber {0} Stimmt alle gleichzeitig ab, die Verlierer trinken [sips]."},
                {"WouldYouRather.Wyr4", "[Player], würdest du lieber {0} Alle, die anderer Meinung sind, trinken [sips]."},
                {"Question.Title", "Frage"},
                {"Question.Question1", "[Player], {0}"},
                {"Question.Question2", "Nachdem [Player1] und [Player2] [sips] getrunken haben: {0}"},
                {"Question.Question3", "An alle: {0}"},
                {"Task.Title", "Aufgabe" },
                {"Task.Task1", "[Player100], {0}. Wenn du dich weigerst, trinke [sips]." },
                {"Task.Task2", "[Player100], {0}." },
                {"Task.Task3", "[Player100], {0}. Wenn du dich weigerst, trinke [sips] und [Player101] muss die Aufgabe machen." },
                {"SocialMedia.Text", "[Player], stell bei {0} einen bekloppten Satz rein, der folgende Wörter beinhaltet: {1} und {2}, und schreib am Ende #Drinctet - Oder trink [sips:5]." },
            };

            public string LanguageKey { get; } = "de";
            public CultureInfo Culture { get; } = CultureInfo.GetCultureInfo("de");

            public string this[string index]
            {
                get
                {
                    if (_translations.TryGetValue(index, out var result))
                        return result;

                    return "Placeholder";
                }
            }
        }
    }
}