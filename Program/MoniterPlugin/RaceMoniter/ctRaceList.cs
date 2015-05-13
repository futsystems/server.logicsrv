using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Protocol;

namespace TradingLib.RaceMoniter
{
    public partial class ctRaceList : UserControl
    {
        public event VoidDelegate QryRaceListEvent;

        public event VoidDelegate OpenNewRaceEvent;
        public ctRaceList()
        {
            InitializeComponent();

            SetPreferences();
            InitTable();
            BindToTable();


            this.Load += new EventHandler(ctRaceList_Load);
        }

        void ctRaceList_Load(object sender, EventArgs e)
        {
            btnQryRaceList.Click += new EventHandler(btnQryRaceList_Click);
            btnOpenNewRace.Click += new EventHandler(btnOpenNewRace_Click);
        }

        void btnOpenNewRace_Click(object sender, EventArgs e)
        {
            if (OpenNewRaceEvent != null)
                OpenNewRaceEvent();
        }

        public void Clear()
        {
            raceGrid.DataSource = null;
            gt.Rows.Clear();
            BindToTable();
        }


        void btnQryRaceList_Click(object sender, EventArgs e)
        {
            Clear();
            if (QryRaceListEvent != null)
            {
                QryRaceListEvent();
            }
        }






        #region 表格
        #region 显示字段

        const string RACEID = "比赛编号";
        const string RACETYPE = "比赛级别";
        const string ENTRYNUM = "参赛人数";
        const string ELIMINATENUM = "淘汰人数";
        const string PROMOTNUM = "晋级人数";
        const string CREATETIME = "创建时间";
        const string STARTTME = "报名开始时间";
        const string ENDTIME = "报名结束时间";

        #endregion

        DataTable gt = new DataTable();
        BindingSource datasource = new BindingSource();

        public void OnRaceList(string jsonstr, bool islast)
        {
            try
            {
                RaceSetting[] list = MoniterControl.MoniterHelper.ParseJsonResponse<RaceSetting[]>(jsonstr);

                if (list != null)
                {
                    foreach (var r in list)
                    {
                        InvokeGotRace(r);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        void InvokeGotRace(RaceSetting race)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<RaceSetting>(InvokeGotRace), new object[] { race });
            }
            else
            {
                DataRow r = gt.Rows.Add(0);
                int i = gt.Rows.Count - 1;//得到新建的Row号

                gt.Rows[i][RACEID] = race.RaceID;
                gt.Rows[i][RACETYPE] = Util.GetEnumDescription(race.RaceType);
                gt.Rows[i][ENTRYNUM] = race.EntryNum;
                gt.Rows[i][ELIMINATENUM] = race.EliminateNum;
                gt.Rows[i][PROMOTNUM] = race.PromotNum;
                if(race.RaceType == QSEnumRaceType.PRERACE)
                {
                    gt.Rows[i][CREATETIME] = Util.ToDateTime(race.StartTime).ToString("yy-MM-dd");
                    gt.Rows[i][STARTTME] = Util.ToDateTime(race.BeginSignTime).ToString("yy-MM-dd");
                    gt.Rows[i][ENDTIME] = Util.ToDateTime(race.EndSignTime).ToString("yy-MM-dd");
                }
                else
                {
                    gt.Rows[i][CREATETIME] = "--";
                    gt.Rows[i][STARTTME] ="--";
                    gt.Rows[i][ENDTIME] ="--";
                }
            }
        }
        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = raceGrid;

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
            gt.Columns.Add(RACEID);//
            gt.Columns.Add(RACETYPE);//
            gt.Columns.Add(ENTRYNUM);//
            gt.Columns.Add(ELIMINATENUM);//
            gt.Columns.Add(PROMOTNUM);
            gt.Columns.Add(CREATETIME);
            gt.Columns.Add(STARTTME);
            gt.Columns.Add(ENDTIME);
        }

        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = raceGrid;


            datasource.DataSource = gt;
            grid.DataSource = datasource;

            //需要在绑定数据源后设定具体的可见性
            //grid.Columns[EXCHANGEID].IsVisible = false;
            //grid.Columns[UNDERLAYINGID].IsVisible = false;
            //grid.Columns[MARKETTIMEID].IsVisible = false;
            //grid.Columns[TRADEABLE].IsVisible = false;
        }





        #endregion


    }
}
