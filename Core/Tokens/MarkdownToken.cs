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

        public MarkdownToken(string tokenType, string content, int rawLength)
        {
            TokenType = tokenType;
            Content = content;
            RawLength = rawLength;
        }

        public override string ToString()
        {
            return $"TokenType: {TokenType}\n Content: {Content}\n RawLength: {RawLength}";
        }
    }
}