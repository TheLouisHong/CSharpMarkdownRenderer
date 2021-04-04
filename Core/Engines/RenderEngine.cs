using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Markdown2HTML.Core.Attributes;
using Markdown2HTML.Core.Debug;
using Markdown2HTML.Core.Interfaces;

namespace Markdown2HTML.Core.Engines
{
    public static class RenderEngine
    {
        private static Dictionary<Type, IMarkdownObjectRenderer> _renderers 
            = new Dictionary<Type, IMarkdownObjectRenderer>();

        static RenderEngine()
        {
            CollectRenderEngine();
        }

        public static string Render(List<IMarkdownObject> markdownObjects)
        {
            StringBuilder output = new StringBuilder();

            foreach (var mdObj in markdownObjects)
            {
                if (_renderers.TryGetValue(mdObj.GetType(), out var renderer))
                {
                    output.Append(renderer.RenderToHTML(mdObj));
                }
                else
                {
                    Logger.LogError($"Missing Renderer for {mdObj.GetType().Name}.");
                }
            }

            return output.ToString();
        }

        private static void CollectRenderEngine()
        {
            // get class that inherits from IBlockLexer and has BlockLexerAttribute
            var blockLexersTypes =
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from @type in assembly.GetTypes()
                where typeof(IMarkdownObjectRenderer).IsAssignableFrom(@type) && !@type.IsInterface
                let attributes = @type.GetCustomAttributes(typeof(MarkdownObjectRendererAttribute), true)
                where attributes != null && attributes.Length > 0
                select new {Type = @type, Attributes = attributes.Cast<MarkdownObjectRendererAttribute>()};

            foreach (var renderType in blockLexersTypes)
            {
                var renderer = (IMarkdownObjectRenderer) Activator.CreateInstance(renderType.Type);
                _renderers.Add(renderType.Attributes.First().MarkdownObjectType, renderer);
            }


        }
    }
}