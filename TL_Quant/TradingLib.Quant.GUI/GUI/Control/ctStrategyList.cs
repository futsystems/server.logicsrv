using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Quant.Base;
using TradingLib.Quant.Plugin;
using TradingLib.Quant.Loader;
using System.Threading;


namespace TradingLib.Quant.GUI
{
    public partial class ctStrategyList : UserControl
    {
        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }
        StrategyLoader _loader;
        List<StrategyInfo> _avabileStrategylist;
        public ctStrategyList()
        {
            InitializeComponent();
            this.Load += new EventHandler(ctStrategyList_Load);
        }

        void InitMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Items.Add("添加策略", null, new EventHandler(AddStrategySetup));
            //menu.Items.Add("指标信息", null, new EventHandler(ShowIndicatorInfo));
            this.strategylist.ContextMenuStrip = menu;

        }
        void AddStrategySetup(object sender, EventArgs e)
        {
            if (strategylist.SelectedIndex < 0)
                return;
            else
            {
                StrategyInfo sinfo = _avabileStrategylist[strategylist.SelectedIndex];
                fmAddNewStrategy fm = new fmAddNewStrategy();
                fm.StrategyInfo = sinfo;
                fm.ShowDialog();
            }

        }




        void ctStrategyList_Load(object sender, EventArgs e)
        {
            try
            {
                _loader = new StrategyLoader(SynchronizationContext.Current);
                ReloadStrategyList();
                InitMenu();
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
            }
        }

        void ReloadStrategyList()
        {
            try
            {
                _loader.RefreshStrategys();
                //MessageBox.Show("Totaly Find :" + _loader.GetAvabileStrategies().Count().ToString() + " Strategys");
                strategylist.Items.Clear();
                _avabileStrategylist = _loader.GetAvabileStrategies();
                foreach (StrategyInfo s in _avabileStrategylist)
                {
                    strategylist.Items.Add(s.StrategyClassName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void reload_Click(object sender, EventArgs e)
        {
            ReloadStrategyList();
        }
    }
}
