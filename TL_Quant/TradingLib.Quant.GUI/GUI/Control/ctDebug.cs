using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;

namespace TradingLib.Quant.GUI
{
    public partial class ctDebug : UserControl
    {
        bool _timestamp = true;
        public bool TimeStamps { get { return _timestamp; } set { _timestamp = value; } }

        public ctDebug()
        {
            InitializeComponent();
        }

        public void GotDebug(string msg)
        {
            debug(msg);

        }
        public string Content { get { return sb.ToString(); } }
        StringBuilder sb = new StringBuilder();


        bool _eu = true;
        public void BeginUpdate()
        {
            if (InvokeRequired)
                Invoke(new VoidDelegate(BeginUpdate));
            else
            {
                _eu = false;
                _msg.BeginUpdate();
            }
        }

        public void EndUpdate()
        {
            if (InvokeRequired)
                Invoke(new VoidDelegate(BeginUpdate));
            else
            {
                _eu = true;
                _msg.EndUpdate();
                _msg.Invalidate();
            }
        }

        public void Clear()
        {
            if (_msg.InvokeRequired)
                Invoke(new VoidDelegate(Clear));
            else
            {

                _msg.Items.Clear();
                _msg.Invalidate(true);
                sb = new StringBuilder();
            }
        }


        delegate void stringdel(string msg);
        bool _useexttime = false;
        /// <summary>
        /// toggle whether an external time stamp is used (timestamps must be enabled)
        /// </summary>
        public bool UseExternalTimeStamp { get { return _useexttime; } set { _useexttime = value; } }
        int _exttime = 0;
        /// <summary>
        /// set an external time stamp
        /// </summary>
        public int ExternalTimeStamp { get { return _exttime; } set { _exttime = value; } }
        void debug(string msg)
        {
            if (_msg.InvokeRequired)
            {
                try
                {
                    _msg.Invoke(new stringdel(debug), new object[] { msg });
                }
                catch (ObjectDisposedException) { }
                catch (System.Threading.ThreadInterruptedException) { }
            }
            else
            {
                try
                {
                    if (!TimeStamps)
                        _msg.Items.Add(msg);
                    else if (UseExternalTimeStamp)
                        _msg.Items.Add(_exttime + ": " + msg);
                    else
                        _msg.Items.Add(DateTime.Now.ToString("HHmmss") + " " + DateTime.Now.Millisecond.ToString() + ": " + msg);
                    _msg.SelectedIndex = _msg.Items.Count - 1;
                    if (_eu)
                        _msg.Invalidate(true);
                    sb.AppendLine(msg.Replace(Environment.NewLine, string.Empty));
                }
                catch { }
            }
        }
    }
}
