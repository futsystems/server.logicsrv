using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Common;


namespace FutsMoniter
{
    public partial class fmSettleManager : ComponentFactory.Krypton.Toolkit.KryptonForm,IEventBinder
    {
        public fmSettleManager()
        {
            InitializeComponent();
            this.Load += new EventHandler(fmSettleManager_Load);
        }

        public void OnInit()
        {
            Globals.LogicEvent.RegisterCallback("SettleCentre", "RollBackToDay", OnRollBack);
            Globals.LogicEvent.RegisterCallback("SettleCentre", "QrySettleStatus", OnQrySettleStatus);
        }

        public void OnDisposed()
        {
            Globals.LogicEvent.UnRegisterCallback("SettleCentre", "RollBackToDay", OnRollBack);
            Globals.LogicEvent.UnRegisterCallback("SettleCentre", "QrySettleStatus", OnQrySettleStatus);
        }

        void OnQrySettleStatus(string json)
        {
            var data = TradingLib.Mixins.Json.JsonMapper.ToObject(json)["Payload"];

            InvokeGotSettleStatus(data);
        }

        /// <summary>
        /// 相应回滚到某个交易日的回报
        /// </summary>
        /// <param name="json"></param>
        void OnRollBack(string json)
        {
            Globals.Debug("?????????????? rollbacktoday called finished....");
            int settleday = Util.ToTLDate(dpSettleday.Value);
            //查询结算价信息
            Globals.TLClient.ReqQrySettlementPrice(settleday);

            //查询结算状态信息
            Globals.TLClient.ReqQrySettleStatus();

            //查询持仓数据
            Globals.TLClient.ReqQryPositionHold();
            
        }
        void fmSettleManager_Load(object sender, EventArgs e)
        {
            btnDelSettleInfo.Click += new EventHandler(btnDelSettleInfo_Click);
            btnLoadInfo.Click += new EventHandler(btnLoadInfo_Click);
            btnReSettle.Click += new EventHandler(btnReSettle_Click);
            Globals.RegIEventHandler(this);

            //查询结算状态
            Globals.TLClient.ReqQrySettleStatus();
        }


        void btnReSettle_Click(object sender, EventArgs e)
        {
            int settleday = Util.ToTLDate(dpSettleday.Value);
            if (MoniterUtils.WindowConfirm(string.Format("确认对交易日:{0}执行重新结算操作", settleday)) == System.Windows.Forms.DialogResult.Yes)
            {
                Globals.TLClient.ReqReSettle(settleday);
            }
        }

        void btnLoadInfo_Click(object sender, EventArgs e)
        {
            int settleday = Util.ToTLDate(dpSettleday.Value);
            if (MoniterUtils.WindowConfirm(string.Format("确认回滚到交易日:{0}", settleday)) == System.Windows.Forms.DialogResult.Yes)
            {
                //清空结算价信息表 用于准备获得回滚日期对应的结算价信息
                ctSettlementPrice1.Clear();
                ctPositionHold1.Clear();
                Globals.TLClient.ReqRollBackToDay(settleday);
            }
        }

        void btnDelSettleInfo_Click(object sender, EventArgs e)
        {
            int settleday = Util.ToTLDate(dpSettleday.Value);

            if (MoniterUtils.WindowConfirm(string.Format("确认删除交易日:{0}", settleday)) == System.Windows.Forms.DialogResult.Yes)
            {
                Globals.TLClient.ReqDeleteSettleInfo(settleday);
            }
        }

        void InvokeGotSettleStatus(TradingLib.Mixins.Json.JsonData data)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<TradingLib.Mixins.Json.JsonData>(InvokeGotSettleStatus), new object[] { data });
            }
            else
            {
                lbLastSettleday.Text = data["last_settleday"].ToString();
                lbNextSettleday.Text = data["next_settleday"].ToString();
                lbCurrentday.Text = data["current_settleday"].ToString();
            }
        }
    }
}
