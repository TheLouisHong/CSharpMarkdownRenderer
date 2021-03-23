using System.Text.RegularExpressions;
using Markdown2HTML.Core;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.Debug;
using Markdown2HTML.Core.Tokens;

namespace Markdown2HTML.Lexers.BlockLexers
{

    // 1. [0,3] starting spaces allowed.
    // 2. first character must be bullet point regex: [*+-]|\d{1,0}[.)]
    // 3. followed by at least one space
    // 4. followed by optional amount of of any character [\s\S].
    // 5. interrupt by two or more new lines.
    // 6. interrupt by hr and link definition.

    // 7. possible edge case pending // TODO
    [BlockLexer( (int) BlockLexerOrderHelper.ListLexer )]
    public class ListLexer : IBlockLexer
    {
        public MarkdownToken Lex(string markdownString)
        {
            return null;
        }

    }
}