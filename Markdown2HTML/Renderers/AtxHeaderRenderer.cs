using System;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.Interfaces;
using Markdown2HTML.InlineRenderers;
using Markdown2HTML.Parsers;

namespace Markdown2HTML.Renderers
{
    /// <summary>
    /// Render Markdown Headers. <see cref="MarkdownAtxHeader"/>
    /// </summary>
    [MarkdownObjectRenderer(typeof(MarkdownAtxHeader))]
    public class AtxHeaderRenderer : IMarkdownObjectRenderer
    {
        private readonly EntitiesInlineRenderer _entitiesInline = new EntitiesInlineRenderer();
        private readonly EmphStrongInlineRenderer _emphInlineRenderer = new EmphStrongInlineRenderer();
        public string RenderToHTML(IMarkdownObject markdownObject)
        {
            if (markdownObject is MarkdownAtxHeader headerObj)
            {
                var render = headerObj.Content;

                render = _entitiesInline.Render(render);
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