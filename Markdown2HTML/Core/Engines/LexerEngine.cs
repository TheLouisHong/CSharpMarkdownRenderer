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
    /// A static class that lexes markdown strings.
    ///
    /// This class collects it's lexers by using reflection.
    ///
    /// Lexers that want to be collected and used by the lexer must fit the following criteria:
    /// 1. Implements the IMarkdownTokenParser interface
    /// 2. Class is marked with the MarkdownTokenParserAttribute attribute.
    /// </summary>
    public static class LexerEngine
    {
        private static List<KeyValuePair<int, IBlockLexer>> _blockLexers = new List<KeyValuePair<int, IBlockLexer>>();

        static LexerEngine()
        {
            // populates lexer pool with lexers in this assembly.
            CollectBlockLexers();
        }

        /// <summary>
        /// Preprocess the string to replace CRLF and LF, and tabs with four spaces.
        /// All lexers scripts should assume such.
        /// </summary>
        /// <param name="markdownString"></param>
        /// <returns></returns>
        private static string Preprocess(string markdownString)
        {
            // replace CRLF with LF
            markdownString = markdownString.Replace("\r\n", "\n");
            // replace tabs with 4 spaces
            markdownString = markdownString.Replace("\t", "    ");

            // https://spec.commonmark.org/0.29/#insecure-characters
            // For security reasons, the Unicode character U+0000
            // must be replaced with the REPLACEMENT CHARACTER (U+FFFD).
            markdownString = markdownString.Replace('\u0000', '\uFFFD');

            return markdownString;
        }

        /// <summary>
        /// Used by the user to lex Markdown string into MarkdownTokens.
        /// </summary>
        /// <param name="markdownString">markdown document string</param>
        /// <returns>List of lexed MarkdownTokens.</returns>
        public static List<MarkdownToken> Lex(string markdownString)
        {
            Logger.LogVerbose("...Lexing");

            // preprocess string
            markdownString = Preprocess(markdownString);

            // lex
            var tokens = new List<MarkdownToken>();
            LuxAux(markdownString, ref tokens);

            // debug
            LogTokens(tokens);

            return tokens;
        }

        /// <summary>
        /// Turned into function to Lex function easier to understand.
        /// </summary>
        /// <param name="markdownString">Markdown document string</param>
        /// <param name="tokens">output token list.</param>
        private static void LuxAux(string markdownString, ref List<MarkdownToken> tokens)
        {
            while (markdownString.Length > 0) {
                var processing = false;
                foreach (var blockLexer in _blockLexers)
                {
                    var token = blockLexer.Value.Lex(markdownString);
                    if (token != null)
                    {
                        markdownString = markdownString.Substring(token.RawLength);

                        tokens.Add(token);
                        processing = true;
                        break;
                    }
                }
                
                // if a piece of string is not lexed by any lexer, we discard the string and log as error.
                if (!processing)
                {
                    Logger.LogError("Infinite loop detected in Lexer.");
                    Logger.LogError($"Discarding String: \n{markdownString}");
                    break;
                }
            }
        }

        /// <summary>
        /// Reflection function that collects all the lexers scripts within the assembly.
        /// </summary>
        private static void CollectBlockLexers()
        {
            // get class that inherits from IBlockLexer and has BlockLexerAttribute
            var blockLexersTypes =
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from @type in assembly.GetTypes()
                where typeof(IBlockLexer).IsAssignableFrom(@type) && !@type.IsInterface 
                let attributes = @type.GetCustomAttributes(typeof(BlockLexerAttribute), true)
                where attributes != null && attributes.Length > 0
                select new {Type = @type, Attributes = attributes.Cast<BlockLexerAttribute>()};

            // Instantiate lexer and record the order the attribute defines.
            foreach (var lexersType in blockLexersTypes)
            {
                var lexer = (IBlockLexer) Activator.CreateInstance(lexersType.Type);
                _blockLexers.Add(new KeyValuePair<int, IBlockLexer>(lexersType.Attributes.First().Order, lexer));
            }

            // sort lexer by their defined ordered, defined by their BlockLexerAttribute
            _blockLexers.Sort((a, b) =>
            {
                if (a.Key > b.Key)
                {
                    return 1;
                }
                if (a.Key < b.Key)
                {
                    return -1;
                }
                return 0;
            });
        }

        private static void LogTokens(IEnumerable<MarkdownToken> tokens)
        {
            var sb = new StringBuilder();
            foreach (var token in tokens)
            {
                sb.AppendLine(token + "\n----------------------\n");
            } 
            Logger.LogVerbose("Lexed Tokens: \n" + sb);
        }
    }
}