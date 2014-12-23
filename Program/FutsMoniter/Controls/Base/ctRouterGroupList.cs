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
            
            cbrglist.SelectedIndexChanged += new EventHandler(rglist_SelectedIndexChanged);
            Globals.RegIEventHandler(this);
        }

        void rglist_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RouterGroupSelectedChangedEvent != null)
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
            ReloadRouterGroupList();
            Globals.BasicInfoTracker.GotRouterGroupEvent += new Action<RouterGroupSetting>(BasicInfoTracker_GotRouterGroupEvent);

        }

        void BasicInfoTracker_GotRouterGroupEvent(RouterGroupSetting obj)
        {
            ReloadRouterGroupList();
        }

        public void OnDisposed()
        {
            Globals.BasicInfoTracker.GotRouterGroupEvent -= new Action<RouterGroupSetting>(BasicInfoTracker_GotRouterGroupEvent);
        }

        


        /// <summary>
        /// 加载路由组列表
        /// </summary>
        void ReloadRouterGroupList()
        {
            ArrayList list = new ArrayList();
            if (EnableAny)
            {
                ValueObject<int> vo = new ValueObject<int>();
                vo.Name = "<Any>";
                vo.Value = 0;
                list.Add(vo);
            }

            foreach (RouterGroupSetting rg in Globals.BasicInfoTracker.RouterGroups)
            {
                ValueObject<int> vo = new ValueObject<int>
                {
                    Name = string.Format("{0}-{1}", rg.Name, rg.ID),
                    Value = rg.ID,
                };
                list.Add(vo);
            }
            if (list.Count > 0)//如果list为空则绑定下拉列表时会出错
            {
                Factory.IDataSourceFactory(cbrglist).BindDataSource(list);
            }
        }


        public int SelectedIndex
        {
            get
            {
                return cbrglist.SelectedIndex;
            }
        }

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
                    return Globals.BasicInfoTracker.GetRouterGroup(rgid);
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
    }
}
