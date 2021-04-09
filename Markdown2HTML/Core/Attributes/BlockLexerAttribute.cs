using System;

namespace Markdown2HTML.Core.Attributes
{
    /// <summary>
    /// Used by the lexers to define the order to be ran.
    /// </summary>
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