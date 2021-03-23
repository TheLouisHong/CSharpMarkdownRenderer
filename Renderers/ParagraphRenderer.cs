using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Markdown2HTML.Core;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.MarkdownObject;
using Markdown2HTML.Parsers;

namespace Markdown2HTML.Renderers
{
    [MarkdownObjectRenderer(typeof(MarkdownParagraph))]
    public class ParagraphRenderer : IMarkdownObjectRenderer
    {
        public readonly Regex brMatch = new Regex(
            @" {2,}\n"
            );
        public string RenderToHTML(IMarkdownObject markdownObject)
        {
            if (markdownObject is MarkdownParagraph paragraph)
            {
                // 1. replace double space with <br />
                var render = new StringBuilder(brMatch.Replace(paragraph.Content, "<br />\n"));

                var lines = render.ToString().Split('\n');

                // 2. trim whitespace and newlines per line
                for (var i = 0; i < lines.Length; i++)
                {
                    var line = lines[i];
                    line = line.Trim('\n', ' ');
                    lines[i] = line;
                }

                render.Clear();

                for (int i = 0; i < lines.Length - 1; i++)
                {
                    render.AppendLine(lines[i]);
                }

                render.Append(lines.Last());

                return $"<p>{render}</p>";
            }
            else
            {
                throw new ArgumentException("Invalid MarkdownObject passed.");
            }
        }
    }
}