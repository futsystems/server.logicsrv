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
    public partial class MarketTimeForm : Telerik.WinControls.UI.RadForm
    {
        public MarketTimeForm()
        {
            InitializeComponent();
            SetPreferences();
            InitTable();
            BindToTable();
        }


        public bool AnyMarketTime
        {
            get {
                return markettimemap.Count > 0;
            }
        }
        Dictionary<int, MarketTime> markettimemap = new Dictionary<int, MarketTime>();
        Dictionary<int, int> markettimeidxmap = new Dictionary<int, int>();

        int MarketTimeIdx(int id)
        {
            int rowid=-1;
            if (markettimeidxmap.TryGetValue(id, out rowid))
            {
                return rowid;
            }
            else
            {
                return -1;
            }
        }


        public void GotMarketTime(MarketTime mt)
        {
            InvokeGotMarketTime(mt);
        }
        delegate void MarketTimeDel(MarketTime mt);

        void InvokeGotMarketTime(MarketTime mt)
        {
            if (InvokeRequired)
            {
                Invoke(new MarketTimeDel(InvokeGotMarketTime), new object[] { mt });
            }
            else
            {
                int r = MarketTimeIdx(mt.ID);
                if (r == -1)
                {
                    gt.Rows.Add(mt.ID);
                    int i = gt.Rows.Count - 1;
                    gt.Rows[i][MTNAME] = mt.Name;
                    gt.Rows[i][MTDESC] = mt.Description;

                    markettimemap.Add(mt.ID, mt);
                    markettimeidxmap.Add(mt.ID, i);
                    
                }
                else
                { 
                    
                }
            }
        }



        #region 表格
        #region 显示字段

        const string MTID = "全局ID";
        const string MTNAME = "名称";
        const string MTDESC = "描述";
        #endregion

        DataTable gt = new DataTable();
        BindingSource datasource = new BindingSource();

        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            Telerik.WinControls.UI.RadGridView grid =mtgrid;
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
            gt.Columns.Add(MTID);//
            gt.Columns.Add(MTNAME);//
            gt.Columns.Add(MTDESC);//
        }

        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            Telerik.WinControls.UI.RadGridView grid = mtgrid;

            //grid.TableElement.BeginUpdate();             
            //grid.MasterTemplate.Columns.Clear(); 
            //accountlist.DataSource = gt;

            datasource.DataSource = gt;
            grid.DataSource = datasource;

        }





        #endregion

        private void MarketTimeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void mtgrid_DoubleClick(object sender, EventArgs e)
        {
            MarketTimeInfoForm fm = new MarketTimeInfoForm();
            MarketTime mt = GetVisibleMarketTime(CurrentMarketTimeID);
            if (mt != null)
            {
                fm.SetMarketTime(mt);
                fm.Show();
            }
        }

        //得到当前选择的行号
        private int CurrentMarketTimeID
        {

            get
            {

                if (mtgrid.SelectedRows.Count > 0)
                {
                    return int.Parse(mtgrid.SelectedRows[0].ViewInfo.CurrentRow.Cells[MTID].Value.ToString());
                }
                else
                {
                    return 0;
                }
            }
        }

        //通过行号得该行的Security
        MarketTime GetVisibleMarketTime(int id)
        {
            MarketTime mt = null;
            if (markettimemap.TryGetValue(id, out mt))
            {
                return mt;
            }
            else
            {
                return null;
            }

        }
    }
}
