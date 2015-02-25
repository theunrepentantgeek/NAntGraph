using System.IO;

namespace Niche.Graphs
{
    public class DotNotFoundException : FileNotFoundException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DotNotFoundException"/> class
        /// when we didn't find dot.exe with a search
        /// </summary>
        /// <param name="message">Message describing the failed search</param>
        public DotNotFoundException(string message)
            : base(message)
        {
            // Nothing
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DotNotFoundException"/> class.
        /// when we didn't find dot.exe in a specific location.
        /// </summary>
        /// <param name="message">Message describing the failure</param>
        /// <param name="filepath">Expected location of dot.exe</param>
        public DotNotFoundException(string message, string filepath)
            : base(message, filepath)
        {
            // Nothing
        }
    }
}