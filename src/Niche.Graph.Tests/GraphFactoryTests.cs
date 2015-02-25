using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace Niche.Graphs.Tests
{
    [TestFixture]
    public class GraphFactoryTests
    {
        [SetUp]
        public void SetUp()
        {
            mFactory = new GraphFactory();
        }

        [Test]
        public void CreateGraph_withNoParameters_returnsGraphWithNoNodes()
        {
            var graph = mFactory.CreateGraph();
            Assert.That(graph.Nodes.Count(), Is.EqualTo(0));
        }

        [Test]
        public void CreateGraph_withNoParameters_returnsGraphWithNoEdges()
        {
            var graph = mFactory.CreateGraph();
            Assert.That(graph.Edges.Count(), Is.EqualTo(0));
        }

        [Test]
        public void CreateGraph_withNoParameters_returnsGraphWithNoSubgraphs()
        {
            var graph = mFactory.CreateGraph();
            Assert.That(graph.SubGraphs.Count(), Is.EqualTo(0));
        }

        [Test]
        public void CreateGraph_withSingleNode_returnsGraphWithSingleNode()
        {
            var node = CreateNode("alpha");
            var graph = mFactory.CreateGraph(node);
            Assert.That(graph.Nodes.Count(), Is.EqualTo(1));
        }

        [Test]
        public void CreateGraph_withMultipleNodes_returnsGraphWithNodes()
        {
            var alpha = CreateNode("alpha");
            var beta = CreateNode("beta");
            var gamma = CreateNode("gamma");

            var graph = mFactory.CreateGraph(alpha, beta, gamma);
            Assert.That(graph.Nodes.Count(), Is.EqualTo(3));
        }

        [Test]
        public void CreateGraph_withNodeList_returnsGraphWithNodes()
        {
            var nodes
                = new List<Node>
                      {
                          CreateNode("alpha"),
                          CreateNode("beta"),
                          CreateNode("gamma")
                      };
             var graph = mFactory.CreateGraph(nodes);
            Assert.That(graph.Nodes.Count(), Is.EqualTo(3));
        }

        [Test]
        public void CreateGraph_withSingleEdge_returnsGraphWithSingleEdge()
        {
            var alpha = CreateNode("alpha");
            var beta = CreateNode("beta");
            var edge = CreateEdge(alpha, beta);
            var graph = mFactory.CreateGraph(edge);
            Assert.That(graph.Edges.Count(), Is.EqualTo(1));
        }

        [Test]
        public void CreateGraph_withMultipleEdges_returnsGraphWithEdges()
        {
            var alpha = CreateNode("alpha");
            var beta = CreateNode("beta");
            var gamma = CreateNode("gamma");
            var edgeA = CreateEdge(alpha, beta);
            var edgeB = CreateEdge(beta, gamma);
            var edgeC = CreateEdge(gamma, alpha);
            var graph = mFactory.CreateGraph(edgeA, edgeB, edgeC);
            Assert.That(graph.Edges.Count(), Is.EqualTo(3));            
        }

        [Test]
        public void CreateGraph_withEdgeList_returnsGraphWithEdges()
        {
            var alpha = CreateNode("alpha");
            var beta = CreateNode("beta");
            var gamma = CreateNode("gamma");
            var edges = new List<Edge>
                            {
                                CreateEdge(alpha, beta),
                                CreateEdge(beta, gamma),
                                CreateEdge(gamma, alpha)
                            };
            var graph = mFactory.CreateGraph(edges);
            Assert.That(graph.Edges.Count(), Is.EqualTo(3));            
        }

        [Test]
        public void CreateGraph_withSingleGraph_returnsGraphWithSingleSubGraph()
        {
            var subGraph = CreateGraph();
            var graph = mFactory.CreateGraph(subGraph);
            Assert.That(graph.SubGraphs.Count(), Is.EqualTo(1));
        }

        [Test]
        public void CreateGraph_withMultipleGraphs_returnsGraphWithGraphs()
        {
            var alpha = CreateGraph();
            var beta = CreateGraph();
            var gamma = CreateGraph();
            var graph = mFactory.CreateGraph(alpha, beta, gamma);
            Assert.That(graph.SubGraphs.Count(), Is.EqualTo(3));
        }

        [Test]
        public void CreateGraph_withGraphList_returnsGraphWithGraphs()
        {
            var subgraphs = new List<Graph>
                                {
                                    CreateGraph(),
                                    CreateGraph(),
                                    CreateGraph()
                                };
            var graph = mFactory.CreateGraph(subgraphs);
            Assert.That(graph.SubGraphs.Count(), Is.EqualTo(3));
        }

        private Node CreateNode(string caption)
        {
            return new Node(caption, caption);
        }

        private Edge CreateEdge(Node start, Node finish)
        {
            return new Edge(start, finish);
        }

        private Graph CreateGraph()
        {
            return new Graph(new List<Node>(), new List<Edge>(), new List<Graph>());
        }

        private GraphFactory mFactory;
    }
}
