namespace SkorpFiles.Memorizer.Api.Web.Exceptions
{
    [Serializable]
    public class InternalAuthenticationErrorException : InternalErrorException
    {
        public InternalAuthenticationErrorException() { }
        public InternalAuthenticationErrorException(string message) : base(message) { }
        public InternalAuthenticationErrorException(string message, Exception inner) : base(message, inner) { }
        protected InternalAuthenticationErrorException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
