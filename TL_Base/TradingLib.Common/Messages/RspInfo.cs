///////////////////////////////////////////////////////////////////////////////////////
// 服务端消息返回
// 用于返回服务端的相关消息 比如连接失败，提交委托不合法，资金不足等相关通知
// 这里可以考虑建立一个XML错误代码表文件，将错误代码放到对应的文件中，有程序运行时统一加载
// 这样容易实现两端错误代码的统一
//
///////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{

    /// <summary>
    /// 委托信息
    /// </summary>
    /// <param name="o"></param>
    /// <param name="info"></param>
    public delegate void ErrorOrderRspInoDel(Order o,string error);


    public static class RspInfoUtils
    { 
        public static void Fill(this RspInfo info,XMLRspInfo xml)
        {
            info.ErrorID = xml.Code;
            info.ErrorMessage = xml.Message;
        }

        public static void Fill(this RspInfo info, FutsRspError error)
        {
            info.ErrorID = error.ErrorID;
            info.ErrorMessage = error.ErrorMessage;
        }


        /// <summary>
        /// 通过key设定具体的错误信息
        /// </summary>
        /// <param name="error_key"></param>
        public static void Fill(this RspInfo info, string error_key)
        {
            info.Fill(XMLRspInfoHelper.Tracker[error_key]);
        }

        /// <summary>
        /// 通过code设定具体的错误信息
        /// </summary>
        /// <param name="error_code"></param>
        public static void Fill(this RspInfo info, int error_code)
        {
            info.Fill(XMLRspInfoHelper.Tracker[error_code]);
        }

    }
    /// <summary>
    /// 回报消息
    /// 用于向客户端回报错误提示
    /// 正常查询内也会附带对应的回报消息,逻辑数据包会自行进行解析并形成对应的逻辑包
    /// </summary>
    public class RspInfoImpl:RspInfo
    {
        ///// <summary>
        ///// 设定具体的错误信息
        ///// </summary>
        ///// <param name="error"></param>
        //public void FillError(XMLRspInfo error)
        //{
        //    ErrorID = error.Code;
        //    ErrorMessage = error.Message;
        //}

        ///// <summary>
        ///// 通过异常来填充RspInfo
        ///// </summary>
        ///// <param name="error"></param>
        //public void Fill(FutsRspError error)
        //{
        //    ErrorID = error.ErrorID;
        //    ErrorMessage = error.ErrorMessage;
        //}

        

        ///// <summary>
        ///// 通过key设定具体的错误信息
        ///// </summary>
        ///// <param name="error_key"></param>
        //public void FillError(string error_key)
        //{
        //    this.FillError(XMLRspInfoHelper.Tracker[error_key]);
        //}

        ///// <summary>
        ///// 通过code设定具体的错误信息
        ///// </summary>
        ///// <param name="error_code"></param>
        //public void FillError(int error_code)
        //{
        //    this.FillError(XMLRspInfoHelper.Tracker[error_code]);
        //}



        public RspInfoImpl()
        {
            ErrorID = 0;
            ErrorMessage = string.Empty;
        }

        public int ErrorID { get; set; }

        public string ErrorMessage { get; set; }

        public static RspInfo Fill(string errorkey)
        {
            RspInfo info = new RspInfoImpl();
            info.Fill(errorkey);
            return info;
        }

        public static RspInfo Fill(int code)
        {
            RspInfo info = new RspInfoImpl();
            info.Fill(code);
            return info;
        }

        public static string Serialize(RspInfo info)
        {
            return info.ErrorID.ToString() + "," + info.ErrorMessage.Replace(',', ' ');
        }

        public static RspInfo Deserialize(string str)
        {
            RspInfo info = new RspInfoImpl();
            string[] rec = str.Split(',');
            int errorid = 0;
            if (rec.Length < 2)
            {
                int.TryParse(rec[0], out errorid);
                info.ErrorID = errorid;
                return info;
            }
            int.TryParse(rec[0], out errorid);
            info.ErrorID = errorid;
            info.ErrorMessage = rec[1];
            return info;
        }
       
    }
}
