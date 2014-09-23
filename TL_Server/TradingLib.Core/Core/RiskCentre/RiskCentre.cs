using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.Common;

using System.Diagnostics;//记得加入此引用
using System.Collections.Concurrent;
using TradingLib.API;
using System.Reflection;
using System.Threading;

namespace TradingLib.Core
{
    

    //服务端风险控制模块,根据每个账户的设定，实时的检查Order是否符合审查要求予以确认或者决绝
    public partial class RiskCentre : BaseSrvObject, IRiskCentre,ICore
    {

        const string CoreName = "RiskCentre";

        public event ClientTrackerInfoSessionDel ClientSessionEvent;
        //public event GetFinServiceDel GetFinServiceDelEvent;

        

        

        //账户检查日志
        Log _accountcheklog = new Log("Risk_Account", true, true, LibGlobal.LOGPATH, true);//日志组件
        //委托检查日志
        Log _ordercheklog = new Log("Risk_Order", true, true, LibGlobal.LOGPATH, true);//日志组件
        //其他日志
        Log _othercheklog = new Log("Risk_Other", true, true, LibGlobal.LOGPATH, true);//日志组件

        

       
        /// <summary>
        /// 清算中心
        /// </summary>
        ClearCentre _clearcentre = null;

        /// <summary>
        /// 服务端止盈止损
        /// </summary>
        PositionOffsetTracker _posoffsetracker = null;

        /// <summary>
        /// 主力合约列表
        /// </summary>
        //Basket hotbasket = new BasketImpl();

        public string CoreId { get { return CoreName; } }

        ConfigDB _cfgdb;

        bool _marketopencheck = true;
        public bool MarketOpenTimeCheck { get { return _marketopencheck; } }

        int _orderlimitsize = 10;
        string commentNoPositionForFlat = "无可平持仓";
        string commentOverFlatPositionSize = "可平持仓数量不足";
        public RiskCentre(ClearCentre clearcentre):base(CoreName)
        {
            //1.加载配置文件
            _cfgdb = new ConfigDB(RiskCentre.CoreName);
            if (!_cfgdb.HaveConfig("MarketOpenTimeCheck"))
            {
                _cfgdb.UpdateConfig("MarketOpenTimeCheck", QSEnumCfgType.Bool,"True", "是否进行时间检查,交易日/合约交易时间段等");
            }

            

            if (!_cfgdb.HaveConfig("OrderLimitSize"))
            {
                _cfgdb.UpdateConfig("OrderLimitSize", QSEnumCfgType.Int, 50, "单笔委托最大上限");
            }

            if (!_cfgdb.HaveConfig("CommentNoPositionForFlat"))
            {
                _cfgdb.UpdateConfig("CommentNoPositionForFlat", QSEnumCfgType.String,"无可平持仓", "无可平持仓消息");
            }
            commentNoPositionForFlat = _cfgdb["CommentNoPositionForFlat"].AsString();

            if (!_cfgdb.HaveConfig("CommentOverFlatPositionSize"))
            {
                _cfgdb.UpdateConfig("CommentOverFlatPositionSize", QSEnumCfgType.String, "可平持仓数量不足", "可平持仓数量不足消息");
            }
            commentOverFlatPositionSize = _cfgdb["CommentOverFlatPositionSize"].AsString();

            //是否执行合约开市检查
            _marketopencheck = _cfgdb["MarketOpenTimeCheck"].AsBool();
            //最大委托数量
            _orderlimitsize = _cfgdb["OrderLimitSize"].AsInt();


            _clearcentre = clearcentre;

            _posoffsetracker = new PositionOffsetTracker(_clearcentre as ClearCentre);
            _posoffsetracker.SendDebugEvent +=new DebugDelegate(msgdebug);
            _posoffsetracker.SendOrderEvent +=new OrderDelegate(SendOrder);
            _posoffsetracker.CancelOrderEvent += new LongDelegate(CancelOrder);
            _posoffsetracker.AssignOrderIDEvent += new AssignOrderIDDel(AssignOrderID);

            //加载主力合约
            InitHotBasket();

            //加载风空规则
            LoadRuleSet();

            //初始化日内平仓任务
            InitFlatTask();
        }

        //public void CacheAccount(IAccount account)
        //{
        //    account.RiskCentre = new RiskCentreAdapterToAccount(account, this);
        //}

        /// <summary>
        /// 查询当前是否是交易日
        /// </summary>
        public bool IsTradingday
        {
            get
            {
                return TLCtxHelper.Ctx.SettleCentre.IsTradingday;
            }
        }


        #region 获得帐户的配资服务

        /// <summary>
        /// 获得账户的配资服务
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        //IFinService GetFinService(string account)
        //{
            //if (GetFinServiceDelEvent != null)
            //    return GetFinServiceDelEvent(account);
            //return null;
        //}
        #endregion

        #region 辅助功能函数
        /// <summary>
        /// 加载主力合约列表
        /// </summary>
        public void InitHotBasket()
        {
            //hotbasket.Clear();
            /*
            //获得主力合约组,并从配置文件夹在主力合约
            string[] l = CoreGlobal.HotSymbolBaskets.Split(',');
            foreach (string h in l)
            {
                Basket b = BasketTracker.getBasket(h);
                if (b != null && b.Count > 0)
                {
                    hotbasket.Add(b);
                }
            }**/
            //debug("got basket:" + string.Join(",", hotbasket.ToSymArray()).ToString(), QSEnumDebugLevel.MUST);
        }


        #endregion





        #region 【服务端止盈止损】客户端提交上来的持仓止盈止损 服务端检查

        //void SendOrder(Order o)
        //{
        //    if (SendOrderEvent != null)
        //        SendOrderEvent(o);
        //}

        //void CancelOrder(long oid)
        //{
        //    if (CancelOrderEvent != null)
        //        CancelOrderEvent(oid);
        //}

        //void AssignOrderID(ref Order o)
        //{
        //    if (AssignOrderIDEvent != null)
        //        AssignOrderIDEvent(ref o);
        //}

        /// <summary>
        /// 获得某个账户的所有持仓止盈止损参数
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public PositionOffsetArgs[] GetPositionOffset(string account)
        {
            return _posoffsetracker.GetPositionOffset(account);
        }

        /// <summary>
        /// 客户端提交止盈止损参数更新到监控器
        /// </summary>
        /// <param name="args"></param>
        public void GotPositionOffsetArgs(PositionOffsetArgs args)
        {
            _posoffsetracker.GotPositionOffsetArgs(args);

        }

        /// <summary>
        /// 行情驱动止盈止损监控器进行工作,满足条件对外触发平仓指令
        /// </summary>
        /// <param name="k"></param>
        public void GotTick(Tick k)
        {
            _posoffsetracker.GotTick(k);
        }

        /// <summary>
        /// 获得取消
        /// </summary>
        /// <param name="oid"></param>
        public void GotCancel(long oid)
        {
            foreach (PositionFlatSet ps in posflatlist)
            {
                //如果取消的委托是平仓所发出的委托 则将orderid置0 表面该委托已经被取消
                if (ps.OrderID == oid)
                    ps.OrderID = 0;
            }
            _posoffsetracker.GotCancel(oid);
        }

        /// <summary>
        /// 响应交易服务返回过来的ErrorOrder
        /// 比如风控中心强平 发送委托 但是委托被拒绝，则需要对该事件进行响应
        /// 否则超时后 会出现强平系统无法正常撤单的问题。而无法正常撤单则没有撤单回报,导致强平系统一直试图撤单
        /// </summary>
        /// <param name="error"></param>
        public void GotErrorOrder(ErrorOrder error)
        {
            debug("~~~~~~~~~~~~~~~~~~~~~ riskcentre got errororder orderid:" + error.Order.id.ToString(), QSEnumDebugLevel.INFO);
            foreach (PositionFlatSet ps in posflatlist)
            {
                //如果委托被拒绝 并且委托ID是本地发送过去的ID 则将positionflatset的委托ID置0
                if (ps.OrderID == error.Order.id && error.Order.Status == QSEnumOrderStatus.Reject)
                    ps.OrderID = 0;
            }

            //止损 止盈
            //_posoffsetracker.GotCancel(oid);
        }

        #endregion

        #region 【客户端信息跟踪】跟踪客户端注册 注销记录
        /// <summary>
        /// 获得某个客户端的信息记录
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public ClientTrackerInfo GetClientTracker(string acc)
        {
            if (!trackermap.Keys.Contains(acc))
            {
                return null;
            }
            return trackermap[acc];//获得该account所对应的追踪信息

        }
        ConcurrentDictionary<string, ClientTrackerInfo> trackermap = new ConcurrentDictionary<string, ClientTrackerInfo>();

        /// <summary>
        /// 检查某个账户是否登入
        /// </summary>
        /// <param name="account"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool Islogin(string account, out ClientTrackerInfo info)
        {
            info = null;
            if (trackermap.TryGetValue(account, out info))
                return info.IsLogined;

            return false;
        }


        /// <summary>
        /// 获得账户登入或者登出信息
        /// tradingserver只负责触发登入 登出信息 风控中心记录客户端的登入 登出状态 以及客户端的本地信息
        /// 
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="login"></param>
        /// <param name="info"></param>
        public void GotLoginInfo(string acc, bool login, ClientInfoBase info)
        {
            try
            {
                if (!_clearcentre.HaveAccount(acc)) return;//

                if (!trackermap.Keys.Contains(acc))
                {
                    trackermap.TryAdd(acc, new ClientTrackerInfo(acc,info));
                }
                ClientTrackerInfo ti = trackermap[acc];//获得该account所对应的追踪信息

                //如果是登入
                if (login)
                {
                    if (!ti.IsLogined)//原先没有登入 则登入次数++
                        ti.LoginNum++;
                    //如果没有登入，则记录当前时间为登入时间
                    if (ti.IsLogined == false)//没有登入则记录登入时间
                        ti.LoginTime = DateTime.Now;
                    ti.IsLogined = true;
                }
                else
                {
                    ti.IsLogined = false;
                    //注销时记录我们的时长
                    if (ti.LoginTime != DateTime.MinValue)
                        ti.OnlineSecend += (int)(DateTime.Now - ti.LoginTime).TotalSeconds;

                }
                //更新采集到的信息
                ti.IPAddress = info.IPAddress;//IP地址
                ti.HardWareCode = info.HardWareCode;//硬件码
                ti.FrontID = string.IsNullOrEmpty(info.Location.FrontID) ? "Direct" : info.Location.FrontID;//前置机地址
                //记录客户端的类型和版本
                //ti.API_Type = "";
                //ti.API_Version = "";
                //对外触发客户端信息
                if (ClientSessionEvent != null)
                    ClientSessionEvent(ti, login);
            }
            catch (Exception ex)
            {
                debug(PROGRAME + ":riskcenter got clientinfo error:" + ex.ToString());
            }
        }

        void CheckClientTrackerInfo()
        {

            IEnumerable<ClientTrackerInfo> infos = trackermap.Values;

            //条件搜索语句
            IEnumerable<ClientTrackerInfo> set =
                from info in infos
                where info.TotalTime.TotalHours > 2 //找出在线时长大于2小时的客户端
                select info;

        }
        #endregion

        #region 重置
        /// <summary>
        /// 风空中心重置风控规则
        /// </summary>
        public void Reset()
        {
            debug("重置风控中心", QSEnumDebugLevel.INFO);
            foreach (IAccount a in _clearcentre.Accounts)
            {
                a.Reset();//重置交易帐户 交易帐户对象内存复位
            }

            //清空帐户的止盈止损参数设置
            _posoffsetracker.Clear();

            //清空帐户当日登入信息
            trackermap.Clear();

            Notify("风控中心重置(结算后)[" + DateTime.Now.ToShortDateString() + "]", " ");
        }

        #endregion


        public override void Dispose()
        {
            _posoffsetracker.Dispose();

            debug("RiskCentre Dispose Called",QSEnumDebugLevel.INFO);
            base.Dispose();

            
        }
    }


    
    


}
