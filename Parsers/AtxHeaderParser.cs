using Markdown2HTML.Core;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.MarkdownObject;
using Markdown2HTML.Core.Tokens;

namespace Markdown2HTML.Parsers
{
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

    [MarkdownTokenParser(TokenTypeHelper.HEADER)]
    public class AtxHeaderParser : IMarkdownTokenParser
    {
        public IMarkdownObject Parse(MarkdownToken token)
        {
            int level = token.RegexMatch.Groups[1].Length;
            string text = token.RegexMatch.Groups[2].Value.Substring(1);

            return new AtxMarkdownHeader(level, text);
        }
    }
}