using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;

namespace Niche.Graphs.Tests
{
    [TestFixture]
    public class GraphTests
    {
        [SetUp]
        public void SetUp()
        {
            mMockRepository = new MockRepository();
        }

        [Test]
        public void Constructor_givenNodeList_setsProperty()
        {
            var nodes = new List<Node>()
                            {
                                CreateNode("alpha"),
                                CreateNode("beta"),
                                CreateNode("gamma")
                            };
            var edges = new List<Edge>();
            var subgraphs = new List<Graph>();

            var graph = new Graph(nodes, edges, subgraphs);
            Assert.That(graph.Nodes, NUnit.Framework.Is.EquivalentTo(nodes));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_missingNodeList_throwsException()
        {
            var edges = new List<Edge>();
            var subgraphs = new List<Graph>();
            new Graph(null, edges, subgraphs);
        }

        [Test]
        public void Constructor_givenEdgeList_setsProperty()
        {
            var alpha = CreateNode("alpha");
            var beta = CreateNode("beta");
            var gamma = CreateNode("gamma");

            var nodes = new List<Node>();
            var edges = new List<Edge>()
                            {
                                CreateEdge(alpha, beta),
                                CreateEdge(beta, gamma),
                                CreateEdge(gamma, alpha),
                                CreateEdge(alpha, gamma)
                            };
            var subgraphs = new List<Graph>();

            var graph = new Graph(nodes, edges, subgraphs);
            Assert.That(graph.Edges, NUnit.Framework.Is.EquivalentTo(edges));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_missingEdgeList_throwsException()
        {
            var nodes = new List<Node>();
            var subgraphs = new List<Graph>();
            new Graph(nodes, null, subgraphs);
        }

        [Test]
        public void Constructor_givenGraphList_setsProperty()
        {
            var nodes = new List<Node>();
            var edges = new List<Edge>();

            var graphs = new List<Graph>()
                             {
                                 CreateGraph(),
                                 CreateGraph()
                             };

            var graph = new Graph(nodes, edges, graphs);
            Assert.That(graph.SubGraphs, NUnit.Framework.Is.EquivalentTo(graphs));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_missingSubGraphList_throwsException()
        {
            var nodes = new List<Node>();
            var edges = new List<Edge>();
            new Graph(nodes, edges, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Visit_missingVisitor_throwsException()
        {
            var graph = CreateGraph();
            graph.Visit<string>(null);
        }

        [Test]
        public void Visit_givenVisitor_callsVisitGraph()
        {
            var visitor = mMockRepository.DynamicMock<IGraphVisitor<string>>();
            var graph = CreateGraph();

            using (mMockRepository.Record())
            {
                Expect.Call(visitor.VisitGraph(null, null, null, null))
                    .IgnoreArguments()
                    .Return("graph")
                    .Repeat.Once();
            }

            using (mMockRepository.Playback())
            {
                graph.Visit(visitor);
            }
        }

        [Test]
        public void Visit_givenVisitor_returnsVisitGraphResult()
        {
            const string VisitGraphResult = "graph";

            var visitor = mMockRepository.DynamicMock<IGraphVisitor<string>>();
            var graph = CreateGraph();

            using (mMockRepository.Record())
            {
                Expect.Call(visitor.VisitGraph(null, null, null, null))
                    .IgnoreArguments()
                    .Return(VisitGraphResult)
                    .Repeat.Once();
            }

            var result = graph.Visit(visitor);
            Assert.That(result, NUnit.Framework.Is.EqualTo(VisitGraphResult));
        }

        [Test]
        public void Visit_graphWithSingleNode_callsVisitNodeAndVisitGraph()
        {
            const string VisitGraphResult = "graph";
            const string VisitNodeResult = "node";

            var node = CreateNode("node");
            var visitor = mMockRepository.DynamicMock<IGraphVisitor<string>>();
            var graph = CreateGraph(node);

            using (mMockRepository.Record())
            {
                Expect.Call(visitor.VisitNode(null))
                    .IgnoreArguments()
                    .Return(VisitNodeResult)
                    .Repeat.Once();

                Expect.Call(visitor.VisitGraph(null, null, null, null))
                    .IgnoreArguments()
                    .Return(VisitGraphResult)
                    .Repeat.Once();
            }

            using(mMockRepository.Playback())
            {
                graph.Visit(visitor);
            }
        }

        [Test]
        public void Visit_graphWithSingleEdge_callsVisitEdgeAndVisitGraph()
        {
            const string VisitGraphResult = "graph";
            const string VisitNodeResult = "node";

            var node = CreateNode("node");
            var edge = CreateEdge(node, node);
            var visitor = mMockRepository.DynamicMock<IGraphVisitor<string>>();
            var graph = CreateGraph(edge);

            using (mMockRepository.Record())
            {
                Expect.Call(visitor.VisitEdge(null))
                    .IgnoreArguments()
                    .Return(VisitNodeResult)
                    .Repeat.Once();

                Expect.Call(visitor.VisitGraph(null, null, null, null))
                    .IgnoreArguments()
                    .Return(VisitGraphResult)
                    .Repeat.Once();
            }

            using (mMockRepository.Playback())
            {
                graph.Visit(visitor);
            }            
        }

        [Test]
        public void Visit_graphWithSingleSubGraph_callsVisitGraphTwice()
        {
            const string VisitGraphResult = "graph";

            var visitor = mMockRepository.DynamicMock<IGraphVisitor<string>>();
            var subgraph = CreateGraph();
            var graph = CreateGraph(subgraph);

            using (mMockRepository.Record())
            {
                Expect.Call(visitor.VisitGraph(null, null, null, null))
                    .IgnoreArguments()
                    .Return(VisitGraphResult)
                    .Repeat.Twice();
            }

            using (mMockRepository.Playback())
            {
                graph.Visit(visitor);
            }
        }

        /// <summary>
        /// Create a graph to test with
        /// </summary>
        /// <returns>New graph instance</returns>
        private Graph CreateGraph(params object[] content)
        {
            var factory = new GraphFactory();
            return factory.CreateGraph(content);
        }

        /// <summary.
        /// Create a node to test with
        /// </summary>
        /// <param name="caption">Caption for test node</param>
        /// <returns>New node instance</returns>
        private Node CreateNode(string caption)
        {
            return new Node(caption, caption);
        }

        /// <summary>
        /// Create an edge to test with
        /// </summary>
        /// <param name="start">Start of edge</param>
        /// <param name="finish">Finish of edge</param>
        /// <returns>New edge instance.</returns>
        private Edge CreateEdge(Node start, Node finish)
        {
            return new Edge(start, finish);
        }

        private MockRepository mMockRepository;
    }
}
