using System;

namespace Markdown2HTML.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class BlockLexerAttribute : Attribute
    {
        public readonly int Order;
        public BlockLexerAttribute(int order)
        {
            Order = order;
        }
    }
}