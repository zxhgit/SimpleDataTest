using System;
using System.Collections.Generic;
using Nancy;
using System.IO;
using Newtonsoft.Json;
using System.Text;

namespace Zx.Web.ApiContainer.external
{
    public class JsonDotNetJsonSerializer : ISerializer
    {
        public bool CanSerialize(string contentType)
        {
            return IsJsonType(contentType);
        }

        public IEnumerable<string> Extensions
        {
            get { yield return "json"; }
        }

        public void Serialize<TModel>(string contentType, TModel model, Stream outputStream)
        {
            var strRes = JsonConvert.SerializeObject(model);
            var mStream = new MemoryStream(Encoding.UTF8.GetBytes(strRes));
            mStream.CopyTo(outputStream);
        }

        private static bool IsJsonType(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
            {
                return false;
            }

            var contentMimeType = contentType.Split(';')[0];

            return contentMimeType.Equals("application/json", StringComparison.InvariantCultureIgnoreCase) ||
                   contentMimeType.StartsWith("application/json-", StringComparison.InvariantCultureIgnoreCase) ||
                   contentMimeType.Equals("text/json", StringComparison.InvariantCultureIgnoreCase) ||
                  (contentMimeType.StartsWith("application/vnd", StringComparison.InvariantCultureIgnoreCase) &&
                   contentMimeType.EndsWith("+json", StringComparison.InvariantCultureIgnoreCase));
        }
    }
}