namespace Drinctet.Core.Parsing.TextDecoder
{
    internal static class TextDecoderConsts
    {
        public const char VarStartChar = '[';
        public const char VarEndChar = ']';
        public const char EscapeChar = '\\';
        public const char SelectionStartChar = '{';
        public const char SelectionModifierStartChar = '!';
        public const char SelectionEndChar = '}';

        public const string PlayerVariable = "Player";
        public const string SipsVariable = "Sips";
        public const char VariableParametersStart = ':';

        public const char SelectionSplitterChar = '/';
        public const char SelectionReferenceChar = '|';
        public const char ArrayDelimiter = ',';
    }
}