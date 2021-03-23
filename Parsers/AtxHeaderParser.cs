using System;
using Markdown2HTML.Core;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.MarkdownObject;
using Markdown2HTML.Core.Tokens;

namespace Markdown2HTML.Parsers
{
    [MarkdownTokenParser(TokenTypeHelper.HEADER)]
    public class AtxHeaderParser : IMarkdownTokenParser
    {
        public IMarkdownObject Parse(MarkdownToken token)
        {
            throw new NotImplementedException();
        }
    }

    public class AtxMarkdownHeader : IMarkdownObject
    {
        public int Level;
        public string Text;

        public AtxMarkdownHeader(int level, string text)
        {
            Level = level;
            Text = text;
        }

        public override string ToString()
        {
            return $"level {Level} \"{Text}\"";
        }
    }
}