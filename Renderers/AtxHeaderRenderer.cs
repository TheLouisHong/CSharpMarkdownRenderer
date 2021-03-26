using System;
using Markdown2HTML.Core;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.MarkdownObject;
using Markdown2HTML.InlineRenderers;
using Markdown2HTML.Parsers;

namespace Markdown2HTML.Renderers
{
    /// <summary>
    /// Render Markdown Headers.
    /// </summary>
    [MarkdownObjectRenderer(typeof(MarkdownAtxHeader))]
    public class AtxHeaderRenderer : IMarkdownObjectRenderer
    {
        private readonly AutoEscapeInlineRenderer _autoEscapeInline = new AutoEscapeInlineRenderer();
        private readonly NaiveStrongEmphasisInlineRenderer _naiveStrongEmphasis = new NaiveStrongEmphasisInlineRenderer();
        public string RenderToHTML(IMarkdownObject markdownObject)
        {
            if (markdownObject is MarkdownAtxHeader headerObj)
            {
                var render = headerObj.Content;

                render = _autoEscapeInline.Render(render);
                render = _naiveStrongEmphasis.Render(render);

                return $"<h{headerObj.Level}>{render}</h{headerObj.Level}>\n";
            }
            else
            {
                throw new ArgumentException("Invalid MarkdownObject passed.");
            }
        }
    }
}