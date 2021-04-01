namespace Markdown2HTML.Core.Interfaces
{
    public interface IMarkdownObjectRenderer
    {
        string RenderToHTML(IMarkdownObject markdownObject);
    }
}