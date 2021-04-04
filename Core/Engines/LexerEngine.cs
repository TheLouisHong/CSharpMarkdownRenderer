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
    public static class LexerEngine
    {
        private static List<KeyValuePair<int, IBlockLexer>> _blockLexers = new List<KeyValuePair<int, IBlockLexer>>();

        static LexerEngine()
        {
            CollectBlockLexers();
        }

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

        private static void LuxAux(string markdownString, ref List<MarkdownToken> tokens)
        {
            while (markdownString.Length > 0) {
                bool processing = false;
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

                if (!processing)
                {
                    Logger.LogError("Infinite loop detected in Lexer.");
                    Logger.LogError($"Discarding String: \n{markdownString}");
                    break;
                }
            }
        }

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