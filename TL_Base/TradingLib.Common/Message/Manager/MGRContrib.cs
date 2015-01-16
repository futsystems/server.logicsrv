﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{

    /// <summary>
    /// 管理端扩展请求
    /// 
    /// </summary>
    public class MGRContribRequest:RequestPacket
    {
        public MGRContribRequest()
        {
            _type = MessageTypes.MGRCONTRIBREQUEST;
            this.ModuleID = string.Empty;
            this.CMDStr = string.Empty;
            this.Parameters = string.Empty;
        }

        /// <summary>
        /// 模块ID
        /// </summary>
        public string ModuleID { get; set; }

        /// <summary>
        /// 命令名
        /// </summary>
        public string CMDStr { get; set; }

        /// <summary>
        /// 命令参数
        /// </summary>
        public string Parameters { get; set; }

        public override string ContentSerialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(ModuleID);
            sb.Append(d);
            sb.Append(this.CMDStr);
            sb.Append(d);
            sb.Append(this.Parameters);
            return sb.ToString();
        }

        public override void ContentDeserialize(string contentstr)
        {
            string[] rec = contentstr.Split(new char[]{','}, 3);
            this.ModuleID = rec[0];
            this.CMDStr = rec[1];
            this.Parameters = rec[2];
        }
    }

    /// <summary>
    /// 管理端扩展回报 一个请求 多个回复 用于通知其他管理客户端
    /// 比如管理端确认出入金请求 会同步通知登入的代理客户端
    /// </summary>
    public class NotifyMGRContribNotify : NotifyResponsePacket
    {
        public NotifyMGRContribNotify()
        {
            _type = MessageTypes.MGRCONTRIBRNOTIFY;
        }

        /// <summary>
        /// 模块ID
        /// </summary>
        public string ModuleID { get; set; }

        /// <summary>
        /// 命令名
        /// </summary>
        public string CMDStr { get; set; }

        /// <summary>
        /// 返回的Json字符串
        /// </summary>
        public string Result { get; set; }

        public override string ContentSerialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(this.ModuleID);
            sb.Append(d);
            sb.Append(this.CMDStr);
            sb.Append(d);
            sb.Append(this.Result);

            return sb.ToString();
        }

        public override void ContentDeserialize(string content)
        {
            string[] rec = content.Split(new char[] { ',' }, 3);
            this.ModuleID = rec[0];
            this.CMDStr = rec[1];
            this.Result = rec[2];
        }
    }
    /// <summary>
    /// 管理端扩展回报 一个请求 一个回复
    /// </summary>
    public class RspMGRContribResponse : RspResponsePacket
    {
        public RspMGRContribResponse()
        {
            _type = MessageTypes.MGRCONTRIBRESPONSE;
        }

        /// <summary>
        /// 模块ID
        /// </summary>
        public string ModuleID { get; set; }

        /// <summary>
        /// 命令名
        /// </summary>
        public string CMDStr { get; set; }

        /// <summary>
        /// 返回的Json字符串
        /// </summary>
        public string Result { get; set; }


        public override string ResponseSerialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(this.ModuleID);
            sb.Append(d);
            sb.Append(this.CMDStr);
            sb.Append(d);
            sb.Append(this.Result);

            return sb.ToString();
        }

        public override void ResponseDeserialize(string content)
        {
            string[] rec = content.Split(new char[] { ',' }, 3);
            this.ModuleID = rec[0];
            this.CMDStr = rec[1];
            this.Result = rec[2];
        }

    }
}