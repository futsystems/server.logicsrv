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
                argvalue.Text = _argument.ArgValue;
                argvalue.Enabled = _argument.Editable;
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
            _argument.ArgValue = argvalue.Text;
            return true;
        }
    }
}
