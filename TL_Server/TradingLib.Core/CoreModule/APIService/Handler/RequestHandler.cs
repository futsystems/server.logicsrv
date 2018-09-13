using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHttp;

namespace TradingLib.Contrib.APIService
{
    public class RequestHandler
    {
        /// <summary>
        /// 模块名称
        /// </summary>
        public string Module { get; protected set; }

        /// <summary>
        /// 处理HttpRequest
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual object Process(HttpRequest request)
        {
            return string.Empty;
        }
    }
}
