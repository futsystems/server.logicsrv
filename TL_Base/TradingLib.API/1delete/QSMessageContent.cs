//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace TradingLib.API
//{
//    public class QSMessageContent
//    {
//        public const string PREFIX = "乐透交易平台:";
//        /// <summary>
//        /// 非交易日提醒
//        /// 如果当天不是交易日,客户端提交委托或操作时候直接回复该消息
//        /// </summary>
//        public const string NOT_TRADINGDAY = "非交易日";

//        /// <summary>
//        /// 清算中心非接受委托状态
//        /// 清算中心会在开市前打开,用于等待客户端提交委托
//        /// </summary>
//        public const string CLEARCENTRE_CLOSED = PREFIX + "交易中心关闭";

//        /// <summary>
//        /// tl_mq消息层检查委托是否有效o.isvalid
//        /// </summary>
//        public const string ORDER_INVALID = "委托格式错误";

//        /// <summary>
//        /// 在客户端提交操作时,tl_mq检查帐户是否登入
//        /// </summary>
//        public const string ACCOUNT_NOTAUTHORIZED = "交易帐号未授权";

//        /// <summary>
//        /// 每个回话登入后会与交易帐号进行绑定,如果提交委托的帐号与绑定帐号不符
//        /// </summary>
//        public const string ORDER_ACCOUNT_CLIENT_NOT_MATCH = "回话登入帐号与委托帐号不符";
//        /// <summary>
//        /// 密码修改过程中 老密码验证错误
//        /// </summary>
//        public const string OLDPASS_ERROR = PREFIX + "原始密码错误";

//        /// <summary>
//        /// 登入服务器是,提交的请求不完整
//        /// </summary>
//        public const string LOGINREQUEST_FORM_ERROR = "登入信息不完整或格式错误";

//        /// <summary>
//        /// 密码修改正确
//        /// </summary>
//        public const string PASSWD_CHANGED_SUCCESS = "密码修改正确";

//        /// <summary>
//        /// 交易帐号登入成功
//        /// </summary>
//        public const string LOGIN_SUCCESS = PREFIX + "登入成功";

//        /// <summary>
//        /// 交易帐号登入失败
//        /// </summary>
//        public const string LOGIN_FAILED = PREFIX + "登入失败";

//        /// <summary>
//        /// 交易委托所对应的合约无效
//        /// </summary>
//        public const string SYMBOL_NOT_EXISTED = PREFIX + "合约无效";

//        /// <summary>
//        /// 交易合约没有有效价格 为获得市场行情
//        /// </summary>
//        public const string SYMBOL_TICK_ERROR = PREFIX +"合约行情异常,无法接受委托";

//        /// <summary>
//        /// 当前交易日超过了下一个交易日
//        /// </summary>
//        public const string SETTLECENTRE_NOT_RESET = PREFIX + "交易柜台未重置,等待结算";


//        #region 风控部分

//        /// <summary>
//        /// 风控检查 保证金不足
//        /// </summary>
//        public const string INSUFFICIENT_MOENY = PREFIX + "资金不足";

//        public const string INSTRUMENT_NOT_FOUND = PREFIX + "找不到合约";

//        public const string INSTRUMENT_NOT_TRADING = PREFIX + "合约不能交易";

//        #endregion

//        #region BrokerRouter
//        public const string POSITION_NOT_HOLDING = PREFIX + "无有效可平持仓";

//        public const string POS_ORDER_CANNOT_CLOSE = PREFIX + "委托与当前持仓方向相同,无法平仓";

//        public const string POS_ORDER_CANNOT_OPEN = PREFIX + "委托与当前持仓方向相反,无法开仓";

//        public const string OVER_CLOSE_POSITION = PREFIX + "平仓量超过可平持仓";

//        public const string OVER_CLOSE_POSITION_MARKET = PREFIX + "市价平仓量超过持仓总数";

//        public const string OVER_CLOSE_POSITION_AUTOCLEAR = PREFIX + "市价平仓溢出,系统自动撤单并平仓";

//        #endregion





//    }
//}
