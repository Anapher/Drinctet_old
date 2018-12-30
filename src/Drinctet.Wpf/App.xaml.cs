using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml;
using Drinctet.Core;
using Drinctet.ViewModels.ViewModelBase;

namespace Drinctet.Wpf
{
    public class SimpleDependencyService : IDependencyService
    {
        private readonly IDictionary<Type, object> _objects = new Dictionary<Type, object>();

        public T Get<T>() where T : class => (T) _objects[typeof(T)];

        public void Register<TInterface>(object obj) where TInterface : class
        {
            _objects.Add(typeof(TInterface), obj);
        }
    }

    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var dependencyService = new SimpleDependencyService();
            dependencyService.Register<ICardsProvider>(GetCards());

            DependencyServiceInitializer.DependencyService = dependencyService;
        }

        private static ICardsProvider GetCards()
        {
            var sourceFiles = new List<string>(new DirectoryInfo(@"F:\Projects\Drinctet\cards").GetFiles("*.xml")
                .Select(x => x.FullName));

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
    }
}