using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using System.ComponentModel;


using System.Drawing;

namespace FutSystems.GUI
{
    [ToolboxItem(true)]
    public class MyPanel : RadControl
    {
        private MyPanelElement panelElement;


        public MyPanel()
        {
            this.AutoSize = true;
        }

        public MyPanelElement PanelElement
        {
            get
            {
                return this.panelElement;
            }
        }

        public bool Active
        {
            get
            {
                return this.panelElement.Active;
            }
            set
            {
                
                this.panelElement.Active = value;
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(200, 100);
            }
        }

        protected override void CreateChildItems(RadElement parent)
        {
            this.panelElement = new MyPanelElement();
            this.RootElement.Children.Add(panelElement);
            base.CreateChildItems(parent);
        }
    }
}
