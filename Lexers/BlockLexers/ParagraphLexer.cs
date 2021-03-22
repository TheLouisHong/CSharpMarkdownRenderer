using System;
using System.Text.RegularExpressions;
using Markdown2HTML.Core;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.Tokens;

namespace Markdown2HTML.Lexers.BlockLexers
{

    [BlockLexer( order: (int) BlockLexerOrderHelper.ParagraphLexer) ]
    public class ParagraphLexer : IBlockLexer
    {
        private readonly Regex _match = new Regex(
            // group 1 : match from the top,  
            @"^([^\n]+(?:\n(?!heading|list| +\n)[^\n]+)*)"

                .Replace("heading", @" {0,3}#{1,6} ")
                .Replace("list", @" {0,3}(?:[*+-]|1[.)]) ")
            );
        public Match Match(string markdownString)
        {
            return _match.Match(markdownString);
        }

        public MarkdownToken Lex(string markdownString, Match match)
        {
            var token = new MarkdownToken(TokenTypeHelper.PARAGRAPH, match.Groups[1].Value, match);
            return token;
        }
    }
}