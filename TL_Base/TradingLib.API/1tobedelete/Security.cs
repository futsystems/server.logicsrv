//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace TradingLib.API
//{
//    /// <summary>
//    /// security definition
//    /// 交易品种定义
//    /// </summary>
//    public interface Security0
//    {
//        /// <summary>
//        /// symbol name
//        /// </summary>
//        string Symbol { get; set; }
        
//        /// <summary>
//        /// 品种描述
//        /// </summary>
//        string Description { get; set; }

//        /// <summary>
//        /// 对应的品种
//        /// </summary>
//        string MasterSecurity { get; set; }
        
//        /// <summary>
//        /// symbol with underscores represented as spaces
//        /// </summary>
//        string Symbol_Spaces { get; }
        
//        /// <summary>
//        /// entire representation of security
//        /// </summary>
//        string FullName { get;  }
        
//        /// <summary>
//        /// exchange associated with security
//        /// </summary>
//        string DestEx { get; set; }
        
//        /// <summary>
//        /// type associated with security
//        /// </summary>
//        SecurityType Type { get; set; }
        
//        /// <summary>
//        /// whether security is valid
//        /// </summary>
//        bool isValid { get; }
        
//        /// <summary>
//        /// whether security has an exchange
//        /// </summary>
//        bool hasDest { get; }
        
//        /// <summary>
//        /// whether security has an explicit type
//        /// </summary>
//        bool hasType { get; }
        
//        /// <summary>
//        /// date associated with security (eg expiration date)
//        /// </summary>
//        int Date { get; set; }
        
//        /// <summary>
//        /// details associated with security (eg put/call for options)
//        /// </summary>
//        string Details { get; set; }
        
//        /// <summary>
//        /// whether security is a call
//        /// </summary>
//        bool isCall { get; }
        
//        /// <summary>
//        /// whether security is put
//        /// </summary>
//        bool isPut { get; }
        
//        /// <summary>
//        /// strike price of options securities
//        /// 期权中的行权价
//        /// </summary>
//        decimal Strike { get; set; }
        
//        /// <summary>
//        /// 计费货币
//        /// </summary>
//        CurrencyType Currency { get; set; }
        
//        /// <summary>
//        /// 合约乘数
//        /// </summary>
//        int Multiple { get; set; }
        
//        /// <summary>
//        /// 合约最小价格变动
//        /// </summary>
//        decimal PriceTick { get; set; }
        
//        /// <summary>
//        /// 合约最小保证金
//        /// </summary>
//        decimal Margin { get; set; }
        
//        /// <summary>
//        /// 开仓佣金
//        /// </summary>
//        decimal EntryCommission { get; set; }
        
//        /// <summary>
//        /// 平仓佣金
//        /// </summary>
//        decimal ExitCommission { get; set; }

//    }

//    public class InvalidSecurity : Exception { }
//}
