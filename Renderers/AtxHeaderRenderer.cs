using System;
using Markdown2HTML.Core;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.Interfaces;
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
        private readonly EntitiesLineRenderer _entitiesLine = new EntitiesLineRenderer();
        private readonly EmphStrongInlineRenderer _emphInlineRenderer = new EmphStrongInlineRenderer();
        public string RenderToHTML(IMarkdownObject markdownObject)
        {
            if (markdownObject is MarkdownAtxHeader headerObj)
            {
                var render = headerObj.Content;

                render = _entitiesLine.Render(render);
                render = _emphInlineRenderer.Render(render);

                return $"<h{headerObj.Level}>{render}</h{headerObj.Level}>\n";
            }
            else
            {
                throw new ArgumentException("Invalid MarkdownObject passed.");
            }
        }
    }
}