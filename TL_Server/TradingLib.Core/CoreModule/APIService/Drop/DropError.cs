using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;

namespace TradingLib.Contrib.APIService
{
    /// <summary>
    /// 用于填充error模板的数据
    /// </summary>
    public class DropError:Drop
    {
        public DropError()
        {
            this.ErrorID = 1;
            this.ErrorMsg = "异常";
        }

        public DropError(int errorID,string errorMsg)
        {
            this.ErrorID = errorID;
            this.ErrorMsg = errorMsg;
        }
        /// <summary>
        /// 错误代码
        /// </summary>
        public int ErrorID { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMsg{get;set;}
    }
}
