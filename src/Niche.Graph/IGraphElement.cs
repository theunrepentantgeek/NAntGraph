using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Niche.Graph
{
    /// <summary>
    /// Interface used to unify elements within a graph
    /// </summary>
    public interface IGraphElement
    {
        /// <summary>
        /// Accept a specified visitor
        /// </summary>
        /// <param name="visitor">An instance interested in visiting Elements</param>
        void Accept(IGraphVisitor visitor);
    }
}
