using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using TradingLib.API;
using TradingLib.Common;

using FutSystems.GUI;

namespace FutsMoniter
{
    public partial class ctHistPosition : UserControl
    {
        public ctHistPosition()
        {
            InitializeComponent();

            SetPreferences();
            InitTable();
            BindToTable();
        }

        public void Clear()
        {
            positiongrid.DataSource = null;
            gt.Rows.Clear();
            BindToTable();
        }


        delegate void SettlePositionDel(SettlePosition pos);
        public void GotHistPosition(SettlePosition pos)
        {

            if (InvokeRequired)
            {
                try
                {
                    Invoke(new SettlePositionDel(GotHistPosition), new object[] { pos });
                }
                catch (Exception ex)
                { }
            }
            else
            {
                DataRow r = gt.Rows.Add(pos.Symbol);
                int i = gt.Rows.Count - 1;//得到新建的Row号

                //如果不存在,则我们将该account-symbol对插入映射列表我们的键用的是account_symbol配对
                ;
                int size = pos.Size;
                gt.Rows[i][DIRECTION] = true;//getDirection(size);
                gt.Rows[i][SIZE] = Math.Abs(size);
                //gt.Rows[i][CANFLATSIZE] = getCanFlatSize(pos);
                gt.Rows[i][AVGPRICE] = pos.AVGPrice;
                gt.Rows[i][REALIZEDPL] = 0;
                //if (guiSide == QSEnumGUISide.Client)
                //    updatePositionStrategyImg(i);//更新界面图标
                //updateCurrentRowPositionNum();

            }

        }



        const string SYMBOL = "合约";
        const string DIRECTION = "买卖";
        const string SIZE = "总持仓";
        const string CANFLATSIZE = "可平量";//用于计算当前限价委托可以挂单数量
        const string LASTPRICE = "最新";//最新成交价
        const string AVGPRICE = "持仓均价";
        const string UNREALIZEDPL = "浮盈";
        const string REALIZEDPL = "平盈";
        const string ACCOUNT = "账户";
        const string PROFITTARGET = "止盈";
        const string STOPLOSS = "止损";
        const string KEY = "编号";
        //const string STRATEGY = "出场策略";
        DataTable gt = new DataTable();
        //BindingSource datasource = new BindingSource();

        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            Telerik.WinControls.UI.RadGridView grid = positiongrid;
            grid.ShowRowHeaderColumn = false;//显示每行的头部
            grid.MasterTemplate.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill;//列的填充方式
            grid.ShowGroupPanel = false;//是否显示顶部的panel用于组合排序
            grid.MasterTemplate.EnableGrouping = false;//是否允许分组
            grid.EnableHotTracking = true;
            //this.radRadioDataReader.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On; 

            grid.AllowAddNewRow = false;//不允许增加新行
            grid.AllowDeleteRow = false;//不允许删除行
            grid.AllowEditRow = false;//不允许编辑行
            grid.AllowRowResize = false;
            grid.EnableSorting = false;
            grid.TableElement.TableHeaderHeight = UIGlobals.HeaderHeight;
            grid.TableElement.RowHeight = UIGlobals.RowHeight;

            grid.EnableAlternatingRowColor = true;//隔行不同颜色
        }
        /// <summary>
        /// 初始化数据表格
        /// </summary>
        private void InitTable()
        {
            gt.Columns.Add(SYMBOL);
            gt.Columns.Add(DIRECTION);
            gt.Columns.Add(SIZE, typeof(int));
            gt.Columns.Add(CANFLATSIZE, typeof(int));
            gt.Columns.Add(LASTPRICE);
            gt.Columns.Add(AVGPRICE);
            gt.Columns.Add(UNREALIZEDPL);
            gt.Columns.Add(REALIZEDPL);
            gt.Columns.Add(ACCOUNT);
            gt.Columns.Add(PROFITTARGET);
            gt.Columns.Add(STOPLOSS);
            gt.Columns.Add(KEY);
            //gt.Columns.Add(STRATEGY, typeof(Image));

        }
        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            Telerik.WinControls.UI.RadGridView grid = positiongrid;
            //grid.TableElement.BeginUpdate();             
            //grid.MasterTemplate.Columns.Clear(); 
            //datasource.DataSource = gt;
            grid.DataSource = gt;
            grid.Columns[ACCOUNT].IsVisible = false;
            grid.Columns[KEY].IsVisible = false;

            //set width
            grid.Columns[SYMBOL].Width = 45;
            grid.Columns[SYMBOL].TextAlignment = ContentAlignment.MiddleCenter;
            grid.Columns[DIRECTION].Width = 45;
            grid.Columns[DIRECTION].TextAlignment = ContentAlignment.MiddleCenter;
            grid.Columns[SIZE].Width = 45;
            grid.Columns[SIZE].TextAlignment = ContentAlignment.MiddleCenter;
            grid.Columns[CANFLATSIZE].Width = 45;
            grid.Columns[CANFLATSIZE].TextAlignment = ContentAlignment.MiddleCenter;
            grid.Columns[LASTPRICE].Width = 65;
            grid.Columns[LASTPRICE].TextAlignment = ContentAlignment.MiddleRight;
            grid.Columns[AVGPRICE].Width = 65;
            grid.Columns[AVGPRICE].TextAlignment = ContentAlignment.MiddleRight;
            grid.Columns[UNREALIZEDPL].Width = 65;
            grid.Columns[UNREALIZEDPL].TextAlignment = ContentAlignment.MiddleRight;
            grid.Columns[REALIZEDPL].Width = 65;
            grid.Columns[REALIZEDPL].TextAlignment = ContentAlignment.MiddleRight;
            grid.Columns[PROFITTARGET].Width = 75;
            grid.Columns[PROFITTARGET].TextAlignment = ContentAlignment.MiddleCenter;
            grid.Columns[STOPLOSS].Width = 75;
            grid.Columns[STOPLOSS].TextAlignment = ContentAlignment.MiddleCenter;


        }
    }
}
