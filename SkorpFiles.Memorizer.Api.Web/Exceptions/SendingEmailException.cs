namespace SkorpFiles.Memorizer.Api.Web.Exceptions
{

	[Serializable]
	public class SendingEmailException : Exception
	{
		public SendingEmailException() { }
		public SendingEmailException(string message) : base(message) { }
		public SendingEmailException(string message, Exception inner) : base(message, inner) { }
	}
}
