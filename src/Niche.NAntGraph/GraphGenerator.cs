using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Niche.Graphs;
using Niche.Shared;

namespace Niche.NAntGraph
{
    /// <summary>
    /// Generator class to create a graph from one or more NAnt files
    /// </summary>
    public class GraphGenerator
    {
        /// <summary>
        /// Gets a sequence of projects for which a graph will be generated
        /// </summary>
        public IEnumerable<NAntProject> Projects
        {
            get
            {
                return mProjects;
            }
        }

        /// <summary>
        /// Gets the generated image
        /// </summary>
        public Image Image
        {
            get
            {
                return mImage;
            }
        }

        /// <summary>
        /// Gets the text of the generated dot script
        /// </summary>
        public string DotText
        {
            get
            {
                return mDotText;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphGenerator"/> class.
        /// </summary>
        /// <param name="projects">Projects to include in the graph.</param>
        /// <param name="nodeGenerator">Generator for graph nodes</param>
        /// <param name="edgeGenerator">Generator for graph edges</param>
        public GraphGenerator(
            IEnumerable<NAntProject> projects, 
            NodeGenerator nodeGenerator, 
            EdgeGenerator edgeGenerator)
        {
            Require.NotNull("projects", projects);
            Require.NotNull("nodeGenerator", nodeGenerator);
            Require.NotNull("edgeGenerator", edgeGenerator);
            Require.NotEmpty("projects", projects);

            mProjects = new List<NAntProject>(projects);
            mNodeGenerator = nodeGenerator;
            mEdgeGenerator = edgeGenerator;
        }

        /// <summary>
        /// Generate statements depicting the graph of our projects' targets.
        /// </summary>
        /// <returns>Generated Graph</returns>
        public Graph GenerateGraph()
        {
            var subGraphs
                = mProjects.Select(p => CreateSubGraph(p))
                    .ToList();

            var nodes = subGraphs.SelectMany(g => g.Nodes);
            mEdgeGenerator.Reset(nodes);

            foreach (var p in mProjects)
            {
                p.Visit(mEdgeGenerator);
            }

            var graph = mFactory.CreateGraph(subGraphs, mEdgeGenerator.Edges);
            return graph;
        }

        /// <summary>
        /// Generate an image depicting the graph of our projects' targets.
        /// </summary>
        /// <returns>Generated Graph Image.</returns>
        public Image GenerateGraphImage()
        {
            var graph = GenerateGraph();
            var renderer = new DotRenderer(graph);
            mImage = renderer.RenderImage();
            mDotText = renderer.DotText;
            return Image;
        }

        /// <summary>
        /// Create a subgraph to depict the passed project
        /// </summary>
        /// <param name="project">Project to display.</param>
        /// <returns>Graph for this project's nodes</returns>
        private Graph CreateSubGraph(NAntProject project)
        {
            //var nodeGenerator
            //    = new NodeGenerator
            //          {
            //              IncludeDescriptions = mOptions.Includes(GraphGeneratorOptions.IncludeDescriptions)
            //          };

            project.Visit(mNodeGenerator);
            return mFactory.CreateGraph(mNodeGenerator.Nodes);
        }

        /// <summary>
        /// Storage for the Projects Property
        /// </summary>
        private readonly List<NAntProject> mProjects;

        /// <summary>
        /// Factory to use to generate graphs
        /// </summary>
        private readonly GraphFactory mFactory = new GraphFactory();

        /// <summary>
        /// Generated image
        /// </summary>
        private Image mImage;

        /// <summary>
        /// Text of the generated dot script
        /// </summary>
        private string mDotText;

        /// <summary>
        /// Generator to create nodes
        /// </summary>
        private readonly NodeGenerator mNodeGenerator;

        /// <summary>
        /// Generator to create edges
        /// </summary>
        private EdgeGenerator mEdgeGenerator;
    }
}
