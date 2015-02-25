using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace Niche.Graphs.IntegrationTests
{
    [TestFixture]
    public class ArrowShapeTests
    {
        [Test]
        public void RenderGraph_givenArrowShape_generatesImage(
            [Values(ArrowShape.Box, ArrowShape.Crow, ArrowShape.Diamond, ArrowShape.Dot, ArrowShape.Inv, ArrowShape.None, ArrowShape.Normal, ArrowShape.Tee, ArrowShape.Vee)]
            ArrowShape arrowHead)
        {
            var edgeStyle = new EdgeStyle();
            edgeStyle.ArrowHead = arrowHead;

            var startStyles
                = new List<ArrowShape>
                      {
                          ArrowShape.Box,
                          ArrowShape.Crow,
                          ArrowShape.Diamond,
                          ArrowShape.Dot,
                          ArrowShape.Inv,
                          ArrowShape.None,
                          ArrowShape.Normal,
                          ArrowShape.Tee,
                          ArrowShape.Vee
                      };

            var starts = new List<Node>();
            var edges = new List<Edge>();
            var finish = new Node("finish", "Finish");

            foreach (var shape in startStyles)
            {
                var n = Enum.GetName(typeof(ArrowShape), shape);
                var s = new Node(n, n);
                edgeStyle.ArrowTail = shape;
                var e = new Edge(s, finish, edgeStyle);

                starts.Add(s);
                edges.Add(e);
            }

            var graphFactory = new GraphFactory();
            var graph = graphFactory.CreateGraph(starts, finish, edges);

            var renderer = new DotRenderer(graph);

            var head = Enum.GetName(typeof(ArrowShape), arrowHead);
            var filename = head + ".png";
            var image = renderer.RenderImage();
            image.Save(filename);
        }
    }
}