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
        }

        void WireEvent()
        {
            Globals.RegIEventHandler(this);
            cbrglist.SelectedIndexChanged += new EventHandler(rglist_SelectedIndexChanged);
        }

        //bool _gotdata = false;
        void rglist_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RouterGroupSelectedChangedEvent != null && _gotrglist)
            {
                RouterGroupSelectedChangedEvent();
            }
        }

        //属性获得和设置
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
            }
        }


        public void OnInit()
        {
            Globals.LogicEvent.RegisterCallback("ConnectorManager", "QryRouterGroup", this.OnQryRouterGroup);
            Globals.LogicEvent.RegisterCallback("ConnectorManager", "NotifyRouterGroup", this.OnNotifyRouterGroup);
            Globals.TLClient.ReqQryRouterGroup();
        }

        public void OnDisposed()
        {
            Globals.LogicEvent.UnRegisterCallback("ConnectorManager", "QryRouterGroup", this.OnQryRouterGroup);
        }

        
        public void OnNotifyRouterGroup(string json)
        {
            RouterGroupSetting rg = MoniterUtils.ParseJsonResponse<RouterGroupSetting>(json);
            if (rg != null)
            {
                rgmap[rg.ID] = rg;
                InvokeGotRouterGroup(rgmap.Values.ToArray());
            }
            else//如果没有配资服
            {

            }
        }
        public event VoidDelegate RouterGroupInitEvent;
            
        bool _gotrglist = false;
        void OnQryRouterGroup(string json)
        {
            RouterGroupSetting[] objs = MoniterUtils.ParseJsonResponse<RouterGroupSetting[]>(json);
            if (objs != null)
            {
                foreach(RouterGroupSetting obj in objs)
                {
                    rgmap[obj.ID] = obj;
                }
                this.InvokeGotRouterGroup(objs);
                if (!_gotrglist)
                {
                    _gotrglist = true;
                    //对外触发初始化事件
                    if (RouterGroupInitEvent != null)
                        RouterGroupInitEvent();
                }
            }
            else//如果没有配资服
            {

            }
        }

        /// <summary>
        /// 获得某个路由组对应的名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetRrouterGroupName(int id)
        {
            //Globals.Debug("rg ID....:" + id.ToString());
            RouterGroupSetting rg= null;
            if (rgmap.TryGetValue(id, out rg))
            {
                return rg.Name;
            }
            return "";
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
                if (EnableAny)
                {
                    ValueObject<int> vo = new ValueObject<int>();
                    vo.Name = "<Any>";
                    vo.Value = 0;
                    list.Add(vo);
                }

                foreach (RouterGroupSetting rg in rglist)
                {
                    ValueObject<int> vo = new ValueObject<int>
                    {
                        Name = string.Format("{0}-{1}",rg.Name,rg.ID),
                        Value = rg.ID,
                    };
                    //Globals.Debug("it is here ----------------:"+rg.Name +" rg.id:"+rg.ID.ToString());
                    list.Add(vo);
                }

                Factory.IDataSourceFactory(cbrglist).BindDataSource(list);
                //_gotdata = true;
            }
        }

        public int SelectedIndex
        {
            get
            {
                return cbrglist.SelectedIndex;
            }
        }
        int _rgselected = 0;

        public int RouterGroupID
        {
            get
            {
                try
                {
                    return int.Parse(cbrglist.SelectedValue.ToString());
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
            set
            {
                try
                {
                    cbrglist.SelectedValue = value;
                }
                catch (Exception ex)
                { 
                    
                }
            }

        }
        public RouterGroupSetting RouterGroup
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
