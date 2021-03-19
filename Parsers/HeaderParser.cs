using Markdown2HTML.Attributes;
using Markdown2HTML.MarkdownObject;
using Markdown2HTML.Token;

namespace Markdown2HTML.Parsers
{
    public class MarkdownHeader : IMarkdownObject
    {
    }

    [MarkdownTokenParser(TokenTypes.HEADER)]
    public class HeaderParser : IMarkdownTokenParser
    {
        public IMarkdownObject Parse(MarkdownToken token)
        {
            return null;
        }
    }
}