using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Common;
using FutsMoniter.Common;


namespace FutsMoniter
{
    public partial class fmAcctFilter : ComponentFactory.Krypton.Toolkit.KryptonForm,IEventBinder
    {
        public event VoidDelegate FilterArgsChangedEvent;
        public fmAcctFilter()
        {
            InitializeComponent();
            accexecute.Items.Add(MoniterUtil.AnyCBStr);
            accexecute.Items.Add("允许");
            accexecute.Items.Add("冻结");
            accexecute.SelectedIndex = 0;
            this.Load += new EventHandler(fmAcctFilter_Load);
        }
        /// <summary>
        /// 所有ct组件初始化完毕后才会调用OnInit 这样才可以正常的将参数显示到界面 如果组件没有OnInit成功，则对应的SelectedValue无法正常的显示到界面
        /// </summary>
        public void OnInit()
        {
            if (_args.AcctExecuteEnable)
            {
                accexecute.SelectedIndex = _args.AcctExecute ? 1 : 2;
            }

            if (_args.AcctTypeEnable)
            {
                ctAccountType1.AccountType = _args.AcctType;
            }

            if (_args.AgentEnable)
            {
                ctAgentList1.CurrentAgentFK = _args.AgentID;
            }

            if (_args.OrderRouterTypeEnable)
            {
                ctRouterType1.RouterType = _args.OrderRouterType;
            }

            if (_args.RouterGroupEnable)
            {
                ctRouterGroupList1.RouterGroupID = _args.RouterGroupID;
            }

            if (!Globals.Domain.Super)
            {
                ctRouterType1.Visible = Globals.Manager.IsRoot();
                ctAccountType1.Visible = Globals.Manager.IsRoot();
                //有实盘交易权限的才需要查看路由组
                ctRouterGroupList1.Visible = Globals.Domain.Router_Live;
                

                if (!Globals.Manager.IsRoot())
                {
                    this.Height = this.Height - (ctRouterType1.Height + 5) * (2+(!Globals.Domain.Router_Live?1:0));
                }
            }
        }

        public void OnDisposed()
        { 
            
        }
        FilterArgs _args = null;
        /// <summary>
        /// 设定过滤参数
        /// </summary>
        /// <param name="args"></param>
        internal void SetFilterArgs(ref FilterArgs args)
        {
            _args = args;
        }
        void fmAcctFilter_Load(object sender, EventArgs e)
        {
            this.Deactivate += new EventHandler(fmAcctFilter_Deactivate);

            accexecute.SelectedIndexChanged += new EventHandler(accexecute_SelectedIndexChanged);
            ctAccountType1.AccountTypeSelectedChangedEvent += new VoidDelegate(ctAccountType1_AccountTypeSelectedChangedEvent);
            ctAgentList1.AgentSelectedChangedEvent += new VoidDelegate(ctAgentList1_AgentSelectedChangedEvent);
            ctRouterType1.RouterTypeSelectedChangedEvent += new VoidDelegate(ctRouterType1_RouterTypeSelectedChangedEvent);
            ctRouterGroupList1.RouterGroupSelectedChangedEvent += new VoidDelegate(ctRouterGroupList1_RouterGroupSelectedChangedEvent);
            //kryptonPanel1.Paint += new PaintEventHandler(kryptonPanel1_Paint);
            
            Globals.RegIEventHandler(this);
        }

        //void kryptonPanel1_Paint(object sender, PaintEventArgs e)
        //{
        //    ControlPaint.DrawBorder(e.Graphics,
        //                        kryptonPanel1.ClientRectangle,
        //                        Color.LightSeaGreen,         //left
        //                       5,
        //                        ButtonBorderStyle.Solid,
        //                        Color.LightSeaGreen,         //top
        //                        5,
        //                        ButtonBorderStyle.Solid,
        //                        Color.LightSeaGreen,        //right
        //                        5,
        //                        ButtonBorderStyle.Solid,
        //                        Color.LightSeaGreen,        //bottom
        //                        5,
        //                        ButtonBorderStyle.Solid);
        //}

        void ctRouterGroupList1_RouterGroupSelectedChangedEvent()
        {
            if (ctRouterGroupList1.SelectedIndex == 0)
            {
                _args.RouterGroupEnable = false;
            }
            else
            {
                _args.RouterGroupEnable = true;
                _args.RouterGroupID = ctRouterGroupList1.RouterGroupID;
            }
            FilterAccount();
        }

        void ctRouterType1_RouterTypeSelectedChangedEvent()
        {
            if (ctRouterType1.SelectedIndex == 0)
            {
                _args.OrderRouterTypeEnable = false;
            }
            else
            {
                _args.OrderRouterTypeEnable = true;
                _args.OrderRouterType = ctRouterType1.RouterType;
            }
            FilterAccount();
        }



        void ctAgentList1_AgentSelectedChangedEvent()
        {
            if (ctAgentList1.SelectedIndex == 0)
            {
                _args.AgentEnable = false;
            }
            else
            {
                _args.AgentEnable = true;
                _args.AgentID = ctAgentList1.CurrentAgentFK;
            }
            FilterAccount();
        }

        void ctAccountType1_AccountTypeSelectedChangedEvent()
        {
            if (ctAccountType1.SelectedIndex == 0)
            {
                _args.AcctTypeEnable = false;
            }
            else
            {
                _args.AcctTypeEnable = true;
                _args.AcctType = ctAccountType1.AccountType;
            }
            FilterAccount();
        }

        void accexecute_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (accexecute.SelectedIndex == 0)
            {
                _args.AcctExecuteEnable = false;
            }
            else
            {
                _args.AcctExecuteEnable = true;
                _args.AcctExecute = accexecute.SelectedIndex == 1 ? true : false;
            }
            FilterAccount();
        }

        void fmAcctFilter_Deactivate(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 过滤帐户
        /// </summary>
        void FilterAccount()
        {
            if (FilterArgsChangedEvent!=null)
                FilterArgsChangedEvent();
        }
    }
}
