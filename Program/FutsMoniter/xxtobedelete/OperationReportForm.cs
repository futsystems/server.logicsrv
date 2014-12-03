//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Windows.Forms;

//namespace FutsMoniter
//{
//    public partial class OperationReportForm : Telerik.WinControls.UI.RadForm
//    {
//        public OperationReportForm()
//        {
//            InitializeComponent();
//            SetPreferences();
//            InitTable();
//            BindToTable();
//            PrepareTable();
//            //ctGridExport1.Grid = opgrid;
//        }


//        #region 显示字段

//        const string A = "A";
//        const string B = "B";
//        const string C = "C";
//        const string D = "D";
//        const string E = "E";
//        const string F = "F";
//        const string G = "G";
//        const string H = "H";
//        const string I = "I";
//        const string J = "J";

//        #endregion

//        DataTable gt = new DataTable();
//        BindingSource datasource = new BindingSource();

//        /// <summary>
//        /// 添加一行
//        /// </summary>
//        /// <returns></returns>
//        int addrow()
//        {
//            DataRow r = gt.Rows.Add("");
//            return gt.Rows.Count - 1;//得到新建的Row号
//        }
//        void SetCell(int i, string Col,object value)
//        {
//            gt.Rows[i][Col] = value;
//        }
//        void PrepareTable()
//        {
//            int i = 0;
//            addrow();//0
//            i=addrow();//1
//            gt.Rows[i][A] = "系统运营报表";
//            addrow();//2
//            i = addrow();//3
//            gt.Rows[i][A] = "帐户数目";
//            i = addrow();//4
//            SetCell(i, A, "实盘帐户"); SetCell(i, B, "模拟帐户"); SetCell(i, C, "交易员帐户"); SetCell(i, D, "代理帐户");
//            addrow();//5
//            addrow();//6
//            i = addrow();//7
//            SetCell(i, A, "交易统计");
//            i = addrow();//8
//            SetCell(i, A, "实盘统计");  SetCell(i, C, "模拟帐户统计"); SetCell(i, E, "交易员帐户统计");
//            i = addrow();//9
//            SetCell(i, A, "委托手数");  SetCell(i, C, "委托手数"); SetCell(i, E, "委托手数");
//            i = addrow();//10
//            SetCell(i, A, "成交手数");  SetCell(i, C, "成交手数"); SetCell(i, E, "成交手数");
//            i = addrow();//11
//            SetCell(i, A, "手续费");    SetCell(i, C, "手续费"); SetCell(i, E, "手续费");
//            i = addrow();//12
//            SetCell(i, A, "平仓盈亏"); SetCell(i, C, "平仓盈亏"); SetCell(i, E, "平仓盈亏");
//            i = addrow();//13
//            SetCell(i, A, "浮动盈亏"); SetCell(i, C, "浮动盈亏"); SetCell(i, E, "浮动盈亏");
//            i = addrow();//14
//            SetCell(i, A, "净盈亏"); SetCell(i, C, "净盈亏"); SetCell(i, E, "净盈亏");
//            i = addrow();//15
//            i = addrow();//16
//            i = addrow();//17
//            SetCell(i, A, "实盘财务统计");
//            i = addrow();//18
//            SetCell(i, A, "新开户"); SetCell(i, B, "累计入金"); SetCell(i, C, "累计出金"); SetCell(i, D, "交易人数"); SetCell(i, E, "手续费"); SetCell(i, F, "平仓盈亏"); SetCell(i, G, "浮动盈亏"); SetCell(i, H, "净盈亏");
//            i = addrow();//18
//            i = addrow();//18
//            i = addrow();//18
//            i = addrow();//18


//        }
//        /// <summary>
//        /// 设定表格控件的属性
//        /// </summary>
//        private void SetPreferences()
//        {
//            Telerik.WinControls.UI.RadGridView grid = opgrid;
//            grid.ShowRowHeaderColumn = false;//显示每行的头部
//            grid.MasterTemplate.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill;//列的填充方式
//            grid.ShowGroupPanel = false;//是否显示顶部的panel用于组合排序
//            grid.MasterTemplate.EnableGrouping = false;//是否允许分组
//            grid.EnableHotTracking = true;

//            grid.AllowAddNewRow = false;//不允许增加新行
//            grid.AllowDeleteRow = false;//不允许删除行
//            grid.AllowEditRow = false;//不允许编辑行
//            grid.AllowRowResize = false;
//            //grid.EnableSorting = false;
//            grid.TableElement.TableHeaderHeight = Globals.HeaderHeight;
//            grid.TableElement.RowHeight = Globals.RowHeight;

//            grid.EnableAlternatingRowColor = true;//隔行不同颜色
//            //this.radRadioDataReader.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On; 

//        }

//        //初始化Account显示空格
//        private void InitTable()
//        {
//            gt.Columns.Add(A);//
//            gt.Columns.Add(B);//
//            gt.Columns.Add(C);//
//            gt.Columns.Add(D);//
//            gt.Columns.Add(E);//
//            gt.Columns.Add(F);//
//            gt.Columns.Add(G);//
//            gt.Columns.Add(H);//
//            gt.Columns.Add(I);//
//            gt.Columns.Add(J);//

//        }

//        /// <summary>
//        /// 绑定数据表格到grid
//        /// </summary>
//        private void BindToTable()
//        {
//            Telerik.WinControls.UI.RadGridView grid = opgrid;

//            datasource.DataSource = gt;
//            grid.DataSource = datasource;
//        }

//        private void btnRefresh_Click(object sender, EventArgs e)
//        {

//        }
//    }
//}
