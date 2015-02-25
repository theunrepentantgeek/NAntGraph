using System;
using System.Collections.Generic;
using System.Drawing;

using NUnit.Framework;

namespace Niche.Graphs.Tests
{
    [TestFixture]
    public class DotRendererTests
    {
        public const string NodeName = "Node";

        public const string NodeLabel = "Label";

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_missingGraph_throwsException()
        {
            new DotRenderer(null);
        }

        [Test]
        public void Constructor_suppliedGraph_setsProperty()
        {
            var graph = CreateGraph();
            var renderer = new DotRenderer(graph);
            Assert.That(renderer.Graph, Is.EqualTo(graph));
        }

        [Test]
        public void Constructor_suppliedGraph_setsImage()
        {
            var graph = CreateGraph();
            var renderer = new DotRenderer(graph);
            Assert.That(renderer.RenderImage(), Is.Not.Null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void VisitGraph_missingGraph_throwsException()
        {
            var renderer = new DotRenderer();
            var nodes = new List<IDotStatement>();
            var edges = new List<IDotStatement>();
            var subGraphs = new List<IDotStatement>();
            renderer.VisitGraph(null, nodes, edges, subGraphs);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void VisitGraph_missingNodes_throwsException()
        {
            var renderer = new DotRenderer();
            var graph = CreateGraph();
            var edges = new List<IDotStatement>();
            var subGraphs = new List<IDotStatement>();
            renderer.VisitGraph(graph, null, edges, subGraphs);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void VisitGraph_missingEdges_throwsException()
        {
            var renderer = new DotRenderer();
            var graph = CreateGraph();
            var nodes = new List<IDotStatement>();
            var subGraphs = new List<IDotStatement>();
            renderer.VisitGraph(graph, nodes, null, subGraphs);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void VisitGraph_missingSubGraphs_throwsException()
        {
            var renderer = new DotRenderer();
            var graph = CreateGraph();
            var nodes = new List<IDotStatement>();
            var edges = new List<IDotStatement>();
            renderer.VisitGraph(graph, nodes, edges, null);
        }
        
        [Test]
        public void VisitGraph_withParameters_returnsDotStatement()
        {
            var renderer = new DotRenderer();
            var graph = CreateGraph();
            var nodes = new List<IDotStatement>();
            var edges = new List<IDotStatement>();
            var subGraphs = new List<IDotStatement>();
            var statement = renderer.VisitGraph(graph, nodes, edges, subGraphs);
            Assert.That(statement, Is.Not.Null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void VisitNode_missingNode_throwsException()
        {
            var renderer = new DotRenderer();
            renderer.VisitNode(null);
        }

        [Test]
        public void VisitNode_withNode_returnsDotStatement()
        {
            var renderer = new DotRenderer();
            var node = CreateNode();
            var statement = renderer.VisitNode(node);
            Assert.That(statement, Is.Not.Null);
        }

        [Test]
        public void VisitNode_withNode_setsStatementText()
        {
            var renderer = new DotRenderer();
            var node = CreateNode();
            var statement = renderer.VisitNode(node);
            Assert.That(statement.Text, Is.StringContaining(node.Name));
        }

        [Test]
        public void VisitNode_withNode_setsStatementAttributeLabel()
        {
            var renderer = new DotRenderer();
            var node = CreateNode();
            var statement = renderer.VisitNode(node);
            Assert.That(statement.Attributes, Has.Member(Dot.Label));
        }

        [Test]
        public void VisitNode_nodeWithFillColor_setsStatementAttributeFillColor()
        {
            var style = new NodeStyle
                            {
                                FillColor = Color.Green
                            };
            var renderer = new DotRenderer();
            var node = new Node("Name", "Label", style);
            var statement = renderer.VisitNode(node);
            Assert.That(statement.Attributes, Has.Member(Dot.FillColor));
        }

        [Test]
        public void VisitNode_nodeWithFillColor_setsStatementAttributeStyle()
        {
            var style = new NodeStyle
            {
                FillColor = Color.Green
            };
            var renderer = new DotRenderer();
            var node = new Node("Name", "Label", style);
            var statement = renderer.VisitNode(node);
            Assert.That(statement.Attributes, Has.Member(Dot.Style));
        }

        [Test]
        public void VisitNode_nodeWithFontcolor_setsStatementAttributeFontColor()
        {
            var style = new NodeStyle
                            {
                                FontColor = Color.Green
                            };
            var renderer = new DotRenderer();
            var node = new Node("Name", "Label", style);
            var statement = renderer.VisitNode(node);
            Assert.That(statement.Attributes, Has.Member(Dot.FontColor));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void VisitEdge_missingEdge_throwsException()
        {
            var renderer = new DotRenderer();
            renderer.VisitEdge(null);
        }

        [Test]
        public void VisitEdge_withEdge_returnsDotStatement()
        {
            var edge = CreateEdge("start", "finish");
            var renderer = new DotRenderer();
            var statement = renderer.VisitEdge(edge);
            Assert.That(statement, Is.Not.Null);
        }

        [Test]
        public void VisitEdge_withEdge_includesStartInText()
        {
            var edge = CreateEdge("start", "finish");
            var renderer = new DotRenderer();
            var statement = renderer.VisitEdge(edge);
            Assert.That(statement.Text, Is.StringContaining(edge.Start.Name));            
        }

        [Test]
        public void VisitEdge_withEdge_includesFinishInText()
        {
            var edge = CreateEdge("start", "finish");
            var renderer = new DotRenderer();
            var statement = renderer.VisitEdge(edge);
            Assert.That(statement.Text, Is.StringContaining(edge.Finish.Name));                        
        }

        [Test]
        public void VisitEdge_edgeWithColor_setsStatementAttributeColor()
        {
            var style = new EdgeStyle();
            style.Color = Color.SkyBlue;
            var edge = CreateEdge("start", "finish", style);
            var renderer = new DotRenderer();
            var statement = renderer.VisitEdge(edge);
            Assert.That(statement.Attributes, Has.Member(Dot.Color));            
        }

        [Test]
        public void VisitEdge_edgeWithArrowHead_setsStatementAttributeArrowHead()
        {
            var style = new EdgeStyle();
            style.ArrowHead = ArrowShape.Normal;
            var edge = CreateEdge("start", "finish", style);
            var renderer = new DotRenderer();
            var statement = renderer.VisitEdge(edge);
            Assert.That(statement.Attributes, Has.Member(Dot.ArrowHead));
        }

        [Test]
        public void VisitEdge_edgeWithArrowHead_setsStatementAttributeValueArrowHead()
        {
            var style = new EdgeStyle();
            style.ArrowHead = ArrowShape.Crow;
            var edge = CreateEdge("start", "finish", style);
            var renderer = new DotRenderer();
            var statement = renderer.VisitEdge(edge);
            Assert.That(statement.AttributeValue(Dot.ArrowHead), Is.EqualTo("crow"));
        }

        [Test]
        public void VisitEdge_edgeWithArrowTail_setsStatementAttributeArrowTail()
        {
            var style = new EdgeStyle();
            style.ArrowTail = ArrowShape.Box;
            var edge = CreateEdge("start", "finish", style);
            var renderer = new DotRenderer();
            var statement = renderer.VisitEdge(edge);
            Assert.That(statement.Attributes, Has.Member(Dot.ArrowTail));
        }

        [Test]
        public void VisitEdge_edgeWithArrowTail_setsStatementAttributeValueArrowTail()
        {
            var style = new EdgeStyle();
            style.ArrowTail = ArrowShape.Crow;
            var edge = CreateEdge("start", "finish", style);
            var renderer = new DotRenderer();
            var statement = renderer.VisitEdge(edge);
            Assert.That(statement.AttributeValue(Dot.ArrowTail), Is.EqualTo("crow"));
        }

        [Test]
        public void VisitEdge_edgeWithConstraing_setsStatementAttributeConstraint()
        {
            var style = new EdgeStyle();
            style.Constraining = false;
            var edge = CreateEdge("start", "finish", style);
            var renderer = new DotRenderer();
            var statement = renderer.VisitEdge(edge);
            Assert.That(statement.Attributes, Has.Member(Dot.EdgeConstraint));
        }

        [Test]
        public void VisitEdge_edgeWithConstraing_setsStatementAttributeConstraintValue()
        {
            var style = new EdgeStyle();
            style.Constraining = false;
            var edge = CreateEdge("start", "finish", style);
            var renderer = new DotRenderer();
            var statement = renderer.VisitEdge(edge);
            Assert.That(statement.AttributeValue(Dot.EdgeConstraint), Is.EqualTo("false"));
        }

        private Graph CreateGraph()
        {
            var factory = new GraphFactory();
            return factory.CreateGraph();
        }

        private Node CreateNode()
        {
            return new Node(NodeName, NodeLabel);
        }

        private Edge CreateEdge(string start, string finish)
        {
            var s = new Node(start, start);
            var f = new Node(finish, finish);
            return new Edge(s, f);
        }

        private Edge CreateEdge(string start, string finish, EdgeStyle style)
        {
            var s = new Node(start, start);
            var f = new Node(finish, finish);
            return new Edge(s, f, style);
        }
    }
}
