using System.Text;
using System.Text.RegularExpressions;

namespace Markdown2HTML.Core.Tokens
{
    public class MarkdownToken
    {
        /// <summary>
        /// type of token
        /// </summary>
        public string TokenType; 

        /// <summary>
        /// String with inlined applied
        /// </summary>
        public string Text;

        /// <summary>
        /// Optional, can be passed from the lexer to the parser for optimization.
        /// </summary>
        public Match RegexMatch;

        public MarkdownToken(string tokenType, string text, Match regexMatch)
        {
            TokenType = tokenType;
            Text = text;
            RegexMatch = regexMatch;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder($"TokenType: {TokenType}\n Text: {Text}\n Match: ");

            if (RegexMatch == null)
            {
                stringBuilder.Append("none");
            }
            else
            {
                stringBuilder.Append('\n');
                for (int i = 0; i < RegexMatch.Groups.Count; i++)
                {
                    stringBuilder.AppendLine($"    {i}:{RegexMatch.Groups[i]}");
                }
            }

            return stringBuilder.ToString();
        }
    }
}