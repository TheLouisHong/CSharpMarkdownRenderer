using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Markdown2HTML.Core.Tokens
{
    public class MarkdownToken
    {
        /// <summary>
        /// type of token
        /// </summary>
        public readonly string TokenType; 

        /// <summary>
        /// String with inlined applied
        /// </summary>
        public readonly string Content;

        /// <summary>
        /// Length of markdown consumed during lexing, including white space truncation.
        /// </summary>
        public readonly int RawLength;

        /// <summary>
        /// sub-tokens list. Stores tokens recursively. Used for nested blocks, like list items.
        /// </summary>
        public List<MarkdownToken> Child; // TODO

        public MarkdownToken(string tokenType, string content, int rawLength, List<MarkdownToken> children)
        {
            TokenType = tokenType;
            Content = content;
            RawLength = rawLength;
            Child = children;
        }


        public override string ToString()
        {
            return $"TokenType: {TokenType}\n Content: {Content}\n RawLength: {RawLength}";
        }
    }
}