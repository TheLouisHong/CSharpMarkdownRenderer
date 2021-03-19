namespace Markdown2HTML.Token
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

        public override string ToString()
        {
            return $"TokenType: {TokenType}\n Text: {Text}";
        }
    }
}