using System.Collections.Generic;
using System.IO;
using Markdown2HTML.Core.Debug;
using Markdown2HTML.Core.MarkdownObject;
using Markdown2HTML.Core.Tokens;

namespace Markdown2HTML.Core
{
    public class MarkdownEngine
    {
        public void MarkdownToHtml(string inputMarkdownFilePath, string outputMarkdownFilePath)
        {
            string output = MarkdownToHtmlString(File.ReadAllText(inputMarkdownFilePath));
            Logger.LogVerbose('\n' + output);
            File.WriteAllText(outputMarkdownFilePath, output);
        }

        public string MarkdownToHtmlString(string markdownString)
        {
            var lexer = new LexerEngine();
            var parser = new ParserEngine();
            var renderer = new RenderEngine();

            List<MarkdownToken> mdTokens = lexer.Lex(markdownString);
            List<IMarkdownObject> mdObjs = parser.Parse(mdTokens);
            string renderedHtml = renderer.Render(mdObjs);
            return renderedHtml;
        }

    }
}