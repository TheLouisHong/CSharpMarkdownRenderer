using System.Collections.Generic;
using System.IO;
using Markdown2HTML.Core.Debug;
using Markdown2HTML.Core.Interfaces;
using Markdown2HTML.Core.Tokens;

namespace Markdown2HTML.Core.Engines
{
    public class MarkdownEngine
    {
        /// <summary>
        /// Renders Markdown to HTML, and writes to file path.
        /// </summary>
        /// <param name="inputMarkdownFilePath">markdown document file path.</param>
        /// <param name="outputMarkdownFilePath">HTML document output file path.</param>
        public void MarkdownToHtml(string inputMarkdownFilePath, string outputMarkdownFilePath)
        {
            string output = MarkdownToHtmlString(File.ReadAllText(inputMarkdownFilePath));
            Logger.LogVerbose('\n' + output);
            File.WriteAllText(outputMarkdownFilePath, output);
        }

        /// <summary>
        /// Renders markdown string to HTML string.
        /// </summary>
        /// <param name="markdownString">Markdown document string</param>
        /// <returns></returns>
        public string MarkdownToHtmlString(string markdownString)
        {

            List<MarkdownToken> mdTokens = LexerEngine.Lex(markdownString);
            List<IMarkdownObject> mdObjs = ParserEngine.Parse(mdTokens);
            string renderedHtml = RenderEngine.Render(mdObjs);
            return renderedHtml;
        }

    }
}