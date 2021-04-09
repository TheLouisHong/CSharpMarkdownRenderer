namespace Markdown2HTML.Helpers
{
    /// <summary>
    /// Helps keep track of Lexer Order. Place Lexers in the order you want them to be ran.
    /// </summary>
    public enum BlockLexerOrderHelper : int
    {
        AtxHeaderLexer = 0,
        ListLexer,
        Emptyline,
        ParagraphLexer, // ParagraphLexer be last, to not catch all other types of blocks
    }
}