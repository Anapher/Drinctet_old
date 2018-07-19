using System.Xml;
using Drinctet.Core.Cards.Base;

namespace Drinctet.Core.Parsing
{
    public interface ICardParser
    {
        BaseCard Parse(XmlReader xmlReader);
    }
}