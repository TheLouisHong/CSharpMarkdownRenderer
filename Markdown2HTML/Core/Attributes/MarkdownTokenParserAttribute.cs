using System;

namespace Markdown2HTML.Core.Attributes
{
    /// <summary>
    /// Used by the parser to define the MarkdownToken they parse.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MarkdownTokenParserAttribute : Attribute
    {
        public readonly string TokenType;
        public MarkdownTokenParserAttribute(string _tokenType)
        {
            TokenType = _tokenType;
        }
    }
}