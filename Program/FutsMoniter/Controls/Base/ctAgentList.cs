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
using TradingLib.Common;
using TradingLib.Mixins.LitJson;


namespace FutsMoniter
{
    public partial class ctAgentList : UserControl
    {
        public event VoidDelegate AgentSelectedChangedEvent;
        bool _gotdata = false;

        //属性获得和设置
        [DefaultValue(true)]
        bool _enableselected = true;
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

        /// <summary>
        /// 是否允许显示Any 即所有代理商
        /// 比如查询时查询所有统计 则选择Any 帐户列表过滤时候选择Any则表示不用代理上过滤
        /// </summary>
        [DefaultValue(false)]
        bool _enableany = false;
        public bool EnableAny
        {
            get
            {
                return _enableany;
            }
            set
            {
                _enableany = value;
                if (Globals.EnvReady)
                {
                    InitAgentList();
                }
                //agent.Enabled = _enableselected;
            }
        }

        [DefaultValue(true)]
        bool _defaultbasemgr = true;
        public bool EnableDefaultBaseMGR
        {
            get
            {
                return _defaultbasemgr;
            }
            set
            {
                _defaultbasemgr = value;
                if (Globals.EnvReady)
                {
                    InitAgentList();
                }
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
                if (Globals.CallbackCentreReady)//回调初始化后在加入回调，当系统初始化完毕后 通过回调更新列表
                {
                    Globals.RegInitCallback(OnInitFinished);

                    //订阅Manager变动事件
                    Globals.CallBackCentre.RegisterCallback("MgrExchServer", "NotifyManagerUpdate", OnManagerNotify);
                }
            }
            this.Disposed += new EventHandler(ctAgentList_Disposed);
            this.Load += new EventHandler(ctAgentList_Load);
        }

        void OnManagerNotify(string jsonstr)
        {
            JsonData jd = TradingLib.Mixins.LitJson.JsonMapper.ToObject(jsonstr);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                Manager obj = TradingLib.Mixins.LitJson.JsonMapper.ToObject<Manager>(jd["Playload"].ToJson());
                InitAgentList();
            }
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
                Globals.CallBackCentre.UnRegisterCallback("MgrExchServer", "NotifyManagerUpdate", OnManagerNotify);
                //Globals.CallBackCentre.UnRegisterCallback("FinServiceCentre", "QryFinServicePlan", OnInitFinished);
            }
        }

        void InitAgentList()
        {
            Factory.IDataSourceFactory(agent).BindDataSource(Globals.BasicInfoTracker.GetBaseManagerCombList(_enableany));
            if (Globals.Manager.Type != QSEnumManagerType.ROOT)
            {
                if (_defaultbasemgr)//如果默认选择当前域 则设置selectedvalue
                {
                    agent.SelectedValue = Globals.BaseMGRFK;
                }
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

        private void agent_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AgentSelectedChangedEvent != null && _gotdata)
                AgentSelectedChangedEvent();
        }

    }
}
