using Markdown2HTML.Core;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.MarkdownObject;
using Markdown2HTML.Core.Tokens;

namespace Markdown2HTML.Parsers
{
    [MarkdownTokenParser(TokenTypeHelper.EMPTYLINE)]
    public class EmptyLineParser : IMarkdownTokenParser
    {
        public IMarkdownObject Parse(MarkdownToken token)
        {
            return new MarkdownEmptyLine(token.Content);
        }
    }

    public class MarkdownEmptyLine : IMarkdownObject
    {
        public readonly string Content;

        public MarkdownEmptyLine(string content)
        {
            Content = content;
        }
    }
}