using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;
using ComponentFactory.Krypton.Toolkit;
using TradingLib.GUI;

namespace Strategy.GUI
{
    public partial class fmScalperManual : Form
    {
        public decimal Loss { get { return setLoss.Value; } }//止损
        public decimal ProfitTake { get { return setTargetProfit.Value; } }//止盈
        public bool IsProfitTakeOrderPark { get { return setIsProfitOrderPark.Checked; } }//是否挂单
        public bool IsLimitEntryOrder { get { return setLimit.Checked; } }//是否限价
        public bool IsOffset { get { return isOffset.Checked; } }
        public bool IsLimitPrice { get { return isPrice.Checked; } }
        public decimal LastPrice { get { return Convert.ToDecimal(last.Text); } }//最新成交
        public decimal Offset { get { return offset.Value; } }//价格偏移
        public decimal LimitPirce { get { return price.Value; } }//限定价格
        public int TIF { get { return Convert.ToInt16(tif.Value); } }//延迟撤单时间
        public int OrdSize { get {
            int size = Convert.ToInt16(setSize.Value);
            return size > 0 ? size : 1;
        } }

        public fmScalperManual(string symbol,decimal profittarget,decimal loss,bool parked)
        {
            InitializeComponent();
            Text = "炒单:" + symbol;
            setLoss.Value = loss;
            setTargetProfit.Value = profittarget;
            setIsProfitOrderPark.Checked = parked;

            ask.ForeColor = Color.Red;
            bid.ForeColor = Color.Green;
            /*
            isOffset.Enabled = true;
            isPrice.Enabled = true;
            offset.Enabled = true;
            price.Enabled = false;**/
            //InitToolTip();
        }
        /*
        public void setDefaultConfig(decimal profittarget, decimal loss, bool parked)
        {
            setLoss.Value = loss;
            setTargetProfit.Value = profittarget;
            setIsProfitOrderPark.Checked = parked;
        }
        **/


        public void updateForm(Position p)
        {
            Color c = p.UnRealizedPL > 0 ? Color.Red : Color.Green;
            avgCost.Text = formatdisp(p.AvgPrice);
            avgCost.ForeColor = c;
            possize.Text = p.Size.ToString();
            possize.ForeColor = p.isLong ? Color.Red : Color.Green;

            unpl.Text = p.isFlat ? "0" : formatdisp(Math.Abs(p.UnRealizedPL / p.Size));
            unpl.ForeColor = c;
            
        }
        public void updateForm(Tick k, Position p)
        {
            Color c = p.UnRealizedPL > 0 ? Color.Red : Color.Green;
            if (k.hasAsk)
            {
                ask.Text = formatdisp(k.ask);
                askSize.Text = formatdisp(k.os);
            }
            if (k.hasBid)
            {
                bid.Text = formatdisp(k.bid);
                bidSize.Text = formatdisp(k.bs);
            }
            avgCost.Text = formatdisp(p.AvgPrice);
            avgCost.ForeColor = c;
            possize.Text = p.Size.ToString();
            possize.ForeColor = p.isLong ? Color.Red : Color.Green;
            
            unpl.Text = p.isFlat?"0":formatdisp(Math.Abs(p.UnRealizedPL / p.Size));
            unpl.ForeColor = c;

            if (k.isTrade)
            {
                last.Text = formatdisp(k.trade);
            }
            //ctTradeInfo1.GotTick(k);
            button1.Enabled = p.isFlat ? false : true;
        }
        //设定价格显示方式以及最小价格变动
        public void SetSecurity(Security sec)
        {
            decimal a = sec.PriceTick;
            int p = TradingLib.GUI.UIUtil.getDecimalPlace(sec.PriceTick);
            offset.Increment = a;
            offset.DecimalPlaces = p;
            price.Increment = a;
            price.DecimalPlaces = p;
            setLoss.Increment = a;
            setLoss.DecimalPlaces = p;
            setTargetProfit.Increment = a;
            setTargetProfit.DecimalPlaces = p;
            _format = TradingLib.GUI.UIUtil.getDisplayFormat(sec.PriceTick);
        }
        private string _format = "{0:F1}";
        private string formatdisp(decimal d)
        {
            return string.Format(_format, d);
        }

        private void fmScalperManual_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }


        public event VoidDelegate SendFlatPosition;
        public event VoidDelegate SendBuyAction;
        public event VoidDelegate SendSellAction;
        private void flat_Click(object sender, EventArgs e)
        {
            if (SendFlatPosition != null)
                SendFlatPosition();
        }

        private void sell_Click(object sender, EventArgs e)
        {
            if (SendSellAction != null)
                SendSellAction();
        }

        private void buy_Click(object sender, EventArgs e)
        {
            if (SendBuyAction != null)
                SendBuyAction();

        }

        private void setLimit_CheckedChanged(object sender, EventArgs e)
        {
            if (IsLimitEntryOrder)
            {
                if(isOffset.Checked)
                    offset.Enabled = true;
                if(isPrice.Checked)
                    price.Enabled = true;
                isOffset.Enabled = true;
                isPrice.Enabled = true;
                tif.Enabled = true;
                
                setLimit.Text = "限价";
            }
            else
            {
                offset.Enabled = false;
                price.Enabled = false;
                isOffset.Enabled = false;
                isPrice.Enabled = false;
                tif.Enabled = false;
                setLimit.Text = "市价";
            }

        }

        private void memo_LinkClicked(object sender, EventArgs e)
        {
            MemoScalper fm = new MemoScalper();
            fm.Show();
        }

        private void isPrice_CheckedChanged(object sender, EventArgs e)
        {
            if (IsLimitEntryOrder)
            {
                //offset.Value = 0;
                offset.Enabled = false;
                price.Enabled = true;
            }
        }

        private void isOffset_CheckedChanged(object sender, EventArgs e)
        {
            if (IsLimitEntryOrder)
            {
                price.Enabled = false;
                offset.Enabled = true;
            }
        }







    }
}
