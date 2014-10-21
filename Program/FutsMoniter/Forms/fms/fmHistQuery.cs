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


namespace FutsMoniter
{
    //public enum QSHistQryType
    //{
    //    HISTORDER,
    //    HISTTRADE,
    //    HISTPOSITION,
    //    HISTCASH,
    //    HISTSETTLE,
    //}


    public partial class fmHistQuery : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public fmHistQuery()
        {
            InitializeComponent();
            //this.QryType = QSHistQryType.HISTORDER;

            settleday.Value = DateTime.Now;

            this.FormClosing +=new FormClosingEventHandler(fmHistQuery_FormClosing);
        }

       public void GotHistOrder(Order o, bool islast)
        {

            if (islast)
            {
                if (!string.IsNullOrEmpty(o.Account))
                {
                    ctHistOrder1.GotHistOrder(o);
                }
                Globals.TLClient.ReqQryHistTrades(account.Text, Settleday);
            }
            else
            {
                ctHistOrder1.GotHistOrder(o);
            }
        }
        public void GotHistTrade(Trade f, bool islast)
        {
            if (islast)
            {
                if (!string.IsNullOrEmpty(f.Account))
                {
                    ctHistTrade1.GotHistFill(f);
                }
                Globals.TLClient.ReqQryHistPosition(account.Text, Settleday);
            }
            else
            {
                ctHistTrade1.GotHistFill(f);
            }
        }

        public void GotHistPosition(SettlePosition pos, bool islast)
        {
            if (islast)
            {
                if (!string.IsNullOrEmpty(pos.Account))
                {
                    ctHistPosition1.GotHistPosition(pos);
                }
                //Globals.TLClient.ReqQryHistCashTransaction(account.Text, Settleday);
            }
            else
            {
                ctHistPosition1.GotHistPosition(pos);
            }
        }



       

        public void SetAccount(string acc)
        {
            account.Text = acc;
        }

        private void fmHistQuery_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        DateTime lastqrytime = DateTime.Now;
        private void btnQryHist_Click(object sender, EventArgs e)
        {
            if (!(DateTime.Now.Subtract(lastqrytime).TotalSeconds > 10))
            {
                fmConfirm.Show("请不要频繁查询,每隔10秒查询一次!");
                return;
            }
            lastqrytime = DateTime.Now;
            //清空当前数据
            ctHistOrder1.Clear();
            ctHistTrade1.Clear();
            ctHistPosition1.Clear();
            //ctHistCashTransaction1.Clear();
            //sb.Clear();
            Globals.TLClient.ReqQryHistOrders(account.Text, Settleday);
            
        }

        int Settleday
        {
            get
            {
                return TradingLib.Common.Util.ToTLDate(settleday.Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //public QSHistQryType QryType { get; set; }
    }
}
