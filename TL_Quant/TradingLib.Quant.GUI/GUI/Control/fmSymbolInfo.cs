using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using TradingLib.Common;

namespace TradingLib.Quant.GUI
{
    public partial class fmSymbolInfo : KryptonForm
    {

        SecurityImpl _sec;
        
        public fmSymbolInfo(SecurityImpl sec)
        {
            InitializeComponent();
            _sec = sec;
            symbol.Text = sec.Symbol;
            exchange.Text = sec.DestEx;
            muplite.Text = sec.Multiple.ToString();
            pricetick.Text = sec.PriceTick.ToString();
            entrycommission.Text = sec.EntryCommission.ToString();
            exitcommission.Text = sec.ExitCommission.ToString();
        }

        private void ok_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            SaveChanges();
            this.Close();
        }

        void SaveChanges()
        {
            _sec.Symbol = symbol.Text;
        }

        
    }
}
