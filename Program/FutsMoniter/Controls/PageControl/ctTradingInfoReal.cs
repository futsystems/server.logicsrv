using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Common;


namespace FutsMoniter
{
    public partial class ctTradingInfoReal : UserControl, IEventBinder
    {
        public ctTradingInfoReal()
        {
            InitializeComponent();
            this.Load += new EventHandler(ctTradingInfoReal_Load);
        }

        void debug(string msg)
        {
            Globals.Debug(msg);
        }



        void ctTradingInfoReal_Load(object sender, EventArgs e)
        {
            //交易信息显示控件事件
            ctOrderView1.SendDebugEvent += new DebugDelegate(debug);
            ctOrderView1.SendOrderCancel += new LongDelegate(CancelOrder);

            ctPositionView1.SendDebugEvent += new DebugDelegate(debug);
            ctPositionView1.SendCancelEvent += new LongDelegate(CancelOrder);
            ctPositionView1.SendOrderEvent += new OrderDelegate(SendOrder);

            ctTradeView1.SendDebugEvent += new DebugDelegate(debug);

            Globals.RegIEventHandler(this);
        }

        public void OnInit()
        {
            Globals.LogicEvent.GotAccountSelectedEvent += new Action<IAccountLite>(OnAccountSelected);
            Globals.LogicEvent.GotTickEvent += new TickDelegate(GotTick);
            Globals.LogicEvent.GotOrderEvent += new OrderDelegate(GotOrder);
            Globals.LogicEvent.GotFillEvent += new FillDelegate(GotTrade);

            Globals.LogicEvent.GotResumeResponseEvent += new Action<RspMGRResumeAccountResponse>(OnResume);

            ctOrderView1.EnableOperation = Globals.UIAccess.fun_info_operation;
            ctPositionView1.EnableOperation = Globals.UIAccess.fun_info_operation;

        }

        public void OnDisposed()
        {
            Globals.LogicEvent.GotAccountSelectedEvent -= new Action<IAccountLite>(OnAccountSelected);
            Globals.LogicEvent.GotTickEvent -= new TickDelegate(GotTick);
            Globals.LogicEvent.GotOrderEvent -= new OrderDelegate(GotOrder);
            Globals.LogicEvent.GotFillEvent -= new FillDelegate(GotTrade);

            Globals.LogicEvent.GotResumeResponseEvent -= new Action<RspMGRResumeAccountResponse>(OnResume);
            Globals.Debug("ctTradingInfoReal disposed");
        }
        IAccountLite _account = null;
        IAccountLite CurrentAccount { get { return _account; } }
        /// <summary>
        /// 判断是否是当前选中帐户
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        bool IsCurrentAccount(string account)
        {
            if (CurrentAccount == null) return false;
            if (CurrentAccount.Account.Equals(account)) return true;
            return false;
        }

        void OnAccountSelected(IAccountLite account)
        {
            _account = account;
            //清空控件交易记录
            ClearTradingInfo();

            //开始恢复该帐户交易记录
            Globals.TradingInfoTracker.RequetResume(account.Account);


        }

        /// <summary>
        /// 响应交易数据恢复
        /// </summary>
        void OnResume(RspMGRResumeAccountResponse response)
        {
            if (response.IsBegin)
            {
                debug("resume account:" + response.ResumeAccount + " start...");

                string account = response.ResumeAccount;
                IAccountLite acclit = null;
                //if (accountmap.TryGetValue(account, out acclit))
                {

                    Globals.TradingInfoTracker.StartResume(CurrentAccount);
                }
                //else
                {
                    //debug("无法找到对应的帐户:" + account);
                }
            }
            else
            {
                //数据恢复结束
                debug("resume account:" + response.ResumeAccount + " end...");
                Globals.TradingInfoTracker.EndResume();
                LoadAccountInfo(response.ResumeAccount);
            }
        }

        /// <summary>
        /// 清空交易记录
        /// </summary>
        void ClearTradingInfo()
        {
            //清空交易信息缓存
            Globals.TradingInfoTracker.Clear();
            ctOrderView1.Clear();
            ctPositionView1.Clear();
            ctTradeView1.Clear();
        }


        /// <summary>
        /// 加载帐户数据
        /// </summary>
        /// <param name="account"></param>
        void LoadAccountInfo(string account)
        {
            debug("try to load trading info tracker account:" + Globals.TradingInfoTracker.Account.Account + " request account:" + account);
            if (Globals.TradingInfoTracker.Account.Account.Equals(account))
            {
                ctPositionView1.PositionTracker = Globals.TradingInfoTracker.PositionTracker;
                ctPositionView1.OrderTracker = Globals.TradingInfoTracker.OrderTracker;
                ctOrderView1.OrderTracker = Globals.TradingInfoTracker.OrderTracker;

                foreach (Position pos in Globals.TradingInfoTracker.HoldPositionTracker)
                {
                    ctPositionView1.GotPosition(pos);
                }

                foreach (Order o in Globals.TradingInfoTracker.OrderTracker)
                {
                    ctOrderView1.GotOrder(o);
                }

                foreach (Trade f in Globals.TradingInfoTracker.TradeTracker)
                {
                    ctTradeView1.GotFill(f);
                    ctPositionView1.GotFill(f);
                }
            }
            else
            {
                debug("TradingInfoTracker 维护帐户与请求加载帐户不一致..");
            }

        }


        void GotTick(Tick k)
        {
            ctPositionView1.GotTick(k);
        }

        /// <summary>
        /// 获得服务端转发的委托
        /// </summary>
        /// <param name="o"></param>
        void GotOrder(Order o)
        {
            if (IsCurrentAccount(o.Account) && Globals.TradingInfoTracker.IsReady(o.Account))
            {
                ctOrderView1.GotOrder(o);
                ctPositionView1.GotOrder(o);
            }
        }

        /// <summary>
        /// 获得服务端转发的成交
        /// </summary>
        /// <param name="f"></param>
        void GotTrade(Trade f)
        {
            if (IsCurrentAccount(f.Account) && Globals.TradingInfoTracker.IsReady(f.Account))
            {
                ctTradeView1.GotFill(f);
                ctPositionView1.GotFill(f);
            }
        }


        void CancelOrder(long oid)
        {
            OrderAction actoin = new OrderActionImpl();
            actoin.Account = CurrentAccount.Account;
            actoin.ActionFlag = QSEnumOrderActionFlag.Delete;
            actoin.OrderID = oid;
            SendOrderAction(actoin);
        }

        void SendOrderAction(OrderAction action)
        {
            Globals.TLClient.ReqOrderAction(action);
        }

        void SendOrder(Order o)
        {
            o.Account = CurrentAccount.Account;
            Globals.TLClient.ReqOrderInsert(o);
        }
    }
}
