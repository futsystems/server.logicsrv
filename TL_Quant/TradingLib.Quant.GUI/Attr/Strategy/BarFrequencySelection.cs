using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TradingLib.Quant.Common
{
    public class CheckedListBoxUC : ctBarFrequencySelection
    {
        private System.Windows.Forms.Design.IWindowsFormsEditorService m_iws;

        private string m_selectStr = string.Empty;

        
        public CheckedListBoxUC(System.Windows.Forms.Design.IWindowsFormsEditorService iws)
        {
            this.m_iws = iws;
            this.Visible = true;
            this.Height = 100;
            this.BorderStyle = BorderStyle.None;
            //添加事件
            this.Leave += new EventHandler(checkedListBoxUC_Leave);
            
            try
            {
                string[] strsFields = new string[] { "demo1", "demo2" };
                /*
                this.BeginUpdate();
                this.Items.Clear();
                if (null != strsFields)
                {
                    for (int i = 0; i < strsFields.Length; i++)
                    {
                        this.Items.Add(strsFields[i]);
                    }
                }**/
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                //this.EndUpdate();
            }
        }

        void checkedListBoxUC_Leave(object sender, EventArgs e)
        {
            List<string> lstStrs = new List<string>();
            /*
            for (int i = 0; i < this.Items.Count; i++)
            {
                if (this.GetItemChecked(i))
                {
                    lstStrs.Add((string)this.Items[i]);
                }

            }**/
            //MessageBox.Show(this.Selected);
            
            this.m_iws.CloseDropDown();
        }

    }
}
