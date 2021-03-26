using System.Text.RegularExpressions;
using Markdown2HTML.Core;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.Tokens;

namespace Markdown2HTML.InlineRenderers
{

    /// <summary>
    /// StrongEmphasis Inline Renderer, only allows non-nested matches.
    ///
    /// 
    /// unsupported example:
    /// *foo *bar**
    ///
    /// CommonMark 0.29:
    /// <p><em>foo <em>bar</em></em></p 
    ///
    /// My Naive Implementation:
    /// <p>*foo <em>bar</em>*</p>
    ///    ^    ____________^
    ///  parent    nested   parent
    /// Notice, the nested emphasis interrupts the parent emphasis.
    ///
    /// 
    /// 
    /// Key difference between * (star) and _ (underscore)
    /// ---------------------
    /// is * allows intraword matching, _ does not.
    /// https://spec.commonmark.org/0.29/#emphasis-and-strong-emphasis
    ///
    ///
    /// 
    /// @TODO Due to time constraint, this renderer does not support advanced mixing strong and em.  
    /// example: ***strong** in emph* is not supported
    /// supported equivalent ***strong*** *in emp*
    ///
    /// CommonMark 0.29 as Reference.
    /// 
    /// </summary>
    public class NaiveStrongEmphasisInlineRenderer : IInlineRenderer
    {


        /// <summary>
        /// Used in all matches,  
        /// </summary>
        private const string _contentMatch =
            @"((?![\s])(?:[^_\*]*(?<![\s]))?)";


        // star matching
        /// <summary>
        /// Intraword emphasis with * is permitted:
        /// </summary>
        private readonly  Regex _emStarMatch = new Regex(
        @"\*" + _contentMatch + @"\*" 
        );

        /// <summary>
        /// Intraword emphasis with * is permitted:
        /// </summary>
        private readonly Regex _strongStarMatch = new Regex(
        @"\*\*" + _contentMatch + @"\*\*" 
        );

        /// <summary>
        /// Intraword emphasis with * is permitted:
        /// </summary>
        private readonly Regex _bothStarMatch = new Regex(
        @"\*\*\*" + _contentMatch + @"\*\*\*"
        );


        private const string _leftMatch =
            @"(?:^|[\s])";

        private const string _rightMatch =
            @"(?:$|[\s])";

        // underscore matching
        private readonly Regex _emLineMatch = new Regex(
        _leftMatch + @"_" + _contentMatch + @"_" + _rightMatch
        );

        private readonly Regex _strongLineMatch = new Regex(
        _leftMatch + @"__" + _contentMatch + @"__" + _rightMatch
        );

        private readonly Regex _bothLineMatch = new Regex(
        _leftMatch + @"___" + _contentMatch + @"___" + _rightMatch
        );


        // all regex must be using group 1!!!
        private const string _emReplace =
                    "<em>$1</em>";

        private const string _strongReplace =
                    "<strong>$1</strong>";

        private const string _bothReplace =
                    "<em><strong>$1<strong><em>";

        public string Render(string content)
        {
            // TODO not done

            // Very big awkward setup here,
            // reason: leave logic flexible potentially for nested.
            // 
            // otherwise, just have a single regex 
            // with [\*_]{1,3}content[\*_]{1,3}
            // is better .

            Regex[] matches =
            {
                // order matters here
                _bothStarMatch, 
                _strongStarMatch,
                _emStarMatch,

                _bothLineMatch,
                _strongLineMatch,
                _emLineMatch,
            };

            string[] replaces =
            {
                _bothReplace,
                _strongReplace,
                _emReplace,

                _bothReplace,
                _strongReplace,
                _emReplace,
            };

            for (var i = 0; i < matches.Length; i++)
            {
                var reg = matches[i];
                var replace = replaces[i];

                var match = reg.Match(content);

                content = reg.Replace(content, replace);
            }

            return content;
        }
    }
}