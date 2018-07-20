namespace Drinctet.Core.Fragments
{
    public class GenderBasedSelectionFragment : TextFragment
    {
        public string FemaleText { get; set; }
        public string MaleText { get; set; }

        /// <summary>
        ///     The referenced player this selection should be based on. If nothing is specified, the last player varible is taken
        ///     as player or player 1.
        /// </summary>
        public int? ReferencedPlayerIndex { get; set; }

        public override string ToString()
        {
            var result = "{" + FemaleText;
            if (MaleText != null)
                result += "/" + MaleText;
            if (ReferencedPlayerIndex != null)
            {
                result += "|Player";
                if (ReferencedPlayerIndex != 1)
                    result += ReferencedPlayerIndex;
            }

            return result + "}";
        }
    }
}