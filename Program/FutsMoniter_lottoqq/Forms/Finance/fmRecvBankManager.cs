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

using TradingLib.Mixins.JsonObject;

namespace FutsMoniter
{
    public partial class fmRecvBankManager : ComponentFactory.Krypton.Toolkit.KryptonForm,IEventBinder
    {
        public fmRecvBankManager()
        {
            InitializeComponent();
            SetPreferences();
            InitTable();
            BindToTable();
            this.Load += new EventHandler(fmRecvBankManager_Load);
            
        }

        void fmRecvBankManager_Load(object sender, EventArgs e)
        {
            WireEvent();
            Globals.TLClient.ReqQryReceiveableBank();
        }

        void WireEvent()
        {
            Globals.RegIEventHandler(this);
            recvgid.RowPrePaint += new DataGridViewRowPrePaintEventHandler(recvgid_RowPrePaint);
        }

        void recvgid_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            e.PaintParts = e.PaintParts ^ DataGridViewPaintParts.Focus;
        }


        public void OnInit()
        {
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "QryReceiveableBank", this.OnQryRecvBank);
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "NotifyRecvBank", this.OnNotifyRecvBank);
        }

        public void OnDisposed()
        {
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "QryReceiveableBank", this.OnQryRecvBank);
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "NotifyRecvBank", this.OnNotifyRecvBank);
        }

        void OnNotifyRecvBank(string jsonstr)
        {
            JsonWrapperReceivableAccount obj = MoniterUtils.ParseJsonResponse<JsonWrapperReceivableAccount>(jsonstr);
            if (obj != null)
            {
                InvokeGotRecvBank(obj);
            }
            else//如果没有配资服
            {

            }
        }

        void OnQryRecvBank(string jsonstr)
        {

            //JsonData jd = TradingLib.Mixins.LitJson.JsonMapper.ToObject(jsonstr);
            //int code = int.Parse(jd["Code"].ToString());
            JsonWrapperReceivableAccount[] objs = MoniterUtils.ParseJsonResponse<JsonWrapperReceivableAccount[]>(jsonstr);
            if (objs  != null)
            {
                //JsonWrapperReceivableAccount[] objs = TradingLib.Mixins.LitJson.JsonMapper.ToObject<JsonWrapperReceivableAccount[]>(jd["Playload"].ToJson());
                foreach (JsonWrapperReceivableAccount obj in objs)
                {
                    InvokeGotRecvBank(obj);
                }
            }
            else//如果没有配资服
            {

            }
        }
        //得到当前选择的行号
        private JsonWrapperReceivableAccount CurrentRecvBank
        {
            get
            {
                int row = recvgid.SelectedRows.Count > 0 ? recvgid.SelectedRows[0].Index : -1;
                if (row >= 0)
                {
                    int id = int.Parse(recvgid[0, row].Value.ToString());

                    if (recvbankmap.Keys.Contains(id))
                        return recvbankmap[id];
                    else
                        return null;
                }
                else
                {
                    return null;
                }
            }
        }
        ConcurrentDictionary<int, JsonWrapperReceivableAccount> recvbankmap = new ConcurrentDictionary<int, JsonWrapperReceivableAccount>();
        ConcurrentDictionary<int, int> recvbankrowid = new ConcurrentDictionary<int, int>();

        int RecvBankIdx(int id)
        {
            int rowid = -1;
            if (!recvbankrowid.TryGetValue(id, out rowid))
            {
                return -1;
            }
            else
            {
                return rowid;
            }
        }
        void InvokeGotRecvBank(JsonWrapperReceivableAccount bank)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<JsonWrapperReceivableAccount>(InvokeGotRecvBank), new object[] { bank });
            }
            else
            {
                int r = RecvBankIdx(bank.ID);
                if (r == -1)
                {
                    gt.Rows.Add(bank.ID);
                    int i = gt.Rows.Count - 1;

                    gt.Rows[i][ID] = bank.ID;
                    gt.Rows[i][BANK] = bank.BankName;
                    gt.Rows[i][NAME] = bank.Name;
                    gt.Rows[i][BANKAC] = bank.Bank_AC;
                    gt.Rows[i][BRANCH] = bank.Branch;




                    recvbankrowid.TryAdd(bank.ID, i);
                    recvbankmap.TryAdd(bank.ID, bank);
                }
                else
                {
                    //更新状态
                    gt.Rows[r][BANK] = bank.BankName;
                    gt.Rows[r][NAME] = bank.Name;
                    gt.Rows[r][BANKAC] = bank.Bank_AC;
                    gt.Rows[r][BRANCH] = bank.Branch;
                    recvbankmap[bank.ID] = bank;
                }
                
            }
        }



        #region 表格
        #region 显示字段

        const string ID = "全局ID";
        const string BANK = "银行";
        const string NAME = "姓名";
        const string BANKAC= "帐号";
        const string BRANCH = "分行";

        #endregion

        DataTable gt = new DataTable();
        BindingSource datasource = new BindingSource();

        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = recvgid;

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

            grid.ContextMenuStrip = new ContextMenuStrip();

            grid.ContextMenuStrip.Items.Add("添加银行帐号", null, new EventHandler(AddRecvBank_Click));//0
            grid.ContextMenuStrip.Items.Add("修改银行帐号", null, new EventHandler(EditRecvBank_Click));//1

        }

        void AddRecvBank_Click(object sender, EventArgs e)
        {
            fmRecvBankEdit fm = new fmRecvBankEdit();
            fm.ShowDialog();
        }

        void EditRecvBank_Click(object sender, EventArgs e)
        {
            JsonWrapperReceivableAccount bank = CurrentRecvBank;
            if (bank == null)
            {
                ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("请选择收款银行记录");
                return;
            }
            fmRecvBankEdit fm = new fmRecvBankEdit();
            fm.SetRecvBank(bank);
            fm.ShowDialog();
        }


        //初始化Account显示空格
        private void InitTable()
        {
            gt.Columns.Add(ID);//
            gt.Columns.Add(BANK);//
            gt.Columns.Add(NAME);//
            gt.Columns.Add(BANKAC);//
            gt.Columns.Add(BRANCH);//
        }

        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = recvgid;
            datasource.DataSource = gt;
            grid.DataSource = datasource;

            //grid.Columns[MTID].Width = 80;
            //grid.Columns[MTNAME].Width = 200;

        }





        #endregion
    }
}
