using System;
using System.Text.RegularExpressions;
using Markdown2HTML.Core;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.Tokens;

namespace Markdown2HTML.Lexers.BlockLexers
{

    [BlockLexer( order: (int) BlockLexerOrderHelper.AtxHeaderLexer) ]
    public class AtxHeaderLexer : IBlockLexer
    {
        //group 1: #
        //group 2: text with space
        private readonly Regex _match = new Regex(@"^ {0,3}(#{1,6})(?=\s|$)(.*)(?:\n+|$)");


        public Match Match(string markdownString)
        {
            return _match.Match(markdownString);
        }

        public MarkdownToken Lex(string markdownString, Match match)
        {
            MarkdownToken token = new MarkdownToken(TokenTypeHelper.HEADER ,match.Groups[0].Value, match);
            return token;
        }
    }
}