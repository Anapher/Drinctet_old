using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using Drinctet.Core;
using Drinctet.ViewModels;
using Drinctet.ViewModels.Manager;
using Drinctet.ViewModels.Slides;
using Drinctet.ViewModels.Slides.Base;
using Drinctet.ViewModels.ViewModelBase;

namespace Drinctet
{
    public class SimpleDependencyService : IDependencyService
    {
        private readonly IDictionary<Type, object> _objects = new Dictionary<Type, object>();

        public void Register<TInterface>(object obj) where TInterface : class
        {
            _objects.Add(typeof(TInterface), obj);
        }

        public T Get<T>() where T : class
        {
            return (T) _objects[typeof(T)];
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            var dependencyService = new SimpleDependencyService();
            dependencyService.Register<ICardsProvider>(GetCards());

            var status = new DrinctetStatus();
            status.SlideTypes.Clear();
            status.SlideTypes.Add(new WeightedValue<SlideType>(SlideType.Task, 1));
            status.SlideTypes.Add(new WeightedValue<SlideType>(SlideType.Question, 1));

            status.Players.Add(new PlayerInfo(1, Gender.Male){Name = "Vincent"});
            status.Players.Add(new PlayerInfo(4, Gender.Male){Name = "Bursod"});
            status.Players.Add(new PlayerInfo(8, Gender.Female){Name = "Larny"});
            status.Players.Add(new PlayerInfo(9, Gender.Female){Name = "Britta"});

            status.InitializePlayers();
            status.PlayerArrangements.Add(4, 9);

            DependencyServiceInitializer.DependencyService = dependencyService;

            var viewModel = new GameViewModel(status);

            while (true)
            {
                DisplaySlide(viewModel.CurrentSlide);
                Console.ReadKey();

                viewModel.NextSlideCommand.Execute(null);
            }
        }

        private static void DisplaySlide(ISlideViewModel slide)
        {
            switch (slide)
            {
                case DownViewModel downViewModel:
                    WriteText(downViewModel.Text);
                    break;
                case DrinkViewModel drinkViewModel:
                    WriteText(drinkViewModel.Text);
                    break;
                case GroupGameViewModel groupGameViewModel:
                    WriteText(groupGameViewModel.Text);
                    break;
                case NeverEverViewModel neverEverViewModel:
                    WriteText(neverEverViewModel.Text);
                    break;
                case NoIdeaLosesViewModel noIdeaLosesViewModel:
                    WriteText(noIdeaLosesViewModel.Text);
                    break;
                case QuestionViewModel questionViewModel:
                    WriteText(questionViewModel.Text);
                    break;
                case SocialMediaViewModel socialMediaViewModel:
                    WriteText(socialMediaViewModel.Text);
                    break;
                case TaskViewModel taskViewModel:
                    WriteText(taskViewModel.Text);
                    break;
                case VirusViewModel virusViewModel:
                    WriteText(virusViewModel.Text);
                    break;
                case WouldYouRatherViewModel wouldYouRatherViewModel:
                    WriteText(wouldYouRatherViewModel.Text);
                    break;
            }

            Console.WriteLine();
        }

        private static void WriteText(string text)
        {
            Console.WriteLine(text);
        }

        private static ICardsProvider GetCards()
        {
            //var sourceFiles = new List<string>(new DirectoryInfo(@"F:\Projects\Drinctet\cards").GetFiles("*.xml")
            //    .Select(x => x.FullName));
            var sourceFiles = new[] { "F:\\Projects\\Drinctet\\cards\\Bevil.xml" };

            var settings = new XmlReaderSettings {ConformanceLevel = ConformanceLevel.Fragment};
            var provider = new CardsProvider();

            var sw = Stopwatch.StartNew();
            foreach (var sourceFile in sourceFiles)
                using (var reader = XmlReader.Create(sourceFile, settings))
                {
                    provider.AddCards(reader, sourceFile);
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