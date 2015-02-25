using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Niche.Graphs;
using Niche.Shared;

namespace Niche.NAntGraph
{
    /// <summary>
    /// Create edges for our NAnt graph.
    /// </summary>
    public class EdgeGenerator : INAntVisitor
    {
        /// <summary>
        /// Gets the sequence of Edges created by this generator
        /// </summary>
        public IEnumerable<Edge> Edges
        {
            get
            {
                return mEdges;
            }
        }

        /// <summary>
        /// Gets the nodes created for missing dependencies
        /// </summary>
        public IEnumerable<Node> MissingNodes
        {
            get
            {
                return mMissingNodes;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdgeGenerator"/> class.
        /// </summary>
        public EdgeGenerator()
        {
            mNodes = new Dictionary<string, Node>();
            mMissingNodeStyle
                = new NodeStyle
                      {
                          FillColor = Color.Red,
                          FontColor = Color.White,
                          Shape = NodeShape.Octagon
                      };

            mDependencyEdges
                = new EdgeStyle
                      {
                          ArrowHead = ArrowShape.Normal,
                          ArrowTail = ArrowShape.None,
                          Color = Color.Gray
                      };
        }

        /// <summary>
        /// Resets this EdgeGenerator for reuse
        /// </summary>
        /// <param name="nodes">Nodes that make up this graph.</param>
        public void Reset(IEnumerable<Node> nodes)
        {
            Require.NotNull("nodes", nodes);

            mNodes = nodes.ToDictionary(n => n.Name);   
        }

        /// <summary>
        /// Visit the specified project
        /// </summary>
        /// <param name="project">Project to visit.</param>
        public void VisitProject(NAntProject project)
        {
            Require.NotNull("project", project);
        }

        /// <summary>
        /// Visit the specified target
        /// </summary>
        /// <param name="target">Target to visit.</param>
        public void VisitTarget(NAntTarget target)
        {
            Require.NotNull("target", target);
            var targetNode = FindNode(target.Name);

            foreach (var d in target.Depends)
            {
                var dependentNode = FindNode(d);
                var edge = mDependencyEdges.CreateEdge(targetNode, dependentNode);
                mEdges.Add(edge);
            }
        }

        /// <summary>
        /// Find a node based on its name
        /// </summary>
        /// <param name="nodeName">Name of required node</param>
        /// <returns>Node found (may be a new node)</returns>
        private Node FindNode(string nodeName)
        {
            // Find regular Node
            Node result;
            if (mNodes.TryGetValue(nodeName, out result))
            {
                return result;
            }

            // Find missing node
            result = mMissingNodes.FirstOrDefault(n => n.Name.Equals(nodeName));
            if (result != null)
            {
                return result;
            }

            // Create a new missing node
            result = new Node(nodeName, nodeName, mMissingNodeStyle);
            mMissingNodes.Add(result);
            return result;
        }

        /// <summary>
        /// Storage for the Nodes property
        /// </summary>
        private Dictionary<string, Node> mNodes;

        /// <summary>
        /// Storage for the Edges property
        /// </summary>
        private readonly List<Edge> mEdges = new List<Edge>();

        /// <summary>
        /// Storage for missing Nodes we create
        /// </summary>
        private readonly List<Node> mMissingNodes = new List<Node>();

        /// <summary>
        /// Reference style for missing nodes
        /// </summary>
        private readonly NodeStyle mMissingNodeStyle;

        /// <summary>
        /// Reference style for dependencies
        /// </summary>
        private readonly EdgeStyle mDependencyEdges;
    }
}
