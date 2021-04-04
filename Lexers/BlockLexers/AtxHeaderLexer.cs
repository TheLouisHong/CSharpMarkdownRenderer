using System.Text.RegularExpressions;
using Markdown2HTML.Core;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.Interfaces;
using Markdown2HTML.Core.Tokens;

namespace Markdown2HTML.Lexers.BlockLexers
{

    /// <summary>
    /// 1. Opening sequence of 1–6 unescaped # characters and an optional closing sequence of any number of unescaped # characters
    /// 2. Opening sequence of # characters must be followed by a space or by the end of line.
    /// 3. Optional closing sequence of #s must be preceded by a space and may be followed by spaces only.
    /// 4. Opening # character may be indented 0-3 spaces.
    /// 5. Raw contents of the heading are stripped of leading and trailing spaces before being parsed as inline content
    /// 6. The heading level is equal to the number of # characters in the opening sequence.
    ///
    /// CommonMark 0.29 Compliant (example 32-49), with exceptions.
    ///
    ///     Fail unit test because features are not supported elsewhere 
    ///     @TODO example 35, do not support escaping headers.
    ///     @TODO example 36, do not support inline emphasis.
    ///     @TODO example 39, do not support code blocks.
    ///     @TODO example 46, do not support escaping headers.
    ///     @TODO example 47, do not support stx headers or thematic breaks.
    ///
    ///     Fail unit test because of bugs elsewhere.
    ///     @TODO example 48, bug with <see cref="ParagraphLexer"/> when immediately followed by header.
    /// </summary>
    [BlockLexer( order: (int) BlockLexerOrderHelper.AtxHeaderLexer) ]
    public class AtxHeaderLexer : IBlockLexer
    {
        /// <summary>
        /// Precisely matching the 6 rules <see cref="AtxHeaderLexer"/>.
        ///
        /// group 1: #s
        /// group 2: content
        /// </summary>
        private readonly Regex _match = new Regex(@"^ {0,3}(#{1,6})(?: (.*?))??(?: +#* *)?(?:\n|$)");

        public MarkdownToken Lex(string markdownString)
        {
            var match = _match.Match(markdownString);
            if (match.Success)
            {
                return new MarkdownLeafBlock(TokenTypeHelper.ATX_HEADER, match.Value, match.Length);
            }
            return null;
        }
    }
}