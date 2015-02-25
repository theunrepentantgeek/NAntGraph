using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace Niche.Graphs.IntegrationTests
{
    [TestFixture]
    public class NodeColorTests
    {
        [Test]
        public void RenderGraph_givenFillColor_generatesImage()
        {
            var colors = new List<Color>
                             {
                                 Color.Blue,
                                 Color.Red,
                                 Color.Green,
                                 Color.White,
                                 Color.Black,
                                 Color.Aqua,
                                 Color.Yellow,
                                 Color.Magenta,
                                 Color.LightYellow,
                                 Color.SkyBlue,
                                 Color.OldLace,
                                 Color.SteelBlue,
                                 Color.DeepSkyBlue,
                                 Color.Gray,
                                 Color.Cornsilk
                             };

            var nodes = new List<Node>();
            var edges = new List<Edge>();

            foreach (var fill in colors)
            {
                Node prior = null;
                foreach (var font in colors)
                {
                    Node n = CreateNode(fill, font);
                    nodes.Add(n);
                    if (prior != null)
                    {
                        Edge e = new Edge(prior, n);
                        edges.Add(e);
                    }

                    prior = n;
                }
            }

            var graphFactory = new GraphFactory();
            var graph = graphFactory.CreateGraph(nodes, edges);

            var renderer = new DotRenderer(graph);
            var image = renderer.RenderImage();
            image.Save("NodeFillColor.png");
        }

        private Node CreateNode(Color fill, Color font)
        {
            var nodeStyle = new NodeStyle();
            nodeStyle.FillColor = fill;
            nodeStyle.FontColor = font;

            var t = string.Format("{0}", mNodeCounter++);
            return new Node(t, t, nodeStyle);
        }

        public int mNodeCounter = 1;
    }
}
