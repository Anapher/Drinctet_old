using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Drinctet.Core.Parsing
{
    public interface ICardParserFactory
    {
        ICardParser GetParser(string name);
    }

    public class CardParserFactory : ICardParserFactory
    {
        private readonly IReadOnlyDictionary<string, Lazy<ICardParser>> _parsers;

        public CardParserFactory()
        {
            var parserTypes = Assembly.GetExecutingAssembly().GetTypes().Where(x =>
                !x.IsAbstract && !x.IsGenericTypeDefinition &&
                x.GetTypeInfo().ImplementedInterfaces.Any(i => i == typeof(ICardParser)));
            _parsers = parserTypes.ToDictionary(x => x.Name.Replace("Parser", null),
                x => new Lazy<ICardParser>(() => (ICardParser)Activator.CreateInstance(x)), StringComparer.OrdinalIgnoreCase);
        }

        public ICardParser GetParser(string name) => _parsers[name].Value;
    }
}