using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Niche.NAntGraph
{
    /// <summary>
    /// Specified functionality of an instance that can be visited by a NAnt Graph
    /// </summary>
    public interface INAntVisitor
    {
        /// <summary>
        /// Visit the specified project
        /// </summary>
        /// <param name="project">Project to visit.</param>
        void VisitProject(NAntProject project);

        /// <summary>
        /// Visit the specified target
        /// </summary>
        /// <param name="target">Target to visit.</param>
        void VisitTarget(NAntTarget target);
    }
}
