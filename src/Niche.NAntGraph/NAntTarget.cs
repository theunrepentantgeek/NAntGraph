using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Niche.Shared;

namespace Niche.NAntGraph
{
    /// <summary>
    /// Representation of a single NAnt target
    /// </summary>
    public class NAntTarget
    {
        /// <summary>
        /// Gets the name of this Target
        /// </summary>
        public string Name
        {
            get
            {
                return mName;
            }
        }

        /// <summary>
        /// Gets a description of this target
        /// </summary>
        public string Description
        {
            get
            {
                return mDescription;
            }
        }

        /// <summary>
        /// Gets a sequence of dependent targets
        /// </summary>
        public IEnumerable<string> Depends
        {
            get
            {
                return mDepends;
            }
        }

        /// <summary>
        /// Create a NAntTarget from an Xml instance
        /// </summary>
        /// <param name="element">Element to load from</param>
        /// <returns>Newly constructed target.</returns>
        public static NAntTarget FromXml(XElement element)
        {
            Require.NotNull("element", element);

            if (!element.Name.LocalName.Equals("target"))
            {
                throw new ArgumentException("Expected <target> element", "element");
            }

            var name = (string)element.Attribute("name");
            var description
                = (string)element.Attribute("description")
                  ?? string.Empty;
            var depends = (string)element.Attribute("depends") 
                ?? string.Empty;

            return new NAntTarget(name, description, depends);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NAntTarget"/> class.
        /// </summary>
        /// <param name="name">Name of this target.</param>
        /// <param name="description">Description of this target</param>
        /// <param name="depends">Dependencies of this target.</param>
        public NAntTarget(string name, string description, string depends)
        {
            Require.NotEmpty("name", name);
            Require.NotNull("description", description);
            Require.NotNull("depends", depends);

            mName = name;
            mDescription = description;
            mDepends = depends.Split(',', ' ')
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList();
        }

        /// <summary>
        /// Visit this target to perform an action
        /// </summary>
        /// <param name="visitor">Visitor to carry out the action</param>
        public void Visit(INAntVisitor visitor)
        {
            Require.NotNull("visitor", visitor);
            visitor.VisitTarget(this);
        }

        /// <summary>
        /// Storage for the Name property
        /// </summary>
        private readonly string mName;

        /// <summary>
        /// Storage for the Description property
        /// </summary>
        private readonly string mDescription;

        /// <summary>
        /// Storage for the Depends property
        /// </summary>
        private readonly List<string> mDepends;
    }
}

