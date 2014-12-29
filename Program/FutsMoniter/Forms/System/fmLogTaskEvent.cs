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
using TradingLib.Contirb.Protocol;


namespace FutsMoniter
{
    public partial class fmLogTaskEvent : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public fmLogTaskEvent()
        {
            InitializeComponent();
        }

        public void SetLogTaskEvent(LogTaskEvent log)
        {
            settleday.Text = log.Settleday.ToString();
            taskname.Text = log.TaskName;
            tasktype.Text = Util.GetEnumDescription(log.TaskType);
            date.Text = log.Date.ToString();
            time.Text = log.Time.ToString();
            taskmemo.Text = log.TaskMemo;
        }

    }
}
