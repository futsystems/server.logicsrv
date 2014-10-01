using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
//using System.Windows.Forms;
using TradingLib.Quant.Base;

namespace TradingLib.Quant.Plugin
{
    [Serializable]
    public class BrokerWrapper : IExBroker, ISimBroker
    {
        /*
        public DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }**/

        private IExBroker broker;
        //private OrderUpdatedDelegate OrderUpdated;
        //private PositionAvailableDelegate PositionAvailable;
        private ISimBroker simBroker;
        private ServiceWrapper wrapper;

        // Methods
        public BrokerWrapper(ServiceWrapper wrapper, IExBroker broker)
        {
            this.wrapper = wrapper;
            this.broker = broker;
            this.simBroker = broker as ISimBroker;

            this.broker.GotOrderEvent += new OrderDelegate(broker_GotOrderEvent);
            this.broker.GotCancelEvent += new LongDelegate(broker_GotCancelEvent);
            this.broker.GotFillEvent += new FillDelegate(broker_GotFillEvent);
        }

        void broker_GotFillEvent(Trade t)
        {
            if (GotFillEvent != null)
                GotFillEvent(t);
        }

        void broker_GotCancelEvent(long val)
        {
            if (GotCancelEvent != null)
                GotCancelEvent(val);
        }

        void broker_GotOrderEvent(Order o)
        {
            if (GotOrderEvent != null)
                GotOrderEvent(o);
        }

        // Methods
        /// <summary>
        /// 向Broker发送Order
        /// </summary>
        /// <param name="o"></param>
        public void SendOrder(Order o)
        {
            this.wrapper.ClearError();
            try
            {
                this.broker.SendOrder(o);
            }
            catch (Exception exception)
            {
                this.wrapper.DoError(exception);
            }
        }
        /// <summary>
        /// 向broker取消一个order
        /// </summary>
        /// <param name="oid"></param>
        public void CancelOrder(long oid)
        { 
             this.wrapper.ClearError();
             try
             {
                 this.broker.CancelOrder(oid);
             }
             catch (Exception exception)
             {
                 this.wrapper.DoError(exception);
                 //return false;
             }
        }
        /// <summary>
        /// 用于交易通道中需要有Tick进行驱动的逻辑,比如委托触发等
        /// </summary>
        /// <param name="k"></param>


        //事件
        /// <summary>
        /// 当有成交时候回报客户端
        /// </summary>
        public event FillDelegate GotFillEvent;
        /// <summary>
        /// 委托正确回报时回报客户端
        /// </summary>
        public event OrderDelegate GotOrderEvent;
        /// <summary>
        /// 撤单正确回报时回报客户端
        /// </summary>
        public event LongDelegate GotCancelEvent;
        /// <summary>
        /// 某个委托回报相应的委托信息
        /// </summary>
        public event OrderMessageDel GotOrderMessageEvent;
        public bool IsLiveBroker()
        {
            this.wrapper.ClearError();
            try
            {
                return this.broker.IsLiveBroker();
            }
            catch (Exception exception)
            {
                this.wrapper.DoError(exception);
                return false;
            }
        }
        
        public IQService GetService()
        { 
            return this.wrapper;

        }
        #region simbroker相关接口
        private ISimBroker SimBroker
        {
            get
            {
                if (this.simBroker == null)
                {
                    //MessageBox.Show("null simBroker");
                    throw new Exception("The broker did not implement the ISimBroker interface");
                }
                return this.simBroker;
            }
        }
 

        //public void SetBuyingPower(double value);
        public void SimBar(Bar bar)
        {
           // MessageBox.Show("Broker Wraper got simBar called");
            this.wrapper.ClearError();
            try
            {
                //MessageBox.Show("live:" + this.broker.IsLiveBroker().ToString());
                if (!this.broker.IsLiveBroker())
                {
                    this.SimBroker.SimBar(bar);
                }
            }
            catch (Exception exception)
            {
                this.wrapper.DoError(exception);
            }

        }
        public void SimClose(Bar bar)
        {
            this.wrapper.ClearError();
            try
            {
                if (!this.broker.IsLiveBroker())
                {
                    this.SimBroker.SimClose(bar);
                }
            }
            catch (Exception exception)
            {
                this.wrapper.DoError(exception);
                //MessageBox.Show(exception.ToString());
            }
        }

        public void SimTick(Security symbol, Tick tick)
        {
            {
                this.wrapper.ClearError();
                try
                {
                    //MessageBox.Show("call sim broker here a ");
                    if (!this.broker.IsLiveBroker())
                    {
                        //MessageBox.Show("call sim broker here B");
                        this.SimBroker.SimTick(symbol, tick);
                    }
                }
                catch (Exception exception)
                {
                    this.wrapper.DoError(exception);
                    //MessageBox.Show("error:"+exception.ToString());
                }
            }
        }

        #endregion
    }
}
