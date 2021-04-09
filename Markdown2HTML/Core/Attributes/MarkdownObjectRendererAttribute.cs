using System;

namespace Markdown2HTML.Core.Attributes
{
    /// <summary>
    /// Used by the Renderer to define the type of MarkdownObject they handle.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MarkdownObjectRendererAttribute : Attribute
    {
        public readonly Type MarkdownObjectType;
        public MarkdownObjectRendererAttribute(Type markdownObjectType)
        {
            this.MarkdownObjectType = markdownObjectType;
        }
    }
}