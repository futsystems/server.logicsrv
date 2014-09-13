using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
namespace TradingLib.Quant.Base
{
    [Serializable]
    public class ConstantSeries :IndicatorBase
    {
        // Fields
        private double value;

        // Methods
        public ConstantSeries(double value)
            : base(0)
        {
            this.value = value;
        }

        public override double LookBack(int nBars)
        {
            return this.value;
        }

        public override void Caculate()
        {
            
        }
        public override int Count
        {
            get { return 1; }
        }

        public override double[] Data
        {
            get { return new double[] { this.value }; }
        }
        public override double First
        {
            get { return this.value;  }
        }
        public override ISeries[] GetInputs()
        {
            return base.GetInputs();
        }

        public override double Last
        {
            get { return this.value; }
        }

        public override void SetInputs(params ISeries[] newInputs)
        {
            base.SetInputs(newInputs);
        }

        public override double this[int index]
        {
            get { return this.value; }
        }

        public override void Reset()
        {
            
        }
           
    }

 

}
