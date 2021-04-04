namespace Markdown2HTML.Core.Extensions
{
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
    }
}