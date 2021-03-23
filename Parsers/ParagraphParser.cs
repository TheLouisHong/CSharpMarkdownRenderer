using System.Text.RegularExpressions;
using Markdown2HTML.Core;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.MarkdownObject;
using Markdown2HTML.Core.Tokens;

namespace Markdown2HTML.Parsers
{
    public class MarkdownParagraph : IMarkdownObject
    {
        public readonly string Content;

        public MarkdownParagraph(string content)
        {
            Content = content;
        }
    }

    [MarkdownTokenParser(TokenTypeHelper.PARAGRAPH)]
    public class ParagraphParser : IMarkdownTokenParser
    {
        public IMarkdownObject Parse(MarkdownToken token)
        {
            return new MarkdownParagraph(token.Content);
        }
    }
}