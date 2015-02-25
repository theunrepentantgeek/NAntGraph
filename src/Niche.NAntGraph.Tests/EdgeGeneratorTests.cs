using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Niche.Graphs;

using NUnit.Framework;

namespace Niche.NAntGraph.Tests
{
    [TestFixture]
    public class EdgeGeneratorTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void VisitProject_missingProject_throwsException()
        {
            var visitor = CreateVisitor();
            visitor.VisitProject(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void VisitTarget_missingTarget_throwsException()
        {
            var visitor = CreateVisitor();
            visitor.VisitTarget(null);
        }

        [Test]
        public void VisitTarget_targetWithoutDepends_createsNoEdges()
        {
            var target = new NAntTarget("target", "description", string.Empty);
            var visitor = CreateVisitor();
            visitor.VisitTarget(target);
            Assert.That(visitor.Edges.Count(), Is.EqualTo(0));
        }

        [Test]
        public void VisitTarget_targetWithSingleDependency_createsOneEdge()
        {
            var target = new NAntTarget("target", "description", "alpha");
            var visitor = CreateVisitor();
            visitor.VisitTarget(target);
            Assert.That(visitor.Edges.Count(), Is.EqualTo(1));
        }

        [Test]
        public void VisitTarget_targetWithSingleDependency_createsTwoMissingNodes()
        {
            var target = new NAntTarget("target", "description", "alpha");
            var visitor = CreateVisitor();
            visitor.VisitTarget(target);
            Assert.That(visitor.MissingNodes.Count(), Is.EqualTo(2));
        }

        [Test]
        public void VisitTarget_targetWithSeveralDependencies_createsSeveralEdges()
        {
            // Expect edges between targets and dependencies
            // and between dependencies for sequencing
            var target = new NAntTarget("target", "description", "alpha beta gamma delta");
            var visitor = CreateVisitor();
            visitor.VisitTarget(target);
            Assert.That(visitor.Edges.Count(), Is.EqualTo(4));
        }

        private EdgeGenerator CreateVisitor()
        {
            return new EdgeGenerator();
        }

        private NAntProject CreateProject(string name, params string[] targets)
        {
            var t
                = targets.Select(n => CreateTarget(n))
                    .ToList();
            return new NAntProject(name, new List<NAntTarget>(t));
        }

        private NAntTarget CreateTarget(string name)
        {
            return new NAntTarget(name, "description", string.Empty);
        }
    }
}
