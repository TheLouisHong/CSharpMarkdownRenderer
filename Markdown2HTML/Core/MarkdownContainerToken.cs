using System.Collections.Generic;
using System.Text;
using Markdown2HTML.Core.Extensions;

namespace Markdown2HTML.Core.Tokens
{
    /// <summary>
    /// An abstract token that contains other tokens.
    /// An container token should not be in of itself, a be able to be parsed.
    // </summary>
    public class MarkdownContainerToken : MarkdownToken
    {
        public List<MarkdownToken> Subtokens;
        public MarkdownContainerToken(string tokenType, int rawLength, List<MarkdownToken> subtokens) : base(tokenType, rawLength)
        {
            Subtokens = subtokens;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(base.ToString());
            sb.AppendLine("List Content:");
            for (int i = 0; i < Subtokens.Count; i++)
            {
                sb.AppendLine($"Item {i}:\n{{\n{Subtokens[i].ToString().Indent(4)}\n}}");
            }

            return sb.ToString();
        }
    }
}