using System;
using NUnit.Framework;

namespace Niche.Graphs.Tests
{
    [TestFixture]
    public class EdgeStyleTests
    {
        [SetUp]
        public void SetUp()
        {
            mStyle = new EdgeStyle();
            mStart = new Node("start", "Start");
            mFinish = new Node("finish", "Finish");
        }

        [Test]
        public void CreateEdge_providedStart_setsProperty()
        {
            var edge = mStyle.CreateEdge(mStart, mFinish);
            Assert.That(edge.Start, Is.EqualTo(mStart));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateEdge_missingStart_throwsException()
        {
            mStyle.CreateEdge(null, mFinish);
        }

        [Test]
        public void CreateEdge_providedFinish_setsProperty()
        {
            var edge = mStyle.CreateEdge(mStart, mFinish);
            Assert.That(edge.Finish, Is.EqualTo(mFinish));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateEdge_missingFinish_throwsException()
        {
            mStyle.CreateEdge(mStart, null);
        }

        [Test]
        public void CreateEdge_configuredArrowHead_setsProperty()
        {
            mStyle.ArrowHead = ArrowShape.Normal;
            var edge = mStyle.CreateEdge(mStart, mFinish);
            Assert.That(edge.ArrowHead, Is.EqualTo(ArrowShape.Normal));
        }

        [Test]
        public void CreateEdge_configuredArrowTail_setsProperty()
        {
            mStyle.ArrowTail = ArrowShape.Box;
            var edge = mStyle.CreateEdge(mStart, mFinish);
            Assert.That(edge.ArrowTail, Is.EqualTo(ArrowShape.Box));
        }

        [Test]
        public void CreateEdge_configuredConstraining_setsProperty()
        {
            mStyle.Constraining = true;
            var edge = mStyle.CreateEdge(mStart, mFinish);
            Assert.That(edge.Constraining, Is.True);
        }

        private EdgeStyle mStyle;

        private Node mStart;

        private Node mFinish;
    }
}
