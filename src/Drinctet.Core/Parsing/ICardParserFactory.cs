namespace Drinctet.Core.Parsing
{
    public interface ICardParserFactory
    {
        ICardParser GetParser(string name);
    }
}