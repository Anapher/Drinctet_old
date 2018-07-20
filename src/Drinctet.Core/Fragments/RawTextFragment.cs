namespace Drinctet.Core.Fragments
{
    public class RawTextFragment : TextFragment
    {
        public RawTextFragment(string text)
        {
            Text = text;
        }

        public string Text { get; set; }

        public override string ToString() => Text;
    }
}