using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Quant.Base
{
    [Serializable]
    public class ChartCap
    {
        // Fields
        private bool filled;
        private int height;
        private int width;

        // Methods
        public ChartCap()
        {
            this.width = 10;
            this.height = 10;
        }

        public ChartCap(int width, int height, bool filled)
        {
            this.width = 10;
            this.height = 10;
            this.width = width;
            this.height = height;
            this.filled = filled;
        }

        // Properties
        public bool Filled
        {
            get
            {
                return this.filled;
            }
            set
            {
                this.filled = value;
            }
        }

        public int Height
        {
            get
            {
                return this.height;
            }
            set
            {
                this.height = value;
            }
        }

        public int Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.width = value;
            }
        }
    }


}
