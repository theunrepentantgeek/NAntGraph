using System;
using System.Collections.Generic;
using System.Text;

namespace Niche.Graphs
{
    /// <summary>
    /// Represents a statement block from a dot script
    /// </summary>
    public class DotStatementBlock : IDotStatement
    {
        /// <summary>
        /// Gets the caption to display for this block
        /// </summary>
        public string Text
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a sequence of Attribute names
        /// </summary>
        public IEnumerable<string> Attributes
        {
            get
            {
                return mAttributes.Keys;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DotStatementBlock"/> class.
        /// </summary>
        /// <param name="text">Text to display before the block.</param>
        /// <param name="statements">Statements to contain in the block.</param>
        /// <exception cref="ArgumentNullException">
        /// If either argument is missing.
        /// </exception>
        public DotStatementBlock(string text, IEnumerable<IDotStatement> statements)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("text", "Text to display is required");
            }

            if (statements == null)
            {
                throw new ArgumentNullException("statements", "Must have statements to contain.");
            }

            Text = text;
            mContent = new List<IDotStatement>(statements);
            mAttributes = new Dictionary<string, string>();
        }

        /// <summary>
        /// Add an attribute to this DotStatementBlock, returning a new 
        /// immutable DotStatementBlock instance.
        /// </summary>
        /// <param name="identifier">Attribute identifier.</param>
        /// <param name="value">Attribute value.</param>
        /// <returns>
        /// New immutable DotStatementBlock instance with existing text and 
        /// attributes as well as the new attribute.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If either parameter is empty or missing.
        /// </exception>
        public DotStatementBlock AddAttribute(string identifier, string value)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                throw new ArgumentNullException(
                    "identifier",
                    "Attribute identifier is required");
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(
                    "value",
                    "Attribute value is required");
            }

            var attributes = new Dictionary<string, string>(mAttributes);
            attributes[identifier] = value;

            return new DotStatementBlock(Text, mContent, attributes);
        }


        /// <summary>
        /// Convert this statement into text
        /// </summary>
        /// <returns>Rendering as a string</returns>
        public string AsText()
        {
            var result = new StringBuilder();

            result.AppendLine(Text);

            result.AppendLine("{");

            if (mAttributes.Keys.Count > 0)
            {
                foreach (var k in mAttributes.Keys)
                {
                    result.AppendFormat("{0} = \"{1}\"; ", k, mAttributes[k]);
                }
            }
            result.AppendLine();

            foreach (var s in mContent)
            {
                result.AppendLine(s.AsText());
            }

            result.AppendLine("}");

            return result.ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DotStatementBlock"/> class.
        /// </summary>
        /// <param name="text">Text to display before the block.</param>
        /// <param name="statements">Statements to contain in the block.</param>
        /// <param name="attributes">Attributes to display.</param>
        /// <exception cref="ArgumentNullException">
        /// If either argument is missing.
        /// </exception>
        private DotStatementBlock(
            string text, 
            IEnumerable<IDotStatement> statements, 
            Dictionary<string, string> attributes)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("text", "Text to display is required");
            }

            if (statements == null)
            {
                throw new ArgumentNullException("statements", "Must have statements to contain.");
            }

            Text = text;
            mContent = new List<IDotStatement>(statements);
            mAttributes = attributes;
        }

        /// <summary>
        /// Returns the value stored for a given attribute
        /// </summary>
        /// <param name="attribute">Name of attribute</param>
        /// <returns>Value of attribute</returns>
        /// <exception cref="InvalidOperationException">
        /// If the specified attribute is not present.
        /// </exception>
        public string AttributeValue(string attribute)
        {
            try
            {
                return mAttributes[attribute];
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(
                    "Unable to return value of attribute " + attribute,
                    e);
            }
        }

        private readonly Dictionary<string, string> mAttributes;

        private List<IDotStatement> mContent;
    }
}
