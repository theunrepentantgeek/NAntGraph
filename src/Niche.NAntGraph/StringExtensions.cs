using System;
using System.Text;

namespace Niche.NAntGraph
{
    /// <summary>
    /// Extension methods for strings
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Wrap text to the specified number of characters
        /// </summary>
        /// Adapted from a post on StackOverflow
        /// http://stackoverflow.com/questions/17586/best-word-wrap-algorithm
        /// <param name="text">String to wrap.</param>
        /// <param name="width">Maximum number of characters per line.</param>
        /// <returns>Wrapped string.</returns>
        public static string Wrap(this string text, int width)
        {
            return Wrap(text, width, "\n");
        }


        /// <summary>
        /// Wrap text to the specified number of characters with a custom line terminator
        /// </summary>
        /// Adapted from a post on StackOverflow
        /// http://stackoverflow.com/questions/17586/best-word-wrap-algorithm
        /// <param name="text">String to wrap.</param>
        /// <param name="width">Maximum number of characters per line.</param>
        /// <param name="terminator">Terminator to use at the end of each line.</param>
        /// <returns>Wrapped string.</returns>
        public static string Wrap(this string text, int width, string terminator)
        {
            int curLineLength = 0;
            var strBuilder = new StringBuilder();
            foreach (var word in text.Split(' '))
            {
                // If adding the new word to the current line would be too long,
                // then put it on a new line (and split it up if it's too long).
                if (curLineLength + word.Length > width)
                {
                    // Only move down to a new line if we have text on the current line.
                    // Avoids situation where wrapped whitespace causes emptylines in text.
                    if (curLineLength > 0)
                    {
                        strBuilder.Append(terminator);
                        curLineLength = 0;
                    }

                    // If the current word is too long to fit on a line even on it's own then
                    // split the word up.
                    if (word.Length > width)
                    {
                        strBuilder.AppendLine(word);
                    }
                }

                strBuilder.Append(word + " ");
                curLineLength += word.Length;
            }

            // Only move down to a new line if we have text on the current line.
            // Avoids situation where wrapped whitespace causes emptylines in text.
            if (curLineLength > 0)
            {
                strBuilder.Append(terminator);
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// Limit a string to a specified length, adding an ellipsis if truncated
        /// </summary>
        /// <param name="text">Text string to (possibly) truncate.</param>
        /// <param name="limit">Maximum allowable length.</param>
        /// <returns>String with max length</returns>
        public static string Ellipsis(this string text, int limit)
        {
            if (limit <= 3)
            {
                throw new ArgumentException(
                    "Limit must be greater than 3 characters",
                    "limit");
            }

            if (text == null)
            {
                return string.Empty;
            }

            if (text.Length <= limit)
            {
                return text;
            }

            return text.Substring(0, limit - 3) + "...";
        }
    }
}