namespace Markdown2HTML.Lexers
{
    public enum BlockLexerOrderHelper : int
    {
        AtxHeaderLexer = 0,
        ListLexer,
        Emptyline,
        ParagraphLexer, // ParagraphLexer be last, to not catch all other types of blocks
    }
}