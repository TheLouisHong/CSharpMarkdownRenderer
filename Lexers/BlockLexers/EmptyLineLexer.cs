using System.Text.RegularExpressions;
using Markdown2HTML.Core;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.Engines;
using Markdown2HTML.Core.Interfaces;
using Markdown2HTML.Core.Tokens;

namespace Markdown2HTML.Lexers.BlockLexers
{
    // 1. highest priority
    // 2. lexes empty whitespace lines
    // 3. any content interrupts
    [BlockLexer( (int) BlockLexerOrderHelper.Emptyline )]
    public class EmptyLineLexer : IBlockLexer
    {
        public readonly Regex Match = new Regex(@"^(?: *(?:\n+|$))");
        public MarkdownToken Lex(string markdownString)
        {
            var match = Match.Match(markdownString);
            if (match.Success)
            {
                return new MarkdownToken(TokenTypeHelper.EMPTYLINE, match.Value, match.Length, null);
            }
            return null;
        }
    }
}