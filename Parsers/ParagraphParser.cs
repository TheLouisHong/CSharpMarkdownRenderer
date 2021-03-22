using Markdown2HTML.Core;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.MarkdownObject;
using Markdown2HTML.Core.Tokens;

namespace Markdown2HTML.Parsers
{
    public class MarkdownParagraph : IMarkdownObject
    {
        public MarkdownParagraph(string text)
        {
            Text = text;
        }

        public string Text = "";
    }

    [MarkdownTokenParser(TokenTypeHelper.PARAGRAPH)]
    public class ParagraphParser : IMarkdownTokenParser
    {
        public IMarkdownObject Parse(MarkdownToken token)
        {
            var mbObj = new MarkdownParagraph(token.Text);
            return mbObj;
        }
    }
}