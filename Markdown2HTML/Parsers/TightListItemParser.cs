using System;
using System.Text.RegularExpressions;
using Markdown2HTML.Core;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.Interfaces;
using Markdown2HTML.Core.Tokens;
using Markdown2HTML.Helpers;
using Markdown2HTML.InlineRenderers;

namespace Markdown2HTML.Parsers
{
    /// <summary>
    /// Represents a flat list item. This is unexpected only because we do not support nested lists right now.
    /// </summary>
    public class FlatListItemMarkdownObject : IMarkdownObject
    {
        public string Content;

        public FlatListItemMarkdownObject(string content)
        {
            Content = content;
        }

        public override string ToString()
        {
            return $"\"{Content}\"";
        }
    }

    /// <summary>
    /// Parses a tight list item.
    /// </summary>
    [MarkdownTokenParser(TokenTypeHelper.TIGHT_LIST_ITEM)]
    public class TightListItemParser : IMarkdownTokenParser
    {
    
        /// <summary>
        /// group 1: the entire bullet point and padding
        /// group 2: the bullet point
        /// group 3: the content
        /// </summary>
        private readonly Regex _listPattern = new Regex(
            @"^( *((?:\d{1,9}[\.\)])|(?:[\-\*+])) +)(.*)");

        private readonly EntitiesInlineRenderer _entitiesInline = new EntitiesInlineRenderer();
        private readonly brInlineRenderer _brInline = new brInlineRenderer();
        private readonly EmphStrongInlineRenderer _emphInlineRenderer = new EmphStrongInlineRenderer();

        public IMarkdownObject Parse(MarkdownToken token)
        {
            if (token is MarkdownLeafToken leaf)
            {
                var match = _listPattern.Match(leaf.Content);
                if (match.Success)
                {
                    return new FlatListItemMarkdownObject(match.Groups[3].Value);
                }
                else
                {
                    throw new InvalidOperationException("Failed sanity check. Invalid bullet.");
                }
            }
            else
            {
                throw new InvalidOperationException("Failed sanity check. Not leaf block.");
            }
        }
    }
}