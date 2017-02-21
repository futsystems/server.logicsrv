using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Contrib.APIService
{
    public class GateWayBase
    {

        public QSEnumGateWayType GateWayType { get; set; }
        /// <summary>
        /// 通过远端服务器调用参数获得本地CashOperation
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        //public virtual CashOperation GetCashOperation(System.Collections.Specialized.NameValueCollection queryString)
        //{
        //    return null;
        //}

        /// <summary>
        /// 检查远端回调访问是否合法
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public virtual bool CheckParameters(System.Collections.Specialized.NameValueCollection queryString)
        {
            return false;
        }

        public virtual bool CheckPayResult(System.Collections.Specialized.NameValueCollection queryString, CashOperation operation)
        {
            return false;
        }

        public virtual string GetResultComment(System.Collections.Specialized.NameValueCollection queryString)
        {
            return string.Empty;
        }
    }
}
