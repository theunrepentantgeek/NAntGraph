using System;
using System.Collections.Generic;
using System.Text;

namespace Niche.Graphs
{
    /// <summary>
    /// Represents a single statement from a dot script
    /// </summary>
    public class DotStatement : IDotStatement
    {
        /// <summary>
        /// Gets the base text for this Dot statement
        /// </summary>
        public string Text
        {
            get
            {
                return mText;
            }
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
        /// Initializes a new instance of the <see cref="DotStatement"/> class.
        /// </summary>
        /// <param name="text"> Text to display before any attributes</param>
        /// <exception cref="ArgumentNullException">
        /// If the text parameter is missing.
        /// </exception>
        public DotStatement(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException(
                    "text", 
                    "Must have base text supplied");
            }

            mText = text;
            mAttributes = new Dictionary<string, string>();
        }

        /// <summary>
        /// Add an attribute to this DotStatement, returning a new immutable 
        /// DotStatement instance.
        /// </summary>
        /// <param name="identifier">Attribute identifier.</param>
        /// <param name="value">Attribute value.</param>
        /// <returns>
        /// New immutable DotStatement instance with existing text and 
        /// attributes as well as the new attribute.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If either parameter is empty or missing.
        /// </exception>
        public DotStatement AddAttribute(string identifier, string value)
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

            return new DotStatement(Text, attributes);
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

        /// <summary>
        /// Generate a single line of output for this statement
        /// </summary>
        /// <returns></returns>
        public string AsText()
        {
            var result = new StringBuilder();
            result.Append(Text);

            if (mAttributes.Keys.Count > 0)
            {
                result.Append(" [ ");
                foreach (var k in mAttributes.Keys)
                {
                    result.AppendFormat("{0} = \"{1}\" ", k, mAttributes[k]);
                }

                result.Append(" ] ");
            }

            result.Append(";");
            return result.ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DotStatement"/> class with specified attributes.
        /// </summary>
        /// <param name="text"> Text to display before any attributes</param>
        /// <param name="attributes">Attribute values</param>
        private DotStatement(string text, Dictionary<string, string> attributes)
        {
            mText = text;
            mAttributes = attributes;
        }

        /// <summary>
        /// Storage for the Text property
        /// </summary>
        private readonly string mText;

        private readonly Dictionary<string, string> mAttributes;
    }
}
