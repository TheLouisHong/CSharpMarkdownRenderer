using Markdown2HTML.Core.MarkdownObject;
using Markdown2HTML.Core.Tokens;

namespace Markdown2HTML.Core
{
    public interface IMarkdownTokenParser
    {
        IMarkdownObject Parse(MarkdownToken token);
    }
}