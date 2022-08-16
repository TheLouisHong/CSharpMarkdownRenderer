using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Markdown2HTML.Core;
using Markdown2HTML.Core.Debug;
using Markdown2HTML.Core.Engines;
using Newtonsoft.Json;

namespace UnitTest
{
    [TestClass]
    public class UnitTestCommonMark
    {
        public class TestCase
        {
            [JsonProperty("markdown")] public string markdown;

            [JsonProperty("html")] public string html;

            [JsonProperty("example")] public int example;

            [JsonProperty("start_line")] public int start_line;

            [JsonProperty("end_line")] public int end_line;

            [JsonProperty("section")] public string section;
        }

        class UnitTestDriver
        {
            private const string TestFilePath = @"test.json";

            private List<TestCase> _testCases;


            public void LoadTestCase()
            {
                if (!File.Exists(TestFilePath))
                {
                    throw new Exception("Test file doesn't exist.");
                }

                string caseString = File.ReadAllText(TestFilePath);
                _testCases = JsonConvert.DeserializeObject<List<TestCase>>(caseString);
            }

            public void OutputExampleTestCase()
            {
                Directory.CreateDirectory("commonmark_examples");

                var sb = new StringBuilder();
                foreach (var @case in _testCases)
                {
                    sb.AppendLine(@case.section);
                    sb.AppendLine("------------------------------");
                    sb.AppendLine(@case.markdown);
                    sb.AppendLine("------------------------------");
                    sb.AppendLine(@case.html);
                    sb.AppendLine("------------------------------");
                    File.WriteAllText($"commonmark_examples/example{@case.example}.txt", sb.ToString());
                    Directory.CreateDirectory($"commonmark_examples/{@case.section}");
                    File.WriteAllText($"commonmark_examples/{@case.section}/example{@case.example}.txt", sb.ToString());
                    sb.Clear();
                }
            }

            public void OutputSingleExample(int example)
            {
                if (example > _testCases.Count || example < 0)
                {
                    throw new ArgumentException("Example Index out of range.");
                }

                var @case = _testCases[example - 1];

                Console.BackgroundColor = ConsoleColor.DarkCyan;
                Console.Write($"Testing Example {@case.example}");
                Console.ResetColor();
                Console.WriteLine();
                Directory.CreateDirectory("commonmark_examples");

                var markdown2html = new MarkdownEngine();
                var htmlOutput = markdown2html.MarkdownToHtmlString(@case.markdown);
                var sb = new StringBuilder();

                sb.AppendLine(@case.section);
                sb.AppendLine("------------------------------");
                sb.AppendLine(@case.markdown);
                sb.AppendLine("------------------------------");
                sb.AppendLine(htmlOutput);
                sb.AppendLine("------------------------------");
                File.WriteAllText($"commonmark_examples/myout{@case.example}.txt", sb.ToString());
                Directory.CreateDirectory($"commonmark_examples/{@case.section}");
                File.WriteAllText($"commonmark_examples/{@case.section}/myout{@case.example}.txt", sb.ToString());
            }

            public void RunTestBySection(string section = "")
            {
                Directory.CreateDirectory("commonmark_examples");

                var markdown2html = new MarkdownEngine();

                var sb = new StringBuilder();
                int diff_count = 0;
                foreach (var @case in _testCases)
                {
                    if (!section.Equals("") && !section.Equals(@case.section))
                    {
                        continue;
                    }


                    var htmlOutput = markdown2html.MarkdownToHtmlString(@case.markdown);

                    if (htmlOutput.Replace("\r", "").Trim('\n').Equals(@case.html.Trim('\n')))
                    {
                        //sb.AppendLine("Success.");
                    }
                    else
                    {
                        diff_count += 1;

                        Console.BackgroundColor = ConsoleColor.DarkCyan;
                        Console.Write($"Failed Example {@case.example}");
                        Console.ResetColor();
                        Console.WriteLine();
                        
                        sb.AppendLine(@case.section);
                        sb.AppendLine("-Markdown---------------------");
                        sb.AppendLine(@case.markdown);
                        sb.AppendLine("-Mine-------------------------");
                        sb.AppendLine(htmlOutput);
                        sb.AppendLine("-Example----------------------");
                        sb.AppendLine(@case.html);
                    }

                    File.WriteAllText($"commonmark_examples/myout{@case.example}.txt", sb.ToString());
                    Directory.CreateDirectory($"commonmark_examples/{@case.section}");
                    File.WriteAllText($"commonmark_examples/{@case.section}/myout{@case.example}.txt", sb.ToString());

                    Console.Write(sb.ToString());

                    sb.Clear();
                }
                Console.WriteLine($"Diff Count: {diff_count}");
            }

            public void PrintAllTestCase()
            {
                foreach (var @case in _testCases)
                {
                    Console.WriteLine(@case.html);
                }
            }

            public void RunAllTest()
            {
                for (int i = 0; i < _testCases.Count; i++)
                {
                    RunTest(i);
                }
            }

            MarkdownEngine _markdownEngine = new MarkdownEngine();

            public void RunTest(int example_id)
            {
                var testCase = _testCases[example_id - 1];
                var htmlOutput = _markdownEngine.MarkdownToHtmlString(testCase.markdown);

                if (htmlOutput.Equals(testCase.html))
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Example {example_id} passed.");
                    Console.ResetColor();
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"Example {example_id} failed.");
                    Console.ResetColor();
                }

                Directory.CreateDirectory("my_debug_output");
                File.WriteAllText($"my_debug_output/myout{example_id}", htmlOutput);
                File.WriteAllText($"my_debug_output/out{example_id}", _testCases[example_id].html);
            }
        }

        private UnitTestDriver driver;

        public UnitTestCommonMark()
        {
            driver = new UnitTestDriver();
            // set current directory to root of solution
            Directory.SetCurrentDirectory(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\")));
            driver.LoadTestCase();
        }

        [TestMethod]
        public void TestMethodEmphStrong()
        {
            driver.RunTestBySection("Emphasis and strong emphasis");
        }

        [TestMethod]
        public void TestMethodParagraphs()
        {
            driver.RunTestBySection("Paragraphs");
        }

        [TestMethod]
        public void TestMethodATXHeaders()
        {
            driver.RunTestBySection("ATX headings");
        }

        [TestMethod]
        public void TestMethodLists()
        {
            driver.RunTestBySection("Lists");
        }

        [TestMethod]
        public void TestMethodLouis()
        {
            driver.RunTestBySection("Louis");
        }
    }
}
