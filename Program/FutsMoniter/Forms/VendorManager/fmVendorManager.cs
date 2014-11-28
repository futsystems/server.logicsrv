using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Common;
using FutSystems.GUI;
using TradingLib.Mixins;
using TradingLib.Mixins.LitJson;

namespace FutsMoniter
{
    public partial class fmVendorManager : ComponentFactory.Krypton.Toolkit.KryptonForm,IEventBinder
    {
        public fmVendorManager()
        {
            InitializeComponent();
            SetPreferences();
            InitTable();
            BindToTable();

            SetPreferences_Vendor();
            InitTable_Vendor();
            BindToTable_Vendor();

            SetPreferences_RouterItem();
            InitTable_RouterItem();
            BindToTable_RouterItem();

            Factory.IDataSourceFactory(cbrgstrategytype).BindDataSource(UIUtil.genEnumList<QSEnumRouterStrategy>());
            this.Load += new EventHandler(fmConnectorCfg_Load);
        }

        void fmConnectorCfg_Load(object sender, EventArgs e)
        {
            WireEvent();

            
            Globals.TLClient.ReqQryVendor();
        }

        void WireEvent()
        {
            Globals.RegIEventHandler(this);
            configgrid.DoubleClick +=new EventHandler(configgrid_DoubleClick);
            configgrid.RowPrePaint +=new DataGridViewRowPrePaintEventHandler(RowPrePaint);

            vendorgrid.RowPrePaint += new DataGridViewRowPrePaintEventHandler(RowPrePaint);
            routeritemgrid.RowPrePaint +=new DataGridViewRowPrePaintEventHandler(RowPrePaint);
            ctRouterGroupList1.RouterGroupSelectedChangedEvent += new TradingLib.API.VoidDelegate(ctRouterGroupList1_RouterGroupSelectedChangedEvent);
        
            //ctRouterGroupList1.RouterGroupSelectedChangedEvent +=new TradingLib.API.VoidDelegate(ctRouterGroupList1_RouterGroupSelectedChangedEvent);
        }




        public void OnInit()
        {
            Globals.CallBackCentre.RegisterCallback("ConnectorManager", "QryConnectorConfig", this.OnQryConnectorConfig);//查询交易帐户出入金请求
            Globals.CallBackCentre.RegisterCallback("MgrExchServer", "QryVendor", this.OnQryVendor);
            Globals.CallBackCentre.RegisterCallback("ConnectorManager", "QryRouterItem", this.OnQryRouterItem);

        }

        public void OnDisposed()
        {
            Globals.CallBackCentre.UnRegisterCallback("ConnectorManager", "QryConnectorConfig", this.OnQryConnectorConfig);//查询交易帐户出入金请求
            Globals.CallBackCentre.UnRegisterCallback("MgrExchServer", "QryVendor", this.OnQryVendor);
            Globals.CallBackCentre.UnRegisterCallback("ConnectorManager", "QryRouterItem", this.OnQryRouterItem);

        }

        bool _gotconnector = false;
        void OnQryConnectorConfig(string jsonstr)
        {
            JsonData jd = TradingLib.Mixins.JsonReply.ParseJsonReplyData(jsonstr);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                ConnectorConfig[] objs = TradingLib.Mixins.JsonReply.ParsePlayload<ConnectorConfig[]>(jd);
                foreach (ConnectorConfig op in objs)
                {
                    InvokeGotConnector(op);
                }
                _gotconnector = true;
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


        void RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            e.PaintParts = e.PaintParts ^ DataGridViewPaintParts.Focus;
        }

        void configgrid_DoubleClick(object sender, EventArgs e)
        {
            ConnectorConfig cfg = CurrentConnectorConfig;
            if (cfg != null)
            {
                fmConnectorEdit fm = new fmConnectorEdit();
                fm.SetConnectorConfig(cfg);
                fm.Show();
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

                    gt.Rows[i][SRVADDRESS] =c.srvinfo_ipaddress;
                    gt.Rows[i][SRVPORT] = c.srvinfo_port;
                    gt.Rows[i][SRV1] = c.srvinfo_field1;
                    gt.Rows[i][SRV2] = c.srvinfo_field2;
                    gt.Rows[i][SRV3] = c.srvinfo_field3;
                    gt.Rows[i][USERID] = c.usrinfo_userid;
                    gt.Rows[i][PASSWORD] = c.usrinfo_password;
                    gt.Rows[i][USR1] = c.usrinfo_field1;
                    gt.Rows[i][USR2] = c.usrinfo_field2;
                    gt.Rows[i][INTERFACE] = c.interface_fk;
                    string vtitle=string.Empty;
                    if(c.vendor_id==0)
                    {
                        vtitle = "未绑定";
                    }
                    else
                    {
                        VendorSetting setting = ID2VendorSetting(c.vendor_id);
                        if (setting == null)
                        {
                            vtitle = "绑定异常";
                        }
                        else
                        {
                            vtitle = setting.Name;
                        }
                    }
                    gt.Rows[i][VENDORACCOUNT] = vtitle;

                    connectorrowid.TryAdd(c.ID, i);
                    connectormap.TryAdd(c.ID, c);
                }
                else
                {
                    //更新状态
                    //gt.Rows[r][STATUS] = c.Status;
                    //connectormap[c.Token] = c;
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
        const string USERID = "用户名";
        const string PASSWORD = "密码";
        const string USR1 = "参数1/U";
        const string USR2 = "参数2/U";
        const string INTERFACE = "接口";
        const string TOKEN = "标识";
        const string NAME = "名称";
        const string VENDORACCOUNT = "帐户";
        const string ISBINDED = "Binded";

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
            gt.Columns.Add(VENDORACCOUNT);
            gt.Columns.Add(ISBINDED);
            

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


            //grid.Columns[ID].Width = 50;
            //grid.Columns[NAME].Width = 120;
            //grid.Columns[ISXAPI].Width = 50;
            //grid.Columns[TYPE].Width = 50;
            /*
            datasource.Sort = ACCOUNT + " ASC";
            

            accountgrid.Columns[EXECUTE].IsVisible = false;
            accountgrid.Columns[ROUTE].IsVisible = false;
            accountgrid.Columns[LOGINSTATUS].IsVisible = false;

            accountgrid.Columns[ACCOUNT].Width = 60;
            accountgrid.Columns[ROUTEIMG].Width = 20;
            accountgrid.Columns[EXECUTEIMG].Width = 20;
            accountgrid.Columns[PROFITLOSSIMG].Width = 20;
            accountgrid.Columns[LOGINSTATUSIMG].Width = 20;
            accountgrid.Columns[ADDRESS].Width = 120;**/
        }





        #endregion

    }
}
