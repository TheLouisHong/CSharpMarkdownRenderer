using System;
using System.Text.RegularExpressions;
using Markdown2HTML.Attributes;
using Markdown2HTML.Token;

namespace Markdown2HTML.Lexers.BlockLexers
{

    [BlockLexer]
    public class AtxHeaderLexer : IBlockLexer
    {
        //group 1: #
        //group 2: text with space
        Regex match = new Regex(@"^ {0,3}(#{1,6})(?=\s|$)(.*)(?:\n+|$)");


        public Match Match(string markdownString)
        {
            return match.Match(markdownString);
        }

        public MarkdownToken Lex(ref string markdownString, Match match = null)
        {
            if (match == null)
            {
                match = Match(markdownString);
            }

            //for (int i = 0; i < match.Length; i++)
            //{
            //    Console.WriteLine($"{i} : {match.Groups[i]}");
            //}

            // consume the captured string
            markdownString = markdownString.Substring(match.Length);

            MarkdownToken token = new MarkdownToken();

            token.Text = match.Groups[0].Value;
            token.TokenType = TokenTypes.HEADER;

            return token;
        }
    }
}