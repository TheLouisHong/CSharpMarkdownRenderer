using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Markdown2HTML.Core;
using Markdown2HTML.Core.Interfaces;
using Newtonsoft.Json;

namespace Markdown2HTML.InlineRenderers
{
    /// <summary>
    /// Escapes ampersand, should always be the first operation for inline rendering.
    /// https://spec.commonmark.org/0.29/#entity-and-numeric-character-references
    ///
    /// List of Legal HTML Entities:
    /// https://html.spec.whatwg.org/entities.json
    ///
    /// Strategy, In Steps
    /// 1. replace entities using the symbols library &;
    /// 2. replace unicode character &#
    /// 3. replace ampersand and lessthan with escaped html versions & -> &amp;
    ///
    /// example:
    ///     &copy; copyright symbol
    ///     &lt; should not touch, already valid
    ///     &amp; should not touch, already valid
    ///     &notarealcode; treat as normal text
    ///     &#0000; illegal null unicode, replace with \uFFFD
    ///     &#38; unicode version of ampersand, should be replaced with &amp;
    ///
    /// 1. replace entities using the symbols library &;
    ///     © copyright symbol
    ///     &lt; should not touch, already valid
    ///     &amp; should not touch, already valid
    ///     &notarealcode; treat as normal text
    ///     � illegal null unicode, replace with \uFFFD
    ///     & unicode version of ampersand, should be replaced with &amp;
    ///
    /// 2. replace unicode character &#
    ///     © copyright symbol
    ///     &lt; should not touch, already valid
    ///     &amp; should not touch, already valid
    ///     &notarealcode; treat as normal text
    ///     � illegal null unicode, replace with \uFFFD
    ///     & unicode version of ampersand, should be replaced with &amp;
    ///
    /// 3. replace special HTML characters with HTML entities & -> &amp;
    ///     © copyright symbol
    ///     &lt; should not touch, already valid
    ///     &amp; should not touch, already valid
    ///     &amp;notarealcode; treat as normal text
    ///     � illegal null unicode, replace with \uFFFD
    ///     &amp; unicode version of ampersand, should be replaced with &amp;
    ///
    /// CommonMark 0.29 Compliant (example 311-327) , with exceptions:
    /// 
    ///     // TODO Add support for urls and code block.
    ///     // TODO Example 322,
    ///     // TODO Example 321,
    ///     // TODO Example 320,
    ///     // TODO Example 319,
    ///     // TODO Example 318,
    ///     // TODO Example 317, 
    ///
    ///     // TODO Example 324, Add support for lists
    ///
    ///     // BUG Example 323,HTML Entities, like &#42; for *, can be trigger inline rendering.
    ///     // Plan: Render HTML entities into escaped versions instead, when escaping is supported.
    ///     // Add support for code blocks.
    /// </summary>
    public class EntitiesLineRenderer : IInlineRenderer
    {
        /// <summary>
        /// Characters that need to be  into HTML entities for HTML.
        /// </summary>
        private KeyValuePair<string, string>[] _replacement = 
        {
            new KeyValuePair<string, string>("&", "&amp;"), 
            new KeyValuePair<string, string>("<", "&lt;"), 
            new KeyValuePair<string, string>(">", "&gt;"), 
            new KeyValuePair<string, string>("\"", "&quot;"), 
            new KeyValuePair<string, string>("'", "&#39;"), 

        };


        /// <summary>
        /// Entities recognized by the HTML Spec 
        /// https://html.spec.whatwg.org/entities.json
        /// TODO Move entity file path to a config file for easy access.
        /// </summary>
        private const string _entitiesFilePath = "entities.json";
        
        private class JsonEntity
        {
            //[JsonProperty("codepoints")]
            //public string CodePoint;
            [JsonProperty("characters")]
            public string Character;
        }

        /// <summary>
        /// Entity names (the things that follow ampersands) are case-senstive
        /// </summary>
        private Dictionary<string, JsonEntity> _entities;

        public string Render(string content)
        {
            // 1. replace entities using the symbols library &;
            content = RenderEntities(content);

            // 2. replace unicode character &# 
            content = RenderDecimalNumeric(content); 
            content = RenderHexadecimalNumeric(content); 

            // 3. replace ampersand and lessthan with escaped html versions & -> &amp;
            content = RenderHTMLChar(content);

            return content;
        }

        public EntitiesLineRenderer()
        {
            ReadEntities();
        }

        private void ReadEntities()
        {
            if (File.Exists(_entitiesFilePath))
            {
                var str = File.ReadAllText(_entitiesFilePath);
                _entities = JsonConvert.DeserializeObject<Dictionary<string, JsonEntity>>(str);
            }
        }


        /// <summary>
        /// group 1 &amp;
        /// group 2 &lt;
        /// group 3 &gt;
        /// group 4 &quot;
        /// group 5 &#39;
        /// </summary>
        private string RenderHTMLChar(string content)
        {
            foreach (var pair in _replacement)
            {
                content = content.Replace(pair.Key, pair.Value);
            }

            return content;
        }

        /// <summary>
        /// group 1: decimal numeric
        /// </summary>
        private readonly Regex _decimalNumericPattern = new Regex(
            @"&#(\d{1,7});");
        /// <summary>
        /// Parsing for Decimal Numeric
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private string RenderDecimalNumeric(string content)
        {
            content = _decimalNumericPattern.Replace(content, match =>
            {
                if (int.TryParse(match.Groups[1].Value, out var @char))
                {
                    // edge case for security reasons, NUL represents string termination in C
                    // https://spec.commonmark.org/0.29/#decimal-numeric-character
                    if (@char == 0) // illegal NUL char
                    {
                        // replacement char for illegal characters
                        return ((char) 0xFFFD).ToString();
                    }

                    // usual case
                    return ((char) @char).ToString();
                }

                return match.Value;
            });
            return content;
        }

        /// <summary>
        /// group 1:hexadecimal numeric
        /// </summary>
        private readonly Regex _hexadecimalNumericPattern = new Regex(
            @"&#[Xx]([\dABCDEFabcdef]{1,6});");
        private string RenderHexadecimalNumeric(string content)
        {
            content = _hexadecimalNumericPattern.Replace(content, (match) =>
            {
                if (int.TryParse(match.Groups[1].Value, 
                    NumberStyles.HexNumber, 
                    CultureInfo.InvariantCulture,  
                    out var @char))
                {
                    // edge case for security reasons, NUL represents string termination in C
                    // https://spec.commonmark.org/0.29/#decimal-numeric-character
                    if (@char == 0) // illegal NUL char
                    {
                        // replacement char for illegal characters
                        return ((char) 0xFFFD).ToString();
                    }

                    // usual case
                    return ((char) @char).ToString();
                }

                return match.Value;
            });
            return content;
        }

        private readonly Regex _entityPattern = new Regex(
            "(&[^#;]+;)");

        private string RenderEntities(string content)
        {
            content = _entityPattern.Replace(content,  (match) =>
            {
                if (_entities.TryGetValue(match.Value, out var entity))
                {
                    return entity.Character;
                }

                return match.Value;
            });

            return content;
        }
    }
}