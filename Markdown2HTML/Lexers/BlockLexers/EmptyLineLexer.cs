using System.Text.RegularExpressions;
using Markdown2HTML.Core;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.Interfaces;
using Markdown2HTML.Core.Tokens;
using Markdown2HTML.Helpers;
using Markdown2HTML.Parsers;

namespace Markdown2HTML.Lexers.BlockLexers
{
    /// <summary>
    /// Lexes empty lines within the document.
    /// </summary>
    [BlockLexer( (int) BlockLexerOrderHelper.Emptyline )]
    public class EmptyLineLexer : IBlockLexer
    {
        /// <summary>
        /// Match empty lines with any amount of space bars.
        /// </summary>
        private readonly Regex _emptyLinePattern = new Regex(@"^(?: *(?:\n+|$))");
        public MarkdownToken Lex(string markdownString)
        {
            var match = _emptyLinePattern.Match(markdownString);
            if (match.Success)
            {
                return new MarkdownLeafToken(TokenTypeHelper.EMPTYLINE, match.Value, match.Length);
            }
            return null;
        }
    }
}