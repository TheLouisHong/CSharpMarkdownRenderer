using System.Text.RegularExpressions;
using Markdown2HTML.Core.Tokens;

namespace Markdown2HTML.Core
{
    public interface IBlockLexer
    {
        Match Match(string markdownString);
        MarkdownToken Lex(string markdownString, Match match);
    }
}