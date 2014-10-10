using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace TradingLib.Mixins.TNetStrings
{
    public enum TnetStringType
    {
        Int,
        Dict,
        List,
        Bool,
        Null,
        String,
        Float,
    }
    /// <summary>
    /// 放置了具体解析的内容
    /// </summary>
    public class TnetString
    {
        /// <summary>
        /// 从字符串转换成ArraySegment
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static ArraySegment<byte> GetArraySegment(string msg)
        {
            byte[] b = Encoding.UTF8.GetBytes(msg);
            return new ArraySegment<byte>(b);
        }
        /// <summary>
        /// 从ArraySegment转换成string
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string GetString(ArraySegment<byte> data)
        {
            return data.ToUTF8String();
        }
        public TnetString(int data)
        {
            this.IntValue = data;
            this.Type = TnetStringType.Int;
        }
        public TnetString(double data)
        {
            this.FloatValue = data;
            this.Type = TnetStringType.Float;
        }
        public TnetString(Dictionary<string, TnetString> data)
        {
            this.DictValue = data;
            this.Type = TnetStringType.Dict;
        }
        public TnetString(List<TnetString> data)
        {
            this.ListValue = data;
            this.Type = TnetStringType.List;
        }
        public TnetString(bool data)
        {
            this.BoolValue = data;
            this.Type = TnetStringType.Bool;
        }
        public TnetString(ArraySegment<byte> data)
        {
            this.StringValue = data;
            this.Type = TnetStringType.String;
        }
        /// <summary>
        /// 直接从字符串string 转换成 ArraySegment<bypte>
        /// </summary>
        /// <param name="msg"></param>
        public TnetString(string msg)
        {
            byte[] b = Encoding.UTF8.GetBytes(msg);
            this.StringValue = new ArraySegment<byte>(b);
            this.Type = TnetStringType.String;
        }

        public TnetString()
        {
            this.Type = TnetStringType.Null;
        }

        public TnetStringType Type;
        public int IntValue;
        public double FloatValue;
        public Dictionary<string, TnetString> DictValue;
        public List<TnetString> ListValue;
        public bool BoolValue = false;
        public ArraySegment<byte> StringValue;

        public override string ToString()
        {
            switch (Type)
            {
                case TnetStringType.Bool:
                    return BoolValue ? "true" : "false";
                case TnetStringType.Dict:
                    return "";
                case TnetStringType.Float:
                    return FloatValue.ToString();
                case TnetStringType.Int:
                    return IntValue.ToString();
                case TnetStringType.List:
                    return "";
                case TnetStringType.Null:
                    return "";
                case TnetStringType.String:
                    return StringValue.ToUTF8String();
                default:
                    return "Invalid TnetString type";

            }
        }
    }

    public static class Tnetstring
    {
        /// <summary>
        /// 负载解析，包含解析类型 内容 和剩余内容
        /// </summary>
        private struct TnetstringPayload
        {
            public TnetstringPayload(ArraySegment<byte> payload, byte payloadType, ArraySegment<byte> remain)
            {
                this.Payload = payload;
                this.PayloadType = payloadType;
                this.Remain = remain;
            }

            public ArraySegment<byte> Payload;
            public byte PayloadType;
            public ArraySegment<byte> Remain;
        }

        /// <summary>
        /// 解析结果 包含解析出的当前内容和剩余的byte
        /// </summary>
        public struct TParseResult
        {
            public TParseResult(TnetString data, ArraySegment<byte> remain)
            {
                this.Data = data;
                this.Remain = remain;
            }

            public TnetString Data;
            public ArraySegment<byte> Remain;
        }



        /// <summary>
        /// 将一个TnetString加密成一个TNetstring
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string TDump(TnetString data)
        {
            string fordump = "";
            switch (data.Type)
            {
                //#
                case TnetStringType.Int:
                    fordump = data.IntValue.ToString();
                    return string.Format("{0}:{1}#", fordump.Length, fordump);
                //^
                case TnetStringType.Float:
                    fordump = data.FloatValue.ToString();
                    return string.Format("{0}:{1}^", fordump.Length, fordump);
                //}
                case TnetStringType.Dict:
                    return "";
                //]
                case TnetStringType.List:
                    return "";
                //!
                case TnetStringType.Bool:
                    fordump = data.BoolValue ? "true" : "false";
                    return string.Format("{0}:{1}!", fordump.Length, fordump);
                //~
                case TnetStringType.Null:
                    fordump = "";
                    return string.Format("{0}:{1}~", fordump.Length, fordump);
                //,
                case TnetStringType.String://注意一个中文字符 在utf8计算字符串长度时只显示1位，但是ascii arraysegement中占三位
                    fordump = data.StringValue.ToUTF8String();
                    return string.Format("{0}:{1},", data.StringValue.Array.Length, fordump);
                default:
                    throw new Exception("Invalid payload type");
            }

        }

        /// <summary>
        /// 静态方法 将一个byte分解后 返回TParseResult
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static TParseResult TParse(this ArraySegment<byte> data)
        {
            var parsed = TParsePayload(data);
            var payload = parsed.Payload;
            var payloadType = (char)parsed.PayloadType;
            var remain = parsed.Remain;

            switch (payloadType)
            {
                case '#':
                    return new TParseResult(new TnetString(int.Parse(payload.ToUTF8String())), remain);
                case '^':
                    return new TParseResult(new TnetString(double.Parse(payload.ToUTF8String())), remain);
                case '}':
                    return new TParseResult(ParseDict(payload), remain);
                case ']':
                    return new TParseResult(ParseList(payload), remain);
                case '!':
                    return new TParseResult(new TnetString(payload.ToUTF8String() == "true"), remain);
                case '~':
                    if (payload.Count != 0)
                        throw new Exception("Payload must be 0 length for null.");
                    return new TParseResult(new TnetString(), remain);
                case ',':
                    return new TParseResult(new TnetString(payload), remain);
                default:
                    throw new Exception("Invalid payload type: " + payloadType);
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// 将TnetString playload按照协议规则进行分解  num:content,#
        /// 长度:内容然后加入类型标识
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        static TnetstringPayload TParsePayload(this ArraySegment<byte> data)
        {
            //assert data, "Invalid data to parse, it's empty."
            if (data == null)
                throw new ArgumentNullException("data");

            //length, extra = data.split(':', 1)
            //length = int(length)
            var dataSplit = data.Split(':', 2);
            var length = int.Parse(dataSplit[0].ToUTF8String());
            var extra = dataSplit[1];

            //payload, extra = extra[:length], extra[length:]
            var payload = extra.Substring(0, length);
            extra = extra.Substring(length);
            //Utils.Debug("length:" + length.ToString() + "extra:" + Encoding.UTF8.GetString(extra.ToArray()) + " playload;" + payload.ToString() + " after exta:" + Encoding.UTF8.GetString(extra.ToArray()));
            //assert extra, "No payload type: %r, %r" % (payload, extra)
            if (extra.Count == 0)
                throw new Exception("No payload type");

            //payload_type, remain = extra[0], extra[1:]
            var payloadType = extra.Get(0);
            var remain = extra.Substring(1);

            //assert len(payload) == length, "Data is wrong length %d vs %d" % (length, len(payload))
            if (payload.Count != length)
                throw new Exception("Data is wrong length");

            //return payload, payload_type, remain
            return new TnetstringPayload(payload, payloadType, remain);
        }









        /// <summary>
        /// 解析列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        static TnetString ParseList(ArraySegment<byte> data)
        {
            var ret = new List<TnetString>();

            while (data.Count != 0)
            {
                var parsed = data.TParse();
                ret.Add(parsed.Data);
                data = parsed.Remain;
            }

            return new TnetString(ret);
        }

        private struct TPair
        {
            public string Key;
            public TnetString Value;
            public ArraySegment<byte> Extra;
        }

        /// <summary>
        /// 解析map
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        static TPair ParsePair(ArraySegment<byte> data)
        {
            var parsed = data.TParse();

            if (parsed.Data.Type != TnetStringType.String)
                throw new Exception("Dictionary key must be a string.");

            var key = parsed.Data.StringValue.ToUTF8String();
            var extra = parsed.Remain;

            if (extra.Count == 0)
                throw new Exception("Unbalanced dictionary store.");

            parsed = extra.TParse();
            extra = parsed.Remain;

            return new TPair() { Key = key, Value = parsed.Data, Extra = extra };
        }

        /// <summary>
        /// 解析map
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        static TnetString ParseDict(ArraySegment<byte> data)
        {
            var ret = new Dictionary<string, TnetString>();

            while (data.Count != 0)
            {
                var pair = ParsePair(data);
                ret[pair.Key] = pair.Value;
                data = pair.Extra;
            }

            return new TnetString(ret);
        }
    }
}
