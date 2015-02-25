using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Niche.Graphs
{
    /// <summary>
    /// Factory class to generate immutable configured Edges
    /// </summary>
    public class EdgeStyle
    {
        /// <summary>
        /// Gets or sets the shape of the arrowhead on edges created by this factory
        /// </summary>
        public ArrowShape ArrowHead
        {
            get
            {
                return mArrowHead;
            }

            set
            {
                mArrowHead = value;
            }
        }

        /// <summary>
        /// Gets or sets the shape of the arrowtail on edges created by this factory
        /// </summary>
        public ArrowShape ArrowTail
        {
            get
            {
                return mArrowTail;
            }

            set
            {
                mArrowTail = value;
            }
        }

        /// <summary>
        /// Gets or sets the colour to use when drawing the edge
        /// </summary>
        public Color Color
        {
            get
            {
                return mColor;
            }

            set
            {
                mColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this edge should be used for ranking
        /// </summary>
        public bool Constraining
        {
            get
            {
                return mConstraining;
            }

            set
            {
                mConstraining = value;
            }
        }

        public EdgeStyle()
        {
            Constraining = true;
        }

        /// <summary>
        /// Creates a new edge with configuration as specified by this factory
        /// </summary>
        /// <param name="start">Node from which the edge starts</param>
        /// <param name="finish">Node on which the edge finishes</param>
        /// <returns>Newly created edge</returns>
        public Edge CreateEdge(Node start, Node finish)
        {
            if (start == null)
            {
                throw new ArgumentNullException("start", "Start node is mandatory");
            }

            if (finish == null)
            {
                throw new ArgumentNullException("finish", "Finish node is mandatory");
            }

            var edge = new Edge(start, finish, this);
            return edge;
        }

        /// <summary>
        /// Storage for the ArrowHead property
        /// </summary>
        private ArrowShape mArrowHead;

        /// <summary>
        /// Storage for the ArrowTail property
        /// </summary>
        private ArrowShape mArrowTail;

        private Color mColor;

        private bool mConstraining;
    }
}
