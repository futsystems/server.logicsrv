using System;
using System.Collections.Generic;
using System.Collections;
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
using TradingLib.Mixins.JsonObject;

namespace FutsMoniter.Controls.Base
{
    public partial class ctRouterGroupList : UserControl,IEventBinder
    {
        public event VoidDelegate RouterGroupSelectedChangedEvent;
        public ctRouterGroupList()
        {
            InitializeComponent();

            this.Load += new EventHandler(ctRouterGroupList_Load);
        }

        void ctRouterGroupList_Load(object sender, EventArgs e)
        {
            WireEvent();
            if (Globals.EnvReady)
            {
                if (!_gotdata)//请求银行列表
                {
                    Globals.TLClient.ReqQryRouterGroup();
                }
            }
        }

        void WireEvent()
        {
            Globals.RegIEventHandler(this);
            cbrglist.SelectedIndexChanged += new EventHandler(rglist_SelectedIndexChanged);
        }

        bool _gotdata = false;
        void rglist_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RouterGroupSelectedChangedEvent != null && _gotdata)
            {
                RouterGroupSelectedChangedEvent();
            }
        }

        public void OnInit()
        {
            Globals.CallBackCentre.RegisterCallback("ConnectorManager", "QryRouterGroup", this.OnQryRouterGroup);
        }

        public void OnDisposed()
        {
            Globals.CallBackCentre.UnRegisterCallback("ConnectorManager", "QryRouterGroup", this.OnQryRouterGroup);
        }

        public void OnNotifyRouterGroup(string json)
        {
            JsonData jd = TradingLib.Mixins.JsonReply.ParseJsonReplyData(json);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                RouterGroupSetting rg = TradingLib.Mixins.JsonReply.ParsePlayload<RouterGroupSetting>(jd);
                rgmap[rg.ID] = rg;
                InvokeGotRouterGroup(rgmap.Values.ToArray());
            }
            else//如果没有配资服
            {

            }
        }
        void OnQryRouterGroup(string json)
        {
            JsonData jd = TradingLib.Mixins.JsonReply.ParseJsonReplyData(json);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                RouterGroupSetting[] objlist = TradingLib.Mixins.JsonReply.ParsePlayload<RouterGroupSetting[]>(jd);
                this.InvokeGotRouterGroup(objlist);
            }
            else//如果没有配资服
            {

            }
        }
        Dictionary<int, RouterGroupSetting> rgmap = new Dictionary<int, RouterGroupSetting>();
        void InvokeGotRouterGroup(RouterGroupSetting[] rglist)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<RouterGroupSetting[]>(InvokeGotRouterGroup), new object[] { rglist });
            }
            else
            {
                ArrayList list = new ArrayList();
                foreach (RouterGroupSetting rg in rglist)
                {
                    rgmap[rg.ID]=rg;
                    ValueObject<int> vo = new ValueObject<int>
                    {
                        
                        Name = string.Format("{0}-{1}",rg.Name,rg.ID),
                        Value = rg.ID,
                    };
                    Globals.Debug("it is here ----------------:"+rg.Name +" rg.id:"+rg.ID.ToString());
                    list.Add(vo);
                }

                Factory.IDataSourceFactory(cbrglist).BindDataSource(list);
                _gotdata = true;
            }
        }

        int _rgselected = 0;
        public RouterGroupSetting RouterGroudSelected
        {
           get
            {
                try
                {
                    int rgid =  int.Parse(cbrglist.SelectedValue.ToString());
                    if (rgmap.Keys.Contains(rgid))
                        return rgmap[rgid];
                    return null;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    _rgselected = value.ID;
                    cbrglist.SelectedValue = value.ID;
                }
                catch (Exception ex)
                { 
                    
                }
            }
        }
    }
}
