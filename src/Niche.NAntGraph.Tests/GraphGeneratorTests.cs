using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Niche.Graphs;

using NUnit.Framework;

namespace Niche.NAntGraph.Tests
{
    [TestFixture]
    public class GraphGeneratorTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_withoutNAntProjectSequence_throwsException()
        {
            new GraphGenerator(null, CreateNodeGenerator(), CreateEdgeGenerator());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_withoutNodeGenerator_throwsException()
        {
            var project = CreateProject("sample");
            new GraphGenerator(CreateProjectList(project), null, CreateEdgeGenerator());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_withoutEdgeGenerator_throwsException()
        {
            var project = CreateProject("sample");
            new GraphGenerator(CreateProjectList(project), CreateNodeGenerator(), null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_withNoProject_throwsException()
        {
            new GraphGenerator(CreateEmptyProjectList(), CreateNodeGenerator(), CreateEdgeGenerator());
        }

        [Test]
        public void Constructor_withProject_setsProperty()
        {
            var project = CreateProject("sample");
            var generator = new GraphGenerator(CreateProjectList(project), CreateNodeGenerator(), CreateEdgeGenerator());
            Assert.That(generator.Projects, Has.Count.EqualTo(1));
        }

        [Test]
        public void GenerateGraph_withProject_returnsGraph()
        {
            var project = CreateProject("sample");
            var generator = new GraphGenerator(CreateProjectList(project), CreateNodeGenerator(), CreateEdgeGenerator());
            Assert.That(generator.GenerateGraph(), Is.Not.Null);
        }

        [Test]
        public void GenerateGraph_withSimpleProject_returnsGraph()
        {
            var project
                = CreateProject(
                    "sample",
                    CreateTarget("build", "clean compile"),
                    CreateTarget("clean"),
                    CreateTarget("compile"));
            var generator = new GraphGenerator(CreateProjectList(project), CreateNodeGenerator(), CreateEdgeGenerator());
            Assert.That(generator.GenerateGraph(), Is.Not.Null);
        }

        [Test]
        public void GenerateGraph_withSimpleProject_returnsSingleSubGraph()
        {
            var project
                = CreateProject(
                    "sample",
                    CreateTarget("build", "clean compile"),
                    CreateTarget("clean"),
                    CreateTarget("compile"));
            var generator = new GraphGenerator(CreateProjectList(project), CreateNodeGenerator(), CreateEdgeGenerator());
            var graph = generator.GenerateGraph();
            Assert.That(graph.SubGraphs, Has.Count.EqualTo(1));
        }

        [Test]
        public void GenerateGraph_withSimpleProject_returnsSingleSubGraphWithExpectedNodes()
        {
            var project
                = CreateProject(
                    "sample",
                    CreateTarget("build", "clean compile"),
                    CreateTarget("clean"),
                    CreateTarget("compile"));
            var generator = new GraphGenerator(CreateProjectList(project), CreateNodeGenerator(), CreateEdgeGenerator());
            var graph = generator.GenerateGraph();
            var subgraph = graph.SubGraphs.Single();
            Assert.That(subgraph.Nodes, Has.Count.EqualTo(3));
        }

        [Test]
        public void GenerateGraphImage_withSimpleProject_returnsImage()
        {
            var project
                = CreateProject(
                    "sample",
                    CreateTarget("build", "clean compile"),
                    CreateTarget("clean"),
                    CreateTarget("compile"));
            var generator = new GraphGenerator(CreateProjectList(project), CreateNodeGenerator(), CreateEdgeGenerator());
            var graph = generator.GenerateGraphImage();
            Assert.That(graph, Is.Not.Null);
        }

        private IEnumerable<NAntProject> CreateProjectList(NAntProject project)
        {
            return new List<NAntProject> { project };
        }

        private IEnumerable<NAntProject> CreateEmptyProjectList()
        {
            return new List<NAntProject>();
        }

        private NodeGenerator CreateNodeGenerator()
        {
            return new NodeGenerator();
        }

        private EdgeGenerator CreateEdgeGenerator()
        {
            return new EdgeGenerator();
        }

        private NAntProject CreateProject(string projectName, params NAntTarget[] targets)
        {
            return new NAntProject(projectName, targets);
        }

        private NAntTarget CreateTarget(string name)
        {
            return new NAntTarget(name, "description", string.Empty);
        }

        private NAntTarget CreateTarget(string name, string depends)
        {
            return new NAntTarget(name, "description", depends);
        }
    }
}