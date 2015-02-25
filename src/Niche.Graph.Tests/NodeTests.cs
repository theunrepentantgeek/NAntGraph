using System;
using NUnit.Framework;
using Rhino.Mocks;

namespace Niche.Graphs.Tests
{
    [TestFixture]
    public class NodeTests
    {
        [SetUp]
        public void SetUp()
        {
            mMockRepository = new MockRepository();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_nullName_thowsException()
        {
            new Node(null, "Label");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_emptyName_throwsException()
        {
            new Node(string.Empty, "Label");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_nullLabel_throwsException()
        {
            new Node("name", null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_emptyLabel_throwsException()
        {
            new Node("name", string.Empty);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_nullStyle_throwsException()
        {
            new Node("name", "label", null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Visit_missingVisitor_throwsException()
        {
            var node = CreateNode();
            node.Visit<string>(null);
        }

        [Test]
        public void Visit_givenVisitor_callsVisitEdge()
        {
            var visitor = mMockRepository.DynamicMock<IGraphVisitor<string>>();
            var node = CreateNode();

            using (mMockRepository.Record())
            {
                Expect.Call(visitor.VisitNode(null))
                    .IgnoreArguments()
                    .Return("graph")
                    .Repeat.Once();
            }

            using (mMockRepository.Playback())
            {
                node.Visit(visitor);
            }
        }

        [Test]
        public void Visit_givenVisitor_returnsVisitNodeResult()
        {
            const string VisitNodeResult = "node";

            var visitor = mMockRepository.DynamicMock<IGraphVisitor<string>>();
            var node = CreateNode();

            using (mMockRepository.Record())
            {
                Expect.Call(visitor.VisitNode(null))
                    .IgnoreArguments()
                    .Return(VisitNodeResult)
                    .Repeat.Once();
            }

            var result = node.Visit(visitor);
            Assert.That(result, Is.EqualTo(VisitNodeResult));
        }

        private Node CreateNode()
        {
            return new Node("alpha", "Alpha");
        }

        private MockRepository mMockRepository;
    }
}
