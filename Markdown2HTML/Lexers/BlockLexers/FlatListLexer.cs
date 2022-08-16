/*
Because lists are so complicated, I've decide to write some additional documentation here.
- Louis Hong

Definitions:
List is a container block.
Meaning it in of it self does not contain anything as content but other leaf blocks. Think of it like C# List<Token>.

TODO List Tight/Loose is not supported
1. Lists can be either loose or tight. 
   • Loose meaning all text content are wrapped in <p></p>
     • A list becomes loose when any of it's text contains an empty white-space line.
   • Tight meaning all text are are plain text inside <li></li>
     • vice versa to loose-lists.

TODO nested list not supported
2. Lists are recursive, lists can contain lists, so the lexing is recursive as well.

3. Lists also have requirement for content the follows the bullet point. 
   1. It must be padded correctly
   2. Bullet points must remain consistent. (TODO Louis: This is not implemented for unordered lists.)

               (the above list is a loose list because of the new line between 1. and 2. and 3.)

Here is an example of the padding requirement for nested content. 
=========================================================================
Permalink to this example
(1)
     |   1.    bullet point defines legal padding for it's nested content.

      ^^^^^^    bullet length, 3 spaces indent (of [0-3] allowed), 3 char bullet = 6 length bullet
            ^^^ 3 space padding 

• Calculation: 6+3=9, the next item needs to be indented 12 spaces.
• Every bullet-item sets the padding requirements for it's nested content.


so say, the next line is nested bullet, it must be indented 12 spaces before it can be considered a nested item.

(2)
     |         * appropriately indented nested list, a nested tight list.
      ^^^^^^^^^  9 spaces

same goes for the first line of loose-content
• iff when an white-space line has been introduced in any of the list's bullet-items, the list is considered a loose-list.

so say, the next following line is an white-space line, following another line with loose content.

(3)
     |
     |         this list is now a loose list because of the empty line above,
      ^^^^^^^^^ 9 spaces
     |note the line following the first line of loose content need not be padded as long no empty lines interrupt.
     |also note, the nested list defined in (2), is still tight despite it's parent (1) is now loose. 
     |2. the next list-item does not have the same padding requirements and can define it's own padding,
     |also, if there are no empty lines, content do not need padding.



same example, without my comments
|   1.    bullet point defines legal padding for it's nested content.
|         * appropriately indented nested list, a nested tight list.
|
|         this list is now a loose list because of the empty line above,
|note the line following the first line of loose content need not be padded as long no empty lines interrupt.
|also note, the nested list defined in (2), is still tight despite it's parent (1) is now loose. 
|2. the next list-item does not have the same padding requirements and can define it's own padding,
|also, if there are no empty lines, content do not need padding.
 
HTML Output: 
| <ol>
| <li>
| <p>bullet point defines legal padding for it's nested content.</p>
   ^  notice the wrapped paragraph, this is because this is a loose list.

| <ul>
| <li>appropriately indented nested list, a nested tight list.</li>
   ^ notice the lack of wrapped paragraph, this is a tight list.

| </ul>
| <p>this list is now a loose list because of the empty line above,
| note the line following the first line of loose content need not be padded as long no empty lines interrupt.
| also note, the nested list defined in (2), is still tight despite it's parent (1) is now loose.</p>
| </li>
| <li>
   ^ notice this list-item is still part of our parent list, despite not padded the same.
     notice the empty line is consumed. markdown consumes arbitrary amount of whitespace lines.

| <p>the next list-item does not have the same padding requirements and can define it's own padding,
| also, if there are no empty lines, content do not need padding.</p>
| </li>
| </ol>
| <ol>
| <li>
| <p>content</p>
| <ul>
| <li>appropriately indented nested list, a nested tight list.</li>
| </ul>
| <p>this list is now a loose list because of the empty line above,
| note the following line need not be padded as long no empty lines interrupt.
| also note, the nested list is still tight despite it's parent is now loose.</p>
| </li>
| <li>
| <p>the next list-item does not have such requirements,
| it may start where ever, but must be the same bullet type.
| this is still be a tight list-item, but is forced to be loose because of the previous list-item's empty line.</p>
| </li>
| </ol>

The way to interrupt a list is simply breaking any of the above defined rules!
Good luck! 
*/


using System.Collections.Generic;
using System.Text.RegularExpressions;
using Markdown2HTML.Core;
using Markdown2HTML.Core.Algorithms;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.Extensions;
using Markdown2HTML.Core.Interfaces;
using Markdown2HTML.Core.Tokens;
using Markdown2HTML.Helpers;
using Markdown2HTML.Parsers;

// state machine is organized to another file for cleanliness
using static Markdown2HTML.Lexers.BlockLexers.ListLexerStateMachine;

namespace Markdown2HTML.Lexers.BlockLexers
{


    /// <summary>
    /// list lexer that does not support nested lists or recursive lexing,
    /// but is written so that it could be extended to do recursive lexing.
    ///
    /// TODO Be CommonMark Compliant
    /// https://spec.commonmark.org/0.29/#list-items
    /// </summary>
    [BlockLexer( (int) BlockLexerOrderHelper.ListLexer )]
    public class FlatListLexer : IBlockLexer
    {
        private readonly Regex _listPattern = new Regex(
            @"^( *((?:\d{1,9}[\.\)])|([\-\*+])) +)(.*)");

        /// <summary>
        /// Lexes a markdown list.
        /// </summary>
        /// <param name="markdownString"></param>
        /// <returns></returns>
        public MarkdownToken Lex(string markdownString)
        {
            // create children for this block, this is a recursive container block.
            var children = new List<MarkdownToken>();

            // list match
            var match = _listPattern.Match(markdownString);

            // ... is this list?
            if (match.Success)
            {
                var initialLength = markdownString.Length;

                // lex each item as it's own token, until the list is interrupted or items can no longer be lexed.
                MarkdownToken item;
                do
                {
                    // lex one list item.
                    item = LexListItem(ref markdownString, ref children);
                    if (item != null)
                    {
                        children.Add(item);
                    }

                } while (item != null);

                // create a container tokens of list items.
                var listToken = new MarkdownContainerToken(TokenTypeHelper.LIST, initialLength - markdownString.Length, children);
                // finished
                return listToken;
            }
            else // not a list, skip
            {
                return null;
            }
        }

        /// <summary>
        /// Lex a single list item, assumes that the passed in string contains a list at the top.
        /// </summary>
        /// <param name="markdownString"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        private MarkdownToken LexListItem(ref string markdownString, ref List<MarkdownToken> children)
        {
            // build a list lex state machine
            var lexStateMachine = BuildStateStateMachine();
            
            // create state machine data
            var data = new ItemData(markdownString);

            // lex item
            lexStateMachine.Run(ref data); // state machine lexing

            // check if any items have been lexed
            var leftOver = data.MarkdownString;
            var consumedLength = markdownString.Length - leftOver.Length;

            // no items have been lexed, finished.
            if (consumedLength == 0)
            {
                return null;
            }

            // items were lexed
            var content = markdownString.Substring(0, consumedLength);
            markdownString = markdownString.SpliceRight(consumedLength);
            // create list item token
            var listToken = new MarkdownLeafToken(TokenTypeHelper.TIGHT_LIST_ITEM, content, content.Length);

            // finished.
            return listToken;
        }

        /// <summary>
        /// Build lexing state machine.
        /// </summary>
        /// <returns></returns>
        private static PriorityDiscreteStateMachine<ItemData> BuildStateStateMachine()
        {
            var states = new List<DescreteState<ItemData>>
            {
                new BulletLineState(), // first search for bullet
                new EmptyLinesState(), // if not bullet, check empty line
                new LooseContentLineState(), // then check for loose content
            };

            var lexStateMachine = new PriorityDiscreteStateMachine<ItemData>(states);
            return lexStateMachine;
        }
    }
}