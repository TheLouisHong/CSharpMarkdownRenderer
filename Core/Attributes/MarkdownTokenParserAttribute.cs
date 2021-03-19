using System;

namespace Markdown2HTML.Attributes
{
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