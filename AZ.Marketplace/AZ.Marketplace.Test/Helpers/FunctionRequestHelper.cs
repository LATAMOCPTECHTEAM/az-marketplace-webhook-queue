using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AZ.Marketplace.Test.Functions
{
	public static class FunctionRequestHelper
	{

		public static DefaultHttpRequest CreateHttpRequest(Dictionary<string, StringValues> headers = null, Dictionary<string, StringValues> query = null, IDictionary<string, string> body = null)
		{
			var request = new DefaultHttpRequest(new DefaultHttpContext());

			if (query != null)
				request.Query = new QueryCollection(query);

			if (body != null)
				request.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body)));

			if (headers != null)
				foreach (var header in headers)
					request.Headers.Add(header.Key, header.Value);

			return request;
		}

	}
}