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
            Globals.RegIEventHandler(this);
            Globals.TLClient.ReqQryInterface();
        }

        public void OnInit()
        {
            Globals.CallBackCentre.RegisterCallback("ConnectorManager", "QryAccountCashOperationTotal", this.OnQryInterface);//查询交易帐户出入金请求
            //Globals.CallBackCentre.RegisterCallback("MgrExchServer", "NotifyCashOperation", this.OnNotifyCashOperation);

        }

        public void OnDisposed()
        {
            Globals.CallBackCentre.UnRegisterCallback("ConnectorManager", "QryAccountCashOperationTotal", this.OnQryInterface);//查询交易帐户出入金请求
            //Globals.CallBackCentre.UnRegisterCallback("MgrExchServer", "NotifyCashOperation", this.OnNotifyCashOperation);

        }

        void OnQryInterface(string jsonstr)
        {
            JsonData jd =  TradingLib.Mixins.JsonReply.ParseJsonReplyData(jsonstr);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                ConnectorInterface[] objs =TradingLib.Mixins.JsonReply.ParsePlayload<ConnectorInterface[]>(jd["Playload"].ToJson());
                foreach (ConnectorInterface op in objs)
                {
                    //ctCashOperationAccount.GotJsonWrapperCashOperation(op);
                    InvokeGotInterface(op);
                }
            }
            else//如果没有配资服
            {

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
