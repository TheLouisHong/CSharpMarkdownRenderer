using System.Text.RegularExpressions;
using Markdown2HTML.Core;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.Tokens;

namespace Markdown2HTML.Lexers.BlockLexers
{
    [BlockLexer( (int) BlockLexerOrderHelper.Newline )]
    public class NewlineLexer : IBlockLexer
    {
        public readonly Regex match = new Regex(@"^(?: *(?:\n|$))+");
        public Match Match(string markdownString)
        {
            return match.Match(markdownString);
        }

        public MarkdownToken Lex(string markdownString, Match match)
        {
            return new MarkdownToken(TokenTypeHelper.SPACE, match.Value, match);
        }
    }
}