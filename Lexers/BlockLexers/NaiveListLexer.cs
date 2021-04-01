using System.Collections.Generic;
using System.Text.RegularExpressions;
using Markdown2HTML.Core;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.Debug;
using Markdown2HTML.Core.Engines;
using Markdown2HTML.Core.Interfaces;
using Markdown2HTML.Core.Tokens;

namespace Markdown2HTML.Lexers.BlockLexers
{

    /// <summary>
    /// list lexer that does not support nested lists.
    ///
    /// TODO Be CommonMark Compliant
    /// https://spec.commonmark.org/0.29/#list-items
    /// </summary>
    [BlockLexer( (int) BlockLexerOrderHelper.ListLexer )]
    public class NaiveListLexer : IBlockLexer
    {
        private Regex _unorderedListStart = new Regex(
            @"(?:\n|^) {0,3}([-\*+]) \s*(.*)");

        private Regex _orderedListStart = new Regex(
            @"(?:\n|^) {0,3}(\d{1,9}[)\.]) \s*(.*)");

        private Regex _endLine = new Regex(
            @"(?:\n)[\s]*[^\s].*(?=\n{2}|$)");

        public MarkdownToken Lex(string markdownString)
        {
            // start matching
            if (true)
            {
                return null;
            }

            List<MarkdownToken> children = new List<MarkdownToken>();
            var content = "";
            var subcontent = "";

            // lex children, recursively
            lexerEngine.LexBlocks(subcontent, ref children);

            var token = new MarkdownToken(TokenTypeHelper.LIST, content, content.Length, children);

            return token;

        }

    }
}