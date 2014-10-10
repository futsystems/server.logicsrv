using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;

namespace TradingLib.Quant.Common
{
    public class BarFrequencyEdit : UITypeEditor
    {
        StrategyBarFrequencyConverter _convert;
        public BarFrequencyEdit(StrategyBarFrequencyConverter convert)
        {
            _convert = convert;
        }

        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {

            return UITypeEditorEditStyle.Modal;

        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {

            IWindowsFormsEditorService edSvc =
                (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

            if (edSvc != null)
            {

                // 可以打开任何特定的对话框  
                /*
                OpenFileDialog dialog = new OpenFileDialog();

                dialog.AddExtension = false;

                if (dialog.ShowDialog().Equals(DialogResult.OK))
                {

                    return dialog.FileName;

                }**/

                CheckedListBoxUC chkListBoxUC = new CheckedListBoxUC(edSvc);
                edSvc.DropDownControl(chkListBoxUC);
                //MessageBox.Show(chkListBoxUC.Selected);
                //利用转换器将我们编辑设定的string转换成我们需要的类型
                return _convert.ConvertFrom(chkListBoxUC.Selected);
                //return chkListBoxUC.Selected;
                
            }

            return value;

        }

    }
}
