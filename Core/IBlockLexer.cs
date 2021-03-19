using System;
using System.Text.RegularExpressions;
using Markdown2HTML.Token;

namespace Markdown2HTML
{
    public interface IBlockLexer
    {
        Match Match(string markdownString);
        MarkdownToken Lex(ref string markdownString, Match match = null);
    }
}