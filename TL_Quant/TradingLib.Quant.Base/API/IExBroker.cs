using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Quant.Base
{
    /// <summary>
    /// 成交服务接口
    /// </summary>
    public interface IExBroker
    {
        
        
        /// <summary>
        /// 向Broker发送Order
        /// </summary>
        /// <param name="o"></param>
        void SendOrder(Order o);
        /// <summary>
        /// 向broker取消一个order
        /// </summary>
        /// <param name="oid"></param>
        void CancelOrder(long oid);
        

        //事件
        /// <summary>
        /// 当有成交时候回报客户端
        /// </summary>
        event FillDelegate GotFillEvent;
        /// <summary>
        /// 委托正确回报时回报客户端
        /// </summary>
        event OrderDelegate GotOrderEvent;
        /// <summary>
        /// 撤单正确回报时回报客户端
        /// </summary>
        event LongDelegate GotCancelEvent;
        /// <summary>
        /// 某个委托回报相应的委托信息
        /// </summary>
        event OrderMessageDel GotOrderMessageEvent;
        /// <summary>
        /// 是否是实盘成交接口
        /// </summary>
        /// <returns></returns>
        bool IsLiveBroker();

        IQService GetService();
        /*
        void AddOrderUpdatedDelegate(OrderUpdatedDelegate orderUpdated);
        void AddPositionAvailableDelegate(PositionAvailableDelegate positionAvailable);
        bool CancelAllOrders();
        bool CancelOrder(string orderId);
        void CustomMessage(string type, object data);
        double GetBuyingPower();
        double GetMargin();
        BrokerOrder GetOpenOrder(string id);
        List<BrokerOrder> GetOpenOrders();
        
        int GetShares(Symbol symbol);
        double GetShortedCash();
        void RemoveOrderUpdatedDelegate(OrderUpdatedDelegate orderUpdated);
        void RemovePositionAvailableDelegate(PositionAvailableDelegate positionAvailable);
        void SetAccountState(BrokerAccountState state);
        bool SubmitOrder(BrokerOrder order, out string orderId);
        void SyncAccountState();**/

       
    }


    public interface ISimBroker : IExBroker
    {
        // Methods
        //ReturnCode Deposit(double amount);
        //void SetAccountInfo(IAccountInfo accountInfo);
        //void SetBuyingPower(double value);
        void SimBar(Bar bar);
        void SimClose(Bar bar);
        void SimTick(Security symbol, Tick tick);
        //ReturnCode Withdraw(double amount);
    }

 

 


 

}
