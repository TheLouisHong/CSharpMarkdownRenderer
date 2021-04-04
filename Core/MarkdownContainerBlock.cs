using System.Collections.Generic;

namespace Markdown2HTML.Core.Tokens
{
    public class MarkdownContainerBlock : MarkdownToken
    {
        public List<MarkdownToken> Subtokens;
        public MarkdownContainerBlock(string tokenType, int rawLength, List<MarkdownToken> subtokens) : base(tokenType, rawLength)
        {
            Subtokens = subtokens;
        }
    }
}