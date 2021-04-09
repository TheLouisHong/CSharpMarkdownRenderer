namespace Markdown2HTML.Helpers
{
    /// <summary>
    /// Helps keep track of TokenType strings.
    /// These are used to communicate between the Lexer and the Parser to identify MarkdownTokens.
    /// </summary>
    public static class TokenTypeHelper
    {
        public const string EMPTYLINE = "emptyline";
        public const string LIST = "list";
        public const string TIGHT_LIST_ITEM = "tight-list-item";
        public const string ATX_HEADER = "header";
        public const string PARAGRAPH = "paragraph";
    }
}