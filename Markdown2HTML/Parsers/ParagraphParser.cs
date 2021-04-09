using System;
using Markdown2HTML.Core;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.Interfaces;
using Markdown2HTML.Core.Tokens;
using Markdown2HTML.Helpers;

namespace Markdown2HTML.Parsers
{
    /// <summary>
    /// Represents a markdown paragraph.
    /// </summary>
    public class MarkdownParagraph : IMarkdownObject
    {
        public readonly string Content;

        public MarkdownParagraph(string content)
        {
            Content = content;
        }
        public override string ToString()
        {
            return $"{nameof(MarkdownParagraph)}\ncontent: {Content}";
        }
    }

    [MarkdownTokenParser(TokenTypeHelper.PARAGRAPH)]
    public class ParagraphParser : IMarkdownTokenParser
    {
        public IMarkdownObject Parse(MarkdownToken token)
        {
            if (!(token is MarkdownLeafToken leafBlock))
            {
                throw new InvalidOperationException("Only parses leaf-blocks.");
            }
            return new MarkdownParagraph(leafBlock.Content);
        }

    }
}