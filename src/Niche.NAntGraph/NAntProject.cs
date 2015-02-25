using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Niche.Shared;

namespace Niche.NAntGraph
{
    /// <summary>
    /// Representation of a complete NAnt file
    /// </summary>
    public class NAntProject
    {
        /// <summary>
        /// Gets the name of this NAnt Build File
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a list of targets within this NAnt Build file
        /// </summary>
        public IEnumerable<NAntTarget> Targets
        {
            get;
            private set;
        }

        /// <summary>
        /// Create a NAntProject to represent the passed NAnt file
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static NAntProject FromXml(XElement element)
        {
            Require.NotNull("element", element);

            if (!element.Name.LocalName.Equals(NAntXml.Project))
            {
                var message = string.Format("Expected <{0}> element", NAntXml.Project);
                throw new ArgumentException(message, "element");
            }

            var ns = element.Name.Namespace;
            var name = (string)element.Attribute(NAntXml.ProjectName);
            var targets
                = element.Elements(ns + NAntXml.Target)
                    .Select(e => NAntTarget.FromXml(e))
                    .ToList();

            return new NAntProject(name, targets);
        }

        /// <summary>
        /// Load a NAntProject file from the given URI and return a NAntProject 
        /// instance representing the contents.
        /// </summary>
        /// <param name="uri">URI for the file to load</param>
        /// <returns>Newly constructed NAntProject file.</returns>
        public static NAntProject Load(string uri)
        {
            try
            {
                var document = XDocument.Load(uri);
                return FromXml(document.Root);
            }
            catch (Exception ex)
            {
                string message
                    = String.Format(
                        "Failed to load NAntProject {0}",
                        uri);
                throw new NAntProjectException(message, ex);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NAntProject"/> class.
        /// </summary>
        /// <param name="name">Name of the build file.</param>
        /// <param name="targets">Targets within the file.</param>
        public NAntProject(string name, IEnumerable<NAntTarget> targets)
        {
            Require.NotNull("name", name);
            Require.NotNull("targets", targets);

            Name = name;
            Targets = new List<NAntTarget>(targets);
        }

        /// <summary>
        /// Visit this project to perform an action
        /// </summary>
        /// <param name="visitor">Visitor to carry out the action</param>
        public void Visit(INAntVisitor visitor)
        {
            Require.NotNull("visitor", visitor);
            visitor.VisitProject(this);

            foreach (var t in Targets)
            {
                t.Visit(visitor);
            }
        }
    }
}
