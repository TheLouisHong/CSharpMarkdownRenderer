using System;
using Markdown2HTML.Core;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.Interfaces;
using Markdown2HTML.Core.Tokens;
using Markdown2HTML.Helpers;

namespace Markdown2HTML.Parsers
{
    /// <summary>
    /// Parses empty lines.
    /// </summary>
    [MarkdownTokenParser(TokenTypeHelper.EMPTYLINE)]
    public class EmptyLineParser : IMarkdownTokenParser
    {
        public IMarkdownObject Parse(MarkdownToken token)
        {
            if (!(token is MarkdownLeafToken leafBlock))
            {
                throw new InvalidOperationException("Only parses leaf-blocks.");
            }
            return new MarkdownEmptyLine(leafBlock.Content);
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