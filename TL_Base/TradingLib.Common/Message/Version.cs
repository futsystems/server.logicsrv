///////////////////////////////////////////////////////////////////////////////////////
// 版本交换与密钥交换
// 
//
///////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Common
{
//////////////////////////////////////////////////////////////
//用于客户端与服务端交互版本信息,客户端向服务端告知版本信息
//服务端向客户端通知服务端版本信息
//////////////////////////////////////////////////////////////
    public class VersionRequest : RequestPacket
    {
        public VersionRequest()
        {
            _type = API.MessageTypes.VERSIONREQUEST;
        }

        public string ClientVersion {get;set;}

        public string DeviceType {get;set;}

        public override string ContentSerialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(this.ClientVersion);
            sb.Append(d);
            sb.Append(this.DeviceType);
            return sb.ToString();
        }

        public override void ContentDeserialize(string reqstr)
        {
            string [] rec = reqstr.Split(',');
            this.ClientVersion = rec[0];
            this.DeviceType = rec[1];
        }
    }


    public class VersionResponse : RspResponsePacket
    {
        public VersionResponse()
        {
            _type = API.MessageTypes.VERSIONRESPONSE;
        }

        public TLVersion Version { get; set; }

        public override string ResponseSerialize()
        {
            if (this.Version == null)
                return string.Empty;
            return TLVersion.Serialize(this.Version);
        }


        public override void ResponseDeserialize(string repstr)
        {
            if (string.IsNullOrEmpty(repstr))
                this.Version = null;
            this.Version = TLVersion.Deserialize(repstr);
        }
        
    }
}
