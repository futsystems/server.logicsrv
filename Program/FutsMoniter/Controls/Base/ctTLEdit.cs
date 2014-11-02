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
                if (!Argument.Editable)
                {
                    argvalue.Visible = false;
                    argvalue_label.Visible = true;
                    argvalue_label.Text = _argument.ArgValue;
                }
                else
                {
                    argvalue.Visible = true;
                    argvalue_label.Visible = false;
                    argvalue.Text = _argument.ArgValue;
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

                _argument.ArgValue = argvalue.Text;
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
