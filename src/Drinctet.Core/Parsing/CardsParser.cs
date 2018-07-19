using System.Collections.Generic;
using System.Xml;
using Drinctet.Core.Cards.Base;

namespace Drinctet.Core.Parsing
{
    public class CardsParser
    {
        private readonly ICardParserFactory _parserFactory;

        public CardsParser()
        {
            Cards = new List<BaseCard>();
            _parserFactory = new CardParserFactory();
        }

        public IList<BaseCard> Cards { get; }

        public void Parse(XmlReader xmlReader)
        {
            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        var parser = _parserFactory.GetParser(xmlReader.Name);
                        Cards.Add(parser.Parse(xmlReader.ReadSubtree()));
                        break;
                }
            }
        }
    }
}