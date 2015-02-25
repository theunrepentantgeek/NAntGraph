using System.Drawing;

namespace Niche.Graphs
{
    /// <summary>
    /// Represents styling for a node
    /// </summary>
    public class NodeStyle
    {
        /// <summary>
        /// Gets or sets the shape of the node 
        /// </summary>
        public NodeShape Shape
        {
            get
            {
                return mShape;
            }

            set
            {
                mShape = value;
            }
        }

        /// <summary>
        /// Gets or sets the colour of the node
        /// </summary>
        public Color FillColor
        {
            get
            {
                return mFillColor;
            }

            set
            {
                mFillColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the font face to use for the node
        /// </summary>
        public string Font
        {
            get
            {
                return mFont;
            }
            set
            {
                mFont = value;
            }
        }

        /// <summary>
        /// Gets or sets the font size to use for the node
        /// </summary>
        public int FontSize
        {
            get
            {
                return mFontSize;
            }
            set
            {
                mFontSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the colour for the label of the node
        /// </summary>
        public Color FontColor
        {
            get
            {
                return mFontColor;
            }

            set
            {
                mFontColor = value;
            }
        }

        /// <summary>
        /// Create a new node
        /// </summary>
        /// <param name="name">Name for the new node</param>
        /// <param name="label">Label to display for the new node</param>
        /// <returns>Returns the new node.</returns>
        public Node CreateNode(string name, string label)
        {
            var result = new Node(name, label, this);
            return result;
        }

        /// <summary>
        /// Storage for the Shape property
        /// </summary>
        private NodeShape mShape;

        /// <summary>
        /// Storage for the FillColor property;
        /// </summary>
        private Color mFillColor;

        /// <summary>
        /// Storage for the Font property
        /// </summary>
        private string mFont;

        /// <summary>
        /// Storage for the FontColor property
        /// </summary>
        private Color mFontColor;

        /// <summary>
        /// Storage for the Font Size property
        /// </summary>
        private int mFontSize;
    }
}
