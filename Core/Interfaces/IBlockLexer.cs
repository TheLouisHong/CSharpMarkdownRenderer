using Markdown2HTML.Core.Engines;
using Markdown2HTML.Core.Tokens;

namespace Markdown2HTML.Core.Interfaces
{
    public interface IBlockLexer
    {
        MarkdownToken Lex(string markdownString);
    }
}