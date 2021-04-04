using System.Collections.Generic;
using System.Text;

namespace Markdown2HTML.Core.Tokens
{
    public class MarkdownContainerBlock : MarkdownToken
    {
        public List<MarkdownToken> Subtokens;
        public MarkdownContainerBlock(string tokenType, int rawLength, List<MarkdownToken> subtokens) : base(tokenType, rawLength)
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
                sb.AppendLine($"    Item {i}:\n{{\n{Subtokens[i]}\n}}");
            }

            return sb.ToString();
        }
    }
}