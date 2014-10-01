using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Quant.Base;

namespace TradingLib.Quant.Plugin
{
    [Serializable]
    internal class BarDataWrapper : MarshalByRefObject, IBarDataRetrieval
    {
        // Fields
        private IBarDataRetrieval retriever;
        private ServiceWrapper wrapper;

        // Methods
        public BarDataWrapper(ServiceWrapper wrapper, IBarDataRetrieval retriever)
        {
            this.wrapper = wrapper;
            this.retriever = retriever;
        }

        public IQService GetService()
        {
            return this.wrapper;
        }


        public void RetrieveData(Security symbol, BarFrequency freq, DateTime startDate, DateTime endDate)
        { 
        
        }
        public  event BarDelegate GotHistBarEvent;
        public List<Bar> RetrieveData(Security symbol, int frequency, DateTime startDate, DateTime endDate, BarConstructionType barConstruction)
        {
            this.wrapper.ClearError();
            try
            {
                return null;
                //return this.retriever.RetrieveData(symbol, frequency, startDate, endDate, barConstruction);
            }
            catch (Exception exception)
            {
                this.wrapper.DoError(exception);
                return null;
            }
        }
    }


}
