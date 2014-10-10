using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    /// <summary>
    /// 定义了某个websock connection 所请求的数据集合
    /// </summary>
    internal class WebAdminInfoEx
    {
        string _uuid="";
        /// <summary>
        /// websock connection的标识
        /// </summary>
        public string UUID { get { return _uuid; } }

        bool sent=false;
        /// <summary>
        /// 是否已经发送
        /// web端查询数据时并不返回对应的session信息
        /// 在循环发送liteinfo时 系统判断 如果是设定后第一次发送 则需要采集session信息进行发送
        /// </summary>
        public bool IsSent { get { return sent; } set { sent = value; } }
 
        /// <summary>
        /// 定义了帐号观察列表,用于计算这些帐户的相关信息 向对应的connection进行推送
        /// </summary>
        public ThreadSafeList<IAccount> WatchAccounts = new ThreadSafeList<IAccount>();

        IAccount _account = null;
        /// <summary>
        /// 选定帐户 用于管理端显示选中的帐号交易信息
        /// </summary>
        public IAccount SelectedAccount { get { return _account; } }


        public WebAdminInfoEx(string uuid)
        {
            _uuid = uuid;
        }

        public void GotWathAccount(List<IAccount> acclist)
        {
            WatchAccounts.Clear();
            foreach (IAccount a in acclist)
            {
                WatchAccounts.Add(a);
            }
            sent = false;
        }

    }
}
