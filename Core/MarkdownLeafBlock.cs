namespace Markdown2HTML.Core.Tokens
{
    public class MarkdownLeafBlock : MarkdownToken
    {
        /// <summary>
        /// String with inlined applied
        /// </summary>
        public string Content;

        public MarkdownLeafBlock(string tokenType, string content, int rawLength) :
            base(tokenType, rawLength)
        {
            Content = content;
        }
        public override string ToString()
        {
            return $"TokenType: {TokenType}\nContent:\n\"{Content}\"\nRawLength: {RawLength}";
        }

    }
}