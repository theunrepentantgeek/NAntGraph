using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

using Niche.NAntGraph;

namespace NAntGraph2
{
    /// <summary>
    /// Driver used to generate NAntGraphs
    /// </summary>
    public class Driver
    {
        /// <summary>
        /// Clear build files
        /// </summary>
        public void ClearBuildFiles()
        {
            mBuildFiles.Clear();
        }

        /// <summary>
        /// Add a build file to be included in the diagram
        /// </summary>
        /// <param name="filePath">Full file path to the build file</param>
        public void AddBuildFile(string filePath)
        {
            mBuildFiles.Add(filePath);
        }

        /// <summary>
        /// Sets the output file to write
        /// </summary>
        /// <param name="outputFile">File path for the image file</param>
        public void SetImageFile(string outputFile)
        {
            if (!string.IsNullOrEmpty(mOutputFile))
            {
                throw new InvalidOperationException(
                    "Image Output File has already been specified.");
            }

            mOutputFile = outputFile;
        }

        /// <summary>
        /// Sets the filename into which to write the dot script
        /// </summary>
        /// <param name="dotFile">File path for the dot script.</param>
        public void SetDotFile(string dotFile)
        {
            if (!string.IsNullOrEmpty(mDotFile))
            {
                throw new InvalidOperationException(
                    "Dot Output File has already been specified.");
            }

            mDotFile = dotFile;
        }

        /// <summary>
        /// Sets the font to use 
        /// </summary>
        /// <param name="fontName">Name of the font to use.</param>
        public void SetFont(string fontName)
        {
            mFontName = fontName;
        }

        /// <summary>
        /// Sets the font size to use 
        /// </summary>
        /// <param name="fontSize">Size of the font to use.</param>
        public void SetFontSize(string fontSize)
        {
            mFontSize = int.Parse(fontSize);
        }

        /// <summary>
        /// Configure whether to include descriptions
        /// </summary>
        /// <param name="showDescriptions">Indicate whether to show descriptions</param>
        public void ShowDescriptions(bool showDescriptions)
        {
            mShowDescriptions = showDescriptions;
        }

        /// <summary>
        /// Generate an image based on our configuration
        /// </summary>
        public void Generate()
        {
            if (mBuildFiles.Count == 0)
            {
                throw new InvalidOperationException(
                    "No Build files specified");
            }

            if (string.IsNullOrEmpty(mOutputFile))
            {
                mOutputFile
                    = Path.ChangeExtension(
                        mBuildFiles.First(),
                        ".png");
            }

            var projects
                = mBuildFiles.Select(f => LoadNAntProject(f))
                .ToList();

            Console.WriteLine("Generating Graph");
            var renderer = new GraphRenderer(mShowDescriptions, mFontName, mFontSize);
            Image graph = renderer.Render(projects);

            if (graph != null)
            {
                Console.WriteLine("Saving graph image to {0}", mOutputFile);
                graph.Save(mOutputFile);
            }
            else
            {
                Console.WriteLine("Failed to generate image.");
            }

            if (!string.IsNullOrEmpty(mDotFile))
            {
                Console.WriteLine("Saving dot script file to {0}", mDotFile);
                using (var dotFile = new FileStream(mDotFile, FileMode.Create))
                {
                    using (var writer = new StreamWriter(dotFile))
                    {
                        writer.Write(renderer.DotScript);
                    }

                    dotFile.Close();
                }
            }
        }

        private NAntProject LoadNAntProject(string filename)
        {
            Console.WriteLine("Loading {0}", filename);
            return NAntProject.Load(filename);
        }

        /// <summary>
        /// The file to write the image into
        /// </summary>
        private string mOutputFile;

        /// <summary>
        /// NAnt project files to read
        /// </summary>
        private readonly List<string> mBuildFiles = new List<string>();

        /// <summary>
        /// Flag indicating whether to include target descriptions
        /// </summary>
        private bool mShowDescriptions;

        /// <summary>
        /// The file to write the dot script into
        /// </summary>
        private string mDotFile;

        /// <summary>
        /// The font to use in generation
        /// </summary>
        private string mFontName;

        /// <summary>
        /// The font size to use in generation
        /// </summary>
        private int mFontSize;
    }
}