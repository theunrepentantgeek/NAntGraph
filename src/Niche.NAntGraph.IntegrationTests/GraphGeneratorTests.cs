using System.Collections.Generic;

using Niche.Graphs;

using NUnit.Framework;

namespace Niche.NAntGraph.IntegrationTests
{
    [TestFixture]
    public class GraphGeneratorTests
    {
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
            graph.Save("GenerateGraphImage.png");
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
