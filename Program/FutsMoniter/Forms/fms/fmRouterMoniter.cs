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
    public partial class fmRouterMoniter : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public fmRouterMoniter()
        {
            InitializeComponent();

            SetPreferences();
            InitTable();
            BindToTable();

            //connectorgird.ContextMenuStrip = menu;
            menu.Items.Add("启动", null, new EventHandler(StartConnector_Click));
            menu.Items.Add("停止", null, new EventHandler(StopConnector_Click));
            //connectorgird.ContextMenuStrip.Items.Add("修改信息", null, new EventHandler(ChangeInvestor_Click));
            //connectorgird.ContextMenuStrip.Items.Add("历史查询", null, new EventHandler(QryHist_Click));

            connectorgird.CellMouseClick += new DataGridViewCellMouseEventHandler(connectorgird_CellMouseClick);
            this.FormClosing +=new FormClosingEventHandler(fmRouterMoniter_FormClosing);
        }

        void connectorgird_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                GetRightMenu().Show(Control.MousePosition);
            }
        }

        ContextMenuStrip menu = new ContextMenuStrip();
        void StopConnector_Click(object sender, EventArgs e)
        {
            ConnectorInfo connector = GetVisibleConnector(CurrentConnector);
            if (connector.Type.Equals("Broker"))
            {
                Globals.TLClient.ReqStopBroker(connector.ClassName);
            }
            else if (connector.Type.Equals("DataFeed"))
            {
                Globals.TLClient.ReqStopDataFeed(connector.ClassName);
            }
            else
            {
                fmConfirm.Show("所操作的通道类型无法识别");
            }
        }

        /// <summary>
        /// 编辑某个交易帐号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void StartConnector_Click(object sender, EventArgs e)
        {
            ConnectorInfo connector = GetVisibleConnector(CurrentConnector);
            if (connector.Type.Equals("Broker"))
            {
                Globals.TLClient.ReqStartBroker(connector.ClassName);
            }
            else if (connector.Type.Equals("DataFeed"))
            {
                Globals.TLClient.ReqStartDataFeed(connector.ClassName);
            }
            else
            {
                fmConfirm.Show("所操作的通道类型无法识别");
            }
        }



        //得到当前选择的行号
        private string CurrentConnector
        {

            get
            {
                int row = connectorgird.SelectedRows.Count > 0 ? connectorgird.SelectedRows[0].Index : -1;
                if (row >= 0)
                {
                    return connectorgird[TITIE, row].Value.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }



        //通道的fullname获得Connectorinfo
        ConnectorInfo GetVisibleConnector(string fullname)
        {
            if (connectormap.Keys.Contains(fullname))
            {
                return connectormap[fullname];
            }
            else
            {
                return null;
            }
        }



        private void fmRouterMoniter_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        public void GotConnector(ConnectorInfo c, bool islast)
        {
            InvokeGotConnecotr(c);

        }

        ConcurrentDictionary<string, ConnectorInfo> connectormap = new ConcurrentDictionary<string, ConnectorInfo>();
        ConcurrentDictionary<string, int> connectorrowid = new ConcurrentDictionary<string, int>();

        int ConnectorIDx(string fullname)
        {
            int rowid = -1;
            if (!connectorrowid.TryGetValue(fullname, out rowid))
            {
                return -1;
            }
            else
            {
                return rowid;
            }
        }
        delegate void ConnectorInfoDel(ConnectorInfo c);
        void InvokeGotConnecotr(ConnectorInfo c)
        {
            if (InvokeRequired)
            {
                Invoke(new ConnectorInfoDel(InvokeGotConnecotr), new object[] { c });
            }
            else
            {
                int r = ConnectorIDx(c.ConnectorName);
                if (r == -1)
                {
                    gt.Rows.Add(c.ConnectorName);
                    int i = gt.Rows.Count - 1;
                    gt.Rows[i][CLASSNAME] = c.ClassName;
                    gt.Rows[i][STATUS] = c.Status;
                    gt.Rows[i][TYPE] = c.Type;

                    connectormap.TryAdd(c.ConnectorName, c);
                    connectorrowid.TryAdd(c.ConnectorName, i);
                }
                else
                {
                    //更新状态
                    gt.Rows[r][STATUS] = c.Status;
                    connectormap[c.ConnectorName] = c;
                }

            }
        }

        #region 表格
        #region 显示字段

        const string TITIE = "名称";
        const string CLASSNAME = "对象全名";
        const string TYPE = "路由类被";
        const string STATUS = "状态";

        #endregion

        DataTable gt = new DataTable();
        BindingSource datasource = new BindingSource();

        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = connectorgird;

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
            gt.Columns.Add(TITIE);//0
            gt.Columns.Add(CLASSNAME);//1
            //gt.Columns.Add(TYPE, typeof(Image));//2

            gt.Columns.Add(TYPE);//3
            //gt.Columns.Add(EXECUTEIMG, typeof(Image));//4
            //gt.Columns.Add(PROFITLOSSIMG, typeof(Image));//5
            gt.Columns.Add(STATUS);//6
        }

        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = connectorgird;

            datasource.DataSource = gt;
            connectorgird.DataSource = datasource;

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

        ContextMenuStrip GetRightMenu()
        {
            ConnectorInfo connector = GetVisibleConnector(CurrentConnector);
            if (connector.Status)
            {
                menu.Items[0].Enabled = false;
                menu.Items[1].Enabled = true;
            }
            else
            {
                menu.Items[0].Enabled = true;
                menu.Items[1].Enabled = false;
            }
            return menu;
        }

       

    }
}
