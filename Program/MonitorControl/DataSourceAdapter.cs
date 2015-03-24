using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using System.Windows.Forms;
using System.Collections;
using ComponentFactory.Krypton.Toolkit;

namespace TradingLib.MoniterControl
{
    public interface IDataSource
    {
        object DataSource { get; set; }
        string DisplayMember { get; set; }
        string ValueMember { get; set; }
        void BindDataSource(ArrayList list);
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


    public class KryptonComboBox2IDataSource : IDataSource
    {
        KryptonComboBox _lc;
        public KryptonComboBox2IDataSource(KryptonComboBox lc)
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

    public class CheckedListBox2IDataSource : IDataSource
    {
        CheckedListBox _clb;
        public CheckedListBox2IDataSource(CheckedListBox c)
        {
            _clb = c;
        }
        public object DataSource { get { return _clb.DataSource; } set { _clb.DataSource = value; } }
        public string DisplayMember { get { return _clb.DisplayMember; } set { _clb.DisplayMember = value; } }
        public string ValueMember { get { return _clb.ValueMember; } set { _clb.ValueMember = value; } }

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
    public class KryptonListBox2IDataSource : IDataSource
    {
        KryptonListBox _lc;
        public KryptonListBox2IDataSource(KryptonListBox lc)
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
