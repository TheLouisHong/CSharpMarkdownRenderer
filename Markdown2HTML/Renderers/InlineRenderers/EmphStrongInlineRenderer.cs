using System;
using System.Text;
using System.Text.RegularExpressions;
using Markdown2HTML.Core.Algorithms;
using Markdown2HTML.Core.Extensions;
using Markdown2HTML.Core.Interfaces;

namespace Markdown2HTML.InlineRenderers
{
    /// <summary>
    /// Renders Emphasis and Strong (bold and italic).
    ///
    /// Algorithm Described at the bottom of the page, named "Process Emphasis"
    /// https://spec.commonmark.org/0.29/#an-algorithm-for-parsing-nested-emphasis-and-links
    ///
    /// Importance Difference Between * and _
    ///     * allows intraword match. 
    ///     _ does not allow intraword match.
    ///
    ///     ex: outside**strong**outside -> outside<strong>strong</strong>outside
    ///     ex: outside__normal__outside -> outside__normal__outside
    ///  
    /// CommonMark 0.29 as *Reference* (example 350-480), with LOTS of exceptions.
    ///
    ///     Read CommonMark Example 350-480 diff At the bottom of this source file.
    /// 
    ///     @TODO Do not support url
    ///     @TODO Do not support code or code block 
    ///     @BUG Cannot handle "*why_like*this_" will produce incorrect HTML (that happens to be syntactically correct)
    ///     @TODO Cannot handle " **foo**bar**" correctly, need to add modulo-3 rule. 
    ///         https://spec.commonmark.org/0.29/#example-410
    ///     @TODO Cannot handle " ***strong**emp*" correctly 
    ///     @TODO Do not support escaping using backslash, like so "\*"
    ///     @TODO Consolidate star and line functions into one unified pipeline
    /// </summary>
    public class EmphStrongInlineRenderer : IInlineRenderer
    {
        private static readonly Regex StarTokenPattern = new Regex(
            @"(\*+)");

        private static readonly Regex LineTokenPattern = new Regex(
            @"(_+)");

        public string Render(string content)
        {
            // BUG bug in situation: *word_like*this_ -> <em>word<em>like</em>this</em>
            // luckily, this makes no difference in terms of HTML rendering and syntax.

            // follow two func calls need to interact to resolve that issue.
            content = RenderAux(content, StarTokenPattern, '*', true);
            content = RenderAux(content, LineTokenPattern, '_', false);
            return content;
        }

        /// <summary>
        /// Helper function to render * and _ with similar logic.
        /// </summary>
        /// <param name="content">markdown to be rendered</param>
        /// <param name="tokenPattern">Regex to match the delim runs. Example for star: (\*+)</param>
        /// <param name="delimChar">The delim character, must be the same as the token pattern.</param>
        /// <param name="allowIntraword">intraword means *is*this*allowed*. If this the * around this is rendered, then intraword is enabled</param>
        /// <returns>rendered string</returns>
        private static string RenderAux(string content, Regex tokenPattern, char delimChar, bool allowIntraword)
        {
            var tokens = TokenizeByDelim(content, tokenPattern);
            var delimStack = BuildDelimStack(tokens, allowIntraword, delimChar);

            DoubleLinkedList<DelimStackNode>.Node openerBottom = null;
            //DoubleLinkedList<DelimStackNode>.Node stackBottom = null;

            // look for closer
            var closer = delimStack.Head;
            while (closer != null)
            {
                if (!closer.Value.Potential.HasFlag(DelimStackNode.DelimPotential.Closer))
                {
                    closer = closer.Next;
                }
                else // found closer
                {
                    // look for opener
                    var opener = closer.Prev;

                    var foundOpener = false;
                    while (opener != null && opener != openerBottom)
                    {
                        if (!opener.Value.Potential.HasFlag(DelimStackNode.DelimPotential.Opener))
                        {
                            opener = opener.Prev;
                        }
                        else // found opener
                        {
                            var openerTextNode = opener.Value.TextNode;
                            var closerTextNode = closer.Value.TextNode;

                            var openerStr = openerTextNode.Value;
                            var closerStr = closerTextNode.Value;

                            var strength = Math.Min(openerStr.Length, closerStr.Length);

                            // is strong
                            if (strength >= 2)
                            {
                                // consume two delims
                                openerTextNode.Value = openerStr.Substring(0, openerStr.Length - 2);
                                closerTextNode.Value = closerStr.Substring(0, closerStr.Length - 2);

                                openerTextNode.InsertAfter("<strong>");
                                closerTextNode.InsertBefore("</strong>");
                            }
                            // or emph
                            else
                            {
                                // consume two delims
                                openerTextNode.Value = openerStr.Substring(0, openerStr.Length - 1);
                                closerTextNode.Value = closerStr.Substring(0, closerStr.Length - 1);

                                openerTextNode.InsertAfter("<em>");
                                closerTextNode.InsertBefore("</em>");
                            }

                            // can be used to fix the bug *like_this*in the future_
                            // any other delim between can no longer be used
                            // var extraDelim = opener.Next.Value.textNode;
                            // while (extraDelim != closer.Value.textNode)
                            // {
                            //      ... remove other competing delims that are between opener and closer
                            // }

                            // remove empty delim text nodes 
                            // all used up
                            if (openerTextNode.Value.Length == 0)
                            {
                                delimStack.Remove(opener);
                            }
                            if (closerTextNode.Value.Length == 0)
                            {
                                delimStack.Remove(closer);
                                closer = closer.Next;
                            }

                            foundOpener = true;
                            break;
                        }
                    }

                    if (!foundOpener)
                    {
                        openerBottom = closer.Prev;
                        closer = closer.Next;
                    }
                }
            }
            
            return RenderTokenList(tokens);
        }

        private static bool IsOpener(DoubleLinkedList<string>.Node curr, bool allowIntraword)
        {
            /*
         * A left-flanking delimiter run is a delimiter run that is
         * (1) not followed by Unicode whitespace, and either
         * (2a) not followed by a punctuation character,
         * or
         * (2b) followed by a punctuation character and preceded by Unicode whitespace or a punctuation character.
         * For purposes of this definition, the beginning and the end of the line count as Unicode whitespace.
         */
            if (curr.Next == null)
            {
                return false;
            }

            // negated (1)
            if (char.IsWhiteSpace(curr.Next.Value.FirstChar()))
            {
                return false;
            }

            if (!allowIntraword // disallow intraword
                && curr.Prev != null // has left neighbor 
                && !(char.IsWhiteSpace(curr.Prev.Value.LastChar()) // not whitespace
                     || char.IsPunctuation(curr.Prev.Value.LastChar()))) // or punctuation
            {
                return false; // intraword found
            }

            // (2a) not followed by whitespace and not followed by punctuation
            if (!char.IsPunctuation(curr.Next.Value.FirstChar())) 
            {
                return true;
            }
            // statements below imply followed by a punctuation

            // (2b) followed by punctuation and preceded by whitespace or punctuation 
            // For purposes of this definition, the beginning and the end of the line count as Unicode whitespace.
            if (curr.Prev == null 
                || char.IsWhiteSpace(curr.Prev.Value.LastChar()) 
                || char.IsPunctuation(curr.Prev.Value.LastChar()))
            {
                return true;
            }

            return false;

        }

        private static bool IsCloser(DoubleLinkedList<string>.Node curr, bool allowIntraword)
        {
            /*
         * A right-flanking delimiter run is a delimiter run that is
         * (1) not preceded by Unicode whitespace, and either
         * (2a) not preceded by a punctuation character,
         * or
         * (2b) preceded by a punctuation character and followed by Unicode whitespace or a punctuation character.
         * For purposes of this definition, the beginning and the end of the line count as Unicode whitespace.
         */
            if (curr.Prev == null)
            {
                return false;
            }

            // negated (1)
            if (char.IsWhiteSpace(curr.Prev.Value.LastChar()))
            {
                return false;
            }

            if (!allowIntraword // disallow intraword
                && curr.Next != null // has left neighbor 
                && !(char.IsWhiteSpace(curr.Next.Value.FirstChar()) // not whitespace
                     || char.IsPunctuation(curr.Next.Value.FirstChar()))) // or punctuation
            {
                return false; // intraword found
            }

            // (2a) not preceded by a punctuation character
            if (!char.IsPunctuation(curr.Prev.Value.LastChar())) 
            {
                return true;
            }
            // statements below imply preceded by a punctuation

            // (2b) preceded by a punctuation and followed by whitespace or a punctuation
            // For purposes of this definition, the beginning and the end of the line count as Unicode whitespace.
            if (curr.Next == null 
                || char.IsWhiteSpace(curr.Next.Value.FirstChar()) 
                || char.IsPunctuation(curr.Next.Value.FirstChar()))
            {
                return true;
            }

            return false;

        }

        private static DoubleLinkedList<DelimStackNode> BuildDelimStack(DoubleLinkedList<string> tokens, bool allowIntraword, char delimChar)
        {
            var delimStack = new DoubleLinkedList<DelimStackNode>();

            var curr = tokens.Head;
            while (curr != null)
            {
                if (curr.Value.StartsWith(delimChar.ToString()))
                {
                    var potential = DelimStackNode.DelimPotential.None;

                    if (IsCloser(curr, allowIntraword))
                    {
                        potential |= DelimStackNode.DelimPotential.Closer;
                    }

                    if (IsOpener(curr, allowIntraword))
                    {
                        potential |= DelimStackNode.DelimPotential.Opener;
                    }

                    var delimNode = new DelimStackNode(curr, potential);

                    delimStack.Append(delimNode);
                }
                curr = curr.Next;
            }

            return delimStack;

        }


        /// <summary>
        /// Render token list back into string.
        /// </summary>
        /// <param name="tokenList">list of tokens</param>
        /// <returns>rendered string</returns>
        public static string RenderTokenList(DoubleLinkedList<string> tokenList)
        {
            var sb = new StringBuilder();
            foreach (var str in tokenList)
            {
                sb.Append(str);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Tokenize text using token pattern into a double-linked-list.
        /// For instance:
        ///     *this**text*
        ///     ->
        ///     token: *
        ///     token: this
        ///     token: **
        ///     token: text
        ///     token: *
        /// 
        /// This is so the renderer can efficiently manipulate text around the delims.
        /// </summary>
        /// <param name="content">content to be rendered.</param>
        /// <param name="delimMatch">Regex to match the delim runs. Example for star: (\*+)</param>
        /// <returns>Tokenized text stored in double-linked-list.</returns>
        public static DoubleLinkedList<string> TokenizeByDelim(string content, Regex delimMatch)
        {
            var tokenized = new DoubleLinkedList<string>();
            var matches = delimMatch.Matches(content);

            var prevIndex = 0;
            foreach (Match match in matches)
            {
                // before
                if (match.Index - prevIndex > 0)
                {
                    var prefix = content.Substring(prevIndex, match.Index - prevIndex);
                    tokenized.Append(prefix);
                }

                // after
                tokenized.Append(match.Value);
                prevIndex = match.Index + match.Length;
            }

            if (prevIndex != content.Length)
            {
                tokenized.Append(content.Substring(prevIndex, content.Length - prevIndex));
            }

            return tokenized;
        }

        /// <summary>
        /// DelimStack only stores delimiters.
        /// DelimStack has a reference to the nodes in the tokenized text double-linked-list.
        /// The DelimStack is used to stores and retrieve runtime parsing information during the parse. 
        /// </summary>
        private class DelimStackNode
        {
            /// <summary>
            /// Reference to the tokenized text token.
            /// </summary>
            public DoubleLinkedList<string>.Node TextNode;

            // <summary>
            // Unused. Will be needed for URL parsing in the future.
            // </summary>
            //public bool Active;

            /// <summary>
            /// Depending on the parse rules, a delim can either be opener, closer, or both.
            /// example:
            /// ***potential opener
            /// potential closer***
            /// potentially***both
            /// * none
            ///
            /// This variable stores such property about the delim.
            /// </summary>
            public DelimPotential Potential;

            /// <summary>
            /// Creates a DelimStackNode.
            /// </summary>
            /// <param name="textNode">TextNode this delim references in the tokenized text double-linked-list</param>
            /// <param name="potential">Stores parsing information about delim. <see cref="Potential"/></param>
            public DelimStackNode(DoubleLinkedList<string>.Node textNode, DelimPotential potential)
            {
                TextNode = textNode;
                //Active = active;
                Potential = potential;
            }

            /// <summary>
            /// Stored parsing information about this delim.
            /// Depending on the parse rules, a delim can either be opener, closer, or both.
            /// example:
            /// ***potential opener
            /// potential closer***
            /// potentially***both
            /// * none
            /// </summary>
            [Flags]
            public enum DelimPotential
            {
                /// <summary>
                /// * none
                /// </summary>
                None = 0,

                /// <summary>
                /// ***potential opener
                /// </summary>
                Opener = 1 << 0, 

                /// <summary>
                /// potential closer***
                /// </summary>
                Closer = 1 << 1, 

                /// <summary>
                /// potentially***both
                /// </summary>
                Both = Opener | Closer
            }

            public override string ToString()
            {
                return $"{{Value={TextNode.Value}, Potential={Potential}}} ";
            }
        }
    }
}


// CommonMark diff
/*
--- example403.txt	2021-03-22 11:01:00.063413500 -0700
+++ myout403.txt	2021-03-31 04:27:28.865928600 -0700
@@ -3,6 +3,6 @@
 *foo [bar](/url)*
 
 ------------------------------
-<p><em>foo <a href="/url">bar</a></em></p>
+<p><em>foo [bar](/url)</em></p>
 
 ------------------------------
--- example410.txt	2021-03-22 11:01:00.071392000 -0700
+++ myout410.txt	2021-03-31 04:27:29.064397200 -0700
@@ -3,6 +3,6 @@
 *foo**bar**baz*
 
 ------------------------------
-<p><em>foo<strong>bar</strong>baz</em></p>
+<p><em>foo</em><em>bar</em><em>baz</em></p>
 
 ------------------------------
--- example411.txt	2021-03-22 11:01:00.072389500 -0700
+++ myout411.txt	2021-03-31 04:27:29.086339000 -0700
@@ -3,6 +3,6 @@
 *foo**bar*
 
 ------------------------------
-<p><em>foo**bar</em></p>
+<p><em>foo</em><em>bar</em></p>
 
 ------------------------------
--- example414.txt	2021-03-22 11:01:00.075380600 -0700
+++ myout414.txt	2021-03-31 04:27:29.160684100 -0700
@@ -3,6 +3,6 @@
 *foo**bar***
 
 ------------------------------
-<p><em>foo<strong>bar</strong></em></p>
+<p><em>foo</em><em>bar</em>**</p>
 
 ------------------------------
--- example418.txt	2021-03-22 11:01:00.081463900 -0700
+++ myout418.txt	2021-03-31 04:27:29.247358300 -0700
@@ -3,6 +3,6 @@
 *foo [*bar*](/url)*
 
 ------------------------------
-<p><em>foo <a href="/url"><em>bar</em></a></em></p>
+<p><em>foo [<em>bar</em>](/url)</em></p>
 
 ------------------------------
--- example421.txt	2021-03-22 11:01:00.083460200 -0700
+++ myout421.txt	2021-03-31 04:27:29.317680700 -0700
@@ -3,6 +3,6 @@
 **foo [bar](/url)**
 
 ------------------------------
-<p><strong>foo <a href="/url">bar</a></strong></p>
+<p><strong>foo [bar](/url)</strong></p>
 
 ------------------------------
--- example428.txt	2021-03-22 11:01:00.090442300 -0700
+++ myout428.txt	2021-03-31 04:27:29.487226800 -0700
@@ -3,6 +3,6 @@
 **foo*bar*baz**
 
 ------------------------------
-<p><strong>foo<em>bar</em>baz</strong></p>
+<p><em><em>foo</em>bar</em>baz**</p>
 
 ------------------------------
--- example432.txt	2021-03-22 11:01:00.095427600 -0700
+++ myout432.txt	2021-03-31 04:27:29.577787300 -0700
@@ -3,6 +3,6 @@
 **foo [*bar*](/url)**
 
 ------------------------------
-<p><strong>foo <a href="/url"><em>bar</em></a></strong></p>
+<p><strong>foo [<em>bar</em>](/url)</strong></p>
 
 ------------------------------
--- example436.txt	2021-03-22 11:01:00.100412700 -0700
+++ myout436.txt	2021-03-31 04:27:29.669051000 -0700
@@ -3,6 +3,6 @@
 foo *\**
 
 ------------------------------
-<p>foo <em>*</em></p>
+<p>foo <em>\</em>*</p>
 
 ------------------------------
--- example439.txt	2021-03-22 11:01:00.103404800 -0700
+++ myout439.txt	2021-03-31 04:27:29.735873100 -0700
@@ -3,6 +3,6 @@
 foo **\***
 
 ------------------------------
-<p>foo <strong>*</strong></p>
+<p>foo <strong>\</strong>*</p>
 
 ------------------------------
--- example448.txt	2021-03-22 11:01:00.114376300 -0700
+++ myout448.txt	2021-03-31 04:27:30.008144400 -0700
@@ -3,6 +3,6 @@
 foo _\__
 
 ------------------------------
-<p>foo <em>_</em></p>
+<p>foo <em>\</em>_</p>
 
 ------------------------------
--- example451.txt	2021-03-22 11:01:00.117367600 -0700
+++ myout451.txt	2021-03-31 04:27:30.076959800 -0700
@@ -3,6 +3,6 @@
 foo __\___
 
 ------------------------------
-<p>foo <strong>_</strong></p>
+<p>foo <strong>\</strong>_</p>
 
 ------------------------------
--- example460.txt	2021-03-22 11:01:00.127342000 -0700
+++ myout460.txt	2021-03-31 04:27:30.311334000 -0700
@@ -3,6 +3,6 @@
 *_foo_*
 
 ------------------------------
-<p><em><em>foo</em></em></p>
+<p><em>_foo_</em></p>
 
 ------------------------------
--- example468.txt	2021-03-22 11:01:00.137314000 -0700
+++ myout468.txt	2021-03-31 04:27:30.491612400 -0700
@@ -3,6 +3,6 @@
 *foo _bar* baz_
 
 ------------------------------
-<p><em>foo _bar</em> baz_</p>
+<p><em>foo <em>bar</em> baz</em></p>
 
 ------------------------------
--- example469.txt	2021-03-22 11:01:00.138310900 -0700
+++ myout469.txt	2021-03-31 04:27:30.521697800 -0700
@@ -3,6 +3,6 @@
 *foo __bar *baz bim__ bam*
 
 ------------------------------
-<p><em>foo <strong>bar *baz bim</strong> bam</em></p>
+<p>*foo <strong>bar <em>baz bim</strong> bam</em></p>
 
 ------------------------------
--- example472.txt	2021-03-22 11:01:00.142300800 -0700
+++ myout472.txt	2021-03-31 04:27:30.590514100 -0700
@@ -3,6 +3,6 @@
 *[bar*](/url)
 
 ------------------------------
-<p>*<a href="/url">bar*</a></p>
+<p><em>[bar</em>](/url)</p>
 
 ------------------------------
--- example473.txt	2021-03-22 11:01:00.143298300 -0700
+++ myout473.txt	2021-03-31 04:27:30.609465300 -0700
@@ -3,6 +3,6 @@
 _foo [bar_](/url)
 
 ------------------------------
-<p>_foo <a href="/url">bar_</a></p>
+<p><em>foo [bar</em>](/url)</p>
 
 ------------------------------
--- example474.txt	2021-03-22 11:01:00.145292300 -0700
+++ myout474.txt	2021-03-31 04:27:30.629410900 -0700
@@ -3,6 +3,6 @@
 *<img src="foo" title="*"/>
 
 ------------------------------
-<p>*<img src="foo" title="*"/></p>
+<p><em>&lt;img src=&quot;foo&quot; title=&quot;</em>&quot;/&gt;</p>
 
 ------------------------------
--- example475.txt	2021-03-22 11:01:00.146290300 -0700
+++ myout475.txt	2021-03-31 04:27:30.654892500 -0700
@@ -3,6 +3,6 @@
 **<a href="**">
 
 ------------------------------
-<p>**<a href="**"></p>
+<p><strong>&lt;a href=&quot;</strong>&quot;&gt;</p>
 
 ------------------------------
--- example476.txt	2021-03-22 11:01:00.147288500 -0700
+++ myout476.txt	2021-03-31 04:27:30.675837400 -0700
@@ -3,6 +3,6 @@
 __<a href="__">
 
 ------------------------------
-<p>__<a href="__"></p>
+<p><strong>&lt;a href=&quot;</strong>&quot;&gt;</p>
 
 ------------------------------
--- example477.txt	2021-03-22 11:01:00.148284400 -0700
+++ myout477.txt	2021-03-31 04:27:30.693788300 -0700
@@ -3,6 +3,6 @@
 *a `*`*
 
 ------------------------------
-<p><em>a <code>*</code></em></p>
+<p><em>a `</em>`*</p>
 
 ------------------------------
--- example478.txt	2021-03-22 11:01:00.150279300 -0700
+++ myout478.txt	2021-03-31 04:27:30.712737200 -0700
@@ -3,6 +3,6 @@
 _a `_`_
 
 ------------------------------
-<p><em>a <code>_</code></em></p>
+<p><em>a `_`</em></p>
 
 ------------------------------
--- example479.txt	2021-03-22 11:01:00.151276800 -0700
+++ myout479.txt	2021-03-31 04:27:30.730699600 -0700
@@ -3,6 +3,6 @@
 **a<http://foo.bar/?q=**>
 
 ------------------------------
-<p>**a<a href="http://foo.bar/?q=**">http://foo.bar/?q=**</a></p>
+<p><strong>a&lt;http://foo.bar/?q=</strong>&gt;</p>
 
 ------------------------------
--- example480.txt	2021-03-22 11:01:00.152274000 -0700
+++ myout480.txt	2021-03-31 04:27:30.748644100 -0700
@@ -3,6 +3,6 @@
 __a<http://foo.bar/?q=__>
 
 ------------------------------
-<p>__a<a href="http://foo.bar/?q=__">http://foo.bar/?q=__</a></p>
+<p><strong>a&lt;http://foo.bar/?q=</strong>&gt;</p>
 
 ------------------------------

 */
