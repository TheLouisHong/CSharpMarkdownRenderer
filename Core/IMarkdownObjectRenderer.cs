using Markdown2HTML.Core.MarkdownObject;

namespace Markdown2HTML.Core
{
    public interface IMarkdownObjectRenderer
    {
        string RenderToHTML(IMarkdownObject markdownObject);
    }
}