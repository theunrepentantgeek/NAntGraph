using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Niche.NAntGraph
{
    public class NAntProjectException : Exception
    {
        public NAntProjectException(string message, Exception innerException)
            : base(message, innerException)
        {
            // Nothing
        }
    }
}
