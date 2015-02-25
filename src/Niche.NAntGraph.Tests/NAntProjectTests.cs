using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using NUnit.Framework;

using Rhino.Mocks;

namespace Niche.NAntGraph.Tests
{
    [TestFixture]
    public class NAntProjectTests
    {
        [SetUp]
        public void SetUp()
        {
            mRepository = new MockRepository();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_missingName_throwsException()
        {
            var targets = new List<NAntTarget>();
            new NAntProject(null, targets);
        }

        [Test]
        public void Constructor_givenName_setsProperty()
        {
            const string FileName = "sample";
            var targets = new List<NAntTarget>();
            var file = new NAntProject(FileName, targets);
            Assert.That(file.Name, Is.EqualTo(FileName));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_missingTargets_throwsException()
        {
            new NAntProject("name", null);
        }

        [Test]
        public void Constructor_givenEmptyTargets_setsProperty()
        {
            const string FileName = "sample";
            var targets = new List<NAntTarget>();
            var statement = new NAntProject(FileName, targets);
            Assert.That(statement.Targets, Has.Count.EqualTo(0));
        }

        [Test]
        public void Constructor_givenTargets_setsProperty()
        {
            const string FileName = "sample";
            var targets
                = new List<NAntTarget>
                      {
                          new NAntTarget("alpha", "description", string.Empty),
                          new NAntTarget("beta", "description", string.Empty)
                      };
            var statement = new NAntProject(FileName, targets);
            Assert.That(statement.Targets, Has.Count.EqualTo(targets.Count));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FromXml_missingElement_throwsException()
        {
            NAntProject.FromXml(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void FromXml_wrongElement_throwsException()
        {
            var element = new XElement("unexpected");
            NAntProject.FromXml(element);
        }

        [Test]
        public void FromXml_projectElement_returnsNAntBuildFile()
        {
            const string ProjectName = "sample";
            XElement element = CreateProjectXml(ProjectName);
            var file = NAntProject.FromXml(element);
            Assert.That(file, Is.Not.Null);
        }

        [Test]
        public void FromXml_projectNameAttribute_setsProperty()
        {
            const string ProjectName = "sample";
            XElement element = CreateProjectXml(ProjectName);
            var file = NAntProject.FromXml(element);
            Assert.That(file.Name, Is.EqualTo(ProjectName));
        }

        [Test]
        public void FromXml_emptyProject_hasZeroTargets()
        {
            const string ProjectName = "sample";
            XElement element = CreateProjectXml(ProjectName);
            var file = NAntProject.FromXml(element);
            Assert.That(file.Targets, Has.Count.EqualTo(0));
        }

        [Test]
        public void FromXml_projectWithSingleTarget_hasOneTarget()
        {
            const string ProjectName = "sample";
            XElement element = CreateProjectXml(ProjectName, "target");
            var file = NAntProject.FromXml(element);
            Assert.That(file.Targets, Has.Count.EqualTo(1));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Visit_missingVisitor_throwsException()
        {
            var project = CreateProject("sample");
            project.Visit(null);
        }

        [Test]
        public void Visit_givenVisitor_callsVisitProject()
        {
            var visitor = mRepository.DynamicMock<INAntVisitor>();
            using (mRepository.Record())
            {
                visitor.VisitProject(null);
                LastCall.IgnoreArguments()
                    .Repeat.Once();
            }

            using (mRepository.Playback())
            {
                var project = CreateProject("sample");
                project.Visit(visitor);
            }
        }

        [Test]
        public void Visit_givenVisitor_callsVisitForTargets()
        {
            var visitor = mRepository.DynamicMock<INAntVisitor>();
            using (mRepository.Record())
            {
                visitor.VisitProject(null);
                LastCall.IgnoreArguments()
                    .Repeat.Once();

                visitor.VisitTarget(null);
                LastCall.IgnoreArguments()
                    .Repeat.Once();
            }

            using (mRepository.Playback())
            {
                var project = CreateProject("sample", new NAntTarget("target", "description", string.Empty));
                project.Visit(visitor);
            }
        }

        private NAntProject CreateProject(string name, params NAntTarget[] targets)
        {
            return new NAntProject(name, targets);
        }

        private XElement CreateProjectXml(
            string projectName,
            params string[] targets)
        {
            return new XElement(
                NAntXml.Project,
                new XAttribute(NAntXml.ProjectName, projectName),
                targets.Select(
                    s => new XElement(
                             NAntXml.Target,
                             new XAttribute(NAntXml.TargetName, s))));
        }

        private MockRepository mRepository;
    }
}
