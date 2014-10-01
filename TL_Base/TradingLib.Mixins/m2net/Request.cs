using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.Mixins.TNetStrings;
using TradingLib.Mixins.LitJson;

namespace TradingLib.Mixins.m2net
{
    public class Request
    {
        public string Sender { get; private set; }
        public string ConnId { get; private set; }
        public string Path { get; private set; }
        public Dictionary<string, string> Headers { get; private set; }
        public byte[] Body { get; private set; }
        public JsonData Data { get; private set; }

        public override string ToString()
        {
            return "Sender:"+Sender +" ConnId:"+ConnId +  " Path:"+Path +" Headers Method:" + Headers["METHOD"] + " Body:"+Encoding.UTF8.GetString(Body);
        }
        public bool IsDisconnect
        {
            get
            {
                if (this.Headers["METHOD"] != "JSON")
                    return false;
                
                foreach (string key in Data.Keys)
                {
                    if (key == "type" && Data[key].ToString() == "disconnect")
                        return true;
                }
                return false;
            }
        }

        bool isjson = false;
        public bool IsJson
        {
            get
            {
                return isjson;
            }
        }
        internal Request(string sender, string conn_id, string path, Dictionary<string, string> headers, byte[] body)
        {
            this.Sender = sender;
            this.ConnId = conn_id;
            this.Path = path;
            this.Headers = headers;
            this.Body = body;

            if (this.Headers["METHOD"] == "JSON")
            {
                isjson = true;
                this.Data = JsonMapper.ToObject(body.ToUTF8String());//body may content chinese string
            }
            else
                this.Data = new JsonData();//new JsonBuffer();
        }

         

        public static Request Parse(byte[] msg)
        {
            //http请求解析
            //34f9ceee-cd52-4b7f-b197-88bf2f0ec378 12 /demo 507:{"PATH":"/demo","x-forwarded-for":"192.168.2.180","accept-language":"zh-CN,zh;q=0.8,en;q=0.6","accept-encoding":"gzip,deflate,sdch","connection":"keep-alive","accept":"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8","user-agent":"Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/30.0.1599.101 Safari/537.36","host":"192.168.2.72:6767","METHOD":"GET","VERSION":"HTTP/1.1","URI":"/demo","PATTERN":"/demo","URL_SCHEME":"http","REMOTE_ADDR":"192.168.2.180"},0:,.168.2.180"},0:,
            var threeChunks = msg.Split(' ', 4);
            var sender = threeChunks[0].ToAsciiString();//sender
            var conn_id = threeChunks[1].ToAsciiString();//conn_id
            var path = threeChunks[2].ToAsciiString();//path
            var rest = threeChunks[3];//http request

            var headersAndRest = rest.TParse();
            var headers = headersAndRest.Data;
            rest = headersAndRest.Remain;

            var body = rest.ParseNetstring()[0];

            Dictionary<string, string> headersDic;
            if (headers.Type == TnetStringType.String)
                headersDic = ParseJsonHeaders(headers.StringValue);
            else if (headers.Type == TnetStringType.Dict)
                headersDic = ParseTnsHeaders(headers.DictValue);
            else
                throw new Exception("Unknown header payload.");

            return new Request(sender, conn_id, path, headersDic, body.ToArray());
        }

        private static Dictionary<string, string> ParseJsonHeaders(ArraySegment<byte> data)
        {
            var headers = new Dictionary<string, string>();
            JsonData jsonbuffer = JsonMapper.ToObject(data.ToUTF8String());
            foreach (string key in jsonbuffer.Keys)
            {
                JsonData jd = jsonbuffer[key];
                if (jd.IsArray)
                {
                    foreach (string k2 in jd.Keys)
                    {
                        headers.Add(k2, jd[k2].ToString());
                    }
                }
                else
                {
                    headers.Add(key, jsonbuffer[key].ToString());
                }
                
            }
            /*
            foreach (var h in JsonBuffer.From(data.ToAsciiString()).GetMembers())
            {
                var key = h.Name;
                if (h.Buffer.IsScalar)
                    headers.Add(key, h.Buffer.GetString());
                else if (h.Buffer.IsArray)
                {
                    for (int i = 1; i < h.Buffer.GetArrayLength(); i++)
                    {
                        var v = h.Buffer[i];
                        if (v.Class != JsonTokenClass.String)
                            throw new Exception("non-string value header");
                        if (headers.ContainsKey(key))
                            break; //TODO: support many header values
                        headers.Add(key, v.Text);
                    }
                }
                else
                {
                    throw new Exception("Unexpected JSON type.");
                }
            }**/
            return headers;
        }

        private static Dictionary<string, string> ParseTnsHeaders(Dictionary<string, TnetString> objDic)
        {
            var ret = new Dictionary<string, string>();

            foreach (var kvp in objDic)
            {
                var key = kvp.Key;
                if (kvp.Value.Type == TnetStringType.String)
                {
                    ret.Add(key, kvp.Value.StringValue.ToAsciiString());
                }
                else if (kvp.Value.Type == TnetStringType.List)
                {
                    foreach (TnetString value in kvp.Value.ListValue)
                    {
                        if (ret.ContainsKey(key))
                            break; //TODO: many headers
                        if (value.Type != TnetStringType.String)
                            throw new Exception("Expected string values.");
                        ret.Add(key, value.StringValue.ToAsciiString());
                    }
                }
                else
                {
                    throw new Exception("Unexpected header type.");
                }
            }

            return ret;
        }
    }
}
