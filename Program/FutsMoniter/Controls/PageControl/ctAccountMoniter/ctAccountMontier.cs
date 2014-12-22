using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Common;
using FutSystems.GUI;


namespace FutsMoniter
{
    public partial class ctAccountMontier : UserControl,IEventBinder
    {

        const string PROGRAME = "AccountMontier";
        public ctAccountMontier()
        {
            try
            {
                InitializeComponent();
                SetPreferences();
                InitTable();
                BindToTable();
                this.Load += new EventHandler(ctAccountMontier_Load);
            }
            catch (Exception ex)
            {
                MessageBox.Show("error ex:" + ex.ToString());
            }
            
        }

        void ctAccountMontier_Load(object sender, EventArgs e)
        {
            

            //初始表格右键化右键菜单
            InitMenu();

            WireEvents();
            
        }



        private void accountgrid_Click(object sender, EventArgs e)
        {
            debug("grid mouse clicked...", QSEnumDebugLevel.INFO);
        }



        private void accountgrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            { 
                
            }
        }

        void WireEvents()
        {
            //交易帐户过滤控件
            accLogin.CheckedChanged+=new EventHandler(accLogin_CheckedChanged);
            acct.TextChanged+=new EventHandler(acct_TextChanged);
            acchodpos.CheckedChanged +=new EventHandler(acchodpos_CheckedChanged);
            
            btnAcctFilter.Click +=new EventHandler(btnAcctFilter_Click);
            //帐户表格事件
            accountgrid.CellDoubleClick +=new DataGridViewCellEventHandler(accountgrid_CellDoubleClick);//双击单元格
            accountgrid.CellFormatting +=new DataGridViewCellFormattingEventHandler(accountgrid_CellFormatting);//格式化单元格
            accountgrid.SizeChanged +=new EventHandler(accountgrid_SizeChanged);//大小改变
            accountgrid.Scroll +=new ScrollEventHandler(accountgrid_Scroll);//滚轮滚动
            accountgrid.RowPrePaint += new DataGridViewRowPrePaintEventHandler(accountgrid_RowPrePaint);

            //绑定事件
            btnAddAccount.Click += new EventHandler(btnAddAccount_Click);

            Globals.RegIEventHandler(this);
        }



        #region  辅助函数

        private string _format = "{0:F2}";
        private string decDisp(decimal d)
        {
            return Util.FormatDecimal(d, _format);
        }

        public event DebugDelegate SendDebugEvent;
        bool _debugEnable = true;
        /// <summary>
        /// 是否输出日志
        /// </summary>
        public bool DebugEnable { get { return _debugEnable; } set { _debugEnable = value; } }

        QSEnumDebugLevel _debuglevel = QSEnumDebugLevel.INFO;
        /// <summary>
        /// 日志输出级别
        /// </summary>
        public QSEnumDebugLevel DebugLevel { get { return _debuglevel; } set { _debuglevel = value; } }

        /// <summary>
        /// 判断日志级别 然后再进行输出
        /// 同时对外输出日志事件,用于被日志模块采集日志或分发
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        protected void debug(string msg, QSEnumDebugLevel level = QSEnumDebugLevel.DEBUG)
        {
            //1.判断日志级别,然后调用日志输出 比如向控件或者屏幕输出显示
            if (_debugEnable && (int)level <= (int)_debuglevel)
                msgdebug("[" + level.ToString() + "] " + PROGRAME + ":" + msg);
        }

        /// <summary>
        /// 日志输出
        /// </summary>
        /// <param name="msg"></param>
        protected void msgdebug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }



        #endregion

        private void btnAcctFilter_Click(object sender, EventArgs e)
        {
            Globals.Debug("clicked");

            fmAcctFilter fm = new fmAcctFilter();
            fm.SetFilterArgs(ref filterArgs);
            fm.FilterArgsChangedEvent += new VoidDelegate(fm_FilterArgsChangedEvent);
            Point p = this.PointToScreen(btnAcctFilter.Location);
            p.X = p.X + btnAcctFilter.Width + 5;
            p.Y = p.Y + btnAcctFilter.Height + 5;
            fm.Location = p;
            fm.Show();
        }

        void fm_FilterArgsChangedEvent()
        {
            //MessageBox.Show("filter changed");
            RefreshAccountQuery();
        }


    }
}
