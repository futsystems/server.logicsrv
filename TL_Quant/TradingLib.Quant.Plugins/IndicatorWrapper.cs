using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using System.ComponentModel;
using System.Threading;

using TradingLib.Quant.Base;

namespace TradingLib.Quant.Plugin
{
    public class IndicatorWrapper:ISeries,IIndicator, IQCustomTypeDescriptor

    {
        private IIndicator calculator;

        public IndicatorWrapper(IIndicator calculator)
        {
            this.calculator = calculator;
        }

        public void HandleException(Exception e)
        {
            if (!(e is ThreadAbortException))
            {
                PluginException exception = new PluginException("The Indicator plugin " + this.calculator.GetType().FullName + " threw an exception of type " + e.GetType().FullName, e)
                {
                    Source = this.calculator.GetType().FullName
                };
                //MessageBox.Show(e.ToString());
                throw exception;
                
            }
        }


        public IList<Attribute> GetQAttributes()
        {
            List<Attribute> list = new List<Attribute>();
            foreach (Attribute attribute in Attribute.GetCustomAttributes(this.calculator.GetType()))
            {
                if (Attribute.GetCustomAttribute(attribute.GetType(), typeof(SerializableAttribute)) != null)
                {
                    list.Add(attribute);
                }
            }
            return list;
        }

        public double LookBack(int nBars)
        {
            try
            {
                return (this.calculator as ISeries).LookBack(nBars);
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
                return double.NaN;
            }
        }

        public void SetInputs(params ISeries[] inputs)
        {
            try
            {
                this.calculator.SetInputs(inputs);
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
        }

        // Properties
        
        public ISeriesChartSettings ChartSettings
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }
        
        public double this[int index]
        {
            get
            {
                try
                {
                    return (this.calculator as ISeries)[index];
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                    return 0;
                }
            }

        }
        public double[] Data
        {
            get
            {
                try
                {
                    return (calculator as ISeries).Data;
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                    return new double[] { 0 };
                }
            }
        }
        public void Reset()
        {

            try
            {
                this.calculator.Reset();
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
        }
        public void Caculate()
        {
            try
            {
                this.calculator.Caculate();
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
        }
        public int Count
        {
            get
            {
                try
                {
                    return  (this.calculator as ISeries).Count;
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                    return 0;
                }
            }
        }

        public double Last
        {
            get
            {
                try
                {
                    return (this.calculator as ISeries).Last;
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                    return double.NaN;
                }
            }
        }

        public double First
        {
            get
            {
                try
                {
                    return (this.calculator as ISeries).First;
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                    return double.NaN;
                }
            }
        }






        public ISeries[] GetInputs()
        {
            try
            {
                return this.calculator.GetInputs();
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
                return null;
            }
        }




    }
}
