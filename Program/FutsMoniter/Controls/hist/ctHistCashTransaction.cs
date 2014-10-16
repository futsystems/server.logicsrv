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
    public partial class ctHistCashTransaction : UserControl
    {
        public ctHistCashTransaction()
        {
            InitializeComponent();
            SetPreferences();
            InitTable();
            BindToTable();
        }

        public void Clear()
        {
            cashgrid.DataSource = null;
            gt.Rows.Clear();
            BindToTable();
        }
        public Telerik.WinControls.UI.RadGridView Grid { get { return cashgrid; } }

        delegate void CashTransactionDel(CashTransaction c);
        public void GotHistCashTransaction(CashTransaction c)
        {

            if (InvokeRequired)
            {
                try
                {
                    Invoke(new CashTransactionDel(GotHistCashTransaction), new object[] { c });
                }
                catch (Exception ex)
                { }
            }
            else
            {
                DataRow r = gt.Rows.Add(c.Settleday);
                int i = gt.Rows.Count - 1;//得到新建的Row号
                gt.Rows[i][DATETIME] = c.DateTime.ToString("yyy-MM-dd HH 时 mm 分 ss 秒");
                gt.Rows[i][OPTYPE] = c.Amount > 0 ? "入金" : "出金";
                gt.Rows[i][AMOUNT] = Math.Abs(c.Amount);
                gt.Rows[i][TRANSREF] = c.TransRef;
            }

        }
        const string SETTLEDAY = "结算日";
        const string DATETIME = "出入金时间";
        const string OPTYPE = "操作类型";
        const string AMOUNT = "金额";
        const string TRANSREF = "流水码";

        DataTable gt = new DataTable();

        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            Telerik.WinControls.UI.RadGridView grid = cashgrid;
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
            gt.Columns.Add(SETTLEDAY);
            gt.Columns.Add(DATETIME);
            gt.Columns.Add(OPTYPE);
            gt.Columns.Add(AMOUNT);
            gt.Columns.Add(TRANSREF);
        }
        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            Telerik.WinControls.UI.RadGridView grid = cashgrid;
            //grid.TableElement.BeginUpdate();             
            //grid.MasterTemplate.Columns.Clear(); 
            //datasource.DataSource = gt;
            grid.DataSource = gt;
        }
    }
}
