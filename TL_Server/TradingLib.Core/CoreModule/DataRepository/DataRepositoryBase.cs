using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    /// <summary>
    /// 数据储存于加载
    /// </summary>
    [CoreAttr(DataRepositoryBase.CoreName, "交易数据储存模块", "交易数据储存模块用于向储存介质储存交易记录，同时从介质加载历史交易记录")]
    public class DataRepositoryBase: BaseSrvObject, IModuleDataRepository
    {
        protected const string CoreName = "DataRepository";
        public string CoreId { get { return this.PROGRAME; } }

        public DataRepositoryBase(string name = DataRepository.CoreName) :
            base(name)
        { 
            
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        public virtual void Start()
        {
            Util.StartStatus(this.PROGRAME);
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public virtual void Stop()
        {
            Util.StopStatus(this.PROGRAME);
        }

        public override void Dispose()
        {
            base.Dispose();
            Util.DestoryStatus(this.PROGRAME);
        }

        #region 插入实时交易数据
        /// <summary>
        /// 插入委托
        /// </summary>
        /// <param name="o"></param>
        public virtual void NewOrder(Order o)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 更新委托
        /// </summary>
        /// <param name="o"></param>
        public virtual void UpdateOrder(Order o)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 插入委托操作
        /// </summary>
        /// <param name="actoin"></param>
        public virtual void NewOrderAction(OrderAction actoin)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 插入成交
        /// </summary>
        /// <param name="f"></param>
        public virtual void NewTrade(Trade f)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 插入平仓明细
        /// </summary>
        /// <param name="d"></param>
        public virtual void NewPositionCloseDetail(PositionCloseDetail d)
        {
            throw new NotImplementedException();
        }
        #endregion


        #region 日内数据查询
        /// <summary>
        /// 获得所有交易帐户日内 成交数据
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<Trade> SelectAcctTrades()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获得所有交易帐户日内 委托数据
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<Order> SelectAcctOrders()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获得所有交易帐户日内 持仓明细数据
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<PositionDetail> SelectAcctPositionDetails()
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<ExchangeSettlement> SelectAcctExchangeSettlemts()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获得所有交易账户日内 委托操作数据
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<OrderAction> SelectAcctOrderActions()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 获得某个成交接口的日内 成交数据
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual IEnumerable<Trade> SelectBrokerTrades(string token)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获得某个成交接口的日内 委托数据
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual IEnumerable<Order> SelectBrokerOrders(string token)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获得某个成交接口的日内 持仓明细数据
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual IEnumerable<PositionDetail> SelectBrokerPositionDetails(string token)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 获得路由侧所有分解委托
        /// 路由侧分解的委托源均时分帐户侧的委托 因此直接调用ClearCentre.SentOrder就可以正确获得该委托
        /// 路由侧委托中的合约对象与对应的父委托合约对象一致
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<Order> SelectRouterOrders()
        {
            throw new NotImplementedException();
        }


        #endregion

    }
}
