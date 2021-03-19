using System;
using System.Collections.Generic;
using System.Linq;
using Markdown2HTML.Attributes;
using Markdown2HTML.MarkdownObject;
using Markdown2HTML.Token;

namespace Markdown2HTML
{
    public class ParserEngine
    {
        private Dictionary<string, IMarkdownTokenParser> _markdownTokenParsers = new Dictionary<string, IMarkdownTokenParser>();

        public ParserEngine()
        {
            InitParsers();
        }

        public List<IMarkdownObject> Parse(List<MarkdownToken> tokens)
        {
            var results = new List<IMarkdownObject>();

            foreach (var token in tokens)
            {
                if (_markdownTokenParsers.TryGetValue(token.TokenType, out var parser))
                {
                    var markdownObject = parser.Parse(token);
                    if (markdownObject != null)
                    {
                        results.Add(markdownObject);
                    }
                    else
                    {
                        Console.Error.WriteLine($"{parser.GetType().Name}.Parse returned null.");
                    }
                }
                else
                {
                    Console.Error.WriteLine($"Missing Parser for {token.TokenType}.");
                }
            }

            return results;
        }

        void InitParsers()
        {
            // get class that inherits from IBlockLexer and has BlockLexerAttribute
            var blockLexersTypes =
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from @type in assembly.GetTypes()
                where typeof(IMarkdownTokenParser).IsAssignableFrom(@type) && !@type.IsInterface
                let attributes = @type.GetCustomAttributes(typeof(MarkdownTokenParserAttribute), true)
                where attributes != null && attributes.Length > 0
                select new {Type = @type, Attributes = attributes.Cast<MarkdownTokenParserAttribute>()};

            foreach (var parserType in blockLexersTypes)
            {
                var parser = (IMarkdownTokenParser) Activator.CreateInstance(parserType.Type);
                _markdownTokenParsers.Add(parserType.Attributes.First().TokenType, parser);
            }

        }

    }
}