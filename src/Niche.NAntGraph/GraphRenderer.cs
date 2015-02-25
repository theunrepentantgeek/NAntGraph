using System;
using System.Collections.Generic;
using System.Drawing;

namespace Niche.NAntGraph
{
    public class GraphRenderer
    {
        /// <summary>
        /// Gets the generated image
        /// </summary>
        public Image Image
        {
            get
            {
                return mImage;
            }
        }

        /// <summary>
        /// Gets the generated dot script
        /// </summary>
        public string DotScript
        {
            get
            {
                return mDotScript;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphRenderer"/> class.
        /// </summary>
        /// <param name="includeDescriptions">
        /// Indicate whether to include descriptions
        /// </param>
        /// <param name="labelFont">
        /// Font to use for text.
        /// </param>
        public GraphRenderer(bool includeDescriptions, string labelFont, int fontSize)
        {
            mIncludeDescriptions = includeDescriptions;
            mLabelFont = labelFont;
            mLabelFontSize = fontSize;
        }

        /// <summary>
        /// Render a graph for the passed NAntProjects
        /// </summary>
        /// <param name="projects">Sequence of projects to render</param>
        /// <returns>Generated image</returns>
        public Image Render(IEnumerable<NAntProject> projects)
        {
            var nodeGenerator
                = new NodeGenerator
                      {
                          IncludeDescriptions = mIncludeDescriptions,
                          LabelFont = mLabelFont,
                          LabelFontSize = mLabelFontSize
                      };

            var edgeGenerator
                = new EdgeGenerator();

            var generator = new GraphGenerator(projects, nodeGenerator, edgeGenerator);

            mImage = generator.GenerateGraphImage();
            mDotScript = generator.DotText;
            return Image;
        }

        /// <summary>
        /// Flag to indicate whether to include target descriptions
        /// </summary>
        private readonly bool mIncludeDescriptions;

        /// <summary>
        /// Font to use for text
        /// </summary>
        private readonly string mLabelFont;

        /// <summary>
        /// Font size to use for text
        /// </summary>
        private readonly int mLabelFontSize;

        /// <summary>
        /// Storage for the generated image
        /// </summary>
        private Image mImage;

        /// <summary>
        /// Storage for the generated dot script
        /// </summary>
        private string mDotScript;
    }
}