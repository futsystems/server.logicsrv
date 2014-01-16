using System;
using System.Collections.Generic;
using System.Collections;
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
using TradingLib.Mixins;
using TradingLib.Mixins.LitJson;
using TradingLib.Mixins.JsonObject;

namespace FutsMoniter
{
    public partial class fmDomainEdit : ComponentFactory.Krypton.Toolkit.KryptonForm,IEventBinder
    {
        public fmDomainEdit()
        {
            InitializeComponent();
            this.Load += new EventHandler(fmDomainEdit_Load);
            this.Text = "添加域分区";
        }

        void fmDomainEdit_Load(object sender, EventArgs e)
        {
            WireEvent();
            if (Globals.EnvReady)
            {
                Globals.TLClient.ReqQryInterface();
                Globals.TLClient.ReqQryServicePlan();
            }
        }

        void WireEvent()
        {
            Globals.RegIEventHandler(this);
            module_finservice.CheckedChanged += new EventHandler(module_finservice_CheckedChanged);
            module_agent.CheckedChanged += new EventHandler(module_agent_CheckedChanged);
        }

        void module_agent_CheckedChanged(object sender, EventArgs e)
        {
            module_subagent.Enabled = module_agent.Checked;
            if (!module_agent.Checked)
            {
                module_subagent.Checked = false;
            }
        }

        void module_finservice_CheckedChanged(object sender, EventArgs e)
        {
            splisttab.Enabled = module_finservice.Checked;
        }

        public void OnInit()
        {
            Globals.LogicEvent.RegisterCallback("ConnectorManager", "QryInterface", this.OnQryInterface);
            Globals.LogicEvent.RegisterCallback("FinServiceCentre", "QryFinServicePlan", this.OnQryFinServicePlan);
        }

        public void OnDisposed()
        {
            Globals.LogicEvent.UnRegisterCallback("ConnectorManager", "QryInterface", this.OnQryInterface);
            Globals.LogicEvent.UnRegisterCallback("FinServiceCentre", "QryFinServicePlan", this.OnQryFinServicePlan);
        }

        #region 底层成交接口
        ArrayList GetInterfaceList()
        {
            ArrayList list = new System.Collections.ArrayList();
            foreach (ConnectorInterface it in interfacemap.Values)
            {
                ValueObject<int> vo = new ValueObject<int>();
                vo.Name = it.Name;
                vo.Value = it.ID;
                list.Add(vo);
            }
            return list;
        }

        string GetInterfaceListString()
        {
            List<string> list = new List<string>();
            foreach(object obj in interfacelist.CheckedItems)
            {
                ValueObject<int> item = (ValueObject<int>)obj;
                list.Add(item.Value.ToString());
            }
            return string.Join(",", list.ToArray());
        }

        void SetInterfaceList(string list)
        {
            IEnumerable<int> clist = list.Split(',').Select(v => int.Parse(v));
            for(int i =0;i<interfacelist.Items.Count;i++)
            { 
                ValueObject<int> item = (ValueObject<int>)interfacelist.Items[i];
                if(clist.Contains(item.Value))
                {
                    interfacelist.SetItemChecked(i, true);
                }
            }
        }
        ConcurrentDictionary<int, ConnectorInterface> interfacemap = new ConcurrentDictionary<int, ConnectorInterface>();
        bool _gotinterface = false;

        void OnQryInterface(string jsonstr)
        {
            JsonData jd = TradingLib.Mixins.JsonReply.ParseJsonReplyData(jsonstr);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                ConnectorInterface[] objs = TradingLib.Mixins.JsonReply.ParsePlayload<ConnectorInterface[]>(jd);
                foreach (ConnectorInterface op in objs)
                {
                    if (!interfacemap.Keys.Contains(op.ID))
                    {
                        interfacemap.TryAdd(op.ID, op);
                        //interfacelist.Items.Add(op.Name);
                    }
                }
                _gotinterface = true;
                
                Factory.IDataSourceFactory(interfacelist).BindDataSource(GetInterfaceList());
                if(_domain != null)
                {
                    SetInterfaceList(_domain.InterfaceList);
                }
            }
            else//如果没有配资服
            {

            }
        }
        #endregion


        #region 配资服务计划
        string GetSPListString()
        {
            List<string> list = new List<string>();
            foreach (object obj in finsplist.CheckedItems)
            {
                ValueObject<int> item = (ValueObject<int>)obj;
                list.Add(item.Value.ToString());
            }
            return string.Join(",", list.ToArray());
        }

        void SetSPList(string list)
        {
            IEnumerable<int> clist = list.Split(',').Select(v => int.Parse(v));
            for (int i = 0; i < interfacelist.Items.Count; i++)
            {
                ValueObject<int> item = (ValueObject<int>)finsplist.Items[i];
                if (clist.Contains(item.Value))
                {
                    finsplist.SetItemChecked(i, true);
                }
            }
        }

        ArrayList GetSPList()
        {
            ArrayList list = new System.Collections.ArrayList();
            foreach (JsonWrapperServicePlane it in servicePlanMap.Values)
            {
                ValueObject<int> vo = new ValueObject<int>();
                vo.Name = string.Format("{0}[{1}]", it.Title, it.Name);
                vo.Value = it.ID;
                list.Add(vo);
            }
            return list;
        }

        ConcurrentDictionary<int, JsonWrapperServicePlane> servicePlanMap = new ConcurrentDictionary<int, JsonWrapperServicePlane>();
        bool _gotsplist = false;
        void OnQryFinServicePlan(string jsonstr)
        {
            //MessageBox.Show(jsonstr);
            JsonData jd = TradingLib.Mixins.JsonReply.ParseJsonReplyData(jsonstr);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                //Globals.Debug("domain edit got serviceplane ...........");
                JsonWrapperServicePlane[] objs = TradingLib.Mixins.JsonReply.ParsePlayload<JsonWrapperServicePlane[]>(jd);
                foreach (JsonWrapperServicePlane sp in objs)
                {
                    if (!servicePlanMap.Keys.Contains(sp.ID))
                    {
                        servicePlanMap.TryAdd(sp.ID, sp);
                    }
                }
                _gotsplist = true;

                Factory.IDataSourceFactory(finsplist).BindDataSource(GetSPList());
                if (_domain != null)
                {
                    SetSPList(_domain.FinSPList);
                }
            }
            else//如果没有配资服
            {

            }
        }
        #endregion

        DomainImpl _domain = null;
        public void SetDomain(DomainImpl domain)
        {
            _domain = domain;
            domainid.Text = _domain.ID.ToString();
            name.Text = _domain.Name;
            linkman.Text = _domain.LinkMan;
            mobile.Text = _domain.Mobile;
            qq.Text = _domain.QQ;
            email.Text = _domain.Email;

            dateexpired.Value = _domain.DateExpired==0? (DateTime.Now+ new TimeSpan(1000,0,0,0,0)):Util.ToDateTime(_domain.DateExpired, 1);
            acclimit.Value = _domain.AccLimit==0?acclimit.Maximum:_domain.AccLimit;
            routergrouplimit.Value = _domain.RouterGroupLimit == 0 ? routergrouplimit.Maximum : _domain.RouterGroupLimit;
            routeritemlimit.Value = _domain.RouterItemLimit == 0 ? routeritemlimit.Maximum : _domain.RouterItemLimit;
            vendorlimit.Value = _domain.VendorLimit;

            module_agent.Checked = _domain.Module_Agent;
            module_subagent.Checked = !module_agent.Checked ? false : _domain.Module_SubAgent;

            module_finservice.Checked = _domain.Module_FinService;
            module_payonline.Checked = _domain.Module_PayOnline;
            router_live.Checked = _domain.Router_Live;
            router_sim.Checked = _domain.Router_Sim;
            splisttab.Enabled = _domain.Module_FinService;

            this.Text = "编辑域分区:" + _domain.Name;

        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (_domain != null)
            {
                _domain.Name = name.Text;
                _domain.LinkMan = linkman.Text;
                _domain.Mobile = mobile.Text;
                _domain.QQ = qq.Text;
                _domain.Email = email.Text;

                _domain.DateExpired = Util.ToTLDate(dateexpired.Value);
                _domain.AccLimit = (int)acclimit.Value;
                _domain.RouterGroupLimit = (int)routergrouplimit.Value;
                _domain.RouterItemLimit = (int)routeritemlimit.Value;
                _domain.VendorLimit = (int)vendorlimit.Value;

                _domain.InterfaceList = GetInterfaceListString();
                _domain.FinSPList = GetSPListString();

                _domain.Module_Agent = module_agent.Checked;
                _domain.Module_SubAgent = module_agent.Checked?module_subagent.Checked:false;

                _domain.Module_FinService = module_finservice.Checked;
                _domain.Module_PayOnline = module_payonline.Checked;

                _domain.Router_Live = router_live.Checked;
                _domain.Router_Sim = router_sim.Checked;

                if (fmConfirm.Show("确认更新分区设置?") == System.Windows.Forms.DialogResult.Yes)
                {
                    Globals.TLClient.ReqUpdateDomain(_domain);
                    this.Close();
                }
            }
            else
            {
                _domain = new DomainImpl();
                _domain.Name = name.Text;
                _domain.LinkMan = linkman.Text;
                _domain.Mobile = mobile.Text;
                _domain.QQ = qq.Text;
                _domain.Email = email.Text;

                _domain.DateExpired = Util.ToTLDate(dateexpired.Value);
                _domain.AccLimit = (int)acclimit.Value;
                _domain.RouterGroupLimit = (int)routergrouplimit.Value;
                _domain.RouterItemLimit = (int)routeritemlimit.Value;
                _domain.VendorLimit = (int)vendorlimit.Value;


                _domain.InterfaceList = GetInterfaceListString();
                _domain.FinSPList = GetSPListString();

                _domain.Module_Agent = module_agent.Checked;
                _domain.Module_SubAgent = module_agent.Checked ? module_subagent.Checked : false;

                _domain.Module_FinService = module_finservice.Checked;
                _domain.Module_PayOnline = module_payonline.Checked;

                _domain.Router_Live = router_live.Checked;
                _domain.Router_Sim = router_sim.Checked;

                if ( (!_domain.Router_Live)&& (!_domain.Router_Sim))
                {
                    ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("需启用1种或全部成交方式");
                    return;
                }
                if (fmConfirm.Show("确认添加分区?") == System.Windows.Forms.DialogResult.Yes)
                {
                    Globals.TLClient.ReqUpdateDomain(_domain);
                    this.Close();
                }
            }

        }
    }
}
