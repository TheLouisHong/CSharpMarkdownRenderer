using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.Debug;
using Markdown2HTML.Core.Interfaces;
using Markdown2HTML.Core.Tokens;

namespace Markdown2HTML.Core.Engines
{
    /// <summary>
    /// Parses MarkdownTokens into MarkdownObjects.
    /// 
    /// This class collects it's parsers by using reflection.
    ///
    /// Parsers that want to be collected and used by the ParserEngine must fit the following criteria:
    /// 1. Implements the IMarkdownTokenParser interface
    /// 2. Class is marked with the MarkdownTokenParserAttribute attribute.
    /// </summary>
    public static class ParserEngine
    {
        /// <summary>
        /// Pool of parsers
        /// </summary>
        private static Dictionary<string, IMarkdownTokenParser> _markdownTokenParsers = new Dictionary<string, IMarkdownTokenParser>();

        static ParserEngine()
        {
            // collect parsers within the assembly. 
            CollectParsers();
        }

        /// <summary>
        /// Parses MarkdownTokens into MarkdownObjects.
        /// </summary>
        /// <param name="tokens">Tokens to be parsed.</param>
        /// <returns>List of markdown objects</returns>
        public static List<IMarkdownObject> Parse(List<MarkdownToken> tokens)
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
                        Logger.LogError($"{parser.GetType().Name}.Parse returned null.");
                    }
                }
                else
                {
                    Logger.LogError($"Missing Parser for {token.TokenType}.");
                }
            }

            LogObjects(results);

            return results;
        }

        /// <summary>
        /// Reflection function that collects all the parser scripts within the assembly.
        /// </summary>
        private static void CollectParsers()
        {
            // get class that inherits from IBlockLexer and has BlockLexerAttribute
            var blockLexersTypes =
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from @type in assembly.GetTypes()
                where typeof(IMarkdownTokenParser).IsAssignableFrom(@type) && !@type.IsInterface
                let attributes = @type.GetCustomAttributes(typeof(MarkdownTokenParserAttribute), true)
                where attributes != null && attributes.Length > 0
                select new {Type = @type, Attributes = attributes.Cast<MarkdownTokenParserAttribute>()};

            // Instantiate parser and record the order the attribute defines.
            foreach (var parserType in blockLexersTypes)
            {
                var parser = (IMarkdownTokenParser) Activator.CreateInstance(parserType.Type);
                _markdownTokenParsers.Add(parserType.Attributes.First().TokenType, parser);
            }

        }

        private static void LogObjects(IEnumerable<IMarkdownObject> mdObjects)
        {
            var sb = new StringBuilder();
            foreach (var mdObj in mdObjects)
            {
                sb.AppendLine(mdObj.ToString());
                sb.AppendLine("======================");
            }
            Logger.LogVerbose(sb.ToString());
        }

    }
}