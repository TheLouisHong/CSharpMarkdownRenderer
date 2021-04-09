
using System.Linq;
using System.Text;

namespace Markdown2HTML.Core.Extensions
{
    /// <summary>
    /// Assortment of string helpers.
    /// </summary>
    public static class StringExtensions
    {
        public static bool EndsWithAny(this string lhs, params string[] ends)
        {
            foreach (var end in ends)
            {
                if (lhs.EndsWith(end))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool StartsWithAny(this string lhs, params string[] starts)
        {
            foreach (var start in starts)
            {
                if (lhs.StartsWith(start))
                {
                    return true;
                }
            }

            return false;
        }

        public static char FirstChar(this string lhs)
        {
            return lhs[0];
        }

        public static char LastChar(this string lhs)
        {
            return lhs[lhs.Length - 1];
        }

        public static string Splice(this string lhs, int startIndexInclusive, int endIndexInclusive)
        {
            return lhs.Substring(startIndexInclusive,endIndexInclusive - startIndexInclusive + 1);
        }
        public static string SpliceRight(this string lhs, int indexInclusive)
        {
            return lhs.Substring(indexInclusive, lhs.Length - indexInclusive);
        }

        public static string Indent(this string lhs, int padding, char padChar = ' ')
        {
            var lines = lhs.Split('\n');
            var sb = new StringBuilder();
            foreach (var line in lines)
            {
                for (int i = 0; i < padding; i++)
                {
                    sb.Append(padChar);
                }
                sb.AppendLine(line);
            }

            return sb.ToString();
        }

        public static string TruncatePerLine(string render)
        {
            var lines = render.Split('\n');

            // 2. trim whitespace and newlines per line
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                line = line.Trim('\n', ' '); // BUG does enot trim unicode whitespace
                lines[i] = line;
            }


            var sb = new StringBuilder();

            for (int i = 0; i < lines.Length - 1; i++)
            {
                sb.AppendLine(lines[i]);
            }

            sb.Append(lines.Last());

            render = sb.ToString();
            return render;
        }
    }
}