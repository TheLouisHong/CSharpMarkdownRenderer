using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Markdown2HTML.Core;
using Markdown2HTML.Core.Algorithms;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.Engines;
using Markdown2HTML.Core.Interfaces;
using Markdown2HTML.Core.Tokens;

namespace Markdown2HTML.Lexers.BlockLexers
{

// Because lists are so complicated, I've decide to write some additional documentation here.
// - Louis Hong
// 
// Definitions:
// List is a container item.
// Meaning it in of it self does not contain anything as content but other leaf blocks. Think of it like C# List<T>.
//
// 1. Lists can be either loose or tight. 
//    • Loose meaning all text content are wrapped in <p></p>
//      • A list becomes loose when any of it's text contains an empty white-space line.
//    • Tight meaning all text are are plain text inside <li></li>
//      • vice versa to loose-lists.
// 
// 2. Lists are recursive, lists can contain lists, so the lexing is recursive as well.
// 
// 3. Lists also have requirement for content the follows the bullet point. 
//    1. It must be padded correctly
//    2. Bullet points must remain consistent. (our our purposes, Bungie does not require this it for unordered lists according to the example provided.)
// 
//                (the above list is a loose list because of the new line between 1. and 2. and 3.)
// 
// Here is an example of the padding requirement for nested content. 
// =========================================================================
// Permalink to this example
// (1)
//      |   1.    bullet point defines legal padding for it's nested content.
//
//       ^^^^^^    bullet length, 3 spaces indent (of [0-3] allowed), 3 char bullet = 6 length bullet
//             ^^^ 3 space padding 
//
// • Calculation: 6+3=9, the next item needs to be indented 12 spaces.
// • Every bullet-item sets the padding requirements for it's nested content.
//
//
// so say, the next line is nested bullet, it must be indented 12 spaces before it can be considered a nested item.
// 
// (2)
//      |         * appropriately indented nested list, a nested tight list.
//       ^^^^^^^^^  9 spaces
//
// same goes for the first line of loose-content
// • iff when an white-space line has been introduced in any of the list's bullet-items, the list is considered a loose-list.
// 
// so say, the next following line is an white-space line, following another line with loose content.
//
// (3)
//      |
//      |         this list is now a loose list because of the empty line above,
//       ^^^^^^^^^ 9 spaces
//      |note the line following the first line of loose content need not be padded as long no empty lines interrupt.
//      |also note, the nested list defined in (2), is still tight despite it's parent (1) is now loose. 
//      |2. the next list-item does not have the same padding requirements and can define it's own padding,
//      |also, if there are no empty lines, content do not need padding.
//
//
//
// same example, without my comments
// |   1.    bullet point defines legal padding for it's nested content.
// |         * appropriately indented nested list, a nested tight list.
// |
// |         this list is now a loose list because of the empty line above,
// |note the line following the first line of loose content need not be padded as long no empty lines interrupt.
// |also note, the nested list defined in (2), is still tight despite it's parent (1) is now loose. 
// |2. the next list-item does not have the same padding requirements and can define it's own padding,
// |also, if there are no empty lines, content do not need padding.
//  
// HTML Output: 
// | <ol>
// | <li>
// | <p>bullet point defines legal padding for it's nested content.</p>
//    ^  notice the wrapped paragraph, this is because this is a loose list.
//
// | <ul>
// | <li>appropriately indented nested list, a nested tight list.</li>
//    ^ notice the lack of wrapped paragraph, this is a tight list.
//
// | </ul>
// | <p>this list is now a loose list because of the empty line above,
// | note the line following the first line of loose content need not be padded as long no empty lines interrupt.
// | also note, the nested list defined in (2), is still tight despite it's parent (1) is now loose.</p>
// | </li>
// | <li>
//    ^ notice this list-item is still part of our parent list, despite not padded the same.
//      notice the empty line is consumed. markdown consumes arbitrary amount of whitespace lines.
//
// | <p>the next list-item does not have the same padding requirements and can define it's own padding,
// | also, if there are no empty lines, content do not need padding.</p>
// | </li>
// | </ol>
// | <ol>
// | <li>
// | <p>content</p>
// | <ul>
// | <li>appropriately indented nested list, a nested tight list.</li>
// | </ul>
// | <p>this list is now a loose list because of the empty line above,
// | note the following line need not be padded as long no empty lines interrupt.
// | also note, the nested list is still tight despite it's parent is now loose.</p>
// | </li>
// | <li>
// | <p>the next list-item does not have such requirements,
// | it may start where ever, but must be the same bullet type.
// | this is still be a tight list-item, but is forced to be loose because of the previous list-item's empty line.</p>
// | </li>
// | </ol>
//
// The way to interrupt a list is simply breaking any of the above defined rules!
// Good luck!

    /// <summary>
    /// list lexer that does not support nested lists.
    ///
    /// TODO Be CommonMark Compliant
    /// https://spec.commonmark.org/0.29/#list-items
    /// </summary>
    [BlockLexer( (int) BlockLexerOrderHelper.ListLexer )]
    public class NaiveListLexer : IBlockLexer
    {
        private Regex _listPattern = new Regex(
            @"^( {0,3}((?:\d{1,9}[\.\)])|([\-\*+])) +)(.*)");

        // private Regex _endLine = new Regex(
        //     @"(?:\n)[\s]*[^\s].*(?=\n{2}|$)");

        public MarkdownToken Lex(string markdownString)
        {
            // create children for this block, this is a recursive container block.
            List<MarkdownToken> children = new List<MarkdownToken>();

            // list match
            var match = _listPattern.Match(markdownString);

            // ... is this list?
            if (match.Success)
            {
                MarkdownToken item = null;
                int initialLength = markdownString.Length;
                do
                {
                    item = LexListItem(ref markdownString, ref children);
                    if (item != null)
                    {
                        children.Add(item);
                    }

                } while (item != null);

                var listToken = new MarkdownContainerBlock(TokenTypeHelper.LIST, initialLength - markdownString.Length, children);
                return listToken;
            }
            else // not a list, skip
            {
                return null;
            }
        }

        private MarkdownToken LexListItem(ref string markdownString, ref List<MarkdownToken> children)
        {
            // strategy, consume line by line, and operate as a state machine.
            // state machine with three states
            //     bullet_line: incoming content matches bullet point. lex line. transition.
            //     content_line: incoming content matches padding. lex line. transition.
            //     empty_line: incoming content is white-space line, record streak. lex line. transition.
            //     finished: matches no other state, exiting state.

            var states = new List<DescreteState<ListItemData>>();
            states.Add(new BulletLine_State());
            states.Add(new ContentLine_State());
            states.Add(new EmptyLine_State());
            states.Add(new Finished_State());
            var lexStateMachine = new BlindDescreteStateMachine<ListItemData>(states);

            var bootData = new ListItemData
            {
                BulletPadding = 0,
                EmptyLineStreak = 0,
                MarkdownString = markdownString
            };

            var success = lexStateMachine.Run(bootData); // start lexing
            if (!success)
            {
                throw new InvalidOperationException("Failed sanity check, lexStateMachine failed on Run().");
            }

            string lexedString;
            if (lexStateMachine.CurrentState is Finished_State finishedState)
            {
                lexedString = finishedState.LeftOverMarkdownString;
            }
            else
            {
                throw new InvalidOperationException("Failed sanity check, lexStateMachine did not finish. (in limbo)");
            }


            var listToken = new MarkdownLeafBlock(TokenTypeHelper.LIST, lexedString, lexedString.Length);

            return listToken;
        }

        private MarkdownToken Lex_ol(string markdownString)
        {
            throw new System.NotImplementedException();
        }

        private class EmptyLine_State : DescreteState<ListItemData>
        {
            public override bool AskForTransition(ListItemData transitionItemData)
            {
                throw new NotImplementedException();
            }

            public override void OnEnter(ListItemData transitionItemData)
            {
                throw new NotImplementedException();
            }

            public override void Update()
            {
                throw new NotImplementedException();
            }

            public override void OnExit()
            {
                throw new NotImplementedException();
            }
        }

        private class ContentLine_State : DescreteState<ListItemData>
        {
            public override bool AskForTransition(ListItemData transitionItemData)
            {
                throw new NotImplementedException();
            }

            public override void OnEnter(ListItemData transitionItemData)
            {
                throw new NotImplementedException();
            }

            public override void Update()
            {
                throw new NotImplementedException();
            }

            public override void OnExit()
            {
                throw new NotImplementedException();
            }
        }

        private class BulletLine_State : DescreteState<ListItemData>
        {
            public override bool AskForTransition(ListItemData transitionItemData)
            {
                throw new NotImplementedException();
            }

            public override void OnEnter(ListItemData transitionItemData)
            {
                throw new NotImplementedException();
            }

            public override void Update()
            {
                throw new NotImplementedException();
            }

            public override void OnExit()
            {
                throw new NotImplementedException();
            }
        }

        private class Finished_State : DescreteState<ListItemData>
        {
            public string LeftOverMarkdownString = null;
            public override bool AskForTransition(ListItemData transitionData)
            {
                throw new NotImplementedException();
            }

            public override void OnEnter(ListItemData transitionData)
            {
                throw new NotImplementedException();
            }

            public override void Update()
            {
                throw new NotImplementedException();
            }

            public override void OnExit()
            {
                throw new NotImplementedException();
            }
        }
    }


    public struct ListItemData
    {
        
        public string MarkdownString;

        public int BulletPadding;

        public int EmptyLineStreak;

    }

}