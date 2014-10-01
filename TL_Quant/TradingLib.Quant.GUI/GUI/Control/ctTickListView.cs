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
    public partial class ctTickListView : UserControl
    {
        public ctTickListView()
        {
            InitializeComponent();
        }

        public void ShowTicks(IList<Tick> list)
        { 
            foreach(Tick k in list)
            {
                GotTick(k);
            }
        
        }
        string f = "{0:F2}";
        public void GotTick(Tick k)
        {
            if (InvokeRequired)
                Invoke(new TickDelegate(GotTick), new object[] { k });
            else
            {
                gridView.Rows.Add(Util.ToDateTime(k.date,k.time).ToString(), LibUtil.FormatDisp(k.trade,f),k.size.ToString(), LibUtil.FormatDisp(k.ask,f),k.os.ToString(), LibUtil.FormatDisp(k.bid,f),k.bs.ToString());

            }
        }
    }
}
