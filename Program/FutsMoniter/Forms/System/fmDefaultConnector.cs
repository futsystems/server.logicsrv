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
using TradingLib.API;
using TradingLib.Common;
using FutSystems.GUI;
using TradingLib.Mixins;
using TradingLib.Mixins.LitJson;

namespace FutsMoniter
{
    public partial class fmDefaultConnector : ComponentFactory.Krypton.Toolkit.KryptonForm,IEventBinder
    {
        public fmDefaultConnector()
        {
            InitializeComponent();
            SetPreferences();
            InitTable();
            BindToTable();

            this.Load += new EventHandler(fmDefaultConnector_Load);
        }

        void fmDefaultConnector_Load(object sender, EventArgs e)
        {
            Globals.RegIEventHandler(this);
            Globals.TLClient.ReqQryDefaultConnectorConfig();
            routergrid.RowPrePaint += new DataGridViewRowPrePaintEventHandler(routergrid_RowPrePaint);
            routergrid.MouseClick += new MouseEventHandler(routergrid_MouseClick);
        }

        void routergrid_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                GetConnectorGridRightMenu().Show(Control.MousePosition);
            }
        }

        void routergrid_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            e.PaintParts = e.PaintParts ^ DataGridViewPaintParts.Focus;
        }

        public void OnInit()
        {
            Globals.LogicEvent.RegisterCallback("ConnectorManager", "QryDefaultConnectorConfig", this.OnQryConnectorConfig);//查询交易帐户出入金请求
            Globals.LogicEvent.RegisterCallback("ConnectorManager", "QryDefaultConnectorStatus", this.OnQryConnectorStatus);
            Globals.LogicEvent.RegisterCallback("ConnectorManager", "NotifyConnectorStatus", this.OnNotifyConnectorStatus);

        }

        public void OnDisposed()
        {
            Globals.LogicEvent.UnRegisterCallback("ConnectorManager", "QryDefaultConnectorConfig", this.OnQryConnectorConfig);//查询交易帐户出入金请求
            Globals.LogicEvent.UnRegisterCallback("ConnectorManager", "QryDefaultConnectorStatus", this.OnQryConnectorStatus);
            Globals.LogicEvent.UnRegisterCallback("ConnectorManager", "NotifyConnectorStatus", this.OnNotifyConnectorStatus);

            
        }

        void OnNotifyConnectorConfig(string jsonstr)
        {
            ConnectorConfig objs = MoniterUtils.ParseJsonResponse<ConnectorConfig>(jsonstr);
            if (objs != null)
            {
                InvokeGotConnector(objs);
            }
            else//如果没有配资服
            {

            }
        }

        bool _gotconnector = false;
        void OnQryConnectorConfig(string jsonstr)
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
                    Globals.TLClient.ReqQryDefaultConnectorStatus();
                }
            }
            else//如果没有配资服
            {

            }
        }

        bool _gotstatus = false;
        void OnQryConnectorStatus(string jsonstr)
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
                int row = routergrid.SelectedRows.Count > 0 ? routergrid.SelectedRows[0].Index : -1;
                if (row >= 0)
                {
                    int id = int.Parse(routergrid[0, row].Value.ToString());

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

                    connectorrowid.TryAdd(c.ID, i);
                    connectormap.TryAdd(c.ID, c);
                }
                else
                {
                    //更新状态
                    gt.Rows[r][NAME] = c.Name;
                    connectormap[c.ID] = c;
                }

            }
        }

        #region 表格
        #region 显示字段

        const string ID = "通道ID";
        const string TOKEN = "标识";
        const string NAME = "名称";
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
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = routergrid;

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
            routergridmenu.Items.Add("启动通道", null, new EventHandler(StartConnector_Click));//6
            routergridmenu.Items.Add("停止通道", null, new EventHandler(StopConnector_Click));//7

        }

        ContextMenuStrip routergridmenu = null;


        void configgrid_MouseClick(object sender, MouseEventArgs e)
        {
            //throw new NotImplementedException();
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                GetConnectorGridRightMenu().Show(Control.MousePosition);
            }
        }
        ContextMenuStrip GetConnectorGridRightMenu()
        {
            ConnectorConfig cfg = CurrentConnectorConfig;
            int r = ConnectorIdx(cfg.ID);
            routergridmenu.Items[0].Enabled = true;
            routergridmenu.Items[1].Enabled = true;
            //MessageBox.Show("id:"+r.ToString() +" status:"+(QSEnumConnectorStatus)Enum.Parse(typeof(QSEnumConnectorStatus), gt.Rows[r][CONSTATUS].ToString()));
            if (r >= 0)
            {
                QSEnumConnectorStatus status = (QSEnumConnectorStatus)Enum.Parse(typeof(QSEnumConnectorStatus), gt.Rows[r][CONSTATUS].ToString());
                
                switch (status)
                {

                    case QSEnumConnectorStatus.Start:
                        {
                            routergridmenu.Items[0].Enabled = false;
                            break;
                        }
                    case QSEnumConnectorStatus.Stop:
                        {
                            routergridmenu.Items[1].Enabled = false;
                            break;
                        }
                    default:
                        {

                            break;
                        }
                }
            }
            return routergridmenu;
        }

        //初始化Account显示空格
        private void InitTable()
        {
            gt.Columns.Add(ID);//0
            gt.Columns.Add(TOKEN);//1
            gt.Columns.Add(NAME);//1

            //gt.Columns.Add(SRVADDRESS);
            //gt.Columns.Add(SRVPORT);//1
            //gt.Columns.Add(SRV1);//1
            //gt.Columns.Add(SRV2);//1
            //gt.Columns.Add(SRV3);//1
            //gt.Columns.Add(USERID);//1
            //gt.Columns.Add(PASSWORD);//1
            //gt.Columns.Add(USR1);//1
            //gt.Columns.Add(USR2);//1

            //gt.Columns.Add(INTERFACE);//1
            //gt.Columns.Add(VENDORACCOUNT);
            //gt.Columns.Add(ISBINDED);
            gt.Columns.Add(CONSTATUS);
            gt.Columns.Add(CONSTATUSIMG, typeof(Image));

        }

        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = routergrid;

            datasource.DataSource = gt;
            grid.DataSource = datasource;
            grid.Columns[CONSTATUS].Visible = false;

            grid.Columns[ID].Width = 50;

            grid.Columns[TOKEN].Width = 100;
            grid.Columns[NAME].Width = 130;
            //grid.Columns[USERID].Width = 100;
            //grid.Columns[INTERFACE].Width = 150;
            grid.Columns[CONSTATUSIMG].Width = 40;
        }






        #endregion

        void StartConnector_Click(object sender, EventArgs e)
        {
            ConnectorConfig cfg = CurrentConnectorConfig;
            if (cfg == null)
            {
                ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("请选择通道");
            }

            if (fmConfirm.Show(string.Format("确认启动通道:{0}", cfg.Token)) == System.Windows.Forms.DialogResult.Yes)
            {
                Globals.TLClient.ReqStartConnector(cfg.ID);
            }
        }

        void StopConnector_Click(object sender, EventArgs e)
        {
            ConnectorConfig cfg = CurrentConnectorConfig;
            if (cfg == null)
            {
                ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("请选择通道");
            }

            if (fmConfirm.Show(string.Format("确认停止通道:{0}", cfg.Token)) == System.Windows.Forms.DialogResult.Yes)
            {
                Globals.TLClient.ReqStopConnector(cfg.ID);
            }
        }

    }
}
