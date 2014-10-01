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
    public partial class fmStopTrailing : Form
    {
        public event VoidDelegate SendFlatPosition;
        public event VoidDelegate SendBuyAction;
        public event VoidDelegate SendSellAction;
        private bool stoplossEnable = false;
        private bool breakevenEnable = false;
        private bool trailing1Enable = false;
        private bool trailing2Enable = false;

        public decimal StopLoss { get { return stoploss.Value; } }
        public decimal BreakEven { get { return breakeven.Value; } }
        public decimal Start1 { get { return start1.Value; } }
        public decimal Loss1 { get { return trailing1.Value; } }
        public decimal Start2 { get { return start2.Value; } }
        public decimal Loss2 { get { return trailing2.Value; } }

        public bool Trailing1Enable {get{ return trailing1Enable;}}
        public bool Trailing2Enable { get { return trailing2Enable; } }
        public bool StopLossEnable { get { return stoplossEnable; } }
        public bool BreakEvenEnable { get { return breakevenEnable; } }

        public int OrdSize { get { return (int)_osize.Value; } }


        //初始化
        public fmStopTrailing(decimal _stoploss,decimal _breakeven,decimal _start1,decimal _loss1,decimal _start2,decimal _loss2)
        {
            InitializeComponent();
            stoploss.Value = _stoploss;
            breakeven.Value = _breakeven;
            start1.Value = _start1;
            start2.Value = _start2;
            trailing1.Value = _loss1;
            trailing2.Value = _loss2;

            UpdateSetting();
        }
        //当模型参数改变后,调用该函数用于更新界面
        private void UpdateSetting()
        {
            if (StopLoss > 0)
                stoplossEnable = true;
            else
            {
                stoplossEnable = false;
                _stoploss.Text = "无止损";
            }

            if (BreakEven > 0)
                breakevenEnable = true;
            else
            {
                breakevenEnable = false;
                _breakEven.Text = "无保本";
            }
            if (Start1 > 0 && Loss1 > 0)
                trailing1Enable = true;
            else
            { 
                trailing1Enable=false;
            }
            if (trailing1Enable && Start2 > 0 && Loss2 > 0)
                trailing2Enable = true;
            else
            {
                trailing2Enable = false;
            }

            if (!Trailing1Enable)
                _profitTakestop.Text = "无止盈";

        }


        //发送信息
        public void message(string msg)
        {
            //debugControl1.GotDebug(msg);
        }
        decimal _stoplossprice;
        decimal _breakevenprice;
        decimal _profitprice1;
        decimal _profitprice2;
        //更新窗体界面
        public void updateForm(Tick k, Position p, decimal stoplossprice, decimal breakevenprice, decimal profitprice1, decimal profitprice2)
        {

            Text = p.Symbol + "[" + "止损止盈" + "]" + ":" + p.UnRealizedPL.ToString("N2") + "@" + p.Size.ToString();
             Color c =  p.UnRealizedPL >= 0 ? System.Drawing.Color.Red : System.Drawing.Color.Green;
            _last.Text = k.isTrade ? formatdisp(k.trade) : _last.Text;
            _last.ForeColor = c;
            _bid.Text = formatdisp(k.bid);
            _bid.ForeColor = c;
            _ask.Text = formatdisp(k.ask);
            _ask.ForeColor = c;
            _pos.Text = p.Size.ToString("N0");

            _avgcost.Text = formatdisp(p.AvgPrice);
            _favor.Text = p.isLong ? formatdisp(p.Highest) : formatdisp(p.Lowest);
            _adverse.Text = p.isLong ? formatdisp(p.Lowest) : formatdisp(p.Highest);

            _unrealizedpl.Text = formatdisp(p.UnRealizedPL);
            _unrealizedpl.ForeColor = c;
            //_unrealizedpl.Text = formatdisp(p.UnRealizedPL);
            //_unrealizedpl.ForeColor = c;
            //_closedpl.Text = formatdisp(p.ClosedPL);
            //_closedpl.ForeColor = p.ClosedPL >0 ? System.Drawing.Color.Red : System.Drawing.Color.Green;
            if(StopLossEnable)
                if(stoplossprice>0)
                    _stoploss.Text = formatdisp(stoplossprice);
                else
                    _stoploss.Text="无";


            if (BreakEvenEnable)
            {
                if (breakevenprice > 0)
                    _breakEven.Text = formatdisp(breakevenprice);
                else
                    _breakEven.Text = "未触发";
            }

            
            if (Trailing1Enable)
            {
                string s = "未触发止盈";
                if (profitprice1 > 0)
                    s = "一级:" + formatdisp(profitprice1);
                if (profitprice2 > 0)
                    s = "二级:" + formatdisp(profitprice2);
                _profitTakestop.Text = s;
            }
            else
                _profitTakestop.Text = "无止盈";
            button1.Enabled = p.isFlat?false:true;
            //刷新绘图
            //ctTimesLineChart1.GotTick(k);

        }
        //静止关闭
        private void fmStopTrailing_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }


        public void SetSecurity(Security sec)
        {
            decimal a = sec.PriceTick;
            int p = TradingLib.GUI.UIUtil.getDecimalPlace(sec.PriceTick);
            stoploss.Increment = a;
            stoploss.DecimalPlaces = p;
            breakeven.Increment = a;
            breakeven.DecimalPlaces = p;
            start1.Increment = a;
            start1.DecimalPlaces = p;
            trailing1.Increment = a;
            trailing1.DecimalPlaces = p;
            trailing2.Increment = a;
            trailing2.DecimalPlaces = p;
            _format = TradingLib.GUI.UIUtil.getDisplayFormat(sec.PriceTick);
        }
        private string _format = "{0:F1}";
        private string formatdisp(decimal d)
        {
            return string.Format(_format, d);
        }
        //平仓
        private void button1_Click(object sender, EventArgs e)
        {
            if (SendFlatPosition != null)
                SendFlatPosition();
        }
        //买入
        private void button3_Click(object sender, EventArgs e)
        {
            if (SendBuyAction != null)
                SendBuyAction();
        }
        //卖出
        private void button2_Click(object sender, EventArgs e)
        {
            if (SendSellAction != null)
                SendSellAction();
        }

        private void stoploss_ValueChanged(object sender, EventArgs e)
        {
            UpdateSetting();
        }

        private void breakeven_ValueChanged(object sender, EventArgs e)
        {
            UpdateSetting();
        }

        private void start1_ValueChanged(object sender, EventArgs e)
        {
            UpdateSetting();
        }

        private void trailing1_ValueChanged(object sender, EventArgs e)
        {
            UpdateSetting();
        }

        private void start2_ValueChanged(object sender, EventArgs e)
        {
            UpdateSetting();
        }

        private void trailing2_ValueChanged(object sender, EventArgs e)
        {
            UpdateSetting();
        }

        private void btnmemo_LinkClicked(object sender, EventArgs e)
        {
            MemoTwoStep fm = new MemoTwoStep();
            fm.Show();
        }

    }
}
