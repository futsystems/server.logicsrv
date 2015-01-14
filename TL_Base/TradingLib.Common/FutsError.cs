﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Common
{
    public class FutsError : Exception
    {
        public FutsError()
        {

        }
    }



    /// <summary>
    /// 回报异常 当操作产生异常时候 通过将异常封装到FutsRspError来向外层抛出报错信息
    /// 
    /// </summary>
    public class FutsRspError : Exception
    {

        public int ErrorID { get; set; }
        public string ErrorMessage { get; set; }
        public Exception RawException { get; set; }


        public FutsRspError()
        {
            ErrorID = 1;
            ErrorMessage = string.Empty;
            RawException = new Exception("");
        }
        /// <summary>
        /// 从一个异常创建一个错误信息
        /// </summary>
        /// <param name="e"></param>
        public FutsRspError(Exception e)
        {
            ErrorID = 1;
            ErrorMessage = string.Empty;
            RawException = e;
        }

        /// <summary>
        /// 从一个错误信息创建一个FutsRspError
        /// </summary>
        /// <param name="error"></param>
        public FutsRspError(string error)
        {
            ErrorID = 1;
            ErrorMessage = error;
            RawException = new Exception(error);
        }

        /// <summary>
        /// 将JsonReply生成对应的FutsRspError
        /// </summary>
        /// <param name="reply"></param>
        public FutsRspError(TradingLib.Mixins.Json.JsonReply reply)
        {
            ErrorID = reply.Code;
            ErrorMessage = reply.Message;
        }
        

        /// <summary>
        /// 用自定义的XMLRspInfo填充错误信息
        /// </summary>
        /// <param name="error"></param>
        public void FillError(XMLRspInfo error)
        {
            ErrorID = error.Code;
            ErrorMessage = error.Message;
        }

        

    }
}
