using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;

namespace NHttp
{
    internal class HttpUrlEncodedRequestParser : HttpRequestParser
    {
        private readonly MemoryStream _stream;

        public HttpUrlEncodedRequestParser(HttpClient client, int contentLength)
            : base(client, contentLength)
        {
            _stream = new MemoryStream();
        }

        public override void Parse()
        {
            Client.ReadBuffer.CopyToStream(_stream, ContentLength);

            if (_stream.Length == ContentLength)
            {
                ParseContent();

                EndParsing();
            }
        }

        private void ParseContent()
        {
            _stream.Position = 0;

            string content;

            using (var reader = new StreamReader(_stream, Encoding.ASCII))
            {
                content = reader.ReadToEnd();
            }
            Client.RawContent = content;//将原始Content设定到Client 当出发RequestContent时附带到Requet对象 这样可以直接以流方式读取Post
            HttpUtil.UrlDecodeTo(content, Client.PostParameters);
        }
    }
}
