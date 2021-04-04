using System.Text.RegularExpressions;
using Markdown2HTML.Core;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.Interfaces;
using Markdown2HTML.Core.Tokens;

namespace Markdown2HTML.Lexers.BlockLexers
{

    /// <summary>
    /// A sequence of non-blank lines that cannot be interpreted as other kinds of blocks forms a paragraph.
    /// The paragraph’s raw content is formed by concatenating the lines and removing initial and final whitespace.
    /// 
    /// 1. lowest in order. other blocks take priority.
    /// 2. [0,3] starting spaces allowed for start lines.
    /// 3. can be interrupted by these:
    ///  3.1 hr
    ///  3.2 heading
    ///  3.3 blockquote
    ///  3.4 fences
    ///  3.5 list
    ///  3.5 html
    ///  3.6 whitespace line
    ///  3.7 double newline
    /// 5. arbitrary white spaces allowed for after the first
    /// 6. consume up to, but not including, the last newline.
    ///
    /// philosophy: regex patterns must be nameable,
    ///              no monster regex allowed. 
    ///
    /// Lexer: Sole purpose is to determines how many characters belongs inside the block.
    ///        Do not do any rendering or parsing inside the lexer.
    ///
    /// CommonMark 0.29 Compliant (example 189-196), with exceptions.
    ///     @TODO example 195, code block not implemented.
    /// </summary>
    [BlockLexer( order: (int) BlockLexerOrderHelper.ParagraphLexer) ]
    public class ParagraphLexer : IBlockLexer
    {
        /// <summary>
        /// match block start, but not used to consume.
        ///
        /// regex explained:
        ///     find one line, that isn't all white space, 
        ///     ending in either a single newline or EOF.
        ///
        /// P.S.
        /// Not used to consume due to difficulty with suffixing newline edge cases with single line vs multi-line.
        /// </summary>
        private readonly Regex _startLine = new Regex(
            @"^[\s]*[^\s].*(?:\n|$)"
            ); 

        /// <summary>
        /// used to find block ending.
        ///
        /// regex explained:
        ///     find one line, with arbitrary amount of whitespace prefix, that isn't pure-whitespace,
        ///     ending in exactly two newlines or EOF.
        /// </summary>
        private readonly Regex _endLine = new Regex(
            @"^[\s]*[^\s].*(?=\n{2}|$)"
            );

        /// <summary>
        /// Used to find pure whitespace lines.
        /// 
        /// regex explained:
        ///     find lines that are purely empty or only white space, ending with arbitrary amount of new lines or EOF.
        /// </summary>
        private readonly Regex _emptyLine = new Regex(
            @"^\s*(?:\n+|$)"
            );


        
        /// <summary>
        /// Header interrupts paragraphs. Use header lexer to check for interrupts.
        /// </summary>
        private readonly AtxHeaderLexer _headerInterrupt = new AtxHeaderLexer();
        /// <summary>
        /// List interrupts paragraphs. Use header lexer to check for interrupts.
        /// </summary>

        private readonly NaiveListLexer _naiveListLexer = new NaiveListLexer();

        /// <summary>
        /// Lex paragraphs.
        /// 
        /// examples:
        /// case 1: aaa\n
        ///         ^ $
        ///  1 height paragraph
        /// 
        /// case 2: aaa\n\n
        ///         ^ $
        /// 1 height paragraph
        /// 
        /// case 3: aaa\nbbb\n\n
        ///         ^      $
        ///  2+ height paragraph
        /// 
        /// case 4: aaa  \nbbb
        ///         ^        $
        ///  br, because of 2+ spaces
        /// 
        /// case 5: aaa
        ///         ^ $
        ///  end of file.
        /// </summary>
        /// <param name="markdownString"></param>
        /// <returns></returns>
        public MarkdownToken Lex(string markdownString)
        {

            // ... is this a paragraph?
            if (!_startLine.IsMatch(markdownString)) return null;

            // discard startMatch, immediately match for ending.
            int length = 0; // use to represent length of consumed string

            // consume until the interrupted.
            while (true)
            {
                var nextLine = markdownString.Substring(length);

                // ... is ending line?
                var endMatch = _endLine.Match(nextLine);
                if (endMatch.Success)
                {
                    // ending line, aka last line. Finished.
                    length += endMatch.Length;
                    break;
                }

                // ... is empty line?
                if (_emptyLine.IsMatch(nextLine))
                {
                    // skip and finish.
                    break;
                }

                // interrupted by other blocks?
                if (OtherBlockInterrupts(nextLine))
                {
                    // yes, skip and finish.
                    break;
                }

                // consume one more line.
                length += nextLine.IndexOf('\n') + 1;
            }
            // finish, lex token for lexer.
            var result = markdownString.Substring(0, length);
            return new MarkdownLeafBlock(TokenTypeHelper.PARAGRAPH, result, length);
        }

        /// <summary>
        /// checks for block interrupts by other lexers.
        /// // TODO Missing BlockInterrupts for paragraph, due to missing implementation.
        /// // 3.1 hr (missing)
        /// // 3.2 heading
        /// // 3.3 blockquote (missing)
        /// // 3.4 fences (missing)
        /// // 3.5 list
        /// // 3.5 html (missing)
        /// </summary>
        /// <param name="markdownString">Markdown Document String</param>
        /// <param name="lexerEngine"></param>
        /// <returns></returns>
        private bool OtherBlockInterrupts(string markdownString)
        {
            if (_headerInterrupt.Lex(markdownString) != null)
            {
                return true;
            }
            else if (_naiveListLexer.Lex(markdownString) != null)
            {
                return true;
            }
            return false;
        }
    }
}