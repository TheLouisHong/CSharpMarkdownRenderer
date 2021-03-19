using System;
using System.Collections.Generic;
using System.IO;
using Markdown2HTML.MarkdownObject;
using Markdown2HTML.Token;

namespace Markdown2HTML
{
    public class MarkdownEngine
    {
        public void MarkdownToHtml(string inputMarkdownFilePath, string outputMarkdownFilePath)
        {
            string output = MarkdownToHtmlString(File.ReadAllText(inputMarkdownFilePath));
            File.WriteAllText(outputMarkdownFilePath, output);
        }

        public string MarkdownToHtmlString(string markdownString)
        {
            var lexer = new LexerEngine();
            var parser = new ParserEngine();
            var renderer = new RenderEngine();

            List<MarkdownToken> mdTokens = lexer.Lex(markdownString);
            PrintTokens(mdTokens);

            List<IMarkdownObject> mdObjs = parser.Parse(mdTokens);
            string renderedHtml = renderer.Render(mdObjs);

            return renderedHtml;
        }

        public void PrintTokens(List<MarkdownToken> tokens)
        {
            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            } 
        }
    }
}