namespace SkorpFiles.Memorizer.Api.Web.Exceptions
{

	[Serializable]
	public class BadRequestException : ArgumentException
	{
		public BadRequestException() { }
		public BadRequestException(string message) : base(message) { }
		public BadRequestException(string message, ArgumentException inner) : base(message, inner) { }
		protected BadRequestException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
