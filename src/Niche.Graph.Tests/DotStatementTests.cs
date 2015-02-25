using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace Niche.Graphs.Tests
{
    [TestFixture]
    public class DotStatementTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_missingStatement_throwsException()
        {
            new DotStatement(null);
        }

        [Test]
        public void Constructor_givenStatement_setsProperty()
        {
            const string s = "sample";
            var statement = new DotStatement(s);
            Assert.That(statement.Text, Is.EqualTo(s));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddAttribute_missingIdentifier_throwsException()
        {
            new DotStatement("text").AddAttribute(null, "value");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddAttribute_missingValue_throwsException()
        {
            new DotStatement("text").AddAttribute("identifier", null);
        }

        [Test]
        public void AddAttribute_fullySpecified_returnsDotStatementWithAttribute()
        {
            const string Attribute = "color";
            const string Value = "red";

            var statement
                = new DotStatement("text")
                    .AddAttribute(Attribute, Value);
            Assert.That(statement.AttributeValue(Attribute), Is.EqualTo(Value));
        }

        [Test]
        public void AsLine_withNoAttributes_includesText()
        {
            const string Text = "Sample";
            var statement = new DotStatement(Text);
            Assert.That(statement.AsText(), Is.StringContaining(Text));
        }

        [Test]
        public void AsLine_withNoAttributes_hasNoAttributeListOpen()
        {
            const string Text = "Sample";
            var statement = new DotStatement(Text);
            Assert.That(statement.AsText(), Is.Not.StringContaining("["));
        }

        [Test]
        public void AsLine_withNoAttributes_hasNoAttributeListClose()
        {
            const string Text = "Sample";
            var statement = new DotStatement(Text);
            Assert.That(statement.AsText(), Is.Not.StringContaining("]"));
        }

        [Test]
        public void AsLine_withAttribute_hasAttributeName()
        {
            const string Attribute = "Color";
            const string Color = "Green";
            var statement
                = new DotStatement("Sample")
                    .AddAttribute(Attribute, Color);
            Assert.That(
                statement.AsText(), 
                Is.StringContaining(Attribute) & Is.StringContaining(Color));
        }

        [Test]
        public void AsLine_withTwoAttributes_hasGapBetween()
        {
            var statement
                = new DotStatement("sample")
                    .AddAttribute("color", "green")
                    .AddAttribute("size", "large");
            Assert.That(
                statement.AsText(),
                Has.No.StringContaining("green\"size"));
        }
    }
}
