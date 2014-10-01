using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Quant.GUI
{

        public class ComboBoxItem
        {
            // Fields
            private string name;
            private object tag;

            // Methods
            public ComboBoxItem()
            {
            }

            public ComboBoxItem(string name)
            {
                this.name = name;
            }

            public ComboBoxItem(string name, object tag)
            {
                this.name = name;
                this.tag = tag;
            }

            public override string ToString()
            {
                return this.name;
            }

            // Properties
            public string Name
            {
                get
                {
                    return this.name;
                }
                set
                {
                    this.name = value;
                }
            }

            public object Tag
            {
                get
                {
                    return this.tag;
                }
                set
                {
                    this.tag = value;
                }
            }
        }


}
