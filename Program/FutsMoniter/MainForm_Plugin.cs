using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FutsMoniter.Common;
using TradingLib.MoniterControl;
using System.Windows.Forms;

namespace FutsMoniter
{
    partial class MainForm
    {


        void LoadMonitorControl()
        {
            //MessageBox.Show("x");
            foreach (Type type in MoniterPlugin.GetMoniterControlTypes())
            {
                
                MoniterControlAttr attr = MoniterPlugin.GetMoniterControlAttr(type);
                if (attr != null)
                {
                    //MessageBox.Show("type:" + type.ToString());
                    Globals.Debug("type:" + type.Name + " 已经标注过");
                    MonitorControl control = MoniterPlugin.CreateMoniterControl(type.FullName);
                    MonitorControlHelper.RegisterControl(control);
                    control.SetClient(Globals.TLClient);

                    AddWorkspacePage(control);
                }
            }
        }
    }
}
