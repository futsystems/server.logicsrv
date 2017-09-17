using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Contrib.Payment.HuiCX
{
    public class HuiCXHelpler
    {
        /// <summary>
        /// 获取支付请求数据
        /// </summary>
        /// <param name="sourceData"></param>
        /// <returns></returns>
        public string generatePayRequest(Dictionary<string, string> sourceData)
        {
            if (!sourceData.ContainsKey("apiName") || string.IsNullOrEmpty(sourceData["apiName"]))
            {
                throw new Exception("apiName不能为空");
            }
            if (!sourceData.ContainsKey("apiVersion") || string.IsNullOrEmpty(sourceData["apiVersion"]))
            {
                throw new Exception("apiVersion不能为空");
            }
            if (!sourceData.ContainsKey("platformID") || string.IsNullOrEmpty(sourceData["platformID"]))
            {
                throw new Exception("platformID不能为空");
            }
            if (!sourceData.ContainsKey("merchNo") || string.IsNullOrEmpty(sourceData["merchNo"]))
            {
                throw new Exception("merchNo不能为空");
            }
            if (!sourceData.ContainsKey("orderNo") || string.IsNullOrEmpty(sourceData["orderNo"]))
            {
                throw new Exception("orderNo不能为空");
            }
            if (!sourceData.ContainsKey("tradeDate") || string.IsNullOrEmpty(sourceData["tradeDate"]))
            {
                throw new Exception("tradeDate不能为空");
            }
            if (!sourceData.ContainsKey("amt") || string.IsNullOrEmpty(sourceData["amt"]))
            {
                throw new Exception("amt不能为空");
            }
            if (!sourceData.ContainsKey("merchUrl") || string.IsNullOrEmpty(sourceData["merchUrl"]))
            {
                throw new Exception("merchUrl不能为空");
            }
            if (!sourceData.ContainsKey("merchParam"))
            {
                throw new Exception("merchParam可以为空，但必须存在！");
            }
            if (!sourceData.ContainsKey("tradeSummary") || string.IsNullOrEmpty(sourceData["tradeSummary"]))
            {
                throw new Exception("tradeSummary不能为空");
            }

            string apiName = sourceData["apiName"];
            string apiVersion = sourceData["apiVersion"];
            string platformID = sourceData["platformID"];
            string merchNo = sourceData["merchNo"];
            string orderNo = sourceData["orderNo"];
            string tradeDate = sourceData["tradeDate"];
            string amt = sourceData["amt"];
            string merchUrl = sourceData["merchUrl"];
            string merchParam = sourceData["merchParam"]; // System.Web.HttpUtility.UrlEncode(Encoding.UTF8.GetBytes(sourceData["merchParam"]));
            string tradeSummary = sourceData["tradeSummary"];
            if (!apiVersion.Equals("1.0.0.0"))
            {
                throw new Exception("apiVersion错误！");
            }

            string result = string.Format("apiName={0}&apiVersion={1}&platformID={2}&merchNo={3}&orderNo={4}&tradeDate={5}&amt={6}&merchUrl={7}&merchParam={8}&tradeSummary={9}",
                apiName, apiVersion, platformID, merchNo, orderNo, tradeDate, amt, merchUrl, merchParam, tradeSummary);
            return result;
        }
    }
}
