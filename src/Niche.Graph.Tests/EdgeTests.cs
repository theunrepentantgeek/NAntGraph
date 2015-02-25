using System;
using NUnit.Framework;
using Rhino.Mocks;

namespace Niche.Graphs.Tests
{
    [TestFixture]
    public class EdgeTests
    {
        [SetUp]
        public void SetUp()
        {
            mMockRepository = new MockRepository();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_missingStart_throwsException()
        {
            var node = CreateNode();
            new Edge(null, node);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_missingFinish_throwsException()
        {
            var node = CreateNode();
            new Edge(node, null);            
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_missingStyle_throwsException()
        {
            var node = CreateNode();
            new Edge(node, node, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Visit_missingVisitor_throwsException()
        {
            var node = CreateNode();
            var edge = new Edge(node, node);
            edge.Visit<string>(null);
        }

        [Test]
        public void Visit_givenVisitor_callsVisitEdge()
        {
            var visitor = mMockRepository.DynamicMock<IGraphVisitor<string>>();
            var node = CreateNode();
            var edge = new Edge(node, node);

            using (mMockRepository.Record())
            {
                Expect.Call(visitor.VisitEdge(null))
                    .IgnoreArguments()
                    .Return("graph")
                    .Repeat.Once();
            }

            using (mMockRepository.Playback())
            {
                edge.Visit(visitor);
            }
        }

        [Test]
        public void Visit_givenVisitor_returnsVisitEdgeResult()
        {
            const string VisitEdgeResult = "graph";

            var visitor = mMockRepository.DynamicMock<IGraphVisitor<string>>();
            var node = CreateNode();
            var edge = new Edge(node, node);

            using (mMockRepository.Record())
            {
                Expect.Call(visitor.VisitEdge(null))
                    .IgnoreArguments()
                    .Return(VisitEdgeResult)
                    .Repeat.Once();
            }

            var result = edge.Visit(visitor);
            Assert.That(result, Is.EqualTo(VisitEdgeResult));
        }

        private Node CreateNode()
        {
            return new Node("alpha", "Alpha");
        }

        private MockRepository mMockRepository;
    }
}
