﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Markdown2HTML.Attributes;
using Markdown2HTML.MarkdownObject;

namespace Markdown2HTML
{
    public class RenderEngine
    {
        private Dictionary<Type, IMarkdownObjectRenderer> _renderers 
            = new Dictionary<Type, IMarkdownObjectRenderer>();

        public RenderEngine()
        {
            InitRenderEngine();
        }

        public string Render(List<IMarkdownObject> markdownObjects)
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
                    Console.Error.WriteLine($"Missing Renderer for {mdObj.GetType().Name}.");
                }
            }

            return output.ToString();
        }

        void InitRenderEngine()
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