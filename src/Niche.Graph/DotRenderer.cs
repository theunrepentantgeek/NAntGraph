using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;

using Niche.Shared;

namespace Niche.Graphs
{
    /// <summary>
    /// Renderer that takes a graph and generates an image of the graph
    /// </summary>
    public class DotRenderer : IGraphVisitor<IDotStatement>
    {
        /// <summary>
        /// Gets the Graph we are going to render
        /// </summary>
        public Graph Graph
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the generated dot statement
        /// </summary>
        public string DotText
        {
            get
            {
                return mDotText;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DotRenderer"/> class.
        /// </summary>
        /// <param name="graph">Graph to render.</param>
        public DotRenderer(Graph graph)
            : this()
        {
            if (graph == null)
            {
                throw new ArgumentNullException(
                    "graph",
                    "Graph must be provided to render");
            }

            Graph = graph;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DotRenderer"/> class.
        /// </summary>
        public DotRenderer()
        {
            // Nothing
        }

        /// <summary>
        /// Render an image from the graph.
        /// </summary>
        /// <returns>Generated image.</returns>
        public Image RenderImage()
        {
            var dotStatement = Graph.Visit(this);
            mDotText = dotStatement.AsText();

            var dotPath = FindDotPath();

            var imageFile
                = Path.GetRandomFileName();

            var startInfo
                = new ProcessStartInfo(dotPath)
                      {
                          FileName = dotPath,
                          Arguments = "-Tpng -o" + imageFile,
                          UseShellExecute = false,
                          RedirectStandardInput = true,
                          RedirectStandardOutput = true,
                          CreateNoWindow = true
                      };

            var reader = new Thread(ReadOutput);

            mProcess = Process.Start(startInfo);
            reader.Start();

            mProcess.StandardInput.Write(DotText);
            mProcess.StandardInput.Close();

            mProcess.WaitForExit();

            reader.Join();

            Image image = null;
            if (File.Exists(imageFile))
            {
                using (var imageStream = new FileStream(imageFile, FileMode.Open))
                {
                    image = Image.FromStream(imageStream);
                }

                File.Delete(imageFile);
            }

            return image;
        }

        /// <summary>
        /// Visit the specified edge
        /// </summary>
        /// <param name="edge">Edge to visit</param>
        /// <returns>Dot Statement for this edge.</returns>
        public IDotStatement VisitEdge(Edge edge)
        {
            Require.NotNull("edge", edge);
            var text = string.Format("\"{0}\" -> \"{1}\"", edge.Start.Name, edge.Finish.Name);
            var result
                = new DotStatement(text)
                    .AddAttribute(Dot.ArrowHead, ValueOf(edge.ArrowHead))
                    .AddAttribute(Dot.ArrowTail, ValueOf(edge.ArrowTail))
                    .AddAttribute(Dot.Color, ValueOf(edge.Color));

            if (edge.Constraining == false)
            {
                result = result.AddAttribute(Dot.EdgeConstraint, "false");
            }

            return result;
        }

        /// <summary>
        /// Visit the specified node
        /// </summary>
        /// <param name="node">Node to visit</param>
        /// <returns>Dot Statement for this node.</returns>
        public IDotStatement VisitNode(Node node)
        {
            Require.NotNull("node", node);
            var result
                = new DotStatement("\"" + node.Name + "\"")
                    .AddAttribute(Dot.Label, node.Label);

            if (node.Shape != NodeShape.None)
            {
                result = result.AddAttribute(Dot.NodeShape, ValueOf(node.Shape));
            }

            if (node.FillColor != Color.Empty)
            {
                result = result.AddAttribute(Dot.FillColor, ValueOf(node.FillColor))
                    .AddAttribute(Dot.Style, "filled");
            }

            if (node.FontColor != Color.Empty)
            {
                result = result.AddAttribute(Dot.FontColor, ValueOf(node.FontColor));
            }

            if (!string.IsNullOrEmpty(node.Font))
            {
                result = result.AddAttribute(Dot.Font, node.Font);
            }

            if (node.FontSize > 0)
            {
                result = result.AddAttribute(Dot.FontSize, node.FontSize.ToString());
            }
            return result;
        }

        /// <summary>
        /// Visit the specified graph
        /// </summary>
        /// <param name="graph">Graph to visit</param>
        /// <param name="nodes">Results from visiting contained nodes</param>
        /// <param name="edges">Results from visiting contained edges</param>
        /// <param name="subgraphs">Results from visiting contained subgraphs</param>
        /// <returns>Result from visiting this graph.</returns>
        public IDotStatement VisitGraph(
            Graph graph,
            IEnumerable<IDotStatement> nodes,
            IEnumerable<IDotStatement> edges,
            IEnumerable<IDotStatement> subgraphs)
        {
            Require.NotNull("graph", graph);
            Require.NotNull("nodes", nodes);
            Require.NotNull("edges", edges);
            Require.NotNull("subgraphs", subgraphs);

            var childStatements
                = nodes
                    .Union(edges)
                    .Union(subgraphs);

            var topLevel = Equals(graph, Graph);
            var keyword = topLevel ? "digraph" : "subgraph";

            var script
                = new DotStatementBlock(keyword + " test ", childStatements);
            //.AddAttribute("ratio", "0.71");

            return script;
        }

        private void ReadOutput()
        {
            mOutput = mProcess.StandardOutput.ReadToEnd();
        }

        /// <summary>
        /// Convert an arrow shape into a string
        /// </summary>
        /// <param name="shape">Enumeration value to convert</param>
        /// <returns>String equivalent.</returns>
        private static string ValueOf(ArrowShape shape)
        {
            switch (shape)
            {
                case ArrowShape.Box:
                    return "box";

                case ArrowShape.Crow:
                    return "crow";

                case ArrowShape.Diamond:
                    return "diamond";

                case ArrowShape.Dot:
                    return "dot";

                case ArrowShape.Inv:
                    return "inv";

                case ArrowShape.None:
                    return "none";

                case ArrowShape.Normal:
                    return "normal";

                case ArrowShape.Tee:
                    return "tee";

                case ArrowShape.Vee:
                    return "vee";

                default:
                    return "normal";
            }
        }

        /// <summary>
        /// Convert a node shape into a string
        /// </summary>
        /// <param name="nodeShape">Enumeration value to convert</param>
        /// <returns>String equivalent.</returns>
        private static string ValueOf(NodeShape nodeShape)
        {
            switch (nodeShape)
            {
                case NodeShape.Box:
                    return "box";

                case NodeShape.Circle:
                    return "circle";

                case NodeShape.Diamond:
                    return "diamond";

                case NodeShape.DoubleCircle:
                    return "doublecircle";

                case NodeShape.DoubleOctagon:
                    return "doubleoctagon";

                case NodeShape.Egg:
                    return "egg";

                case NodeShape.Ellipse:
                    return "ellipse";

                case NodeShape.Hexagon:
                    return "hexagon";

                case NodeShape.House:
                    return "house";

                case NodeShape.InvHouse:
                    return "invhouse";

                case NodeShape.InvTrapezium:
                    return "invtrapezium";

                case NodeShape.InvTriangle:
                    return "invtriangle";

                case NodeShape.MCircle:
                    return "Mcircle";

                case NodeShape.MDiamond:
                    return "Mdiamond";

                case NodeShape.MSquare:
                    return "Msquare";

                case NodeShape.Octagon:
                    return "octagon";

                case NodeShape.Parallelogram:
                    return "parallelogram";

                case NodeShape.Pentagon:
                    return "pentagon";

                case NodeShape.Plaintext:
                    return "plaintext";

                case NodeShape.Point:
                    return "point";

                case NodeShape.Polygon:
                    return "polygon";

                case NodeShape.Record:
                    return "record";

                case NodeShape.Septagon:
                    return "septagon";

                case NodeShape.Trapezium:
                    return "trapezium";

                case NodeShape.Triangle:
                    return "triangle";

                case NodeShape.TripleOctagon:
                    return "tripleoctagon";

                default:
                    return "box";
            }
        }

        /// <summary>
        /// Locate the installation for dot.exe
        /// </summary>
        /// <returns>Path to dot.exe, if found.</returns>
        private static string FindDotPath()
        {
            string path = ConfiguredDotPath() ?? DiscoveredDotPath();

            if (path == null)
            {
                ThrowDotNotFoundException();
            }

            return path;
        }

        /// <summary>
        /// Return the configured location for dot.exe
        /// </summary>
        /// <exception cref="DotNotFoundException">If the configured location does not point at an 
        /// existing copy of dot.exe</exception>
        /// <returns>Configured location (if valid), or an empty string (if not configured).
        /// </returns>
        private static string ConfiguredDotPath()
        {
            // Try to load from application settings
            string path = ConfigurationManager.AppSettings["dot.exe"];

            if (!string.IsNullOrEmpty(path))
            {
                // Configured location - use that
                if (!File.Exists(path))
                {
                    throw new DotNotFoundException(
                        "Dot.exe not found at configured location",
                        path);
                }

                return path;
            }

            return null;
        }

        /// <summary>
        /// Search for GraphViz installations
        /// </summary>
        /// <returns>GraphViz directory, or null if not found.</returns>
        private static string DiscoveredDotPath()
        {
            return DotSearchPath()
                .Select(p => new DirectoryInfo(p))
                .SelectMany(d => d.GetDirectories("graphviz*", SearchOption.TopDirectoryOnly))
                .OrderBy(d => d.Name)
                .Select(d => Path.Combine(d.FullName, "bin\\dot.exe"))
                .Where(File.Exists)
                .LastOrDefault();
        }

        /// <summary>
        /// Search path for places to look for the Dot executable
        /// </summary>
        /// <returns>Sequence of paths to search.</returns>
        private static IEnumerable<string> DotSearchPath()
        {
            var programFiles
                = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            yield return programFiles;

            var programFilesX86
                = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            if (!string.IsNullOrEmpty(programFilesX86)
                && !programFilesX86.Equals(programFiles, StringComparison.Ordinal))
            {
                yield return programFilesX86;
            }
        }

        private static void ThrowDotNotFoundException()
        {
            var message
                = string.Format(
                    "Couldn't find installation of GraphVis.\n"
                    + "Looked in {0}. \n"
                    + "Do you need to configure the location of dot.exe in the .config file?",
                    DotSearchPath().JoinWith(" and "));
            throw new DotNotFoundException(message);
        }

        /// <summary>
        /// Convert a colour to a string value for dot
        /// </summary>
        /// <param name="color">Colour to convert</param>
        /// <returns>String equivalent.</returns>
        private static string ValueOf(Color color)
        {
            return string.Format("#{0:x2}{1:x2}{2:x2}", color.R, color.G, color.B);
        }

        /// <summary>
        /// Execution of dot.exe
        /// </summary>
        private Process mProcess;

        /// <summary>
        /// Output from dot.exe
        /// </summary>
        private string mOutput;

        /// <summary>
        /// Storage for the generated dot statement
        /// </summary>
        private string mDotText;
    }
}
