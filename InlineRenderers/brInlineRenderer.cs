using System.Text.RegularExpressions;
using Markdown2HTML.Core;

namespace Markdown2HTML.InlineRenderers
{

    /// <summary>
    /// Replace double space with br
    /// </summary>
    public class brInlineRenderer : IInlineRenderer
    {
        public readonly Regex brMatch = new Regex(
            @" {2,}\n"
            );

        public string Render(string content)
        {
            var render = brMatch.Replace(content, "<br />\n");
            return render;
        }
    }
}