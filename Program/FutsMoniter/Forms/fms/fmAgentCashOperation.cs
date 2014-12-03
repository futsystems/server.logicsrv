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
using FutSystems.GUI;
using TradingLib.Mixins.LitJson;
using TradingLib.Mixins.JsonObject;

namespace FutsMoniter
{
    public partial class fmAgentCashOperation : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        int mgrfk = 0;
        bool loaded = false;
        public fmAgentCashOperation()
        {
            InitializeComponent();
            Factory.IDataSourceFactory(cashoptype).BindDataSource(UIUtil.GetEnumValueObjects<QSEnumCashOperation>());
            if (Globals.EnvReady)
            {
                mgrfk = (int)Globals.BaseMGRFK;
                lbmgrfk.Text = mgrfk.ToString();
            }
            message.Visible = false;
            loaded = true;
        }

        decimal avabile = 0;
        
        public void SetAvabileBalance(decimal amount)
        {
            avabile = amount;
        }


        private void btnSubmit_Click(object sender, EventArgs e)
        {
            QSEnumCashOperation op = (QSEnumCashOperation)cashoptype.SelectedValue;
            decimal a = amount.Value;
            if (a <= 0)
            {
                fmConfirm.Show("请输入有效金额!");
                return;
            }
            if (a > avabile && op == QSEnumCashOperation.WithDraw)
            {
                fmConfirm.Show("最大提现金额:"+avabile.ToString());
                return;
            }

            if (fmConfirm.Show("确认 " + Util.GetEnumDescription(op) + " " + a.ToString() + " ?") == System.Windows.Forms.DialogResult.Yes)
            { 
                JsonWrapperCashOperation request = new JsonWrapperCashOperation();
                request.Amount = a;
                request.Operation = op;
                request.DateTime = Util.ToTLDateTime();
                Globals.TLClient.ReqRequestCashOperation(TradingLib.Mixins.LitJson.JsonMapper.ToJson(request));
            }
            this.Close();
        }

       
    }
}
