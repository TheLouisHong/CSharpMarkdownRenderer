using Markdown2HTML.MarkdownObject;
using Markdown2HTML.Token;

namespace Markdown2HTML
{
    public interface IMarkdownTokenParser
    {
        IMarkdownObject Parse(MarkdownToken token);
    }
}