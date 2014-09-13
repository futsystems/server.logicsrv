﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using ComponentFactory.Krypton.Toolkit;
using TradingLib.API;

namespace Strategy.GUI
{
    public partial class fmTargetProfit:KryptonForm
    {
        public decimal TargetProfit { get { return _targetProfit.Value; } }
        public int Size { get { return (int)_size.Value; } }

        public fmTargetProfit(string symbol,decimal p, int s)
        {
            InitializeComponent();
            _size.Value = s;
            _targetProfit.Value = p;
            Text = symbol;
            this.FormClosing += new FormClosingEventHandler(fmTargetProfit_FormClosing);
        }

        void fmTargetProfit_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
        public void updateForm(decimal price,decimal cost,bool side)
        {
            _last.Text = formatdisp(price);
            _targetprice.Text = formatdisp(side ? cost + TargetProfit : cost - TargetProfit);

        }
        public void SetSecurity(Security sec)
        { 
            decimal a = sec.PriceTick;
            int p = TradingLib.GUI.UIUtil.getDecimalPlace(sec.PriceTick);
            _format = TradingLib.GUI.UIUtil.getDisplayFormat(sec.PriceTick);
            _targetProfit.Increment = a;
            _targetProfit.DecimalPlaces = p;
        }
        private string _format = "{0:F1}";
        private string formatdisp(decimal d)
        {
            return string.Format(_format, d);
        }

        //public Const  HTCAPTION = 2; 
        const int WM_NCLBUTTONDBLCLK = 0x00A3;
        //private bool _winhide = false;
        private int _winheight = 0;
        protected override void WndProc(ref Message m)
        {
            
            if (m.WParam.ToInt32() == 2 && m.Msg == WM_NCLBUTTONDBLCLK)
            {
                if (_winheight <= 0)
                {
                    _winheight = Height;
                    ClientSize = new Size(ClientSize.Width, 0);


                }
                else
                {
                    ClientSize = new Size(ClientSize.Width, 40);
                    _winheight = 0;

                }

            }
            base.WndProc(ref m);


        }

        private void kryptonLinkLabel1_LinkClicked(object sender, EventArgs e)
        {
            MemoTargetProfit fm = new MemoTargetProfit();
            fm.Show();
        }
       
    }
}
