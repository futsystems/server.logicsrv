using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FutSystems.GUI;
using TradingLib.API;


namespace FutsMoniter
{
    public partial class ctAgentList : UserControl
    {
        public event VoidDelegate AgentSelectedChangedEvent;
        bool _gotdata = false;

        //属性获得和设置
        [DefaultValue(true)]
        bool _enableselected = false;
        public bool EnableSelected
        {
            get
            {
                return _enableselected;
            }
            set
            {
                _enableselected = value;
                agent.Enabled = _enableselected;
            }
        }



        public ctAgentList()
        {
            InitializeComponent();
            
            //如果已经初始化完成 则直接读取数据填充 否则将资金放入事件回调中
            if (Globals.EnvReady)
            {
                InitAgentList();
                
            }
            else
            {
                if (Globals.CallbackCentreReady)
                {
                    Globals.RegInitCallback(OnInitFinished);
                }
            }
            this.Disposed += new EventHandler(ctAgentList_Disposed);
            this.Load += new EventHandler(ctAgentList_Load);
        }

        void ctAgentList_Load(object sender, EventArgs e)
        {
            if (!_enableselected)
            {
                agent.Enabled = false;
            }
            else
            {
                agent.Enabled = true;
            }
        }

        void ctAgentList_Disposed(object sender, EventArgs e)
        {
            if (Globals.CallbackCentreReady)
            {
                //Globals.CallBackCentre.UnRegisterCallback("FinServiceCentre", "QryFinServicePlan", OnInitFinished);
            }
        }

        void InitAgentList()
        {
            Factory.IDataSourceFactory(agent).BindDataSource(Globals.BasicInfoTracker.GetBaseManagerCombList());
            if (Globals.Manager.Type != QSEnumManagerType.ROOT)
            {
                agent.SelectedValue = Globals.BaseMGRFK;
            }
            _gotdata = true;
        }

        /// <summary>
        /// 响应环境初始化完成事件 用于在环境初始化之前创立的空间获得对应的基础数据
        /// </summary>
        public void OnInitFinished()
        { 
            InitAgentList();
        }

        /// <summary>
        /// 获得当前选中的代理ID
        /// </summary>
        public int CurrentAgentFK
        {
            get
            {
                return int.Parse(agent.SelectedValue.ToString());
            }
        }

        private void agent_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            if (AgentSelectedChangedEvent != null && _gotdata)
                AgentSelectedChangedEvent();
        }

    }
}
