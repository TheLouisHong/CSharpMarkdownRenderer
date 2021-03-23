using System;
using System.Text.RegularExpressions;
using Markdown2HTML.Core;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.Tokens;

namespace Markdown2HTML.Lexers.BlockLexers
{

    // 1. [0,3] starting spaces allowed.
    // 2. [1,6] # allowed, representing header level.
    // 3. arbitrary white spaces, except new line, after # allowed, including all white space header.
    // 4. any new line interrupts header.
    [BlockLexer( order: (int) BlockLexerOrderHelper.AtxHeaderLexer) ]
    public class AtxHeaderLexer : IBlockLexer
    {
        private readonly Regex _match = new Regex(@"^ {0,3}(#{1,6})(?=\s|$)(.*)(?:\n+|$)");

        public MarkdownToken Lex(string markdownString)
        {
            return null;
        }
    }
}