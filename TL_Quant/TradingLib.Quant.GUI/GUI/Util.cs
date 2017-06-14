using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TradingLib.Quant.GUI
{
    public class GUIUtils
    {
        public static bool SelectComboItem(ComboBox box, object tag)
        {
            foreach (ComboBoxItem item in box.Items)
            {
                if (item.Tag.Equals(tag))
                {
                    box.SelectedItem = item;
                    return true;
                }
            }
            return false;
        }



    }
}
