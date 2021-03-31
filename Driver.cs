using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.CompilerServices;
using Markdown2HTML.Core;
using Markdown2HTML.Core.Debug;


namespace Markdown2HTML
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

        public void OutputMyTestCase(string section = "")
        {
            Directory.CreateDirectory("commonmark_examples");

            var markdown2html = new MarkdownEngine();

            var sb = new StringBuilder();
            foreach (var @case in _testCases)
            {
                if (!section.Equals("") && !section.Equals(@case.section))
                {
                    continue;
                }

                Console.BackgroundColor = ConsoleColor.DarkCyan;
                Console.Write($"Testing Example {@case.example}");
                Console.ResetColor();
                Console.WriteLine();

                var htmlOutput = markdown2html.MarkdownToHtmlString(@case.markdown);

                sb.AppendLine(@case.section);
                sb.AppendLine("------------------------------");
                sb.AppendLine(@case.markdown);
                sb.AppendLine("------------------------------");
                sb.AppendLine(htmlOutput);
                sb.AppendLine("------------------------------");
                File.WriteAllText($"commonmark_examples/myout{@case.example}.txt", sb.ToString());
                Directory.CreateDirectory($"commonmark_examples/{@case.section}");
                File.WriteAllText($"commonmark_examples/{@case.section}/myout{@case.example}.txt", sb.ToString());

                sb.Clear();
            }
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

    class Driver
    {
        static void Main(string[] args)
        {
            Debugger.Launch();
            var driver = new UnitTestDriver();
            driver.LoadTestCase();


            if (args.Length > 0)
            {
                if (int.TryParse(args[0], out var example))
                {
                    Logger.Log($"Running example {example}");
                    driver.OutputSingleExample(example);
                }
                Logger.Log($"Filter by section {args[0]}.");
                driver.OutputMyTestCase(args[0]);
            }
            else
            {
                driver.OutputMyTestCase();
            }
            //driver.OutputExampleTestCase();
            

            /*
            bool success = int.TryParse(args[0], out int example_id);
            if (success)
            {
                if (example_id == 0)
                {
                    driver.RunAllTest();
                }
                else
                {
                    driver.RunTest(example_id);
                }
            }
            else
            {
                Console.WriteLine($"Trying to read file {args[0]}...");
                var mdHtml = new MarkdownEngine();
                mdHtml.MarkdownToHtml(args[0], "myout.html");
            }
        */
        }
    }
}
