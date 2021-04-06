using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text.RegularExpressions;
using Markdown2HTML.InlineRenderers;

namespace UnitTest
{
    [TestClass]
    public class UnitTestEmphStrong
    {
        private static readonly Regex _starTokenPattern = new Regex(
            @"(\*+)");

        private static readonly Regex _lineTokenPattern = new Regex(
            @"(_+)");

        [TestMethod]
        public void TestMethod1()
        {
            var str = "**strong** big boy **very tall**";
            Console.WriteLine($"original:\n{str}");
            Console.WriteLine("Tokenize:\n");
            var stack = EmphStrongInlineRenderer.TokenizeByDelim(str, _starTokenPattern);
            foreach (var token in stack)
            {
                Console.WriteLine($"Token:{token}");
            }
        }

        [TestMethod]
        public void TestMethod2_EmptyTest()
        {
            var str = "";
            Console.WriteLine($"original:\n{str}");
            Console.WriteLine("Tokenize:\n");
            var stack = EmphStrongInlineRenderer.TokenizeByDelim(str, _starTokenPattern);
            foreach (var token in stack)
            {
                Console.WriteLine($"Token:{token}");
            }
        }

        [TestMethod]
        public void TestMethod3()
        {
            var str = "*";
            Console.WriteLine($"original:\n{str}");
            Console.WriteLine("Tokenize:\n");
            var stack = EmphStrongInlineRenderer.TokenizeByDelim(str, _starTokenPattern);
            foreach (var token in stack)
            {
                Console.WriteLine($"Token:{token}");
            }
        }

        [TestMethod]
        public void TestMethod4()
        {
            var str = "just letters";
            Console.WriteLine($"original:\n{str}");
            Console.WriteLine("Tokenize:\n");
            var stack = EmphStrongInlineRenderer.TokenizeByDelim(str, _starTokenPattern);
            foreach (var token in stack)
            {
                Console.WriteLine($"Token:{token}");
            }
        }

        [TestMethod]
        public void TestMethod5_NewLine()
        {
            var str = "**strong**\n**very\ntall**";
            Console.WriteLine($"original:\n{str}");
            Console.WriteLine("Tokenize:\n");
            var stack = EmphStrongInlineRenderer.TokenizeByDelim(str, _starTokenPattern);
            foreach (var token in stack)
            {
                Console.WriteLine($"Token:{token}");
            }
        }


        [TestMethod]
        public void TestMethod6_Line_WrongRegex()
        {
            var str = "__strong__ big boy __very tall__";
            Console.WriteLine($"original:\n{str}");
            Console.WriteLine("Tokenize:\n");
            var stack = EmphStrongInlineRenderer.TokenizeByDelim(str, _starTokenPattern);
            foreach (var token in stack)
            {
                Console.WriteLine($"Token:{token}");
            }
        }

        [TestMethod]
        public void TestMethod7_Line()
        {
            var str = "__strong__ big boy __very tall__";
            Console.WriteLine($"original:\n{str}");
            Console.WriteLine("Tokenize:\n");
            var stack = EmphStrongInlineRenderer.TokenizeByDelim(str, _lineTokenPattern);
            foreach (var token in stack)
            {
                Console.WriteLine($"Token:{token}");
            }
        }

        [TestMethod]
        public void TestMethod8_Concat()
        {
            var str = "__strong__ big boy __very tall__";
            Console.WriteLine($"original:\n{str}");
            Console.WriteLine("Tokenize:\n");
            var stack = EmphStrongInlineRenderer.TokenizeByDelim(str, _lineTokenPattern);
            foreach (var token in stack)
            {
                Console.WriteLine($"Token:{token}");
            }

            var concat = EmphStrongInlineRenderer.RenderTokenList(stack);
            Console.WriteLine($"Concat:\n{concat}");

            Assert.AreEqual(str, concat);
        }

        [TestMethod]
        public void TestMethod9_Concat()
        {
            var str = "**strong** big boy **very tall**";
            Console.WriteLine($"original:\n{str}");
            Console.WriteLine("Tokenize:\n");
            var stack = EmphStrongInlineRenderer.TokenizeByDelim(str, _starTokenPattern);
            foreach (var token in stack)
            {
                Console.WriteLine($"Token:{token}");
            }

            var concat = EmphStrongInlineRenderer.RenderTokenList(stack);
            Console.WriteLine($"Concat:\n{concat}");

            Assert.AreEqual(str, concat);
        }

        [TestMethod]
        public void TestRender01()
        {
            var str = "**strong**";
            var correct = "<strong>strong</strong>";

            var inlineRenderer = new EmphStrongInlineRenderer();
            var rendered = inlineRenderer.Render(str);

            Console.WriteLine($"orig   : {str}");
            Console.WriteLine($"correct: {correct}");
            Console.WriteLine($"mine   : {rendered}");

            Assert.AreEqual(correct, rendered);
        }

        [TestMethod]
        public void TestRender02()
        {
            var str = "**strong** normal";
            var correct = "<strong>strong</strong> normal";

            var inlineRenderer = new EmphStrongInlineRenderer();
            var rendered = inlineRenderer.Render(str);

            Assert.AreEqual(correct, rendered);
        }

        [TestMethod]
        public void TestRender03()
        {
            var str = "normal **strong** normal";
            var correct = "normal <strong>strong</strong> normal";

            var inlineRenderer = new EmphStrongInlineRenderer();
            var rendered = inlineRenderer.Render(str);

            Console.WriteLine($"orig   : {str}");
            Console.WriteLine($"correct: {correct}");
            Console.WriteLine($"mine   : {rendered}");

            Assert.AreEqual(correct, rendered);
        }

        [TestMethod]
        public void TestRender04()
        {
            var str = "normal**strong**normal";
            var correct = "normal<strong>strong</strong>normal";

            var inlineRenderer = new EmphStrongInlineRenderer();
            var rendered = inlineRenderer.Render(str);

            Assert.AreEqual(correct, rendered);
        }

        [TestMethod]
        public void TestRender05()
        {
            var str = "normal**strong**";
            var correct = "normal<strong>strong</strong>";

            var inlineRenderer = new EmphStrongInlineRenderer();
            var rendered = inlineRenderer.Render(str);

            Console.WriteLine($"orig   : {str}");
            Console.WriteLine($"correct: {correct}");
            Console.WriteLine($"mine   : {rendered}");

            Assert.AreEqual(correct, rendered);
        }

        [TestMethod]
        public void TestRender06()
        {
            var str = "**strong**normal";
            var correct = "<strong>strong</strong>normal";

            var inlineRenderer = new EmphStrongInlineRenderer();
            var rendered = inlineRenderer.Render(str);

            Console.WriteLine($"orig   : {str}");
            Console.WriteLine($"correct: {correct}");
            Console.WriteLine($"mine   : {rendered}");

            Assert.AreEqual(correct, rendered);
        }

        [TestMethod]
        public void TestRender07()
        {
            var str = "normal";
            var correct = "normal";

            var inlineRenderer = new EmphStrongInlineRenderer();
            var rendered = inlineRenderer.Render(str);

            Console.WriteLine($"orig   : {str}");
            Console.WriteLine($"correct: {correct}");
            Console.WriteLine($"mine   : {rendered}");

            Assert.AreEqual(correct, rendered);
        }

        [TestMethod]
        public void TestRender08()
        {
            var str = "**weird*combo**";
            var correct = "<strong>weird*combo</strong>";

            var inlineRenderer = new EmphStrongInlineRenderer();
            var rendered = inlineRenderer.Render(str);

            Console.WriteLine($"orig   : {str}");
            Console.WriteLine($"correct: {correct}");
            Console.WriteLine($"mine   : {rendered}");

            Assert.AreEqual(correct, rendered);
        }

        [TestMethod]
        public void TestRender09()
        {
            var str = "**weird * combo**";
            var correct = "<strong>weird * combo</strong>";

            var inlineRenderer = new EmphStrongInlineRenderer();
            var rendered = inlineRenderer.Render(str);

            Console.WriteLine($"orig   : {str}");
            Console.WriteLine($"correct: {correct}");
            Console.WriteLine($"mine   : {rendered}");

            Assert.AreEqual(correct, rendered);
        }

        [TestMethod]
        public void TestRender10()
        {
            var str = "***strong emph***";
            var correct = "<em><strong>strong emph</strong></em>";

            var inlineRenderer = new EmphStrongInlineRenderer();
            var rendered = inlineRenderer.Render(str);

            Console.WriteLine($"orig   : {str}");
            Console.WriteLine($"correct: {correct}");
            Console.WriteLine($"mine   : {rendered}");

            Assert.AreEqual(correct, rendered);
        }

        [TestMethod]
        public void TestRender11()
        {
            var str = "***strong**emph*";
            var correct = "<em><strong>strong</strong>emph</em>";

            var inlineRenderer = new EmphStrongInlineRenderer();
            var rendered = inlineRenderer.Render(str);

            Console.WriteLine($"orig   : {str}");
            Console.WriteLine($"correct: {correct}");
            Console.WriteLine($"mine   : {rendered}");

            Assert.AreEqual(correct, rendered);
        }

        [TestMethod]
        public void TestRender12()
        {
            var str = "***strong emp***fake*";
            var correct = "<em><strong>strong emp</strong></em>fake*";

            var inlineRenderer = new EmphStrongInlineRenderer();
            var rendered = inlineRenderer.Render(str);

            Console.WriteLine($"orig   : {str}");
            Console.WriteLine($"correct: {correct}");
            Console.WriteLine($"mine   : {rendered}");

            Assert.AreEqual(correct, rendered);
        }

        [TestMethod]
        public void TestRender13()
        {
            var str = @"***strong emph***
***strong** in emph*
***emph* in strong**
**in strong *emph***
*in emph **strong***";
            var correct = @"<em><strong>strong emph</strong></em>
<em><strong>strong</strong> in emph</em>
<strong><em>emph</em> in strong</strong>
<strong>in strong <em>emph</em></strong>
<em>in emph <strong>strong</strong></em>";

            var inlineRenderer = new EmphStrongInlineRenderer();
            var rendered = inlineRenderer.Render(str);

            Console.WriteLine($"orig   : {str}");
            Console.WriteLine($"correct: {correct}");
            Console.WriteLine($"mine   : {rendered}");

            Assert.AreEqual(correct, rendered);
        }

        [TestMethod]
        public void TestRender14()
        {
            var str = "* a *";
            var correct = "* a *";

            var inlineRenderer = new EmphStrongInlineRenderer();
            var rendered = inlineRenderer.Render(str);

            Console.WriteLine($"orig   : {str}");
            Console.WriteLine($"correct: {correct}");
            Console.WriteLine($"mine   : {rendered}");

            Assert.AreEqual(correct, rendered);
        }

    }
}
