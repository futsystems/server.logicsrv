using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Quant.Base;

namespace TradingLib.Quant.Plugin
{
    public class TickWrapper : MarshalByRefObject, ITickRetrieval
    {
        // Fields
        private ITickRetrieval retriever;
        private ServiceWrapper wrapper;

        //public bool IsWatching();
        public bool SubscribeSymbols(Basket b) { return false; }
        public bool Start() { return false; }
        public bool Stop() { return false; }

        // Properties
        //public bool RealTimeDataAvailable { get; }

        /// <summary>
        /// 接口有数据到达时进行的回调
        /// </summary>
        public event TickDelegate GotTickEvent;


        // Methods
        public TickWrapper(ServiceWrapper wrapper, ITickRetrieval retriever)
        {
            this.wrapper = wrapper;
            this.retriever = retriever;
        }

        public IQService GetService()
        {
            this.wrapper.ClearError();
            try
            {
                return this.retriever.GetService();
            }
            catch (Exception exception)
            {
                this.wrapper.DoError(exception);
                return null;
            }
        }

        public bool IsWatching()
        {
            this.wrapper.ClearError();
            try
            {
                return this.retriever.IsWatching();
            }
            catch (Exception exception)
            {
                this.wrapper.DoError(exception);
                return false;
            }
        }

        public bool SetWatchedSymbols(List<Security> symbols)
        {
            this.wrapper.ClearError();
            try
            {
                return true;// this.retriever.SetWatchedSymbols(symbols);
            }
            catch (Exception exception)
            {
                this.wrapper.DoError(exception);
                return false;
            }
        }

        public bool StartWatching()
        {
            this.wrapper.ClearError();
            try
            {
                return true;// this.retriever.StartWatching();
            }
            catch (Exception exception)
            {
                this.wrapper.DoError(exception);
                return false;
            }
        }

        public bool StopWatching()
        {
            this.wrapper.ClearError();
            try
            {
                return true;// this.retriever.StopWatching();
            }
            catch (Exception exception)
            {
                this.wrapper.DoError(exception);
                return false;
            }
        }

        // Properties
        public bool RealTimeDataAvailable
        {
            get
            {
                this.wrapper.ClearError();
                try
                {
                    return this.retriever.RealTimeDataAvailable;
                }
                catch (Exception exception)
                {
                    this.wrapper.DoError(exception);
                    return false;
                }
            }
        }
        /*
        public GotTickData TickListener
        {
            set
            {
                this.wrapper.ClearError();
                try
                {
                    this.retriever.TickListener = value;
                }
                catch (Exception exception)
                {
                    this.wrapper.DoError(exception);
                }
            }
        }**/
    }

}
