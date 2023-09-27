using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.Models.Exceptions
{

	[Serializable]
	public class IncorrectTrainingOptionsException : Exception
	{
		public IncorrectTrainingOptionsException() { }
		public IncorrectTrainingOptionsException(string message) : base(message) { }
		public IncorrectTrainingOptionsException(string message, Exception inner) : base(message, inner) { }
		protected IncorrectTrainingOptionsException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
