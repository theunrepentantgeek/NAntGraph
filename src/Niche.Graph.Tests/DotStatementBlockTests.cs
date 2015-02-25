using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace Niche.Graphs.Tests
{
    [TestFixture]
    public class DotStatementBlockTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_missingText_throwsException()
        {
            new DotStatementBlock(null, new List<IDotStatement>());
        }

        [Test]
        public void Constructor_givenText_setsProperty()
        {
            const string Sample = "TextSample";
            var statement = new DotStatementBlock(Sample, new List<IDotStatement>());
            Assert.That(statement.Text, Is.EqualTo(Sample));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_missingStatements_throwsException()
        {
            new DotStatementBlock("Text", null);
        }

        [Test]
        public void AsText_emptyBlock_includesBlockOpen()
        {
            const string Sample = "TextSample";
            var statement = new DotStatementBlock(Sample, new List<IDotStatement>());
            Assert.That(statement.AsText(), Is.StringContaining("{"));
        }

        [Test]
        public void AsText_emptyBlock_includesBlockClose()
        {
            const string Sample = "TextSample";
            var statement = new DotStatementBlock(Sample, new List<IDotStatement>());
            Assert.That(statement.AsText(), Is.StringContaining("}"));
        }

        [Test]
        public void AsText_withStatement_includesStatementText()
        {
            var sampleStatement
                = new DotStatement("Sample")
                    .AddAttribute("Color", "Red");
            var innerStatements
                = new List<IDotStatement>
                      {
                         sampleStatement
                      };
            var statement = new DotStatementBlock("Block", innerStatements);
            Assert.That(
                statement.AsText(),
                Is.StringContaining(sampleStatement.AsText()));
        }
    }
}
