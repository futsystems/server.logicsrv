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
        /// <summary>
        /// 服务端版本
        /// </summary>
        public string ServerVesion { get; set; }

        /// <summary>
        /// 返回的客户端UUID
        /// </summary>
        public string ClientUUID { get; set; }

        public override string ResponseSerialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(this.ServerVesion);
            sb.Append(d);
            sb.Append(this.ClientUUID);

            return sb.ToString();
        }


        public override void ResponseDeserialize(string repstr)
        {
            string[] rec = repstr.Split(',');
            this.ServerVesion = rec[0];
            this.ClientUUID = rec[1];
        }
        
    }
}
