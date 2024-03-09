namespace SkorpFiles.Memorizer.Api.Web.Exceptions
{

    [Serializable]
    public class IncorrectUserException : Exception
    {
        public IncorrectUserException() { }
        public IncorrectUserException(string message) : base(message) { }
        public IncorrectUserException(string message, Exception inner) : base(message, inner) { }
    }
}
