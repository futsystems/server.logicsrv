using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.LitJson;
using TradingLib.Mixins.JsonObject;
using FutSystems.GUI;



namespace FutsMoniter
{

    public partial class fmPermissionTemplate : ComponentFactory.Krypton.Toolkit.KryptonForm,IEventBinder
    {
        public fmPermissionTemplate()
        {
            InitializeComponent();
            
            InitLaylout();
            if (Globals.EnvReady)
            {
                Globals.TLClient.ReqQryPermmissionTemplateList();
            }
            Globals.RegIEventHandler(this);
            pmlist.SelectedIndexChanged += new EventHandler(pmlist_SelectedIndexChanged);
        }

        void pmlist_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = int.Parse(pmlist.SelectedValue.ToString());
            UIAccess access = accessmap[id];
            
            if (access == null)
            { 
                    
            }
        
        }





        public void OnInit()
        {
            Globals.CallBackCentre.RegisterCallback("MgrExchServer", "QueryPermmissionTemplateList", OnPermissionTemplate);
        }

        public void OnDisposed()
        {
            Globals.CallBackCentre.UnRegisterCallback("MgrExchServer", "QueryPermmissionTemplateList", OnPermissionTemplate);
        }


        void OnPermissionTemplate(string jsonstr)
        {
            JsonData jd = TradingLib.Mixins.LitJson.JsonMapper.ToObject(jsonstr);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                UIAccess[] objs = TradingLib.Mixins.LitJson.JsonMapper.ToObject<UIAccess[]>(jd["Playload"].ToJson());
                foreach (UIAccess access in objs)
                {
                    GotUIAccess(access);
                }

                Factory.IDataSourceFactory(pmlist).BindDataSource(GetPermissionTemplateListCB());
            }
        }

        Dictionary<int, UIAccess> accessmap = new Dictionary<int, UIAccess>();
        void GotUIAccess(UIAccess access)
        {
            accessmap.Add(access.ID, access);
        }


        ArrayList GetPermissionTemplateListCB()
        {
            ArrayList list = new ArrayList();
            
            foreach(UIAccess access in accessmap.Values)
            {
                ValueObject<int> vo = new ValueObject<int>();
                vo.Name = access.Name;
                vo.Value = access.ID;
                list.Add(vo);
            }
            return list;
        }



        Dictionary<string, PermissionField> permissionmap = new Dictionary<string, PermissionField>();
        Dictionary<string, ctTLPermissionEdit> permissioneditmap = new Dictionary<string, ctTLPermissionEdit>();
        void InitLaylout()
        {
            Type type = typeof(UIAccess);
            List<PropertyInfo> list = new List<PropertyInfo>();
            PropertyInfo[] propertyInfos = type.GetProperties();
            foreach (PropertyInfo pi in propertyInfos)
            {
                PermissionFieldAttr attr = (PermissionFieldAttr)Attribute.GetCustomAttribute(pi, typeof(PermissionFieldAttr));
                if (attr != null)
                {
                    list.Add(pi);
                    permissionmap.Add(pi.Name, new PermissionField(pi, attr));

                    ctTLPermissionEdit edit = new ctTLPermissionEdit();
                    edit.PermissionTitle = attr.Title;
                    edit.PermissionDesp = attr.Desp;
                    laylout.Controls.Add(edit);
                    permissioneditmap.Add(pi.Name, edit);
                }
            }
        }
    }

    internal class PermissionField
    {
        public PermissionField(PropertyInfo info, PermissionFieldAttr attr)
        {
            this.Info = info;
            this.Attr = attr;
        }
        public PropertyInfo Info { get; set; }
        public PermissionFieldAttr Attr { get; set; }
    }
}
