﻿using System;

namespace Markdown2HTML.Attributes
{
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