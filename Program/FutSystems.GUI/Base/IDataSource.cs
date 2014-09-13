using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using System.Collections;

namespace FutSystems.GUI
{
    public interface IDataSource
    {
        object DataSource { get; set; }
        string DisplayMember { get; set; }
        string ValueMember { get; set; }
        void BindDataSource(ArrayList list);
    }
    /// <summary>
    /// 实现了DataSource DisplayMember ValueMember 方便GUI绑定对应的数据
    /// 将多个控件 适配 到接口 IDataSource
    /// 方法一实现多个适配器然后再调用的时候手工针对不同的实例调用不同的适配器
    /// 方法二通过统一的接口来自动找到其对应的适配器
    /// </summary>
    /// 
    public class Factory
    {
        public static IDataSource IDataSourceFactory(object obj)
        {

            if (obj is RadDropDownList)
                return new RadDropDownList2IDataSource(obj as RadDropDownList);
            else if (obj is RadListControl)
                return new RadListControl2IDataSource(obj as RadListControl);
            else if (obj is ListBox)
                return new ListBox2IDataSource(obj as ListBox);
            else if (obj is ComboBox)
                return new ComboBox2IDataSource(obj as ComboBox);
            return new Invalid2IDataSource(); ;
        }
    }
    public class Invalid2IDataSource : IDataSource
    {

        public Invalid2IDataSource()
        {

        }

        public object DataSource { get; set; }
        public string DisplayMember { get; set; }
        public string ValueMember { get; set; }

        /// <summary>
        /// 绑定对应的数据
        /// </summary>
        /// <param name="list"></param>
        public void BindDataSource(ArrayList list)
        {
            this.DataSource = list;
            this.ValueMember = "Value";
            this.DisplayMember = "Name";

        }
    }
    
    public class RadDropDownList2IDataSource : IDataSource
    {
        RadDropDownList _lc;
        public RadDropDownList2IDataSource(RadDropDownList lc)
        {
            _lc = lc;
            
        }

        public object DataSource { get { return _lc.DataSource; } set { _lc.DataSource = value; } }
        public string DisplayMember { get { return _lc.DisplayMember; } set { _lc.DisplayMember = value; } }
        public string ValueMember { get { return _lc.ValueMember; } set { _lc.ValueMember = value; } }

        /// <summary>
        /// 绑定对应的数据
        /// </summary>
        /// <param name="list"></param>
        public void BindDataSource(ArrayList list)
        {
            this.DataSource = list;
            this.ValueMember = "Value";
            this.DisplayMember = "Name";

        }
    }

    public class RadListControl2IDataSource : IDataSource
    {
        RadListControl _lc;
        public RadListControl2IDataSource(RadListControl lc)
        {
            _lc = lc;
        }

        public object DataSource { get { return _lc.DataSource; } set { _lc.DataSource = value; } }
        public string DisplayMember { get { return _lc.DisplayMember; } set { _lc.DisplayMember = value; } }
        public string ValueMember { get { return _lc.ValueMember; } set { _lc.ValueMember = value; } }

        /// <summary>
        /// 绑定对应的数据
        /// </summary>
        /// <param name="list"></param>
        public void BindDataSource(ArrayList list)
        {
            this.DataSource = list;
            this.ValueMember = "Value";
            this.DisplayMember = "Name";

        }
    }

    public class ListBox2IDataSource : IDataSource
    {
        ListBox _lc;
        public ListBox2IDataSource(ListBox lc)
        {
            _lc = lc;
        }

        public object DataSource { get { return _lc.DataSource; } set { _lc.DataSource = value; } }
        public string DisplayMember { get { return _lc.DisplayMember; } set { _lc.DisplayMember = value; } }
        public string ValueMember { get { return _lc.ValueMember; } set { _lc.ValueMember = value; } }

        /// <summary>
        /// 绑定对应的数据
        /// </summary>
        /// <param name="list"></param>
        public void BindDataSource(ArrayList list)
        {
            this.DataSource = list;
            this.ValueMember = "Value";
            this.DisplayMember = "Name";

        }
    }

    public class ComboBox2IDataSource : IDataSource
    {
        ComboBox _lc;
        public ComboBox2IDataSource(ComboBox lc)
        {
            _lc = lc;
        }

        public object DataSource { get { return _lc.DataSource; } set { _lc.DataSource = value; } }
        public string DisplayMember { get { return _lc.DisplayMember; } set { _lc.DisplayMember = value; } }
        public string ValueMember { get { return _lc.ValueMember; } set { _lc.ValueMember = value; } }

        /// <summary>
        /// 绑定对应的数据
        /// </summary>
        /// <param name="list"></param>
        public void BindDataSource(ArrayList list)
        {
            this.DataSource = list;
            this.ValueMember = "Value";
            this.DisplayMember = "Name";

        }
    }
}
