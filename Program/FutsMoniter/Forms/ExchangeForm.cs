using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using TradingLib.API;
using TradingLib.Common;
using FutSystems.GUI; 


namespace FutsMoniter
{
    public partial class ExchangeForm : Telerik.WinControls.UI.RadForm
    {
        public ExchangeForm()
        {
            InitializeComponent();
            SetPreferences();
            InitTable();
            BindToTable();

        }

        public bool AnyExchange 
        {
            get
            {
                return exchangemap.Count > 0;
            }
        }
        Dictionary<int, int> exchangeidmap = new Dictionary<int, int>();
        Dictionary<int, Exchange> exchangemap = new Dictionary<int, Exchange>();

        int ExchangeIdx(int id)
        { 
            int rowid = -1;
            if (exchangeidmap.TryGetValue(id, out rowid))
            {
                return rowid;
            }
            else
            {
                return -1;
            }
        }

        public void GotExchange(Exchange ex)
        {
            InvokeGotExchange(ex);
        }

        delegate void ExchangeDel(Exchange ex);
        void InvokeGotExchange(Exchange ex)
        {
            if (InvokeRequired)
            {
                Invoke(new ExchangeDel(InvokeGotExchange), new object[] { ex });
            }
            else
            {
                int r = ExchangeIdx(ex.ID);
                if (r == -1)
                {
                    gt.Rows.Add(ex.ID);
                    int i = gt.Rows.Count - 1;
                    gt.Rows[i][EXNAME] = ex.Name;
                    gt.Rows[i][EXCODE] = ex.EXCode;
                    gt.Rows[i][EXCOUNTRY] = ex.Country;

                    exchangemap.Add(ex.ID, ex);
                    exchangeidmap.Add(ex.ID, i);

                }
                else
                { 
                    
                }
            }
        }


        #region 表格
        #region 显示字段

        const string EXID = "全局ID";
        const string EXNAME = "名称";
        const string EXCODE = "编号";
        const string EXCOUNTRY = "国家";
        #endregion

        DataTable gt = new DataTable();
        BindingSource datasource = new BindingSource();

        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            Telerik.WinControls.UI.RadGridView grid = exchangegrid;
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
            gt.Columns.Add(EXID);//
            gt.Columns.Add(EXNAME);//
            gt.Columns.Add(EXCODE);//
            gt.Columns.Add(EXCOUNTRY);//
        }

        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            Telerik.WinControls.UI.RadGridView grid = exchangegrid;

            //grid.TableElement.BeginUpdate();             
            //grid.MasterTemplate.Columns.Clear(); 
            //accountlist.DataSource = gt;

            datasource.DataSource = gt;
            grid.DataSource = datasource;

        }





        #endregion

        private void ExchangeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

    }
}
