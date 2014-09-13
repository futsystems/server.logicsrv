using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.Common;
using TradingLib.Quant;
using TradingLib.Quant.Base;
using TradingLib.Quant.Plugin;

namespace TradingLib.Quant.GUI
{
    public partial class ctIndicatorList : UserControl
    {
        public event IndicatorPluginDel EventPlotIndicator;

        public ctIndicatorList()
        {
            InitializeComponent();
            InitContextMenu();
            InitIndicatorList();
        }

        void InitContextMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Items.Add("绘制指标", null, new EventHandler(PlotIndicator));
            menu.Items.Add("指标信息", null, new EventHandler(ShowIndicatorInfo));
            treeView1.ContextMenuStrip = menu;
        }
        void ShowIndicatorInfo(object sender, EventArgs e)
        {
            //MessageBox.Show(((IIndicatorPlugin)treeView1.SelectedNode.Tag).GetName());
            fmIndicatorInfo fm = new fmIndicatorInfo();
            fm.ViewIndicator(((IIndicatorPlugin)treeView1.SelectedNode.Tag));
            fm.ShowDialog();

        }
        /// <summary>
        /// 绘制指标到图表显示
        /// </summary>
        void PlotIndicator(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode.Level != 1) return;
            if (EventPlotIndicator != null)
                EventPlotIndicator((IIndicatorPlugin)treeView1.SelectedNode.Tag);
        
        }


        Dictionary<string, IIndicatorPlugin> indicatormap = new Dictionary<string, IIndicatorPlugin>();
        void InitIndicatorList()
        {

            IList<IIndicatorPlugin> indicatorlist = PluginHelper.LoadIndicatorList();
            //MessageBox.Show("ind num:"+indicatorlist.Count.ToString());
            foreach (IIndicatorPlugin plugin in indicatorlist)
            {

                indicatormap.Add(plugin.id(),plugin);
                appendIntoList(plugin);
            }
        }

        void appendIntoList(IIndicatorPlugin plugin)
        { 
            TreeNode node = new TreeNode(plugin.GetName());
            node.Tag = plugin;
            string groupname = plugin.GetGroupName();
            switch (groupname)
            { 
                case "Trend":
                    treeView1.Nodes["Trend"].Nodes.Add(node);
                    return;
                case "Momentum":
                    treeView1.Nodes["Momentum"].Nodes.Add(node);
                    return;
                case "Volume":
                    treeView1.Nodes["Volume"].Nodes.Add(node);
                    return;
                case "Volatility":
                    treeView1.Nodes["Volatility"].Nodes.Add(node);
                    return;
                case "Other":
                    treeView1.Nodes["Other"].Nodes.Add(node);
                    return;
                default :
                    treeView1.Nodes["Other"].Nodes.Add(node);
                    return;

            }
        }
    }
}
