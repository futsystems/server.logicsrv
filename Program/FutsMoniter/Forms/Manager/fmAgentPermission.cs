using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.JsonObject;
using FutSystems.GUI;


namespace FutsMoniter
{
    public partial class fmAgentPermission : ComponentFactory.Krypton.Toolkit.KryptonForm,IEventBinder
    {
        public fmAgentPermission()
        {
            InitializeComponent();

            this.Load += new EventHandler(fmAgentPermission_Load);
            
        }

        void fmAgentPermission_Load(object sender, EventArgs e)
        {
            ctAgentList3.AgentSelectedChangedEvent += new VoidDelegate(ctAgentList1_AgentSelectedChangedEvent);
            pmlist.SelectedIndexChanged += new EventHandler(pmlist_SelectedIndexChanged);
            btnSubmit.Click += new EventHandler(btnSubmit_Click);
            Globals.RegIEventHandler(this);
        }

        void btnSubmit_Click(object sender, EventArgs e)
        {
            if (MoniterUtils.WindowConfirm("确认更新的代理的权限模板?") == System.Windows.Forms.DialogResult.Yes)
            {
                int idx = int.Parse(pmlist.SelectedValue.ToString());
                Globals.TLClient.ReqUpdateAgentPermission(ctAgentList3.CurrentAgentFK, idx);
            }
        }

        void pmlist_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_loaded) return;
            int idx = int.Parse(pmlist.SelectedValue.ToString());
            pmdesp.Text = accessmap[idx].desp;
        }

        void ctAgentList1_AgentSelectedChangedEvent()
        {
            if (!_loaded) return;
            Globals.TLClient.ReqQryAgentPermission(ctAgentList3.CurrentAgentFK);
        }

        public void OnInit()
        {
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "QueryPermmissionTemplateList", OnPermissionTemplate);
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "QueryAgentPermission", OnAgentPermission);
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "NotifyAgentPermission", OnAgentPermission);

            



            Globals.TLClient.ReqQryPermmissionTemplateList();
        }

        public void OnDisposed()
        {
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "QueryPermmissionTemplateList", OnPermissionTemplate);
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "QueryAgentPermission", OnAgentPermission);
        }


        void OnAgentPermission(string jsonstr)
        {
            //JsonData jd = TradingLib.Mixins.LitJson.JsonMapper.ToObject(jsonstr);
            //int code = int.Parse(jd["Code"].ToString());
            UIAccess obj = MoniterUtils.ParseJsonResponse<UIAccess>(jsonstr);
            if (obj != null)
            {   
                pmcurrent.Text = obj.name;
            }
            else
            {
                pmcurrent.Text = "未设置";
            }
        }

        bool _loaded = false;
        void OnPermissionTemplate(string jsonstr)
        {
            UIAccess[] objs = MoniterUtils.ParseJsonResponse<UIAccess[]>(jsonstr);
            if (objs!= null)
            {
                foreach (UIAccess access in objs)
                {
                    GotUIAccess(access);
                }

                Factory.IDataSourceFactory(pmlist).BindDataSource(GetPermissionTemplateListCB());
                _loaded = true;
            }

        }

        Dictionary<int, UIAccess> accessmap = new Dictionary<int, UIAccess>();
        void GotUIAccess(UIAccess access)
        {
            if (!accessmap.Keys.Contains(access.id))
            {
                accessmap.Add(access.id, access);
            }
            else
            {
                accessmap[access.id] = access;
            }
            
        }


        ArrayList GetPermissionTemplateListCB()
        {
            ArrayList list = new ArrayList();

            foreach (UIAccess access in accessmap.Values)
            {
                ValueObject<int> vo = new ValueObject<int>();
                vo.Name = access.id.ToString() + "-" + access.name;

                vo.Value = access.id;
                list.Add(vo);
            }
            return list;
        }


    }
}
