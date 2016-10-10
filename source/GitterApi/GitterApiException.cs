using System;
using System.Net;
using System.Net.Http;

namespace GitterApi
{
	public class GitterApiException : Exception
	{
		public GitterApiException(string message)
			: base(message)
		{
		}

		public GitterApiException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public GitterApiException(HttpResponseMessage message)
			: this(message.ReasonPhrase)
		{
			ResponseMessage = message;
			StatusCode = message.StatusCode;
		}

		public HttpStatusCode StatusCode { get; private set; }

		public HttpResponseMessage ResponseMessage { get; private set; }
	}
}
