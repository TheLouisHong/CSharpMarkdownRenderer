using Markdown2HTML.Core.Tokens;

namespace Markdown2HTML.Core.Interfaces
{
    public interface IMarkdownTokenParser
    {
        IMarkdownObject Parse(MarkdownToken token);
    }
}