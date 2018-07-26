using System.Collections.Generic;
using System.Xml;
using Drinctet.Core.Cards.Base;
using Drinctet.Core.Parsing;

namespace Drinctet.Core
{
    public interface ICardsProvider
    {
        IReadOnlyList<BaseCard> Cards { get; }
    }

    public class CardsProvider : ICardsProvider
    {
        private readonly ICardParserFactory _parserFactory;
        private readonly List<BaseCard> _cards;

        public CardsProvider()
        {
            Cards = new List<BaseCard>();
            _parserFactory = new CardParserFactory();
            _cards = new List<BaseCard>();
            Cards = _cards;
        }

        public IReadOnlyList<BaseCard> Cards { get; }

        public void AddCards(XmlReader xmlReader, string source)
        {
            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        var parser = _parserFactory.GetParser(xmlReader.Name);
                        var subReader = xmlReader.ReadSubtree();
                        subReader.Read();
                        _cards.Add(parser.Parse(subReader, source));
                        break;
                }
            }
        }
    }
}