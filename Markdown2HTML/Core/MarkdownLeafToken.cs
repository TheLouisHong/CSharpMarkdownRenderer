namespace Markdown2HTML.Core.Tokens
{
    /// <summary>
    /// A leaf token contains content that can be parsed. Output from the Lexer.
    // </summary>
    public class MarkdownLeafToken : MarkdownToken
    {
        /// <summary>
        /// String with inlined applied
        /// </summary>
        public string Content;

        public MarkdownLeafToken(string tokenType, string content, int rawLength) :
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