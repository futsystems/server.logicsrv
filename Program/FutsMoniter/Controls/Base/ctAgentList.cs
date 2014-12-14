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
    public partial class ctAgentList : UserControl,IEventBinder
    {
        public event VoidDelegate AgentSelectedChangedEvent;
        bool _gotdata = false;

        //属性获得和设置
        [DefaultValue(true)]
        bool _enableself = true;
        public bool EnableSelf
        {
            get
            {
                return _enableself;
            }
            set
            {
                _enableself = value;
            }
        }


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
                    ReloadList();
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
                    ReloadList();
                }
            }
        }


        public ctAgentList()
        {
            InitializeComponent();

            Globals.RegIEventHandler(this);
            this.Load += new EventHandler(ctAgentList_Load);
        }

        void OnManagerNotify(string jsonstr)
        {
            JsonData jd = TradingLib.Mixins.LitJson.JsonMapper.ToObject(jsonstr);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                Manager obj = TradingLib.Mixins.LitJson.JsonMapper.ToObject<Manager>(jd["Playload"].ToJson());
                ReloadList();
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



        public void OnInit()
        {
            Globals.Debug("agentlist oninit called....");
            ReloadList();
            if (Globals.Manager.Type != QSEnumManagerType.ROOT)
            {
                if (_defaultbasemgr)//如果默认选择当前域 则设置selectedvalue
                {
                    agent.SelectedValue = Globals.BaseMGRFK;
                }
            }

            Globals.LogicEvent.RegisterCallback("MgrExchServer", "NotifyManagerUpdate", OnManagerNotify);
            _gotdata = true;
        }

        public void OnDisposed()
        {
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "NotifyManagerUpdate", OnManagerNotify);
        }


        void ReloadList()
        {
            Factory.IDataSourceFactory(agent).BindDataSource(Globals.BasicInfoTracker.GetBaseManagerCombList(_enableany, _enableself));
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
