using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Markdown2HTML.Core;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.Engines;
using Markdown2HTML.Core.Interfaces;
using Markdown2HTML.Core.Tokens;
using Markdown2HTML.Helpers;

namespace Markdown2HTML.Parsers
{
    /// <summary>
    /// Represents an unordered list.
    /// </summary>
    public class MarkdownUnorderedList : IMarkdownObject
    {
        /// <summary>
        /// TODO Unused variable, for future use to support tight/loose lists
        /// </summary>
        public bool IsTight;
        /// <summary>
        /// The character leading the list.
        /// </summary>
        public char BulletChar;

        public List<IMarkdownObject> Items;

        public MarkdownUnorderedList(bool isTight, char bulletChar, List<IMarkdownObject> items)
        {
            IsTight = isTight;
            BulletChar = bulletChar;
            Items = items;
        }
    }
    /// <summary>
    /// Represents an ordered list.
    /// </summary>
    public class MarkdownOrderedList : IMarkdownObject
    {
        /// <summary>
        /// TODO Unused variable, for future use to support tight/loose lists
        /// </summary>
        public bool IsTight;
        /// <summary>
        /// The starting number of this list.
        /// </summary>
        public int StartNumber;

        /// <summary>
        /// A recursive container of other MarkdownObjects.
        /// </summary>
        public List<IMarkdownObject> Items;

        public MarkdownOrderedList(bool isTight, int startNumber, List<IMarkdownObject> items)
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
            if (!(token is MarkdownContainerToken block))
            {
                throw new InvalidOperationException("Failed sanity check. Wrong type of token passed to parser.");
            }

            // we take the first block in ordered to extract information about the list.
            if (block.Subtokens[0] is MarkdownLeafToken firstItem)
            {
                // ... is it an unordered list?
                var ulMatch = _unorderedListPattern.Match(firstItem.Content);
                if (ulMatch.Success)
                {
                    // record the bullet used for the ul.
                    var bullet = ulMatch.Groups[2].Value[0];
                    // recursively parse content inside the list.
                    var itemsObj = ParserEngine.Parse(block.Subtokens);
                    // create MarkdownObject
                    var mdObj = new MarkdownUnorderedList(true, bullet, itemsObj);
                    // finished.
                    return mdObj;
                }

                // is it an ordered list?
                var olMatch = _orderedListPattern.Match(firstItem.Content);
                if (olMatch.Success)
                {
                    // record the starting value used for the ol.
                    var value = int.Parse(olMatch.Groups[3].Value);
                    // recursively parse content inside the list.
                    var itemsObj = ParserEngine.Parse(block.Subtokens);
                    // create MarkdownObject
                    var mdObj = new MarkdownOrderedList(true, value, itemsObj);
                    // finished.
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