using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FutSystems.GUI;
using TradingLib.API;
using TradingLib.Common;


namespace FutsMoniter
{
    public partial class fmConnectorManager : ComponentFactory.Krypton.Toolkit.KryptonForm, IEventBinder
    {
        public fmConnectorManager()
        {
            InitializeComponent();
            SetPreferences();
            InitTable();
            BindToTable();

            this.Load += new EventHandler(fmConnectorManager_Load);
        }

        void fmConnectorManager_Load(object sender, EventArgs e)
        {
            WireEvent();

            if (Globals.EnvReady)
            {
                Globals.TLClient.ReqQryInterface();
            }
        }

        void WireEvent()
        {
            Globals.RegIEventHandler(this);

            configgrid.DoubleClick += new EventHandler(configgrid_DoubleClick);
            configgrid.RowPrePaint += new DataGridViewRowPrePaintEventHandler(RowPrePaint);
            configgrid.MouseClick += new MouseEventHandler(configgrid_MouseClick);

            //vendorgrid.RowPrePaint += new DataGridViewRowPrePaintEventHandler(RowPrePaint);
            //routeritemgrid.RowPrePaint += new DataGridViewRowPrePaintEventHandler(RowPrePaint);
            //ctRouterGroupList1.RouterGroupSelectedChangedEvent += new TradingLib.API.VoidDelegate(ctRouterGroupList1_RouterGroupSelectedChangedEvent);

            //btnUpdateRouterGroup.Click += new EventHandler(btnUpdateRouterGroup_Click);
            //tabholder.SelectedPageChanged += new EventHandler(tabholder_SelectedPageChanged);
            //ctRouterGroupList1.RouterGroupSelectedChangedEvent +=new TradingLib.API.VoidDelegate(ctRouterGroupList1_RouterGroupSelectedChangedEvent);
        }

        void RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            e.PaintParts = e.PaintParts ^ DataGridViewPaintParts.Focus;
        }

        public void OnInit()
        {
            Globals.LogicEvent.RegisterCallback("ConnectorManager", "QryConnectorConfig", this.OnQryConnectorConfig);//查询交易帐户出入金请求
            //Globals.LogicEvent.RegisterCallback("MgrExchServer", "QryVendor", this.OnQryVendor);
            //Globals.LogicEvent.RegisterCallback("ConnectorManager", "QryRouterItem", this.OnQryRouterItem);
            Globals.LogicEvent.RegisterCallback("ConnectorManager", "QryInterface", this.OnQryInterface);
            Globals.LogicEvent.RegisterNotifyCallback("ConnectorManager", "NotifyConnectorCfg", this.OnNotifyConnectorConfig);
            //Globals.LogicEvent.RegisterNotifyCallback("MgrExchServer", "NotifyVendor", this.OnNotifyVendorBind);
            //Globals.LogicEvent.RegisterNotifyCallback("ConnectorManager", "NotifyVendor", this.OnNotifyVendorBind);
            //Globals.LogicEvent.RegisterNotifyCallback("ConnectorManager", "NotifyRouterItem", this.OnNotifyRouterItem);
            //Globals.LogicEvent.RegisterCallback("ConnectorManager", "NotifyRouterGroup", this.OnNotifyRouterGroup);

            Globals.LogicEvent.RegisterCallback("ConnectorManager", "QryConnectorStatus", this.OnQryConnectorStatus);
            Globals.LogicEvent.RegisterNotifyCallback("ConnectorManager", "NotifyConnectorStatus", this.OnNotifyConnectorStatus);

        }

        public void OnDisposed()
        {
            Globals.LogicEvent.UnRegisterCallback("ConnectorManager", "QryConnectorConfig", this.OnQryConnectorConfig);//查询交易帐户出入金请求
            //Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "QryVendor", this.OnQryVendor);
            //Globals.LogicEvent.UnRegisterCallback("ConnectorManager", "QryRouterItem", this.OnQryRouterItem);
            Globals.LogicEvent.UnRegisterCallback("ConnectorManager", "QryInterface", this.OnQryInterface);
            Globals.LogicEvent.UnRegisterNotifyCallback("ConnectorManager", "NotifyConnectorCfg", this.OnNotifyConnectorConfig);
            //Globals.LogicEvent.UnRegisterNotifyCallback("MgrExchServer", "NotifyVendor", this.OnNotifyVendorBind);
            //Globals.LogicEvent.UnRegisterNotifyCallback("ConnectorManager", "NotifyRouterItem", this.OnNotifyRouterItem);
            //Globals.LogicEvent.UnRegisterNotifyCallback("ConnectorManager", "NotifyVendor", this.OnNotifyVendorBind);

            Globals.LogicEvent.UnRegisterCallback("ConnectorManager", "QryConnectorStatus", this.OnQryConnectorStatus);
            Globals.LogicEvent.UnRegisterNotifyCallback("ConnectorManager", "NotifyConnectorStatus", this.OnNotifyConnectorStatus);

        }

        ConnectorInterface ID2Interface(int id)
        {
            if (interfacemap.Keys.Contains(id))
                return interfacemap[id];
            return null;

        }
        public ArrayList GetInterfaceCBList()
        {
            ArrayList list = new ArrayList();
            foreach (ConnectorInterface item in interfacemap.Values)
            {
                ValueObject<int> vo = new ValueObject<int>();
                vo.Name = item.Name;
                vo.Value = item.ID;
                list.Add(vo);
            }
            return list;
        }


        ConcurrentDictionary<int, ConnectorInterface> interfacemap = new ConcurrentDictionary<int, ConnectorInterface>();
        bool _gotinterface = false;

        void OnQryInterface(string jsonstr, bool islast)
        {
            ConnectorInterface[] objs = MoniterUtils.ParseJsonResponse<ConnectorInterface[]>(jsonstr);
            if (objs != null)
            {
                foreach (ConnectorInterface op in objs)
                {
                    if (!interfacemap.Keys.Contains(op.ID))
                        interfacemap.TryAdd(op.ID, op);
                }
                _gotinterface = true;
                //if (!_gotvendor)
                //{
                //    Globals.TLClient.ReqQryVendor();
                //}
                if (!_gotconnector)
                {
                    Globals.TLClient.ReqQryConnectorConfig();
                }
            }
            else//如果没有配资服
            {

            }
        }







        void OnNotifyConnectorConfig(string jsonstr)
        {
            ConnectorConfig cfg = MoniterUtils.ParseJsonResponse<ConnectorConfig>(jsonstr);
            if (cfg != null)
            {
                InvokeGotConnector(cfg);
            }
            else//如果没有配资服
            {

            }
        }

        bool _gotconnector = false;
        void OnQryConnectorConfig(string jsonstr, bool islast)
        {
            ConnectorConfig[] objs = MoniterUtils.ParseJsonResponse<ConnectorConfig[]>(jsonstr);
            if (objs != null)
            {
                foreach (ConnectorConfig op in objs)
                {
                    InvokeGotConnector(op);
                }
                _gotconnector = true;
                if (!_gotstatus)
                {
                    Globals.TLClient.ReqQryConnectorStatus();
                }
            }
            else//如果没有配资服
            {

            }
        }

        bool _gotstatus = false;
        void OnQryConnectorStatus(string jsonstr, bool islast)
        {
            ConnectorStatus[] objs = MoniterUtils.ParseJsonResponse<ConnectorStatus[]>(jsonstr);
            if (objs != null)
            {
                foreach (ConnectorStatus op in objs)
                {
                    InvokeGotConnectorStatus(op);
                }
                _gotstatus = true;
            }
            else//如果没有配资服
            {

            }
        }

        void OnNotifyConnectorStatus(string jsonstr)
        {
            ConnectorStatus obj = MoniterUtils.ParseJsonResponse<ConnectorStatus>(jsonstr);
            if (obj != null)
            {
                InvokeGotConnectorStatus(obj);
                _gotstatus = true;
            }
            else//如果没有配资服
            {

            }
        }

        //得到当前选择的行号
        private ConnectorConfig CurrentConnectorConfig
        {
            get
            {
                int row = configgrid.SelectedRows.Count > 0 ? configgrid.SelectedRows[0].Index : -1;
                if (row >= 0)
                {
                    int id = int.Parse(configgrid[0, row].Value.ToString());

                    if (connectormap.Keys.Contains(id))
                        return connectormap[id];
                    else
                        return null;
                }
                else
                {
                    return null;
                }
            }
        }

        void configgrid_DoubleClick(object sender, EventArgs e)
        {
            ConnectorConfig cfg = CurrentConnectorConfig;
            if (cfg != null)
            {
                fmConnectorEdit fm = new fmConnectorEdit();
                fm.SetInterfaceCBList(this.GetInterfaceCBList());
                fm.SetConnectorConfig(cfg);
                fm.ShowDialog();
            }
        }


        ConcurrentDictionary<int, ConnectorConfig> connectormap = new ConcurrentDictionary<int, ConnectorConfig>();
        ConcurrentDictionary<int, int> connectorrowid = new ConcurrentDictionary<int, int>();

        int ConnectorIdx(int id)
        {
            int rowid = -1;
            if (!connectorrowid.TryGetValue(id, out rowid))
            {
                return -1;
            }
            else
            {
                return rowid;
            }
        }

        string GetInterfaceTite(int id)
        {
            string title = string.Empty;
            if (id == 0)
            {
                title = "未绑定";
            }
            else
            {
                ConnectorInterface setting = ID2Interface(id);

                if (setting == null)
                {
                    title = "绑定异常";
                }
                else
                {
                    title = string.Format("[{0}]{1}", setting.ID, setting.Name);
                }
            }
            return title;
        }

        //bool GetVendorBindStatus(ConnectorConfig c)
        //{
        //    return false;
        //    //string vtitle = string.Empty;
        //    //if (c.vendor_id == 0)
        //    //{
        //    //    return false;
        //    //}
        //    //else
        //    //{
        //    //    VendorSetting setting = ID2VendorSetting(c.vendor_id);
        //    //    if (setting == null)
        //    //    {
        //    //        return false;
        //    //    }
        //    //    else
        //    //    {
        //    //        return true;
        //    //    }
        //    //}
        //}

        //string GetVendorTitle(ConnectorConfig c)
        //{
        //    return "Vendor";
        //    //string vtitle = string.Empty;
        //    //if (!c.NeedVendor)
        //    //{
        //    //    return "";
        //    //}
        //    //if (c.vendor_id == 0)
        //    //{
        //    //    vtitle = "未绑定";
        //    //}
        //    //else
        //    //{
        //    //    VendorSetting setting = ID2VendorSetting(c.vendor_id);
        //    //    if (setting == null)
        //    //    {
        //    //        vtitle = "绑定异常";
        //    //    }
        //    //    else
        //    //    {
        //    //        vtitle = setting.Name;
        //    //    }
        //    //}
        //    //return vtitle;
        //}

        Image GetStatusImage(QSEnumConnectorStatus status)
        {
            if (status == QSEnumConnectorStatus.Start)
            {
                return (Image)Properties.Resources.online;
            }
            else if (status == QSEnumConnectorStatus.Stop)
            {
                return (Image)Properties.Resources.offline;
            }
            return (Image)Properties.Resources.offline;
        }


        void InvokeGotConnectorStatus(ConnectorStatus status)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<ConnectorStatus>(InvokeGotConnectorStatus), new object[] { status });
            }
            else
            {
                int r = ConnectorIdx(status.ID);
                if (r == -1)
                {
                }
                else
                {
                    gt.Rows[r][CONSTATUS] = status.Status;
                    gt.Rows[r][CONSTATUSIMG] = GetStatusImage(status.Status);
                }
            }
        }
        void InvokeGotConnector(ConnectorConfig c)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<ConnectorConfig>(InvokeGotConnector), new object[] { c });
            }
            else
            {
                int r = ConnectorIdx(c.ID);
                if (r == -1)
                {
                    gt.Rows.Add(c.ID);
                    int i = gt.Rows.Count - 1;

                    gt.Rows[i][TOKEN] = c.Token;
                    gt.Rows[i][NAME] = c.Name;

                    gt.Rows[i][SRVADDRESS] = c.srvinfo_ipaddress;
                    gt.Rows[i][SRVPORT] = c.srvinfo_port;
                    gt.Rows[i][SRV1] = c.srvinfo_field1;
                    gt.Rows[i][SRV2] = c.srvinfo_field2;
                    gt.Rows[i][SRV3] = c.srvinfo_field3;
                    gt.Rows[i][USERID] = c.usrinfo_userid;
                    gt.Rows[i][PASSWORD] = c.usrinfo_password;
                    gt.Rows[i][USR1] = c.usrinfo_field1;
                    gt.Rows[i][USR2] = c.usrinfo_field2;

                    gt.Rows[i][INTERFACE] = GetInterfaceTite(c.interface_fk);

                    //gt.Rows[i][VENDORACCOUNT] = GetVendorTitle(c);
                    //gt.Rows[i][ISBINDED] = GetVendorBindStatus(c);

                    connectorrowid.TryAdd(c.ID, i);
                    connectormap.TryAdd(c.ID, c);
                }
                else
                {
                    //更新状态
                    gt.Rows[r][SRVADDRESS] = c.srvinfo_ipaddress;
                    gt.Rows[r][SRVPORT] = c.srvinfo_port;
                    gt.Rows[r][SRV1] = c.srvinfo_field1;
                    gt.Rows[r][SRV2] = c.srvinfo_field2;
                    gt.Rows[r][SRV3] = c.srvinfo_field3;
                    gt.Rows[r][USERID] = c.usrinfo_userid;
                    gt.Rows[r][PASSWORD] = c.usrinfo_password;
                    gt.Rows[r][USR1] = c.usrinfo_field1;
                    gt.Rows[r][USR2] = c.usrinfo_field2;
                    gt.Rows[r][NAME] = c.Name;
                    //gt.Rows[r][VENDORACCOUNT] = GetVendorTitle(c);
                    //gt.Rows[r][ISBINDED] = GetVendorBindStatus(c);
                    connectormap[c.ID] = c;
                }

            }
        }

        #region 表格
        #region 显示字段

        const string ID = "通道ID";
        const string SRVADDRESS = "服务器地址";
        const string SRVPORT = "端口";
        const string SRV1 = "参数1";
        const string SRV2 = "参数2";
        const string SRV3 = "参数3";
        const string USERID = "交易帐号";
        const string PASSWORD = "密码";
        const string USR1 = "参数1/U";
        const string USR2 = "参数2/U";
        const string INTERFACE = "接口";
        const string TOKEN = "标识";
        const string NAME = "名称";
        //const string VENDORACCOUNT = "实盘帐户";
        const string ISBINDED = "Binded";
        const string CONSTATUS = "status";
        const string CONSTATUSIMG = "状态";
        #endregion

        DataTable gt = new DataTable();
        BindingSource datasource = new BindingSource();

        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = configgrid;

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

            routergridmenu = new ContextMenuStrip();
            routergridmenu.Items.Add("添加主帐户", null, new EventHandler(AddConnector_Click));//0
            routergridmenu.Items.Add("修改主帐户", null, new EventHandler(EditConnector_Click));//1
        }

        ContextMenuStrip routergridmenu = null;


        void configgrid_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                GetConnectorGridRightMenu().Show(Control.MousePosition);
            }
        }

        /// <summary>
        /// 动态生成右键菜单
        /// </summary>
        /// <returns></returns>
        ContextMenuStrip GetConnectorGridRightMenu()
        {
            ConnectorConfig cfg = CurrentConnectorConfig;
            if (cfg == null)
            {
                routergridmenu.Items[0].Visible = true;
                routergridmenu.Items[1].Visible = false;
                //routergridmenu.Items[2].Visible = false;
                //routergridmenu.Items[3].Visible = false;
                //routergridmenu.Items[4].Visible = false;

                return routergridmenu;
            }
            //int r = ConnectorIdx(cfg.ID);
            ////根据当天通道状态 选择性显示启动或者停止
            //QSEnumConnectorStatus status = (QSEnumConnectorStatus)Enum.Parse(typeof(QSEnumConnectorStatus), gt.Rows[r][CONSTATUS].ToString());
            //switch (status)
            //{
            //    case QSEnumConnectorStatus.Start:
            //        {
            //            routergridmenu.Items[3].Enabled = false;
            //            routergridmenu.Items[4].Enabled = true;
            //            break;
            //        }
            //    case QSEnumConnectorStatus.Stop:
            //        {
            //            routergridmenu.Items[3].Enabled = true;
            //            routergridmenu.Items[4].Enabled = false;
            //            break;
            //        }
            //    default:
            //        {
            //            routergridmenu.Items[3].Enabled = false;
            //            routergridmenu.Items[4].Enabled = false;
            //            break;
            //        }
            //}
            ////int r = ConnectorIdx(cfg.ID);
            ////if (r >= 0)
            ////{
            ////    bool isvendorbinded = false;
            ////    //需要绑定Vendor
            ////    if (cfg.NeedVendor)
            ////    {
            ////        isvendorbinded = bool.Parse(gt.Rows[r][ISBINDED].ToString());
            ////        routergridmenu.Items[3].Visible = true;
            ////        routergridmenu.Items[4].Visible = true;
            ////        if (isvendorbinded)
            ////        {
            ////            routergridmenu.Items[3].Enabled = false;
            ////            routergridmenu.Items[4].Enabled = true;
            ////        }
            ////        else
            ////        {
            ////            routergridmenu.Items[3].Enabled = true;
            ////            routergridmenu.Items[4].Enabled = false;
            ////        }

            ////    }
            ////    else
            ////    {
            ////        routergridmenu.Items[3].Visible = false;
            ////        routergridmenu.Items[4].Visible = false;
            ////    }

            ////    //如果不需要绑定Vendor或则已经绑定了Vendor
            ////    if (isvendorbinded || !cfg.NeedVendor)
            ////    {
            ////        routergridmenu.Items[6].Enabled = true;
            ////        routergridmenu.Items[7].Enabled = true;
            ////        //根据当天通道状态 选择性显示启动或者停止
            ////        QSEnumConnectorStatus status = (QSEnumConnectorStatus)Enum.Parse(typeof(QSEnumConnectorStatus), gt.Rows[r][CONSTATUS].ToString());
            ////        switch (status)
            ////        {
            ////            case QSEnumConnectorStatus.Start:
            ////                {
            ////                    routergridmenu.Items[6].Enabled = false;
            ////                    break;
            ////                }
            ////            case QSEnumConnectorStatus.Stop:
            ////                {
            ////                    routergridmenu.Items[7].Enabled = false;
            ////                    break;
            ////                }
            ////            default:
            ////                {
            ////                    routergridmenu.Items[6].Enabled = false;
            ////                    routergridmenu.Items[7].Enabled = false;
            ////                    break;
            ////                }
            ////        }
            ////    }
            ////    else
            ////    {   //如果通道没有绑定 则启动停止不可用
            ////        routergridmenu.Items[6].Enabled = false;
            ////        routergridmenu.Items[7].Enabled = false;
            ////    }
            ////}


            return routergridmenu;
        }

        //初始化Account显示空格
        private void InitTable()
        {
            gt.Columns.Add(ID);//0
            gt.Columns.Add(TOKEN);//1
            gt.Columns.Add(NAME);//1

            gt.Columns.Add(SRVADDRESS);
            gt.Columns.Add(SRVPORT);//1
            gt.Columns.Add(SRV1);//1
            gt.Columns.Add(SRV2);//1
            gt.Columns.Add(SRV3);//1
            gt.Columns.Add(USERID);//1
            gt.Columns.Add(PASSWORD);//1
            gt.Columns.Add(USR1);//1
            gt.Columns.Add(USR2);//1

            gt.Columns.Add(INTERFACE);//1
            //gt.Columns.Add(VENDORACCOUNT);
            gt.Columns.Add(ISBINDED);
            gt.Columns.Add(CONSTATUS);
            gt.Columns.Add(CONSTATUSIMG, typeof(Image));

        }

        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = configgrid;

            datasource.DataSource = gt;
            grid.DataSource = datasource;
            grid.Columns[SRV1].Visible = false;
            grid.Columns[SRV2].Visible = false;
            grid.Columns[SRV3].Visible = false;
            grid.Columns[PASSWORD].Visible = false;
            grid.Columns[USR1].Visible = false;
            grid.Columns[USR2].Visible = false;
            grid.Columns[SRVADDRESS].Visible = false;
            grid.Columns[SRVPORT].Visible = false;
            grid.Columns[ISBINDED].Visible = false;
            grid.Columns[CONSTATUS].Visible = false;

            grid.Columns[ID].Width = 50;

            grid.Columns[TOKEN].Width = 100;
            grid.Columns[NAME].Width = 130;
            grid.Columns[USERID].Width = 100;
            grid.Columns[INTERFACE].Width = 150;
            grid.Columns[CONSTATUSIMG].Width = 40;
        }






        #endregion

        /// <summary>
        /// 添加通道
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AddConnector_Click(object sender, EventArgs e)
        {
            fmConnectorEdit fm = new fmConnectorEdit();
            fm.SetInterfaceCBList(this.GetInterfaceCBList());
            fm.ShowDialog();
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EditConnector_Click(object sender, EventArgs e)
        {
            ConnectorConfig cfg = CurrentConnectorConfig;
            if (cfg != null)
            {
                fmConnectorEdit fm = new fmConnectorEdit();
                fm.SetInterfaceCBList(this.GetInterfaceCBList());
                fm.SetConnectorConfig(cfg);
                fm.ShowDialog();
            }
            else
            {
                ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("请选择需要编辑的通道");
            }
        }

        ///// <summary>
        ///// 添加通道
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //void BindConnector_Click(object sender, EventArgs e)
        //{
        //    //ConnectorConfig cfg = CurrentConnectorConfig;
        //    //if (cfg == null)
        //    //{
        //    //    ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("请选择通道");
        //    //}
        //    //ArrayList list = GetVendorCBList();
        //    //if (list.Count == 0)
        //    //{
        //    //    ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("没有未绑定的帐户");
        //    //}
        //    //else
        //    //{
        //    //    fmVendorSelect fm = new fmVendorSelect();
        //    //    fm.SetConnectorConfig(cfg);


        //    //    fm.SetVendorCBList(list);
        //    //    fm.Show();
        //    //}
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //void UnBindConnector_Click(object sender, EventArgs e)
        //{
        //    ConnectorConfig cfg = CurrentConnectorConfig;
        //    if (cfg == null)
        //    {
        //        ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("请选择通道");
        //    }
        //    if (fmConfirm.Show(string.Format("确认解绑:{0}", cfg.Token)) == System.Windows.Forms.DialogResult.Yes)
        //    {
        //        Globals.TLClient.ReqUnBindVendor(cfg.ID);
        //    }
        //}

        ///// <summary>
        ///// 启动通道
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //void StartConnector_Click(object sender, EventArgs e)
        //{
        //    ConnectorConfig cfg = CurrentConnectorConfig;
        //    if (cfg == null)
        //    {
        //        ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("请选择通道");
        //    }

        //    if (fmConfirm.Show(string.Format("确认启动通道:{0}", cfg.Token)) == System.Windows.Forms.DialogResult.Yes)
        //    {
        //        Globals.TLClient.ReqStartConnector(cfg.ID);
        //    }
        //}

        ///// <summary>
        ///// 停止通道
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //void StopConnector_Click(object sender, EventArgs e)
        //{
        //    ConnectorConfig cfg = CurrentConnectorConfig;
        //    if (cfg == null)
        //    {
        //        ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("请选择通道");
        //    }

        //    if (fmConfirm.Show(string.Format("确认停止通道:{0}", cfg.Token)) == System.Windows.Forms.DialogResult.Yes)
        //    {
        //        Globals.TLClient.ReqStopConnector(cfg.ID);
        //    }
        //}
    }
}
