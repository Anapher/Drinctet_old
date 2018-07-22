using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Drinctet.Core.Parsing
{
    public class CardParserFactory : ICardParserFactory
    {
        private readonly IReadOnlyDictionary<string, Type> _parsers;

        public CardParserFactory()
        {
            var parserTypes = Assembly.GetExecutingAssembly().GetTypes().Where(x =>
                x.Namespace == "Drinctet.Core.Parsing.Parsers" &&
                !x.IsAbstract && !x.IsGenericTypeDefinition &&
                x.GetTypeInfo().ImplementedInterfaces.Any(i => i == typeof(ICardParser)));

            _parsers = parserTypes.ToDictionary(x => x.Name.Replace("Parser", null), x => x,
                StringComparer.OrdinalIgnoreCase);
        }

        public ICardParser GetParser(string name) => (ICardParser) Activator.CreateInstance(_parsers[name]);
    }
}