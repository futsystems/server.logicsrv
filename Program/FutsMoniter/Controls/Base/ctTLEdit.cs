using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.Mixins.JsonObject;


namespace FutsMoniter
{
    public partial class ctTLEdit : UserControl
    {

        JsonWrapperArgument _argument;
        public JsonWrapperArgument Argument
        { 
            get { return _argument; } 
            set { 
                _argument = value;
                argtitle.Text = _argument.ArgTitle;
                //如果不可编辑
                if (!Argument.Editable)
                {
                    argvalue.Visible = false;
                    argboolcheck.Visible = false;
                    argvalue_label.Visible = true;
                    argvalue_label.Text = _argument.ArgValue;
                }
                else
                {
                    if (_argument.ArgType.Equals("BOOLEAN"))
                    {
                        argboolcheck.Visible = true;
                        argvalue.Visible = false;
                        argboolcheck.Checked = _argument.ArgValue.Equals("True") ? true : false;
                    }
                    else
                    {
                        argvalue.Visible = true;
                        argboolcheck.Visible = false;
                        argvalue.Text = _argument.ArgValue;
                    }
                    argvalue_label.Visible = false;
                    
                }
                
            } 
        
        }
        public ctTLEdit()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 解析参数并验证有效性
        /// </summary>
        /// <returns></returns>
        public bool ParseArg()
        {
            string setvalue = string.Empty;
            if (!Argument.Editable)
            {
                _argument.ArgValue = argvalue_label.Text;
            }
            else
            {
                if (_argument.ArgType.Equals("BOOLEAN"))
                {
                    _argument.ArgValue = argboolcheck.Checked ? "True" : "False";
                }
                else
                {
                    _argument.ArgValue = argvalue.Text;
                }
                
            }
            return true;
        }

        public void DisableEdit()
        {
            argvalue.Enabled = false;
            argvalue_label.Enabled = false;
        }
    }
}
