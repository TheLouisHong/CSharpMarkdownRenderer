using System.Text.RegularExpressions;
using Markdown2HTML.Core;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.Debug;
using Markdown2HTML.Core.Tokens;

namespace Markdown2HTML.Lexers.BlockLexers
{
    [BlockLexer( (int) BlockLexerOrderHelper.ListLexer )]
    public class ListLexer : IBlockLexer
    {
        private readonly Regex _match
            = new Regex(
                @"^( {0,3})(bullet) [\s\S]+?(?:\n{2,}(?! )(?! {0,3}bullet )\n*|\s*$)"
                    .Replace("bullet", @"(?:[*+-]|\d{1,9}[.)])")
                );
        public Match Match(string markdownString)
        {
            return _match.Match(markdownString);
        }

        public MarkdownToken Lex(string markdownString, Match match)
        {
            return new MarkdownToken(TokenTypeHelper.LIST, match.Value, match);
        }
    }
}