using Markdown2HTML.Attributes;
using Markdown2HTML.MarkdownObject;
using Markdown2HTML.Parsers;

namespace Markdown2HTML.Renderers
{
    [MarkdownObjectRenderer(typeof(MarkdownHeader))]
    public class HeaderRenderer : IMarkdownObjectRenderer
    {
        public string RenderToHTML(IMarkdownObject markdownObject)
        {
            return "header rendered.";
        }
    }
}