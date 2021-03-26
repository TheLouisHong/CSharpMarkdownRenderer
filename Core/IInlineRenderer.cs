using Markdown2HTML.Core.Tokens;

namespace Markdown2HTML.Core
{
    public interface IInlineRenderer
    {
        string Render(string content);
    }
}