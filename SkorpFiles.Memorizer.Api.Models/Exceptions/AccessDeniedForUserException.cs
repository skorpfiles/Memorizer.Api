using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.Models.Exceptions
{

    [Serializable]
    public class AccessDeniedForUserException : Exception
    {
        public AccessDeniedForUserException() { }
        public AccessDeniedForUserException(string message) : base(message) { }
        public AccessDeniedForUserException(string message, Exception inner) : base(message, inner) { }
        protected AccessDeniedForUserException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
