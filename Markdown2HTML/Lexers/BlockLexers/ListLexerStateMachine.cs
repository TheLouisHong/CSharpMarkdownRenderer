using System;
using System.Text.RegularExpressions;
using Markdown2HTML.Core.Algorithms;
using Markdown2HTML.Core.Extensions;

namespace Markdown2HTML.Lexers.BlockLexers
{
    /// <summary>
    /// The state machine that lexes list items.
    /// </summary>
    public static class ListLexerStateMachine
    {
        public struct ItemData
        {

            public string MarkdownString;

            public int LegalPadding;

            public bool PrecededByEmpty;

            public bool IsTight;

            public ItemData(string markdownString, int legalPadding = 0, bool precededByEmpty = false, bool isTight = true)
            {
                MarkdownString = markdownString;
                LegalPadding = legalPadding;
                PrecededByEmpty = precededByEmpty;
                IsTight = isTight;
            }

            public override string ToString()
            {
                return MarkdownString;
            }
        }

        public class BulletLineState : DescreteState<ItemData>
        {
            /// <summary>
            /// group 1: left padding
            /// group 2: bullet
            /// group 3: right padding
            /// group 4: line content
            /// </summary>
            private readonly Regex _bulletPattern = new Regex(
                @"^(( *)((?:\d{1,9}[\.\)])|([\-\*+])) +)(.*)\n?");

            /// <summary>
            /// match a line that is not empty.
            /// </summary>
            private readonly Regex _tightContentPattern = new Regex(
                @"^ *[\S].*\n?");

            private bool _alreadyFoundBullet = false;

            public override bool Ask(ref ItemData transitionItemData)
            {
                // list item already has bullet
                if (_alreadyFoundBullet)
                {
                    return false;
                }

                if (_bulletPattern.IsMatch(transitionItemData.MarkdownString))
                {
                    return true;
                }

                return false;
            }

            /// <summary>
            /// lex the first line.
            /// </summary>
            /// <param name="data"></param>
            /// <returns></returns>
            public override void Run(ref ItemData data)
            {
                _alreadyFoundBullet = true;

                var match = _bulletPattern.Match(data.MarkdownString);
                if (!match.Success)
                {
                    throw new InvalidOperationException("Failed sanity check.");
                }

                data.LegalPadding = match.Groups[1].Length + match.Groups[2].Length + match.Groups[3].Length;

                data.MarkdownString = data.MarkdownString.SpliceRight(match.Length);
                // consumes all the tight lines
                // TODO: make tight content another state, but in this case, simple hack will do.
                while (true)
                {
                    if (_bulletPattern.IsMatch(data.MarkdownString))
                    {
                        break;
                    }
                    match = _tightContentPattern.Match(data.MarkdownString);
                    if (match.Success)
                    {
                        data.MarkdownString = data.MarkdownString.SpliceRight(match.Length);
                    }
                    else
                    {
                        break;
                    }
                }
            }

        }

        public class LooseContentLineState : DescreteState<ItemData>
        {
            private readonly Regex _looseContentRegex = new Regex(
                @"^[\s]*([\S]+).*\n?");

            public override bool Ask(ref ItemData data)
            {
                if (!data.PrecededByEmpty)
                {
                    return false;
                }

                if (!_looseContentRegex.IsMatch(data.MarkdownString))
                {
                    return false;
                }

                for (int i = 0; i < data.LegalPadding; i++)
                {
                    if (data.MarkdownString[i] != ' ')
                    {
                        return false;
                    }
                }


                return false;
            }

            public override void Run(ref ItemData data)
            {
                var match = _looseContentRegex.Match(data.MarkdownString);
                while (match.Success)
                {
                    data.MarkdownString = data.MarkdownString.SpliceRight(match.Length);
                    match = _looseContentRegex.Match(data.MarkdownString);
                }
            }

        }

        public class EmptyLinesState : DescreteState<ItemData>
        {
            /// <summary>
            /// [^\S\n] is double negative, meaning all white except newline
            /// </summary>
            private readonly Regex _emptyLineRegex = new Regex(
                @"^[^\S\n]*\n");

            public override bool Ask(ref ItemData data)
            {
                return _emptyLineRegex.IsMatch(data.MarkdownString);
            }

            public override void Run(ref ItemData data)
            {
                var match = _emptyLineRegex.Match(data.MarkdownString);
                if (match.Success)
                {
                    data.MarkdownString = data.MarkdownString.SpliceRight(match.Length);
                    data.PrecededByEmpty = true;
                    data.IsTight = false;
                }
                else
                {
                    throw new InvalidOperationException("Failed sanity check.");
                }
            }
        }
    }
}