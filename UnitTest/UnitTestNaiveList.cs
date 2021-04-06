using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Markdown2HTML.Core.Engines;
using Markdown2HTML.Lexers.BlockLexers;

namespace UnitTest
{
    [TestClass]
    public class UnitTestNaiveList
    {
        [TestMethod]
        public void TestMethodUnordered01()
        {
            var str =
                "Paragraph Before\n\n* A\n* B\n* C";
            
            var tokens = LexerEngine.Lex(str);
            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }
        }

        [TestMethod]
        public void TestMethodUnordered02()
        {
            var str =
                "Paragraph Before\n* A\n* B\n* C";
            
            var tokens = LexerEngine.Lex(str);
            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }
        }

        [TestMethod]
        public void TestMethodUnordered03()
        {
            var str =
                "1. A\n2. B\n3. C";
            
            var tokens = LexerEngine.Lex(str);
            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }
        }

        [TestMethod]
        public void TestMethodUnordered04()
        {
            var str =
                "Paragraph Before\n1. A\n2. B\n3. C";
            
            var tokens = LexerEngine.Lex(str);
            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }
        }

        [TestMethod]
        public void TestMethodUnordered05()
        {
            var str =
                "Paragraph Before\n\n1. A\n2. B\n3. C";
            
            var tokens = LexerEngine.Lex(str);
            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }
        }

        [TestMethod]
        public void TestMethodUnordered06()
        {
            var str =
                "- list item\nlazy\nreally lazy  \ndouble space br above \n\noutside";
            
            var tokens = LexerEngine.Lex(str);
            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }
        }

        [TestMethod]
        public void TestMethodUnordered07()
        {
            var str =
                "- list item\n\n  loose lazy\nreally lazy  \nbonker br\n\n  more loose lazy\n\noutside";
            
            var tokens = LexerEngine.Lex(str);
            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }
        }

        [TestMethod]
        public void TestMethodUnordered08()
        {
            var str =
                "* parent\n  * one\n  * two\n* second parent\n\nnormal paragraph";
            
            var tokens = LexerEngine.Lex(str);
            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }
        }

        [TestMethod]
        public void TestUnorderedList01()
        {
            var engine = new MarkdownEngine();
            var output = engine.MarkdownToHtmlString("* parent\n  * one\n  * two\n* second parent\n\nnormal paragraph");
            Console.Write(output);
        }


        [TestMethod]
        public void TestOrderedList01()
        {
            var engine = new MarkdownEngine();
            var output = engine.MarkdownToHtmlString("1. parent\n  2. one\n  3. two\n* bullet\n\nnormal paragraph");
            Console.Write(output);
        }
    }
}
