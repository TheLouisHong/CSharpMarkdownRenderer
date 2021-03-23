using System;
using Markdown2HTML.Core;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.MarkdownObject;
using Markdown2HTML.Parsers;

namespace Markdown2HTML.Renderers
{
    /// <summary>
    /// Render Markdown Headers.
    /// </summary>
    [MarkdownObjectRenderer(typeof(MarkdownAtxHeader))]
    public class AtxHeaderRenderer : IMarkdownObjectRenderer
    {
        public string RenderToHTML(IMarkdownObject markdownObject)
        {
            if (markdownObject is MarkdownAtxHeader headerObj)
            {
                return $"<h{headerObj.Level}>{headerObj.Content}</h{headerObj.Level}>\n";
            }
            else
            {
                throw new ArgumentException("Invalid MarkdownObject passed.");
            }
        }
    }
}