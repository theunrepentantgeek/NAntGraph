using System;
using System.Collections.Generic;
using System.Xml.Linq;

using NUnit.Framework;

using Rhino.Mocks;

namespace Niche.NAntGraph.Tests
{
    [TestFixture]
    public class NAntTargetTests
    {
        [SetUp]
        public void SetUp()
        {
            mRepository = new MockRepository();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_withNullName_throwsException()
        {
            new NAntTarget(null, "description", "dependency");
        }

        [Test]
        public void Constructor_givenName_setsProperty()
        {
            const string Name = "name";
            const string Dependencies = "one, two three";
            var target = new NAntTarget(Name, "description", Dependencies);
            Assert.That(target.Name, Is.EqualTo(Name));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_withNullDependencies_throwsException()
        {
            new NAntTarget("name", "description", null);
        }

        [Test]
        public void Constructor_withEmptyDepends_setsProperty()
        {
            var target = new NAntTarget("name", "description", string.Empty);
            Assert.That(target.Depends, Has.Count.EqualTo(0));
        }

        [Test]
        public void Constructor_givenDependencies_setsProperty()
        {
            const string Name = "name";
            const string Dependencies = "one, two three";
            var target = new NAntTarget(Name, "description", Dependencies);
            Assert.That(target.Depends, Has.Count.EqualTo(3));
        }

        [Test]
        public void Constructor_givenDependencies_setsPropertyElements()
        {
            const string Name = "name";
            const string Dependencies = "one, two three";
            var members = new List<string>
                              {
                                  "one", 
                                  "two", 
                                  "three"
                              };
            var target = new NAntTarget(Name, "description", Dependencies);
            Assert.That(target.Depends, Is.EquivalentTo(members));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FromXml_withNullDocument_throwsException()
        {
            NAntTarget.FromXml(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void FromXml_withNonTarget_throwsException()
        {
            var element = new XElement("notTarget");
            NAntTarget.FromXml(element);
        }

        [Test]
        public void FromXml_withTargetElement_returnsNAntTarget()
        {
            var element
                = new XElement(
                    "target",
                    new XAttribute("name", "name"));
            var target = NAntTarget.FromXml(element);
            Assert.That(target, Is.Not.Null);
        }

        [Test]
        public void FromXml_withTargetName_returnsNAntTargetWithName()
        {
            const string TargetName = "compile.target";
            var element
                = new XElement(
                    "target",
                    new XAttribute("name", TargetName));
            var target = NAntTarget.FromXml(element);
            Assert.That(target.Name, Is.EqualTo(TargetName));
        }

        [Test]
        public void FromXml_withSingleDepends_returnsNAntTargetWithDepends()
        {
            const string TargetName = "compile.target";
            const string Depends = "single";
            var element
                = new XElement(
                    "target",
                    new XAttribute("name", TargetName),
                    new XAttribute("depends", Depends));
            var target = NAntTarget.FromXml(element);
            Assert.That(target.Depends, Has.Member(Depends));
        }

        [Test]
        public void FromXml_withSCalls_returnsNAntTargetWithDepends()
        {
            const string TargetName = "compile.target";
            const string Depends = "compile.dependency";
            var element
                = new XElement(
                    "target",
                    new XAttribute("name", TargetName),
                    new XElement("call", new XAttribute("target", Depends)));
            var target = NAntTarget.FromXml(element);
            Assert.That(target.Depends, Has.Member(Depends));
        }

        
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Visit_missingVisitor_throwsException()
        {
            var target = new NAntTarget("target", "description", "depends");
            target.Visit(null);
        }

        [Test]
        public void Visit_givenVisitor_callsVisitTarget()
        {
            var visitor = mRepository.DynamicMock<INAntVisitor>();
            using (mRepository.Record())
            {
                visitor.VisitTarget(null);
                LastCall.IgnoreArguments()
                    .Repeat.Once();
            }

            using (mRepository.Playback())
            {
                var target = new NAntTarget("target", "description", "depends");
                target.Visit(visitor);
            }
        }

        private MockRepository mRepository;
    }
}
