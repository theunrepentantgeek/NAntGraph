using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Niche.Graphs
{
    /// <summary>
    /// Specified functionality of an instance that can be visited by an IGraphElement
    /// </summary>
    /// <typeparam name="TReturn">Type of results to return from visiting</typeparam>
    public interface IGraphVisitor<TReturn>
        where TReturn : class
    {
        /// <summary>
        /// Visit the specified edge
        /// </summary>
        /// <param name="edge">Edge to visit</param>
        /// <returns>Result from visiting this edge.</returns>
        TReturn VisitEdge(Edge edge);

        /// <summary>
        /// Visit the specified node
        /// </summary>
        /// <param name="node">Node to visit</param>
        /// <returns>Result from visiting this node.</returns>
        TReturn VisitNode(Node node);

        /// <summary>
        /// Visit the specified graph
        /// </summary>
        /// <param name="graph">Graph to visit</param>
        /// <param name="nodes">Results from visiting contained nodes</param>
        /// <param name="edges">Results from visiting contained edges</param>
        /// <param name="subgraphs">Results from visiting contained subgraphs</param>
        /// <returns>Result from visiting this graph.</returns>
        TReturn VisitGraph(Graph graph, IEnumerable<TReturn> nodes, IEnumerable<TReturn> edges, IEnumerable<TReturn> subgraphs);
    }
}
