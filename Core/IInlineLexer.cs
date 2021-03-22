using Markdown2HTML.Core.Tokens;

namespace Markdown2HTML.Core
{
    public interface IInlineLexer
    {
        MarkdownToken Lex(MarkdownToken token);
    }
}