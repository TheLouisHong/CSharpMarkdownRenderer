using System;
using System.Text;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.Extensions;
using Markdown2HTML.Core.Interfaces;
using Markdown2HTML.InlineRenderers;
using Markdown2HTML.Parsers;

namespace Markdown2HTML.Renderers
{
    [MarkdownObjectRenderer(typeof(UnorderedListMarkdownObject))]
    public class UnorderedListRenderer : IMarkdownObjectRenderer
    {
        private readonly EntitiesLineRenderer _entitiesLine = new EntitiesLineRenderer();
        private readonly brInlineRenderer _brInline = new brInlineRenderer();
        private readonly EmphStrongInlineRenderer _emphInlineRenderer = new EmphStrongInlineRenderer();
        public string RenderToHTML(IMarkdownObject markdownObject)
        {
            if (!(markdownObject is UnorderedListMarkdownObject list))
            {
                throw new InvalidOperationException("Failed sanity check. Incorrect object for renderer.");
            }

            var sb = new StringBuilder();
            sb.AppendLine("<ul>");
            foreach (var item in list.Items)
            {
                if (item is FlatListItemMarkdownObject flatItem)
                {
                    var render = flatItem.Content;

                    render = _entitiesLine.Render(render); // must be first

                    render = _brInline.Render(render);
                    render = _emphInlineRenderer.Render(render);
                    render = StringExtensions.TruncatePerLine(render);

                    sb.AppendLine($"<li>{render}</li>");
                }
                else
                {
                    throw new InvalidOperationException("Failed sanity check. Do not support nested lists.");
                }
            }
            sb.AppendLine("</ul>");
            return sb.ToString();
        }

    }
}