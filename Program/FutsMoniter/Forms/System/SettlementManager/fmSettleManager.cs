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
        }

        public void OnDisposed()
        {
            Globals.LogicEvent.UnRegisterCallback("SettleCentre", "RollBackToDay", OnRollBack);
        }

        void OnRollBack(string json)
        {
            Globals.Debug("?????????????? rollbacktoday called finished....");
            int settleday = Util.ToTLDate(dpSettleday.Value);
            Globals.TLClient.ReqQrySettlementPrice(settleday);
        }
        void fmSettleManager_Load(object sender, EventArgs e)
        {
            btnDelSettleInfo.Click += new EventHandler(btnDelSettleInfo_Click);
            btnLoadInfo.Click += new EventHandler(btnLoadInfo_Click);
            btnReSettle.Click += new EventHandler(btnReSettle_Click);
            Globals.RegIEventHandler(this);
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
    }
}
