using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Niche.NAntGraph.Tests
{
    public class NodeGeneratorTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void VisitProject_missingProject_throwsException()
        {
            var generator = new NodeGenerator();
            generator.VisitProject(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void VisitTarget_missingTarget_throwsException()
        {
            var generator = new NodeGenerator();
            generator.VisitTarget(null);
        }

        [Test]
        public void VisitTarget_withTarget_generatesNode()
        {
            var target = CreateTarget("sample");
            var generator = new NodeGenerator();
            generator.VisitTarget(target);
            Assert.That(generator.Nodes, Has.Count.EqualTo(1));
        }

        private NAntProject CreateProject(string name)
        {
            return new NAntProject(name, new List<NAntTarget>());
        }

        private NAntTarget CreateTarget(string name)
        {
            return new NAntTarget(name, "description", string.Empty);
        }
    }
}
