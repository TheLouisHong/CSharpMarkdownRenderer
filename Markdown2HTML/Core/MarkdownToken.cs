namespace Markdown2HTML.Core.Tokens
{
    /// <summary>
    /// Output from the Lexer.
    /// Contains an TokenType string for the parser to identify the correct parser.
    ///
    /// This type is abstract base class for the <see cref="MarkdownLeafToken"/> and <see cref="MarkdownContainerToken"/>.
    /// </summary>
    public abstract class MarkdownToken
    {
        /// <summary>
        /// type of token
        /// </summary>
        public readonly string TokenType; 


        /// <summary>
        /// Length of markdown consumed during lexing, including white space truncation.
        /// </summary>
        public readonly int RawLength;

        protected MarkdownToken(string tokenType, int rawLength)
        {
            TokenType = tokenType;
            RawLength = rawLength;
        }
        public override string ToString()
        {
            return $"TokenType: {TokenType}\nRawLength: {RawLength}";
        }
    }
}