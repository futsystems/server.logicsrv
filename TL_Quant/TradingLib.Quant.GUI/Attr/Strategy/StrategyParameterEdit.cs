using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using TradingLib.Quant.Base;

namespace TradingLib.Quant.Common
{
    public class StrategyParameterEdit : UITypeEditor
    {

        StrategyParameterInfo parameter;
        public StrategyParameterEdit(StrategyParameterInfo p)
        {
            parameter = p;
        }

        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {

            return UITypeEditorEditStyle.Modal;

        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {

            IWindowsFormsEditorService edSvc =(IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

            if (edSvc != null)
            {
                ParameterEditUC pedit = new ParameterEditUC(edSvc,parameter);
                edSvc.DropDownControl(pedit);
                return pedit.Parameter;
            }

            return value;

        }

    }
}
