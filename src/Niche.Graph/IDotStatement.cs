using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Niche.Graphs
{
    public interface IDotStatement
    {
        /// <summary>
        /// Convert this statement into text
        /// </summary>
        /// <returns>Rendering as a string</returns>
        string AsText();

        /// <summary>
        /// Gets the base text for this Dot statement
        /// </summary>
        string Text
        {
            get;
        }

        /// <summary>
        /// Gets a sequence of Attribute names
        /// </summary>
        IEnumerable<string> Attributes
        {
            get;
        }

        /// <summary>
        /// Returns the value stored for a given attribute
        /// </summary>
        /// <param name="attribute">Name of attribute</param>
        /// <returns>Value of attribute</returns>
        /// <exception cref="InvalidOperationException">
        /// If the specified attribute is not present.
        /// </exception>
        string AttributeValue(string attribute);
    }
}
