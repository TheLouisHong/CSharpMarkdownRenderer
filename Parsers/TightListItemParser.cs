using System;
using System.Text.RegularExpressions;
using Markdown2HTML.Core;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.Interfaces;
using Markdown2HTML.Core.Tokens;
using Markdown2HTML.InlineRenderers;

namespace Markdown2HTML.Parsers
{
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

    [MarkdownTokenParser(TokenTypeHelper.TIGHT_LIST_ITEM)]
    public class TightListItemParser : IMarkdownTokenParser
    {
        private Regex _listPattern = new Regex(
            @"^( *((?:\d{1,9}[\.\)])|([\-\*+])) +)(.*)");

        private readonly EntitiesLineRenderer _entitiesLine = new EntitiesLineRenderer();
        private readonly brInlineRenderer _brInline = new brInlineRenderer();
        private readonly EmphStrongInlineRenderer _emphInlineRenderer = new EmphStrongInlineRenderer();

        public IMarkdownObject Parse(MarkdownToken token)
        {
            if (token is MarkdownLeafBlock leaf)
            {
                var match = _listPattern.Match(leaf.Content);
                if (match.Success)
                {
                    return new FlatListItemMarkdownObject(match.Groups[4].Value);
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