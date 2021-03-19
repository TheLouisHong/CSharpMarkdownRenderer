using Markdown2HTML.Token;

namespace Markdown2HTML
{
    public interface IInlineLexer
    {
        MarkdownToken Lex(MarkdownToken token);
    }
}