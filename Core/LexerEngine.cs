using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.Debug;
using Markdown2HTML.Core.Tokens;

namespace Markdown2HTML.Core
{
    public class LexerEngine
    {
        private List<KeyValuePair<int, IBlockLexer>> _blockLexers = new List<KeyValuePair<int, IBlockLexer>>();
        private List<IInlineLexer> _inlineLexers = new List<IInlineLexer>();

        public LexerEngine()
        {
            InitBlockLexers();
            InitInlineLexers();
        }

        public string Preprocess(string markdownString)
        {
            // replace CRLF with LF
            markdownString = markdownString.Replace("\r\n", "\n");
            // replace tabs with 4 spaces
            markdownString = markdownString.Replace("\t", "    ");
            return markdownString;
        }

        public List<MarkdownToken> Lex(string markdownString)
        {
            Logger.LogVerbose("...Lexing");
            markdownString = Preprocess(markdownString);

            var tokens = new List<MarkdownToken>();
            Logger.LogVerbose("...LexingBlocks");

            LexBlocks(markdownString, tokens);
            Logger.LogVerbose("...LexingInlines");

            LexInline(tokens);

            LogTokens(tokens);

            return tokens;
        }

        public void LexBlocks(string markdownString, List<MarkdownToken> tokens)
        {
            while (markdownString.Length > 0) {
                bool processing = false;
                foreach (var blockLexer in _blockLexers)
                {
                    var match = blockLexer.Value.Match(markdownString);
                    if (match != null && match.Success)
                    {
                        var token = blockLexer.Value.Lex(markdownString, match);
                        markdownString = markdownString.Substring(match.Length);
                        if (token != null)
                        {
                            tokens.Add(token);
                            processing = true;
                            break; // 
                        }
                    }
                }

                if (!processing)
                {
                    Logger.LogError("Infinite loop detected in Lexer.");
                    Logger.LogError($"Discarding String: \n {markdownString}");
                    break;
                }
            }
        }

        private void LexInline(List<MarkdownToken> tokens)
        {
            foreach (var token in tokens)
            {
                foreach (var lexer in _inlineLexers)
                {
                    // lex the text inside the token
                    lexer.Lex(token);
                }
            }
        }

        private void InitInlineLexers()
        {
            // get class that inherits from IBlockLexer and has BlockLexerAttribute
            var blockLexersTypes =
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from @type in assembly.GetTypes()
                where typeof(IInlineLexer).IsAssignableFrom(@type) && !@type.IsInterface
                let attributes = @type.GetCustomAttributes(typeof(InlineLexerAttribute), true)
                where attributes != null && attributes.Length > 0
                select new {Type = @type, Attributes = attributes.Cast<InlineLexerAttribute>()};

            foreach (var lexersType in blockLexersTypes)
            {
                var lexer = (IInlineLexer) Activator.CreateInstance(lexersType.Type);
                _inlineLexers.Add(lexer);
            }

        }

        void InitBlockLexers()
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

        private void LogTokens(IEnumerable<MarkdownToken> tokens)
        {
            var sb = new StringBuilder();
            foreach (var token in tokens)
            {
                sb.AppendLine(token.ToString() + "\n----------------------\n");
            } 
            Logger.LogVerbose("Lexed Tokens: \n" + sb);
        }
    }
}