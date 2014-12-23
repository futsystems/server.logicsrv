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

            this.Load += new EventHandler(fmPermissionTemplate_Load);
        }

        void fmPermissionTemplate_Load(object sender, EventArgs e)
        {
            pmlist.SelectedIndexChanged += new EventHandler(pmlist_SelectedIndexChanged);
            btnSubmit.Click += new EventHandler(btnSubmit_Click);
            btnSaveAs.Click += new EventHandler(btnSaveAs_Click);
            Globals.RegIEventHandler(this);
        }

        void btnSaveAs_Click(object sender, EventArgs e)
        {
            UIAccess access = new UIAccess();
            if (fmConfirm.Show("确认保存权限模板为:" + pmname.Text+"?") == System.Windows.Forms.DialogResult.Yes)
            {
                if (string.IsNullOrEmpty(pmname.Text))
                {
                    ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("请输入模板名称");
                    return;
                }
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
        }

        void btnSubmit_Click(object sender, EventArgs e)
        {
            if (fmConfirm.Show("确认更新权限模板?") == System.Windows.Forms.DialogResult.Yes)
            {
                UIAccess access = new UIAccess();
                int id = 0;
                int.TryParse(pmlist.SelectedValue.ToString(), out id);
                access.id = id;
                if (access.id == 0)
                {
                    ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("没有选中权限模板");
                    return;
                }
                foreach (string key in permissionmap.Keys)
                {
                    ctTLPermissionEdit edit = permissioneditmap[key];
                    PermissionField field = permissionmap[key];

                    //设置值
                    field.Info.SetValue(access, edit.Value, null);
                }
                access.name = pmname.Text;
                access.desp = pmdesp.Text;

                Globals.TLClient.ReqUpdatePermissionTemplate(access.ToJson());
            }
        }

        void pmlist_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_loaded) return;
            int id = int.Parse(pmlist.SelectedValue.ToString());
            if (id == 0) return;
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
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "NotifyUIAccess", OnNotifyPermissionTemplate);
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "QueryPermmissionTemplateList", OnQryPermissionTemplate);
            
            Globals.TLClient.ReqQryPermmissionTemplateList();
        }

        public void OnDisposed()
        {
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "NotifyUIAccess", OnNotifyPermissionTemplate);
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "QueryPermmissionTemplateList", OnQryPermissionTemplate);
            
        }

        void OnNotifyPermissionTemplate(string json)
        {
            UIAccess obj = MoniterUtils.ParseJsonResponse<UIAccess>(json);
            if (obj != null)
            {
                GotUIAccess(obj);
                pmlist_SelectedIndexChanged(null, null);
            }
        }


        bool _loaded = false;
        void OnQryPermissionTemplate(string jsonstr)
        {
            UIAccess[] objs = MoniterUtils.ParseJsonResponse<UIAccess[]>(jsonstr);
            if (objs != null)
            {
                foreach (UIAccess access in objs)
                {
                    GotUIAccess(access);
                }

                Factory.IDataSourceFactory(pmlist).BindDataSource(GetPermissionTemplateListCB());
                pmlist.SelectedIndex = 0;
                _loaded = true;
            }
        }


        Dictionary<int, UIAccess> accessmap = new Dictionary<int, UIAccess>();
        void GotUIAccess(UIAccess access)
        {
            if (accessmap.Keys.Contains(access.id))
            {
                accessmap[access.id] = access;
            }
            else
            {
                accessmap.Add(access.id, access);
                Factory.IDataSourceFactory(pmlist).BindDataSource(GetPermissionTemplateListCB());
                pmlist.SelectedIndex = 0;
            }
        }


        ArrayList GetPermissionTemplateListCB()
        {
            ArrayList list = new ArrayList();

            ValueObject<int> va = new ValueObject<int>();
            va.Name = "<请选择>";
            va.Value = 0;
            list.Add(va);

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
