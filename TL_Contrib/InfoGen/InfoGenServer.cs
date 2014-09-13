using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.TNetStrings;

namespace Lottoqq.InfoGen
{
    /// <summary>
    /// 用于采集优秀的交易记录,将记录推送到所有交易客户端
    /// </summary>
    [ContribAttr("TradingInfoGen", "交易信息生产模块", "用于采集系统交易信息,然后将需要的交易记录广播到客户端")]
    public class TradingInfoGen : ContribSrvObject, IContrib
    {

        public TradingInfoGen()
            : base("TradingInfoGen")
        {

        }

        /// <summary>
        /// 加载
        /// </summary>
        public void OnLoad()
        {


        }
        /// <summary>
        /// 销毁
        /// </summary>
        public void OnDestory()
        {
            base.Dispose();
        }
        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {

        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {

        }

        //[TaskAttr("2秒推送一个演示消息", 2, "秒推送一个演示消息")]
        public void demomessage()
        {
            debug("广播演示消息", QSEnumDebugLevel.INFO);
            _Broadcast("demo2", "{'xyz':11.2,'demo':'你好111'}");
        }
        void _Broadcast(string notifyname,string jsonstr)
        {
            string args = Tnetstring.TDump(new TnetString("ALL")) + Tnetstring.TDump(new TnetString(notifyname)) + Tnetstring.TDump(new TnetString(jsonstr));
            //通过MessageWebHandler通过字符串调用指定的函数进行相关操作,避免了强依赖
            TLCtxHelper.Ctx.MessageWebHandler("UCenterAccess", "broadcast",args,true);
        }


        
    }
}
