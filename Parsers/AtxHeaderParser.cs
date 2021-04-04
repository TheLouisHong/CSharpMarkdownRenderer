using System;
using System.Text.RegularExpressions;
using Markdown2HTML.Core;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.Interfaces;
using Markdown2HTML.Core.Tokens;

namespace Markdown2HTML.Parsers
{
    [MarkdownTokenParser(TokenTypeHelper.ATX_HEADER)]
    public class AtxHeaderParser : IMarkdownTokenParser
    {

        /// <summary>
        /// Precisely matching the 6 rules <see cref="AtxHeaderLexer"/>.
        ///
        /// group 1: #s
        /// group 2: content
        /// </summary>
        private readonly Regex _contentParser = new Regex(
            @"^ {0,3}(#{1,6})(?: (.*?))??(?: +#* *)?(?:\n|$)"
            );

        /// <summary>
        /// Trim and parse header token.
        /// </summary>
        /// <param name="token">Markdown Token from <see cref="AtxHeaderLexer"/></param>
        /// <returns></returns>
        public IMarkdownObject Parse(MarkdownToken token)
        {
            if (!(token is MarkdownLeafBlock leafBlock))
            {
                throw new InvalidOperationException("Only parses leaf-blocks.");
            }

            var match = _contentParser.Match(leafBlock.Content);

            // missing groups, aka index out of bounds, here would mean parser is malfunctioning
            int level = match.Groups[1].Length;

            string content = "";

            // missing content, aka, empty header.
            // example: ## ###
            if (match.Groups.Count == 2)
            {
                content = "";
            }
            else
            {
                // rule 5 , see AtxHeaderLexer.cs
                content = match.Groups[2].Value.Trim(' ', '\n'); // BUG Does not trip unicode whitespace
            }
            
            var header = new MarkdownAtxHeader(level, content);
            return header;
        }
    }

    public class MarkdownAtxHeader : IMarkdownObject
    {
        public int Level;
        public string Content;

        public MarkdownAtxHeader(int level, string content)
        {
            Level = level;
            Content = content;
        }

        public override string ToString()
        {
            return $"{nameof(MarkdownAtxHeader)}\ncontent: {Content}\nlevel: {Level}";
        }
    }
}