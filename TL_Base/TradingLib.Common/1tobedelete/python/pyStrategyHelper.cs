//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TradingLib.API;

//namespace TradingLib.Common
//{
//    /// <summary>
//    /// python 策略的helper函数,主要用于向python环境暴露相关下单接口
//    /// </summary>
//    public class pyStrategyHelper
//    {

//        public event OrderDelegate SendOrderEvent;
//        public event LongDelegate SendCancelEvent;

//        void SendOrder(Order o)
//        {
//            if (SendOrderEvent != null)
//                SendOrderEvent(o);
//        }

//        void CancelOrder(long oid)
//        {
//            if (SendCancelEvent != null)
//                SendCancelEvent(oid);
//        }




//        //
//        //为python脚本提供 执行函数
//        //
//        public void Cancel(string symbol)
//        { 
        
//        }
        
//        public void Cancel(long oid)
//        { 
        
//        }
        
//        public void Cancel(bool side)
//        { 
        
//        }
        
//        public void Buy(string symbol, int size)
//        {
//            Order o = new BuyMarket(symbol, size);
//            SendOrder(o);
//        }

//        public void BuyLimit(string symbol, int size, decimal price)
//        {
//            Order o = new BuyLimit(symbol, size, price);
//            SendOrder(o);
//        }

//        public void BuyStop(string symbol, int size, decimal stop)
//        {
//            Order o = new BuyStop(symbol, size, stop);
//            SendOrder(o);
//        }

//        public void Sell(string symbol, int size)
//        {
//            Order o = new SellMarket(symbol, size);
//            SendOrder(o);
//        }

//        public void SellLimit(string symbol, int size, decimal price)
//        {
//            Order o = new SellLimit(symbol, size, price);
//            SendOrder(o);
//        }

//        public void SellStop(string symbol, int size, decimal stop)
//        {
//            Order o = new SellStop(symbol, size, stop);
//            SendOrder(o);
//        }


        

//    }
//}
