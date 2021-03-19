using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.CompilerServices;


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
            var driver = new UnitTestDriver();
            driver.LoadTestCase();

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
                mdHtml.MarkdownToHtml(args[0], "myout");
            }
            Console.WriteLine("Press key to exit.");
            Console.ReadKey(true);
        }
    }
}
