using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using TradingLib.API;
using TradingLib.Common;
using FutSystems.GUI;

namespace FutsMoniter
{
    public partial class RouterMoniterForm : Telerik.WinControls.UI.RadForm
    {

        RadContextMenu menu = new RadContextMenu();

        public RouterMoniterForm()
        {
            InitializeComponent();

            SetPreferences();
            InitTable();
            BindToTable();

            //生成菜单
            Telerik.WinControls.UI.RadMenuItem  MenuItem_stop = new Telerik.WinControls.UI.RadMenuItem("停止");
            MenuItem_stop.Image = Properties.Resources.editAccount_16;
            MenuItem_stop.Click += new EventHandler(StopConnector_Click);
            Telerik.WinControls.UI.RadMenuItem MenuItem_start = new Telerik.WinControls.UI.RadMenuItem("启动");
            MenuItem_start.Image = Properties.Resources.addAccount_16;
            MenuItem_start.Click += new EventHandler(StartConnector_Click);

            menu.Items.Add(MenuItem_start);
            menu.Items.Add(MenuItem_stop);

        }

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
        private string  CurrentConnector {

            get
            { 
                if (connectorgird.SelectedRows.Count > 0)
                {
                    return connectorgird.SelectedRows[0].ViewInfo.CurrentRow.Cells[CLASSNAME].Value.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }



        //通道的fullname获得Connectorinfo
        ConnectorInfo  GetVisibleConnector(string fullname)
        {
            if(connectormap.Keys.Contains(fullname))
            {
                return connectormap[fullname];
            }
            else
            {
                return null;
            }
        }



        private void RouterMoniterForm_FormClosing(object sender, FormClosingEventArgs e)
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
            if (!connectorrowid.TryGetValue(fullname,out rowid))
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
                int r = ConnectorIDx(c.ClassName);
                if (r == -1)
                {
                    gt.Rows.Add(c.ConnectorName);
                    int i = gt.Rows.Count - 1;
                    gt.Rows[i][CLASSNAME] = c.ClassName;
                    gt.Rows[i][STATUS] = c.Status;
                    gt.Rows[i][TYPE] = c.Type;

                    connectormap.TryAdd(c.ClassName, c);
                    connectorrowid.TryAdd(c.ClassName, i);
                }
                else
                {
                    //更新状态
                    gt.Rows[r][STATUS] = c.Status;
                    connectormap[c.ClassName] = c;
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
            Telerik.WinControls.UI.RadGridView grid = connectorgird;
            grid.ShowRowHeaderColumn = false;//显示每行的头部
            grid.MasterTemplate.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill;//列的填充方式
            grid.ShowGroupPanel = false;//是否显示顶部的panel用于组合排序
            grid.MasterTemplate.EnableGrouping = false;//是否允许分组
            grid.EnableHotTracking = true;

            grid.AllowAddNewRow = false;//不允许增加新行
            grid.AllowDeleteRow = false;//不允许删除行
            grid.AllowEditRow = false;//不允许编辑行
            grid.AllowRowResize = false;
            //grid.EnableSorting = false;
            grid.TableElement.TableHeaderHeight = Globals.HeaderHeight;
            grid.TableElement.RowHeight = Globals.RowHeight;

            grid.EnableAlternatingRowColor = true;//隔行不同颜色
            //this.radRadioDataReader.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On; 

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
            Telerik.WinControls.UI.RadGridView grid = connectorgird;

            //grid.TableElement.BeginUpdate();             
            //grid.MasterTemplate.Columns.Clear(); 
            //accountlist.DataSource = gt;

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

        private void RouterMoniterForm_Load(object sender, EventArgs e)
        {
            
        }

        private void connectorgird_ContextMenuOpening(object sender, ContextMenuOpeningEventArgs e)
        {
            e.ContextMenu = menu.DropDown;
            ConnectorInfo connector = GetVisibleConnector(CurrentConnector);
            //MessageBox.Show(connector.ClassName);
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
        }
    }
}
