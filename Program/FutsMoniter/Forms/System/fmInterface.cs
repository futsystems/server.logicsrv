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

namespace FutsMoniter
{
    public partial class fmInterface : ComponentFactory.Krypton.Toolkit.KryptonForm,IEventBinder
    {
        public fmInterface()
        {
            InitializeComponent();
            SetPreferences();
            InitTable();
            BindToTable();

            this.Load += new EventHandler(fmInterface_Load);
        }

        void fmInterface_Load(object sender, EventArgs e)
        {
            WireEvent();
            Factory.IDataSourceFactory(interfaceType).BindDataSource(UIUtil.genEnumList<QSEnumConnectorType>());
            Globals.TLClient.ReqQryInterface();
        }

        void WireEvent()
        {
            Globals.RegIEventHandler(this);
            interfacegrid.RowPrePaint += new DataGridViewRowPrePaintEventHandler(grid_RowPrePaint);
            interfacegrid.DoubleClick += new EventHandler(interfacegrid_DoubleClick);
            isxapi.CheckedChanged += new EventHandler(isxapi_CheckedChanged);
            btnUpdate.Click += new EventHandler(btnUpdate_Click);
        }

        void btnUpdate_Click(object sender, EventArgs e)
        {
            ConnectorInterface itface = new ConnectorInterface();
            itface.ID = int.Parse(id.Text);
            itface.IsXAPI = isxapi.Checked;
            itface.libname_broker = broker_name.Text;
            itface.libpath_broker = broker_path.Text;
            itface.libname_wrapper = wrapper_name.Text;
            itface.libpath_wrapper = wrapper_path.Text;
            itface.Name = name.Text;
            itface.Type = (QSEnumConnectorType)interfaceType.SelectedValue;
            itface.type_name = typename.Text;

            Globals.TLClient.ReqUpdateInterface(TradingLib.Mixins.Json.JsonMapper.ToJson(itface));
        }

        

        

        public void OnInit()
        {
            Globals.LogicEvent.RegisterCallback("ConnectorManager", "QryInterface", this.OnQryInterface);//查询交易帐户出入金请求
            //Globals.LogicEvent.RegisterCallback("MgrExchServer", "NotifyCashOperation", this.OnNotifyCashOperation);

        }

        public void OnDisposed()
        {
            Globals.LogicEvent.UnRegisterCallback("ConnectorManager", "QryInterface", this.OnQryInterface);//查询交易帐户出入金请求
            //Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "NotifyCashOperation", this.OnNotifyCashOperation);

        }

        void OnQryInterface(string jsonstr, bool islast)
        {
            ConnectorInterface[] objs = MoniterUtils.ParseJsonResponse<ConnectorInterface[]>(jsonstr);
            if (objs != null)
            {
                foreach (ConnectorInterface op in objs)
                {
                    InvokeGotInterface(op);
                }
            }
            else//如果没有配资服
            {

            }
        }

        //得到当前选择的行号
        private ConnectorInterface CurrentInterface
        {
            get
            {
                int row = interfacegrid.SelectedRows.Count > 0 ? interfacegrid.SelectedRows[0].Index : -1;
                if (row >= 0)
                {
                    int id= int.Parse(interfacegrid[0, row].Value.ToString());

                    if (interfacemap.Keys.Contains(id))
                        return interfacemap[id];
                    else
                        return null;
                }
                else
                {
                    return null;
                }
            }
        }

        ConcurrentDictionary<int, ConnectorInterface> interfacemap = new ConcurrentDictionary<int, ConnectorInterface>();
        ConcurrentDictionary<int, int> interfacerowid = new ConcurrentDictionary<int, int>();

        int InterfaceIDx(int interfaceid)
        {
            int rowid = -1;
            if (!interfacerowid.TryGetValue(interfaceid, out rowid))
            {
                return -1;
            }
            else
            {
                return rowid;
            }
        }

        void InvokeGotInterface(ConnectorInterface c)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<ConnectorInterface>(InvokeGotInterface), new object[] { c });
            }
            else
            {
                int r = InterfaceIDx(c.ID);
                if (r == -1)
                {
                    gt.Rows.Add(c.ID);
                    int i = gt.Rows.Count - 1;

                    gt.Rows[i][NAME] = c.Name;
                    gt.Rows[i][ISXAPI] = c.IsXAPI;
                    gt.Rows[i][TYPE] = c.Type;
                    gt.Rows[i][CLASSNAME] = c.type_name;
                    gt.Rows[i][WRAPPERNAME] = c.libname_wrapper;
                    gt.Rows[i][WRAPPERPATH] = c.libpath_wrapper;
                    gt.Rows[i][BROKERNAME] = c.libname_broker;
                    gt.Rows[i][BROKERPATH] = c.libpath_broker;

                    interfacerowid.TryAdd(c.ID, i);
                    interfacemap.TryAdd(c.ID, c);
                }
                else
                {
                    //更新状态
                    //gt.Rows[r][STATUS] = c.Status;
                    //connectormap[c.Token] = c;
                }

            }
        }

        void grid_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            e.PaintParts = e.PaintParts ^ DataGridViewPaintParts.Focus;
        }

        void interfacegrid_DoubleClick(object sender, EventArgs e)
        {
            ConnectorInterface itface = CurrentInterface;
            if (itface != null)
            {
                id.Text = itface.ID.ToString();
                name.Text = itface.Name;
                typename.Text = itface.type_name;
                isxapi.Checked = itface.IsXAPI;
                wrapper_path.Text = itface.libpath_wrapper;
                wrapper_name.Text = itface.libname_wrapper;
                broker_path.Text = itface.libpath_broker;
                broker_name.Text = itface.libname_broker;
            }
        }
        void isxapi_CheckedChanged(object sender, EventArgs e)
        {
            wrapper_name.Enabled = isxapi.Checked;
            wrapper_path.Enabled = isxapi.Checked;
            broker_name.Enabled = isxapi.Checked;
            broker_path.Enabled = isxapi.Checked;
        }

        #region 表格
        #region 显示字段

        const string ID = "全局ID";
        const string NAME = "名称";
        const string ISXAPI = "XAPI";
        const string TYPE = "接口类别";
        const string CLASSNAME = "对象全名";
        const string WRAPPERNAME = "Wrapper文件名";
        const string WRAPPERPATH = "Wrapper目录";
        const string BROKERNAME = "Broker文件名";
        const string BROKERPATH = "Broker目录";


        #endregion

        DataTable gt = new DataTable();
        BindingSource datasource = new BindingSource();

        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = interfacegrid;

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
            gt.Columns.Add(NAME);
            gt.Columns.Add(ISXAPI);//1
            gt.Columns.Add(TYPE);//1
            gt.Columns.Add(CLASSNAME);//1
            gt.Columns.Add(WRAPPERPATH);//1
            gt.Columns.Add(WRAPPERNAME);//1
            gt.Columns.Add(BROKERPATH);//1
            gt.Columns.Add(BROKERNAME);//1

        }

        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = interfacegrid;

            datasource.DataSource = gt;
            grid.DataSource = datasource;

            grid.Columns[ID].Width = 50;
            grid.Columns[NAME].Width = 120;
            grid.Columns[ISXAPI].Width =50;
            grid.Columns[TYPE].Width = 50;
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
