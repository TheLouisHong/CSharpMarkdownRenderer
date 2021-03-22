using System;
using Markdown2HTML.Core;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.MarkdownObject;
using Markdown2HTML.Parsers;

namespace Markdown2HTML.Renderers
{
    [MarkdownObjectRenderer(typeof(AtxMarkdownHeader))]
    public class AtxHeaderRenderer : IMarkdownObjectRenderer
    {
        public string RenderToHTML(IMarkdownObject markdownObject)
        {
            if (markdownObject is AtxMarkdownHeader headerObj)
            {
                return $"<h{headerObj.Level}>{headerObj.Text}</h{headerObj.Level}>\n";
            }
            else
            {
                throw new ArgumentException("Invalid MarkdownObject passed.");
            }
        }
    }
}