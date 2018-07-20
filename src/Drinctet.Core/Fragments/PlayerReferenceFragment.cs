using Drinctet.Core.Cards.Base;
using Drinctet.Core.Parsing.Utilities;

namespace Drinctet.Core.Fragments
{
    public class PlayerReferenceFragment : VariableFragment
    {
        public int PlayerIndex { get; internal set; } = 1;
        public RequiredGender RequiredGender { get; internal set; }

        public override string ToString()
        {
            var result = "[Player";
            if (PlayerIndex != 1)
                result += PlayerIndex;
            if (RequiredGender != RequiredGender.None)
                result += ":" +  ParserHelper.GetRequiredGenderShorcut(RequiredGender);
            return result + "]";
        }
    }
}