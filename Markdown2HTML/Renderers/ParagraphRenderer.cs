using System;
using System.Linq;
using System.Text;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.Extensions;
using Markdown2HTML.Core.Interfaces;
using Markdown2HTML.InlineRenderers;
using Markdown2HTML.Parsers;

namespace Markdown2HTML.Renderers
{
    /// <summary>
    /// Renders <see cref="MarkdownParagraph"/>
    /// </summary>
    [MarkdownObjectRenderer(typeof(MarkdownParagraph))]
    public class ParagraphRenderer : IMarkdownObjectRenderer
    {

        private readonly EntitiesInlineRenderer _entitiesInline = new EntitiesInlineRenderer();
        private readonly brInlineRenderer _brInline = new brInlineRenderer();
        private readonly EmphStrongInlineRenderer _emphInlineRenderer = new EmphStrongInlineRenderer();

        public string RenderToHTML(IMarkdownObject markdownObject)
        {
            if (markdownObject is MarkdownParagraph paragraph)
            {
                var render = paragraph.Content;

                render = _entitiesInline.Render(render); // must be first

                render = _brInline.Render(render);
                render = _emphInlineRenderer.Render(render);
                render = StringExtensions.TruncatePerLine(render);

                return $"<p>{render}</p>";
            }
            else
            {
                throw new ArgumentException("Invalid MarkdownObject passed.");
            }
        }

    }
}