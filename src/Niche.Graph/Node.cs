using System;
using System.Diagnostics;
using System.Drawing;

using Niche.Shared;

namespace Niche.Graphs
{
    /// <summary>
    /// Immutable Graph Node
    /// </summary>
    [DebuggerDisplay("{Label} ({Name})")]
    public class Node
    {
        /// <summary>
        /// Gets the internal name of this Node
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a display label for this Node
        /// </summary>
        public string Label
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the shape of this node
        /// </summary>
        public NodeShape Shape
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the colour to fill the node
        /// </summary>
        public Color FillColor
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the name of the font to use for the node label
        /// </summary>
        public string Font
        {
            get; 
            private set;
        }

        /// <summary>
        /// Gets the size of the font to use for the node labels
        /// </summary>
        public int FontSize
        {
            get; 
            private set;
        }

        /// <summary>
        /// Gets the color to use for the node label
        /// </summary>
        public Color FontColor
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        /// <param name="name">Internal name for this node.</param>
        /// <param name="label">Display label for this node.</param>
        /// <exception cref="ArgumentNullException">
        /// If any parameter is left out.
        /// </exception>
        public Node(string name, string label)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name", "Node name is required");
            }

            if (string.IsNullOrEmpty(label))
            {
                throw new ArgumentNullException("label", "Node label is required");
            }

            Name = name;
            Label = label;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        /// <param name="name">Internal name for this node.</param>
        /// <param name="label">Display label for this node.</param>
        /// <param name="style">Style to use for this node. Used to initialise properties.</param>
        /// <exception cref="ArgumentNullException">
        /// If any parameter is left out.
        /// </exception>
        public Node(string name, string label, NodeStyle style)
            : this(name, label)
        {
            if (style == null)
            {
                throw new ArgumentNullException("style", "Node style is required");
            }

            Shape = style.Shape;
            FillColor = style.FillColor;
            Font = style.Font;
            FontSize = style.FontSize;
            FontColor = style.FontColor;
        }

        public TReturn Visit<TReturn>(IGraphVisitor<TReturn> visitor)
            where TReturn : class
        {
            Require.NotNull("visitor", visitor);
            return visitor.VisitNode(this);
        }
    }
}
