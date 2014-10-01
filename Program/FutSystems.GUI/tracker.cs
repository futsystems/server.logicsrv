using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;



namespace FutSystems.GUI
{
    [ToolboxItem(true)]
    public class MyTrackBarControl : RadControl
    {
        protected override void CreateChildItems(RadElement parent)
        {
            RadTrackBarElement radTrackBarElement = new RadTrackBarElement();
            radTrackBarElement.ForeColor = Color.SkyBlue;
            radTrackBarElement.TickColor = Color.Blue;
            radTrackBarElement.BackColor = Color.LightSteelBlue;
            radTrackBarElement.Minimum = 1;
            radTrackBarElement.Maximum = 100;
            radTrackBarElement.TickFrequency = 10;
            radTrackBarElement.Value = 20;
            this.RootElement.Children.Add(radTrackBarElement);
            base.CreateChildItems(parent);
        }
    }
}
