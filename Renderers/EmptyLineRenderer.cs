using System;
using Markdown2HTML.Core;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.MarkdownObject;
using Markdown2HTML.Parsers;

namespace Markdown2HTML.Renderers
{
    [MarkdownObjectRenderer(typeof(MarkdownEmptyLine))]
    public class EmptyLineRenderer : IMarkdownObjectRenderer
    {
        public string RenderToHTML(IMarkdownObject markdownObject)
        {
            if (markdownObject is MarkdownEmptyLine emptyLine)
            {
                return "\n";
            }
            else
            {
                throw new ArgumentException("invalid type.");
            }
        }
    }
}