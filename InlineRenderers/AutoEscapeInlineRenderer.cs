using System;
using Markdown2HTML.Core;

namespace Markdown2HTML.InlineRenderers
{
    /// <summary>
    /// Escapes ampersand, should always be the first operation for inline rendering.
    /// </summary>
    public class AutoEscapeInlineRenderer : IInlineRenderer
    {
        // ampersand must be first, or will replace other people's ampersand
        static readonly string[] characters =
        {
            "&",
            "<",
            ">",
            "\"",
            "'"
        };

        static readonly string[] replacements =
        {
            "&amp;",
            "&lt;",
            "&gt;",
            "&quot;",
            "&#39;"
        };

        public string Render(string content)
        {
            for (int i = 0; i < characters.Length; i++)
            {
                content = content.Replace(characters[i], replacements[i]);
            }
            return content;
        }
    }
}