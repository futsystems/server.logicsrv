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

namespace TradingLib.Quant.View
{
    public partial class ctBarListView : UserControl
    {
        public ctBarListView()
        {
            InitializeComponent();
        }

        public void ShowBars(IList<Bar> list)
        {
            foreach (Bar b in list)
            {
                this.GotBar(b);
            }
        }

        public void GotBar(Bar bar)
        {
            if (InvokeRequired)
                Invoke(new BarDelegate(GotBar), new object[] { bar });
            else
            {
                gridview.Rows.Add(bar.BarStartTime.ToString(), LibUtil.FormatDisp(bar.Open), LibUtil.FormatDisp(bar.High), LibUtil.FormatDisp(bar.Low), LibUtil.FormatDisp(bar.Close),bar.Volume,bar.OpenInterest);
               
            }
        }
    }

    

}
