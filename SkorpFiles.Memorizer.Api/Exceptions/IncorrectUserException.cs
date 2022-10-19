namespace SkorpFiles.Memorizer.Api.Exceptions
{

    [Serializable]
    public class IncorrectUserException : Exception
    {
        public IncorrectUserException() { }
        public IncorrectUserException(string message) : base(message) { }
        public IncorrectUserException(string message, Exception inner) : base(message, inner) { }
        protected IncorrectUserException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
