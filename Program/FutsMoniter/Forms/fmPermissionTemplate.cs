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
            btnSubmit.Click += new EventHandler(btnSubmit_Click);
            btnSaveAs.Click += new EventHandler(btnSaveAs_Click);

        }

        void btnSaveAs_Click(object sender, EventArgs e)
        {
            UIAccess access = new UIAccess();
            access.id = int.Parse(pmlist.SelectedValue.ToString());

            foreach (string key in permissionmap.Keys)
            {
                ctTLPermissionEdit edit = permissioneditmap[key];
                PermissionField field = permissionmap[key];

                //设置值
                field.Info.SetValue(access, edit.Value, null);
            }
            access.id = 0;//用于新建，设置id为0
            access.name = pmname.Text;
            access.desp = pmdesp.Text;
            Globals.TLClient.ReqUpdatePermissionTemplate(access.ToJson());
        }

        void btnSubmit_Click(object sender, EventArgs e)
        {
            UIAccess access = new UIAccess();
            access.id = int.Parse(pmlist.SelectedValue.ToString());

            foreach (string key in permissionmap.Keys)
            {
                ctTLPermissionEdit edit = permissioneditmap[key];
                PermissionField field = permissionmap[key];
                
                //设置值
                field.Info.SetValue(access,edit.Value,null);
            }
            access.name = pmname.Text;
            access.desp = pmdesp.Text;

            Globals.TLClient.ReqUpdatePermissionTemplate(access.ToJson());
        }




        void pmlist_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_loaded) return;
            int id = int.Parse(pmlist.SelectedValue.ToString());
            UIAccess access = accessmap[id];
            
            if (access != null)
            {
                foreach (string key in permissionmap.Keys)
                {
                    ctTLPermissionEdit edit = permissioneditmap[key];
                    PermissionField field = permissionmap[key];
                    edit.Value = (bool)field.Info.GetValue(access, null);
                }
                pmname.Text = access.name;
                pmdesp.Text = access.desp;
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

        bool _loaded = false;
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
                _loaded = true;
            }
            
        }

        Dictionary<int, UIAccess> accessmap = new Dictionary<int, UIAccess>();
        void GotUIAccess(UIAccess access)
        {
            accessmap.Add(access.id, access);
        }


        ArrayList GetPermissionTemplateListCB()
        {
            ArrayList list = new ArrayList();
            
            foreach(UIAccess access in accessmap.Values)
            {
                ValueObject<int> vo = new ValueObject<int>();
                vo.Name = access.id.ToString()+"-"+access.name;
                
                vo.Value = access.id;
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
