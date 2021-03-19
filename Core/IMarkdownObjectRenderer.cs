using Markdown2HTML.MarkdownObject;

namespace Markdown2HTML
{
    public interface IMarkdownObjectRenderer
    {
        string RenderToHTML(IMarkdownObject markdownObject);
    }
}