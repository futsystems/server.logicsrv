using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.WinControls.UI;
using Telerik.WinControls;
using System.Windows.Forms;
using System.Drawing;

namespace FutSystems.GUI
{
    /// <summary>
    /// 封装了RadTextBoxElement
    /// </summary>
    public class FutPriceEditorContentElement:RadTextBoxElement
    {
        public FutPriceEditorContentElement()
        {
            base.TextAlign = HorizontalAlignment.Right;
            this.Alignment = ContentAlignment.MiddleCenter;
            this.Size = new Size(200, 50);
            
            
            //base.TextBoxItem.KeyDown += new KeyEventHandler(this.TextBoxItem_KeyDown);
            //base.TextBoxItem.KeyPress += new KeyPressEventHandler(this.TextBoxItem_KeyPress);
            //base.TextBoxItem.TextChanging += new TextChangingEventHandler(this.TextBoxItem_TextChanging);

            base.Border.SetDefaultValueOverride(RadElement.VisibilityProperty, ElementVisibility.Hidden);


        }

        protected override void DisposeManagedResources()
        {
            base.TextBoxItem.KeyDown -= new KeyEventHandler(this.TextBoxItem_KeyDown);
            base.TextBoxItem.KeyPress -= new KeyPressEventHandler(this.TextBoxItem_KeyPress);
            base.TextBoxItem.TextChanging -= new TextChangingEventHandler(this.TextBoxItem_TextChanging);
            base.DisposeManagedResources();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            /*
            if (e.KeyValue == 0x73)
            {
                if (this.owner != null)
                {
                    if (this.owner.IsPopupOpen)
                    {
                        this.owner.ClosePopup();
                    }
                    else
                    {
                        this.owner.ShowPopup();
                    }
                    e.Handled = true;
                }
            }
            else if ((this.owner != null) && this.owner.IsPopupOpen)
            {
                this.owner.CalculatorContentElement.Focus();
                this.owner.CalculatorContentElement.ProcessKeyDown(e);
                e.Handled = true;
            }
            else
            {
                base.OnKeyDown(e);
            }**/
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            /*
            if ((this.owner != null) && this.owner.IsPopupOpen)
            {
                this.owner.CalculatorContentElement.Focus();
                this.owner.CalculatorContentElement.ProcessKeyPress(e);
                e.Handled = true;
            }
            else
            {
                base.OnKeyPress(e);
                NumberFormatInfo numberFormat = CultureInfo.CurrentCulture.NumberFormat;
                string numberDecimalSeparator = numberFormat.NumberDecimalSeparator;
                string numberGroupSeparator = numberFormat.NumberGroupSeparator;
                string negativeSign = numberFormat.NegativeSign;
                if (e.KeyChar == '.')
                {
                    e.KeyChar = numberDecimalSeparator[0];
                }
                string str4 = e.KeyChar.ToString();
                if (((!char.IsDigit(e.KeyChar) && !str4.Equals(numberDecimalSeparator)) && (!str4.Equals(numberGroupSeparator) && !str4.Equals(negativeSign))) && ((e.KeyChar != '\b') && ((Control.ModifierKeys & (Keys.Alt | Keys.Control)) == Keys.None)))
                {
                    e.Handled = true;
                    NativeMethods.MessageBeep(0);
                }
            }**/
        }

        private void TextBoxItem_KeyDown(object sender, KeyEventArgs e)
        {
            this.OnKeyDown(e);
        }

        private void TextBoxItem_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.OnKeyPress(e);
        }

        private void TextBoxItem_TextChanging(object sender, TextChangingEventArgs e)
        {
            //this.owner.Value = this.Text;
            //e.Cancel = this.owner.CancelValueChanging;
        }

    }
}
