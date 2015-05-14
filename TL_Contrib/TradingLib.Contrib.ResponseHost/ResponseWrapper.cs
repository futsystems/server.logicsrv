using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Contrib.ResponseHost
{
    public class ResponseWrapper : IAccountService
    {
        /// <summary>
        /// 数据库全局ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 绑定的交易帐户
        /// </summary>
        public string Acct { get; set; }

        /// <summary>
        /// 绑定的策略模板ID
        /// </summary>
        public int Response_Template_ID { get; set; }

        /// <summary>
        /// 是否处于运行状态
        /// </summary>
        public bool Active { get; set; }

        IResponse _response = null;
        /// <summary>
        /// 策略实例
        /// </summary>
        public IResponse Response { get { return _response; } }

        /// <summary>
        /// ResponseBase对象
        /// </summary>
        public ResponseBase Wrapper { get { return _response as ResponseBase; } }

        /// <summary>
        /// 加载策略参数
        /// </summary>
        public void LoadArgument()
        {
            //通过模板ID 策略实例ID来获得对应的参数
            Dictionary<string, Argument> args = Tracker.ArgumentTracker.GetInstanceArgument(this.Response_Template_ID, this.ID);
            ResponseBase baseobj = _response as ResponseBase;
            //初始化参数
            baseobj.InitArgument(args);
        }

        /// <summary>
        /// 初始化策略实例
        /// </summary>
        public void InitResponse()
        {
            this.Account = TLCtxHelper.CmdAccount[this.Acct];//如果没有对应的交易帐号 则直接返回
            if (this.Account == null) return;

            Type type = Tracker.ResponseTemplateTracker.GetResponseType(this.Response_Template_ID);
            if (type == null) return;//如果没有获得对应的类型 则直接返回

            //2.生成对应的IFinService
            _response = (IResponse)Activator.CreateInstance(type);

            //3.绑定收费事件,绑定交易帐号,同时将服务绑定到对应的交易帐号对象上
            ResponseBase baseobj = _response as ResponseBase;
            if (baseobj != null)
            {
                //加载配资服务参数
                this.LoadArgument();
                baseobj.TemplateID = this.Response_Template_ID;//模板ID
                baseobj.InstanceID = this.ID;//实例ID
                //绑定交易帐号
                baseobj.BindAccount(this.Account);
                //将服务绑定到帐户
                this.Account.BindService(this);
            }
        }


        #region 响应外部事件

        /// <summary>
        /// 响应市场行情
        /// </summary>
        /// <param name="k"></param>
        public void OnTick(Tick k)
        {
            this._response.OnTick(k);
        }

        /// <summary>
        /// 响应委托
        /// </summary>
        /// <param name="o"></param>
        public void OnOrder(Order o)
        {
            this._response.OnOrder(o);
        }

        /// <summary>
        /// 响应成交
        /// </summary>
        /// <param name="f"></param>
        public void OnTrade(Trade f)
        {
            this._response.OnFill(f);
        }

        #endregion


        #region AccountService接口
        /// <summary>
        /// 返回AccountService的唯一标识
        /// </summary>
        public string SN { get { return "ResponseService"; } }

        /// <summary>
        /// 该服务所绑定的Account
        /// </summary>
        public IAccount Account { get; set; }


        /// <summary>
        /// 是否可以交易某个合约
        /// 限定合约部分
        /// 比如秘籍级别与衍生证券登记的关系
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public bool CanTradeSymbol(Symbol symbol, out string msg)
        {
            msg = string.Empty;
            return true;
        }


        /// <summary>
        /// 检查是否可以接受委托
        /// 这样就可以绕过保证金检查,比如实现1000元开一手股指
        /// 保证金计算部分
        /// 异化的保证金计算
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public bool CanTakeOrder(Order o, out string msg)
        {
            msg = string.Empty;
            return true;
        }

        /// <summary>
        /// 返回帐户可某个合约的手数
        /// 逻辑中包含一些特殊的保证金处理
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public int CanOpenSize(Symbol symbol,bool side,QSEnumOffsetFlag flag)
        {
            return 0;
        }

        public CommissionConfig GetCommissionConfig(Symbol symbol)
        {
            return null;
        }
        /// <summary>
        /// 获得某个合约的可用资金
        /// 1万配资10完的配资服务 需要返回不同于帐户资金的资金
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public decimal GetFundAvabile(Symbol symbol)
        {
            return 0;
        }

        /// <summary>
        /// 当前服务是否可用
        /// 检查服务是否处于激活状态
        /// 检查服务是否valid,有oaccount对象以及finservice对象
        /// </summary>
        public bool IsAvabile
        {
            get
            {
                return true;
            }

        }

        public IEnumerable<string> GetNotice()
        {
            return new List<string>();
        }

        #endregion

    }
}
