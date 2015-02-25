using System;
using System.Drawing;
using NUnit.Framework;

namespace Niche.Graphs.Tests
{
    [TestFixture]
    public class NodeStyleTests
    {
        private const string Name = "alpha";
        private const string Label = "Alpha";
        private const NodeShape Shape = NodeShape.Octagon;

        private Color mFillColor = Color.SkyBlue;

        [SetUp]
        public void SetUp()
        {
            mStyle = new NodeStyle();
        }

        [Test]
        public void Constructor_defaultFillColor()
        {
            Assert.That(mStyle.FillColor, Is.EqualTo(Color.Empty));
        }

        [Test]
        public void CreateNode_givenName_setsProperty()
        {
            var node = mStyle.CreateNode(Name, Label);
            Assert.That(node.Name, Is.EqualTo(Name));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateNode_missingName_throwsException()
        {
            mStyle.CreateNode(null, Label);
        }

        [Test]
        public void CreateNode_givenLabel_setsProperty()
        {
            var node = mStyle.CreateNode(Name, Label);
            Assert.That(node.Label, Is.EqualTo(Label));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateNode_missingLabel_throwsException()
        {
            mStyle.CreateNode(Name, null);
        }

        [Test]
        public void CreateNode_configuredShape_setsProperty()
        {
            mStyle.Shape = Shape;
            var node = mStyle.CreateNode(Name, Label);
            Assert.That(node.Shape, Is.EqualTo(Shape));
        }

        [Test]
        public void CreateNode_configuredFillColor_setsProperty()
        {
            mStyle.FillColor = mFillColor;
            var node = mStyle.CreateNode(Name, Label);
            Assert.That(node.FillColor, Is.EqualTo(mFillColor));
        }

        private NodeStyle mStyle;
    }
}
