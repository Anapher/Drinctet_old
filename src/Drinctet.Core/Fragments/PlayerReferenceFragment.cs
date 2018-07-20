using Drinctet.Core.Cards.Base;

namespace Drinctet.Core.Fragments
{
    public class PlayerReferenceFragment : VariableFragment
    {
        public int PlayerIndex { get; internal set; } = 1;
        public RequiredGender RequiredGender { get; internal set; }
    }

    public class SipsFragment : VariableFragment
    {
        public int SipsIndex { get; set; } = 1;
        public int MinSips { get; set; } = 1;
    }

    public abstract class VariableFragment : TextFragment
    {
    }
}