using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.Globalization;

namespace TradingLib.Quant.ChartObjects
{
    public class FilePickUITypeEditor : UITypeEditor
    {
        // Methods
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (((IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService))) == null)
            {
                return null;
            }
            OpenFileDialog dialog = new OpenFileDialog
            {
                CheckFileExists = true,
                FilterIndex = 1,
                RestoreDirectory = true,
                Title = "Select the file for the property"
            };
            if ((value != null) && (value is string))
            {
                string path = value.ToString();
                if (Path.IsPathRooted(path))
                {
                    dialog.InitialDirectory = Path.GetDirectoryName(path);
                }
                else
                {
                    dialog.InitialDirectory = Environment.CurrentDirectory;
                }
                if (Path.HasExtension(path))
                {
                    string str2 = Path.GetExtension(path).Substring(1);
                    dialog.Filter = string.Format("{0} files (*.{1})|*.{1}|All files (*.*)|*.*", str2.ToUpper(CultureInfo.CurrentCulture), str2);
                }
                else
                {
                    dialog.Filter = "All files (*.*)|*.*";
                }
            }
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.FileName;
            }
            return base.EditValue(context, provider, value);
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }


}
