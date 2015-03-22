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
    public partial class fmFlatPosition : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public fmFlatPosition()
        {
            InitializeComponent();
            DateTime n = DateTime.Now;
            time.Value = new DateTime(n.Year, n.Month, n.Day, 14, 50, 00);
            offsetflag.Items.Add("平仓"); //close
            offsetflag.Items.Add("平昨"); //close_yestoday
            offsetflag.SelectedIndex = 0;
        }

        PositionEx _pos;
        public void SetPosition(PositionEx pos)
        {
            _pos = pos;
            lbAccount.Text = pos.Account;
            lbSymbol.Text = pos.Symbol;
            lbHoldSize.Text = pos.Position.ToString();
            lbAvgPrice.Text = pos.AvgPrice.ToString();

            size.Maximum = Math.Abs(pos.Position);

        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (MoniterUtils.WindowConfirm("确认执行手工平仓(再次确认时间和价格)?") == System.Windows.Forms.DialogResult.Yes)
            {
                var data = new {
                                
                                account = _pos.Account,
                                symbol = _pos.Symbol,
                                side = _pos.Side,

                                time = Util.ToTLTime(time.Value),
                                size = (int)size.Value,
                                price = price.Value,
                                offset_flag = offsetflag.SelectedIndex ==0?2:4
                                
                            };
                string s = TradingLib.Mixins.Json.JsonMapper.ToJson(data);
                MessageBox.Show(s);
                Globals.TLClient.ReqFlatPositionHold(data);
            }
        }
    }
}
