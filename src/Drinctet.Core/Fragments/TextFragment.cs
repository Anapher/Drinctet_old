namespace Drinctet.Core.Fragments
{
    public abstract class TextFragment
    {
        public string OriginalString { get; internal set; }

        public override string ToString() => OriginalString;
    }
}