namespace Drinctet.Core.Fragments
{
    public class SipsFragment : VariableFragment
    {
        public int SipsIndex { get; set; } = 1;
        public int MinSips { get; set; } = 1;

        public override string ToString()
        {
            var result = "[sips";
            if (SipsIndex != 1)
                result += SipsIndex;
            if (MinSips != 1)
                result += ":" + MinSips;

            return result + "]";
        }
    }
}