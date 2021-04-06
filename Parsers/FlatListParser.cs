using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Markdown2HTML.Core;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.Engines;
using Markdown2HTML.Core.Interfaces;
using Markdown2HTML.Core.Tokens;

namespace Markdown2HTML.Parsers
{
    public class UnorderedListMarkdownObject : IMarkdownObject
    {
        public bool IsTight;
        public char BulletChar;

        public List<IMarkdownObject> Items;

        public UnorderedListMarkdownObject(bool isTight, char bulletChar, List<IMarkdownObject> items)
        {
            IsTight = isTight;
            BulletChar = bulletChar;
            Items = items;
        }
    }
    public class OrderedListMarkdownObject : IMarkdownObject
    {
        public bool IsTight;
        public int StartNumber;

        public List<IMarkdownObject> Items;

        public OrderedListMarkdownObject(bool isTight, int startNumber, List<IMarkdownObject> items)
        {
            IsTight = isTight;
            StartNumber = startNumber;
            Items = items;
        }
    }
    
    [MarkdownTokenParser(TokenTypeHelper.LIST)]
    public class FlatListParser : IMarkdownTokenParser
    {
        /// <summary>
        /// group 1: entire bullet and padding
        /// group 3: bullet char
        /// group 4: content
        /// </summary>
        private readonly Regex _unorderedListPattern = new Regex(
            @"^( *([\-\*+]) +)(.*)");

        /// <summary>
        /// group 1: entire bullet and padding
        /// group 2: entire bullet
        /// group 3: ordered bullet number
        /// group 4: content
        /// </summary>
        private readonly Regex _orderedListPattern = new Regex(
            @"^( *((\d{1,9})[\.\)]) +)(.*)");

        public IMarkdownObject Parse(MarkdownToken token)
        {
            if (!(token is MarkdownContainerBlock block))
            {
                throw new InvalidOperationException("Failed sanity check. Wrong type of token passed to parser.");
            }

            if (block.Subtokens[0] is MarkdownLeafBlock firstItem)
            {
                var ulMatch = _unorderedListPattern.Match(firstItem.Content);
                if (ulMatch.Success)
                {
                    var bullet = ulMatch.Groups[2].Value[0];
                    var itemsObj = ParserEngine.Parse(block.Subtokens);

                    var mdObj = new UnorderedListMarkdownObject(true, bullet, itemsObj);
                    return mdObj;
                }

                var olMatch = _orderedListPattern.Match(firstItem.Content);
                if (olMatch.Success)
                {
                    var value = int.Parse(olMatch.Groups[3].Value);
                    var itemsObj = ParserEngine.Parse(block.Subtokens);

                    var mdObj = new OrderedListMarkdownObject(true, value, itemsObj);
                    return mdObj;
                }

                throw new InvalidOperationException("Failed sanity check. Neither ul nor ol.");
            }
            else
            {
                // TODO Support nested list
                throw new InvalidOperationException("Failed sanity check. Only support unnested lists for now.");
            }
        }
    }
}