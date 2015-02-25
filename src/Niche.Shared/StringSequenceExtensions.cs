using System.Collections.Generic;
using System.Text;

namespace Niche.Shared
{
    public static class StringSequenceExtensions
    {
        /// <summary>
        /// Join a sequence of strings with a given separator
        /// </summary>
        /// <param name="sequence">Sequence of strings to join.</param>
        /// <param name="separator">Separator to insert.</param>
        /// <returns>Joined string.</returns>
        public static string JoinWith(
            this IEnumerable<string> sequence,
            string separator)
        {
            var iterator = sequence.GetEnumerator();
            var result = new StringBuilder();

            var more = iterator.MoveNext();
            while (more)
            {
                result.Append(iterator.Current);
                more = iterator.MoveNext();
                if (more)
                {
                    result.Append(separator);
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Join a sequence of strings with a given separator
        /// </summary>
        /// <param name="sequence">Sequence of strings to join.</param>
        /// <param name="separator">Separator to insert.</param>
        /// <param name="lastSeparator">Last separator to use</param>
        /// <returns>Joined string.</returns>
        public static string JoinWith(
            this IEnumerable<string> sequence,
            string separator,
            string lastSeparator)
        {
            var iterator = sequence.GetEnumerator();
            var result = new StringBuilder();

            if (iterator.MoveNext())
            {
                // Have at least one item
                result.Append(iterator.Current);

                if (iterator.MoveNext())
                {
                    // Have at least two items
                    var item = iterator.Current;
                    while (iterator.MoveNext())
                    {
                        result.Append(separator);
                        result.Append(item);
                        item = iterator.Current;
                    }

                    result.Append(lastSeparator);
                    result.Append(item);
                }
            }

            return result.ToString();
        }
    }
}
