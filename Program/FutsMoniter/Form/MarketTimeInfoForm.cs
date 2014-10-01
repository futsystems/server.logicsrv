using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using TradingLib.API;
using TradingLib.Common;

namespace FutsMoniter
{
    public partial class MarketTimeInfoForm : Telerik.WinControls.UI.RadForm
    {
        public MarketTimeInfoForm()
        {
            InitializeComponent();
        }

        public void SetMarketTime(MarketTime mt)
        {
            lbMTName.Text = mt.Name;
            lbDesp.Text = mt.Description;

            StringBuilder sb = new StringBuilder();

            foreach (MktTimeEntry m in mt.MktTimeEntries)
            {
                sb.Append("开始"+GetTimeStr(m.StartTime) + " - " +"结束"+ GetTimeStr(m.EndTime)+(m.NeedFlat?"(强平)":"") + System.Environment.NewLine);    
            }
            sesslist.Text = sb.ToString();
        }

        string GetTimeStr(int time)
        {
            int day = Util.ToTLDate();
            DateTime t = Util.ToDateTime(day,time);
            return t.ToString("HH:mm:ss");
        }
    }
}
