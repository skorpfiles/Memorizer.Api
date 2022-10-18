using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.DataAccess.Exceptions
{

    [Serializable]
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException() { }
        public UserNotFoundException(string userId) : base($"A user with the ID '{userId}' has not been found.") { }
        public UserNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected UserNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
