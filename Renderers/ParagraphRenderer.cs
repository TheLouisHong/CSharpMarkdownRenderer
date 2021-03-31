using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Markdown2HTML.Core;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.MarkdownObject;
using Markdown2HTML.InlineRenderers;
using Markdown2HTML.Parsers;

namespace Markdown2HTML.Renderers
{
    [MarkdownObjectRenderer(typeof(MarkdownParagraph))]
    public class ParagraphRenderer : IMarkdownObjectRenderer
    {

        private readonly EntitiesLineRenderer _entitiesLine = new EntitiesLineRenderer();
        private readonly brInlineRenderer _brInline = new brInlineRenderer();
        private readonly EmphStrongInlineRenderer _emphInlineRenderer = new EmphStrongInlineRenderer();

        public string RenderToHTML(IMarkdownObject markdownObject)
        {
            if (markdownObject is MarkdownParagraph paragraph)
            {
                var render = paragraph.Content;

                render = _entitiesLine.Render(render); // must be first

                render = _brInline.Render(render);
                render = _emphInlineRenderer.Render(render);
                render = TruncatePerLine(render);

                return $"<p>{render}</p>";
            }
            else
            {
                throw new ArgumentException("Invalid MarkdownObject passed.");
            }
        }

        private static string TruncatePerLine(string render)
        {
            var lines = render.Split('\n');

            // 2. trim whitespace and newlines per line
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                line = line.Trim('\n', ' ');
                lines[i] = line;
            }


            var sb = new StringBuilder();

            for (int i = 0; i < lines.Length - 1; i++)
            {
                sb.AppendLine(lines[i]);
            }

            sb.Append(lines.Last());

            render = sb.ToString();
            return render;
        }
    }
}