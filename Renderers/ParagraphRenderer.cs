using System;
using Markdown2HTML.Core;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.MarkdownObject;
using Markdown2HTML.Parsers;

namespace Markdown2HTML.Renderers
{
    [MarkdownObjectRenderer(typeof(MarkdownParagraph))]
    public class ParagraphRenderer : IMarkdownObjectRenderer
    {
        public string RenderToHTML(IMarkdownObject markdownObject)
        {
            if (markdownObject is MarkdownParagraph paragraph)
            {
                return $"<p>{paragraph.Text}</p>";
            }
            else
            {
                throw new ArgumentException("Invalid MarkdownObject passed.");
            }
        }
    }
}