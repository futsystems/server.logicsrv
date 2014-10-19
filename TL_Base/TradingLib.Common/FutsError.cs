using System;
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
        /// 常规错误
        /// 提供一个错误信息 生成对应的FutsRspError
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public static FutsRspError GenericError(string errorMessage)
        {
            return new FutsRspError() { ErrorID = 1, ErrorMessage = errorMessage };
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

        /// <summary>
        /// 通过key设定具体的错误信息
        /// </summary>
        /// <param name="error_key"></param>
        public void FillError(string error_key)
        {
            this.FillError(XMLRspInfoHelper.Tracker[error_key]);
        }

        /// <summary>
        /// 通过code设定具体的错误信息
        /// </summary>
        /// <param name="error_code"></param>
        public void FillError(int error_code)
        {
            this.FillError(XMLRspInfoHelper.Tracker[error_code]);
        }

    }
}
