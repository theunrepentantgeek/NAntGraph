using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Niche.Graphs
{
    /// <summary>
    /// Utility class used to create complete graphs
    /// </summary>
    public class GraphFactory
    {
        /// <summary>
        /// Factory method to create a graph
        /// </summary>
        /// <param name="content">
        /// Sequence of nodes, edges, graphs and sequences of these.</param>
        /// <returns>Newly created graph instance.</returns>
        public Graph CreateGraph(params object[] content)
        {
            var collector = new Collector();
            collector.Scan(content);

            return new Graph(collector.Nodes, collector.Edges, collector.Graphs);
        }

        /// <summary>
        /// Internal class used to recursively scan graph content and isolate 
        /// nodes, edges and subgraphs.
        /// </summary>
        private class Collector
        {
            /// <summary>
            /// Gets the set of collected nodes
            /// </summary>
            public List<Node> Nodes
            {
                get
                {
                    return mNodes;
                }
            }

            /// <summary>
            /// Gets the set of collected edges
            /// </summary>
            public List<Edge> Edges
            {
                get
                {
                    return mEdges;
                }
            }

            /// <summary>
            /// Gets the set of collected graphs
            /// </summary>
            public List<Graph> Graphs
            {
                get
                {
                    return mGraphs;
                }
            }

            /// <summary>
            /// Scan the supplied enumerable and collect nodes, edges and graphs
            /// </summary>
            /// <param name="content">Enumerator to scan</param>
            public void Scan(IEnumerable content)
            {
                foreach (var o in content)
                {
                    if (o == null)
                    {
                        continue;
                    }

                    var node = o as Node;
                    if (node != null)
                    {
                        Nodes.Add(node);
                        continue;
                    }

                    var edge = o as Edge;
                    if (edge != null)
                    {
                        Edges.Add(edge);
                        continue;
                    }

                    var graph = o as Graph;
                    if (graph != null)
                    {
                        Graphs.Add(graph);
                        continue;
                    }

                    var enumerable = o as IEnumerable;
                    if (enumerable != null)
                    {
                        Scan(enumerable);
                        continue;
                    }

                    var message = string.Format("Instance of type {0} not supported", o.GetType());
                    throw new InvalidOperationException(message);
                }
            }

            /// <summary>
            /// Storage for the Nodes property
            /// </summary>
            private readonly List<Node> mNodes = new List<Node>();

            /// <summary>
            /// Storage for the Edges property
            /// </summary>
            private readonly List<Edge> mEdges = new List<Edge>();

            /// <summary>
            /// Storage for the Graphs property
            /// </summary>
            private readonly List<Graph> mGraphs = new List<Graph>();
        }
    }
}
