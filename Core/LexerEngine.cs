using System;
using System.Collections.Generic;
using System.Linq;
using Markdown2HTML.Attributes;
using Markdown2HTML.Token;

namespace Markdown2HTML
{
    public class LexerEngine
    {
        private List<IBlockLexer> _blockLexers = new List<IBlockLexer>();
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
            markdownString = Preprocess(markdownString);

            var tokens = new List<MarkdownToken>();
            LexBlocks(markdownString, tokens);
            LexInline(tokens);

            return tokens;
        }

        public void LexBlocks(string markdownString, List<MarkdownToken> tokens)
        {
            while (markdownString.Length > 0) {
                bool processing = false;
                foreach (var blockLexer in _blockLexers)
                {
                    var group = blockLexer.Match(markdownString);
                    if (group != null && group.Success)
                    {
                        var token = blockLexer.Lex(ref markdownString, group);
                        if (token != null)
                        {
                            tokens.Add(token);
                            processing = true;
                        }
                    }
                }

                if (!processing)
                {
                    Console.Error.WriteLine("Infinite loop detected in Lexer.");
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
                var lexer = (IBlockLexer) Activator.CreateInstance(lexersType.Type);
                _blockLexers.Add(lexer);
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
                _blockLexers.Add(lexer);
            }

        }
    }
}