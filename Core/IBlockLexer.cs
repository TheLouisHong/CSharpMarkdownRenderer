using System.Text.RegularExpressions;
using Markdown2HTML.Core.Tokens;

namespace Markdown2HTML.Core
{
    public interface IBlockLexer
    {
        MarkdownToken Lex(string markdownString);
    }
}