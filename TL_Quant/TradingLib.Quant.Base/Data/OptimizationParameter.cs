using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Quant.Base
{
    [Serializable]
    public class OptimizationParameter
    {
        // Fields
        private int _numSteps;
        private StrategyParameterInfo _parameterInfo;

        // Methods
        public OptimizationParameter(StrategyParameterInfo parameter)
        {
            this._parameterInfo = parameter;
            if (parameter.NumSteps <= 0)
            {
                this.Low = parameter.Value;
                this.High = parameter.Value;
                this.NumSteps = 1;
            }
            else
            {
                this.Low = parameter.Low;
                this.High = parameter.High;
                this.NumSteps = parameter.NumSteps;
            }
        }

        public StrategyParameterInfo GetUpdatedSystemParameter()
        {
            StrategyParameterInfo info = this._parameterInfo.Clone();
            info.Low = this.Low;
            info.High = this.High;
            info.NumSteps = this.NumSteps;
            return info;
        }

        // Properties
        public double High { get; set; }

        public double Low { get; set; }

        public string Name
        {
            get
            {
                return this._parameterInfo.Name;
            }
        }

        public int NumSteps
        {
            get
            {
                return this._numSteps;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Number of steps must be greater than zero.");
                }
                this._numSteps = value;
            }
        }

        public double StepSize
        {
            get
            {
                if (this.NumSteps == 1)
                {
                    return 0.0;
                }
                return (double)(((decimal)(this.High - this.Low)) / (this.NumSteps - 1));
            }
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentException("Step size cannot be negative.");
                }
                if (value == 0.0)
                {
                    this.NumSteps = 1;
                }
                else
                {
                    this.NumSteps = ((int)Math.Ceiling((decimal)(((decimal)(this.High - this.Low)) / ((decimal)value)))) + 1;
                }
            }
        }
    }


}
