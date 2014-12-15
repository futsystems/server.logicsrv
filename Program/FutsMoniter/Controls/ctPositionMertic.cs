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


namespace FutsMoniter
{
    public partial class ctPositionMertic : UserControl,IEventBinder
    {
        VendorSetting _vendor;
        public VendorSetting Vendor { get { return _vendor; } }
        public void SetVendor(VendorSetting vendor)
        {
            _vendor = vendor;
        }


        public void OnInit()
        {
            Globals.TLClient.ReqRegBrokerPM(_vendor.ID);
        }

        public void OnDisposed()
        {
            Globals.TLClient.ReqUnregBrokerPM(_vendor.ID);
        }

        public ctPositionMertic()
        {
            InitializeComponent();
            SetPreferences();
            InitTable();
            BindToTable();

            this.Load += new EventHandler(ctPositionMertic_Load);
        }

        void ctPositionMertic_Load(object sender, EventArgs e)
        {
            Globals.RegIEventHandler(this);
        }

        int PositionMerticIdx(string sym)
        {
            int rowid = -1;
            if (rowmap.TryGetValue(sym, out rowid))
            {
                return rowid;
            }
            return -1;
        }
        Dictionary<string, PositionMetricImpl> pmmap = new Dictionary<string, PositionMetricImpl>();
        Dictionary<string,int> rowmap = new Dictionary<string,int>();

        public void GotPositionMetric(PositionMetricImpl pm)
        {
            OnPositionMetric(pm);
        }
        void OnPositionMetric(PositionMetricImpl pm)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<PositionMetricImpl>(OnPositionMetric), new object[] { pm });
            }
            else
            { 
                int r = PositionMerticIdx(pm.Symbol);
                if (r == -1)
                {
                    gt.Rows.Add(pm.Symbol);
                    int i = gt.Rows.Count - 1;

                    gt.Rows[i][LONGHOLDSIZE] = pm.LongHoldSize;
                    gt.Rows[i][LONGPENTRY] = pm.LongPendingEntrySize;
                    gt.Rows[i][LONGEXIT] = pm.LongPendingExitSize;
                    gt.Rows[i][LONGCANEXIT] = pm.LongCanExitSize;
                    gt.Rows[i][SHORTHOLDSIZE] = pm.ShortHoldSize;
                    gt.Rows[i][SHORTENTRY] = pm.ShortPendingEntrySize;
                    gt.Rows[i][SHORTEXIT] = pm.ShortPendingExitSize;
                    gt.Rows[i][SHORTCANEXIT] = pm.ShortCanExitSaize;

                    rowmap.Add(pm.Symbol, i);
                    pmmap.Add(pm.Symbol, pm);


                }
                else
                {
                    int i = r;
                    gt.Rows[i][LONGHOLDSIZE] = pm.LongHoldSize;
                    gt.Rows[i][LONGPENTRY] = pm.LongPendingEntrySize;
                    gt.Rows[i][LONGEXIT] = pm.LongPendingExitSize;
                    gt.Rows[i][LONGCANEXIT] = pm.LongCanExitSize;
                    gt.Rows[i][SHORTHOLDSIZE] = pm.ShortHoldSize;
                    gt.Rows[i][SHORTENTRY] = pm.ShortPendingEntrySize;
                    gt.Rows[i][SHORTEXIT] = pm.ShortPendingExitSize;
                    gt.Rows[i][SHORTCANEXIT] = pm.ShortCanExitSaize;

                    pmmap[pm.Symbol] = pm;
                
                }
                
            }
        }

        #region 表格
        #region 显示字段

        const string SYMBOL = "合约";
        const string LONGHOLDSIZE = "多头持仓";
        const string LONGPENTRY = "多头待开";
        const string LONGEXIT = "多头待平";
        const string LONGCANEXIT = "多头可平";
        const string SHORTHOLDSIZE = "空头持仓";
        const string SHORTENTRY = "空头待开";
        const string SHORTEXIT = "空头待平";
        const string SHORTCANEXIT = "空头可平";
      
        #endregion

        DataTable gt = new DataTable();
        BindingSource datasource = new BindingSource();

        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = pmgrid;

            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ColumnHeadersHeight = 25;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ReadOnly = true;
            grid.RowHeadersVisible = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke;

            grid.StateCommon.Background.Color1 = Color.WhiteSmoke;
            grid.StateCommon.Background.Color2 = Color.WhiteSmoke;

            //grid.ContextMenuStrip = new ContextMenuStrip();

            //routergridmenu = new ContextMenuStrip();
            //routergridmenu.Items.Add("添加通道", null, new EventHandler(AddConnector_Click));//0
            //routergridmenu.Items.Add("修改通道", null, new EventHandler(EditConnector_Click));//1
            //routergridmenu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            //routergridmenu.Items.Add("绑定通道", null, new EventHandler(BindConnector_Click));//3
            //routergridmenu.Items.Add("解绑通道", null, new EventHandler(UnBindConnector_Click));//4
            //routergridmenu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            //routergridmenu.Items.Add("启动通道", null, new EventHandler(StartConnector_Click));//6
            //routergridmenu.Items.Add("停止通道", null, new EventHandler(StopConnector_Click));//7

        }

        ContextMenuStrip routergridmenu = null;


        void configgrid_MouseClick(object sender, MouseEventArgs e)
        {
            //throw new NotImplementedException();
            //if (e.Button == System.Windows.Forms.MouseButtons.Right)
            //{
            //    GetConnectorGridRightMenu().Show(Control.MousePosition);
            //}
        }


        //ContextMenuStrip GetConnectorGridRightMenu()
        //{
        //    ConnectorConfig cfg = CurrentConnectorConfig;
        //    if (cfg == null)
        //    {
        //        routergridmenu.Items[0].Visible = true;
        //        routergridmenu.Items[1].Visible = false;
        //        routergridmenu.Items[3].Visible = false;
        //        routergridmenu.Items[4].Visible = false;
        //        routergridmenu.Items[6].Visible = false;
        //        routergridmenu.Items[7].Visible = false;
        //        return routergridmenu;
        //    }
        //    int r = ConnectorIdx(cfg.ID);
        //    if (r >= 0)
        //    {
        //        bool isvendorbinded = false;
        //        //需要绑定Vendor
        //        if (cfg.NeedVendor)
        //        {
        //            isvendorbinded = bool.Parse(gt.Rows[r][ISBINDED].ToString());
        //            routergridmenu.Items[3].Visible = true;
        //            routergridmenu.Items[4].Visible = true;
        //            if (isvendorbinded)
        //            {
        //                routergridmenu.Items[3].Enabled = false;
        //                routergridmenu.Items[4].Enabled = true;
        //            }
        //            else
        //            {
        //                routergridmenu.Items[3].Enabled = true;
        //                routergridmenu.Items[4].Enabled = false;
        //            }

        //        }
        //        else
        //        {
        //            routergridmenu.Items[3].Visible = false;
        //            routergridmenu.Items[4].Visible = false;
        //        }

        //        //如果不需要绑定Vendor或则已经绑定了Vendor
        //        if (isvendorbinded || !cfg.NeedVendor)
        //        {
        //            routergridmenu.Items[6].Enabled = true;
        //            routergridmenu.Items[7].Enabled = true;
        //            //根据当天通道状态 选择性显示启动或者停止
        //            QSEnumConnectorStatus status = (QSEnumConnectorStatus)Enum.Parse(typeof(QSEnumConnectorStatus), gt.Rows[r][CONSTATUS].ToString());
        //            switch (status)
        //            {
        //                case QSEnumConnectorStatus.Start:
        //                    {
        //                        routergridmenu.Items[6].Enabled = false;
        //                        break;
        //                    }
        //                case QSEnumConnectorStatus.Stop:
        //                    {
        //                        routergridmenu.Items[7].Enabled = false;
        //                        break;
        //                    }
        //                default:
        //                    {
        //                        routergridmenu.Items[6].Enabled = false;
        //                        routergridmenu.Items[7].Enabled = false;
        //                        break;
        //                    }
        //            }
        //        }
        //        else
        //        {   //如果通道没有绑定 则启动停止不可用
        //            routergridmenu.Items[6].Enabled = false;
        //            routergridmenu.Items[7].Enabled = false;
        //        }
        //    }


        //    return routergridmenu;
        //}

        //初始化Account显示空格
        private void InitTable()
        {
            gt.Columns.Add(SYMBOL);//0
            gt.Columns.Add(LONGHOLDSIZE);//1
            gt.Columns.Add(LONGPENTRY);//1
            gt.Columns.Add(LONGEXIT);
            gt.Columns.Add(LONGCANEXIT);
            gt.Columns.Add(SHORTHOLDSIZE);//1
            gt.Columns.Add(SHORTENTRY);//1
            gt.Columns.Add(SHORTEXIT);//1
            gt.Columns.Add(SHORTCANEXIT);

        }

        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = pmgrid;

            datasource.DataSource = gt;
            grid.DataSource = datasource;

            //grid.Columns[SYMBOL].Width = 80;

        }






        #endregion


    }
}
