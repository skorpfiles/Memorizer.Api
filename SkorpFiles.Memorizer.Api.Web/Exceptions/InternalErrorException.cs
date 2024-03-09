namespace SkorpFiles.Memorizer.Api.Web.Exceptions
{

    [Serializable]
    public class InternalErrorException : Exception
    {
        public InternalErrorException() { }
        public InternalErrorException(string message) : base(message) { }
        public InternalErrorException(string message, Exception inner) : base(message, inner) { }
    }
}
