using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Contrib.ResponseHost
{
    public class ResponseTracker:IEnumerable,IEnumerable<ResponseWrapper>
    {
        Dictionary<string, ResponseWrapper> responseMap = new Dictionary<string, ResponseWrapper>();
        
        public ResponseTracker()
        {
            foreach (ResponseWrapper resp in ORM.MResponse.SelectResponse())
            {
                Util.Info("load response instance:" + resp.Acct + " templateid:" + resp.Response_Template_ID.ToString());
                LoadResponse(resp);
            } 
        }

        public void LoadResponse()
        {

        }

        /// <summary>
        /// 加载策略实例
        /// </summary>
        /// <param name="resp"></param>
        void LoadResponse(ResponseWrapper resp)
        {
            //初始化策略实例
            resp.InitResponse();

            if (responseMap.Keys.Contains(resp.Acct))
            {
                Util.Debug(string.Format("Account:{0} already load response,drop it", resp.Acct));
                return;
            }

            responseMap.Add(resp.Acct, resp);

            if (resp.Response is ResponseBase)
            {
                //执行策略初始化函数
                (resp.Response as ResponseBase).OnInit();
            }
        }


        /// <summary>
        /// 通过交易帐号 获得对应的策略实例
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public ResponseWrapper this[string account]
        {
            get
            {
                ResponseWrapper resp = null;
                if (responseMap.TryGetValue(account, out resp))
                {
                    return resp;
                }
                return null;
            }
        }

        /// <summary>
        /// 判断某个帐户是否有绑定策略实例
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool HaveResponse(string account)
        {
            if (responseMap.Keys.Contains(account))
                return true;
            return false;
        }

        /// <summary>
        /// 为某个交易帐号添加某个策略实例
        /// </summary>
        /// <param name="account"></param>
        /// <param name="sp_fk"></param>
        public void AddResponse(string account, int template_id)
        {
            ResponseWrapper resp = new ResponseWrapper();
            //配资服务帐号
            resp.Acct = account;
            //是否激活
            resp.Active = true;
            //服务计划fk
            resp.Response_Template_ID = template_id;

            //插入到数据库
            ORM.MResponse.InsertResponse(resp);

            //加载Response
            LoadResponse(resp);
        }

        /// <summary>
        /// 删除某个配资服务
        /// </summary>
        /// <param name="service_id"></param>
        public void DeleteResponse(ResponseWrapper resp)
        {
            //将服务从交易帐户上注销
            resp.Account.UnBindService(resp);

            //内存清空相关记录
            if (responseMap.Keys.Contains(resp.Acct))
            {
                responseMap.Remove(resp.Acct);
            }

            //数据库删除记录
            ORM.MResponse.DeleteResponse(resp);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<ResponseWrapper> IEnumerable<ResponseWrapper>.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<ResponseWrapper> GetEnumerator()
        {
            foreach (ResponseWrapper resp in responseMap.Values.ToArray())
                yield return resp;
        }

    }
}
