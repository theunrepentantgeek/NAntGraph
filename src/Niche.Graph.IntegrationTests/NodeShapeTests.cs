using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Niche.Graphs;

using NUnit.Framework;

namespace Niche.Graph.IntegrationTests
{
    [TestFixture]
    public class NodeShapeTests
    {
        [Test]
        public void RenderGraph_givenNodeShape_generatesImage(
            [Values(NodeShape.None,
                    NodeShape.Box,
                    NodeShape.Circle,
                    NodeShape.Diamond,
                    NodeShape.DoubleCircle,
                    NodeShape.DoubleOctagon,
                    NodeShape.Egg,
                    NodeShape.Ellipse,
                    NodeShape.Hexagon,
                    NodeShape.House,
                    NodeShape.InvHouse,
                    NodeShape.InvTrapezium,
                    NodeShape.InvTriangle,
                    NodeShape.MCircle,
                    NodeShape.MDiamond,
                    NodeShape.MSquare,
                    NodeShape.Octagon,
                    NodeShape.Parallelogram,
                    NodeShape.Pentagon,
                    NodeShape.Plaintext,
                    NodeShape.Point,
                    NodeShape.Polygon,
                    NodeShape.Septagon,
                    NodeShape.Trapezium,
                    NodeShape.Triangle,
                    NodeShape.TripleOctagon)]
            NodeShape shape)
        {
            var nodeStyle = new NodeStyle();
            nodeStyle.Shape = shape;

            var n = Enum.GetName(typeof(NodeShape), shape);

            var node = nodeStyle.CreateNode(n, n);

            var graphFactory = new GraphFactory();
            var graph = graphFactory.CreateGraph(node);

            var renderer = new DotRenderer(graph);

            var filename = n + ".png";
            var image = renderer.RenderImage();
            image.Save(filename);
        }
    }
}