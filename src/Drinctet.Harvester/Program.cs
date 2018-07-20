using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using Drinctet.Harvester.Bevil;
using Drinctet.Harvester.Logging;
using Drinctet.Harvester.Logging.LogProviders;
using Drinctet.Harvester.Picolo;
using Serilog;

namespace Drinctet.Harvester
{
    internal class Program
    {
        private static ILog _logger;
        private static HttpClient _httpClient;
        private static XmlWriterSettings _xmlWriterSettings;

        private static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            LogProvider.SetCurrentLogProvider(new SerilogLogProvider());

            _logger = LogProvider.For<Program>();
            _httpClient = new HttpClient();
            _xmlWriterSettings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true,
                ConformanceLevel = ConformanceLevel.Fragment
            };

            var tasks = new List<Task>();
            tasks.Add(RunHarvester(new PicoloHarvester(new PicoloDataFactory(File.ReadAllText("F:\\Projects\\Drinctet.Sources\\Picolo\\rules_default-de.csv")).FetchLines())));
            //tasks.AddRange(RunAllHarvesters());
            //await RunHarvester(new BevilHarvester(File.ReadAllText("F:\\Projects\\Drinctet.Sources\\Bevi!\\data.js")));

            await Task.WhenAll(tasks);

            _logger.Info("Finished. Press any key to exit...");
            Console.ReadKey();
        }

        private static IEnumerable<Task> RunAllHarvesters()
        {
            var harvesterTypes = Assembly.GetExecutingAssembly().GetTypes().Where(x =>
                !x.IsAbstract && !x.IsGenericTypeDefinition && x.GetConstructors().Any(y => !y.GetParameters().Any()) &&
                x.GetTypeInfo().ImplementedInterfaces.Any(i => i == typeof(IHarvester)));
            foreach (var harvesterType in harvesterTypes)
            {
                _logger.Info($"Run harvester {harvesterType.Name}");
                yield return RunHarvester((IHarvester) Activator.CreateInstance(harvesterType));
            }

            _logger.Info($"Run harvester {typeof(BevilHarvester).Name}");
            //has constructor
            yield return RunHarvester(
                new BevilHarvester(File.ReadAllText("F:\\Projects\\Drinctet.Sources\\Bevi!\\data.js")));
            yield return RunHarvester(new PicoloHarvester(
                new PicoloDataFactory(File.ReadAllText("F:\\Projects\\Drinctet.Sources\\Picolo\\rules_default-de.csv"))
                    .FetchLines()));
        }

        private static async Task RunHarvester(IHarvester harvester)
        {
            var name = HarvesterNameAttribute.GetName(harvester.GetType());

            using (var writer = XmlWriter.Create($"{name}.xml", _xmlWriterSettings))
            {
                await harvester.Harvest(writer, new DiskArtifactDirectory(name), _httpClient);
            }
        }
    }
}