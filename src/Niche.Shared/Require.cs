using System;
using System.Collections.Generic;
using System.Linq;

namespace Niche.Shared
{
    /// <summary>
    /// Static utility methods for checking code "contracts"
    /// </summary>
    public static class Require
    {
        /// <summary>
        /// Check that the passed parameter is not null
        /// </summary>
        /// <typeparam name="T">Type of parameter passed</typeparam>
        /// <param name="parameterName">Name of parameter passed</param>
        /// <param name="parameterValue">Value of parameter</param>
        public static void NotNull<T>(string parameterName, T parameterValue)
            where T : class
        {
            if (parameterValue == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        /// <summary>
        /// Check that the passed string parameter is not empty
        /// </summary>
        /// <param name="parameterName">Name of the parameter</param>
        /// <param name="parameterValue">Value of the parameter</param>
        public static void NotEmpty(string parameterName, string parameterValue)
        {
            if (string.IsNullOrEmpty(parameterValue))
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        /// <summary>
        /// Check that the passed sequence is not empty
        /// </summary>
        /// <typeparam name="T">Type of values in the sequence</typeparam>
        /// <param name="parameterName">Name of the parameter</param>
        /// <param name="sequence">Sequence to test.</param>
        public static void NotEmpty<T>(string parameterName, IEnumerable<T> sequence)
        {
            if (sequence.Count() == 0)
            {
                throw new ArgumentException(parameterName);
            }
        }
    }
}