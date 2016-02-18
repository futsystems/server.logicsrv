using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /* lottoqq相关服务的设计与思考
     * 多档秘籍服务
     * 功能描述,选择特定档位后,可以以低于标准保证金额度的资金去购买对应的合约,
     * 比如500，1000，2000，3000等,买入对应的合约后,对应持浮亏要在对应的额度范围内
     * 否则会触发止损,用户在自己服务的范围内可以选择对应的秘籍应用
     * 后台根据委托设定的秘籍应用来实现特殊功能
     * 如无秘籍应用标识,则为普通交易进入正常流程
     * 如果有秘籍应用标识,则对应该秘籍服务进入秘籍服务流程
     * 1.保证金 是按秘籍档位来设定M1,M2,M3 同时委托成交持仓对象中包含了服务标识,用于区分是普通还是特殊服务产生的交易记录
     * 2.同时服务还要对其帐户进行风控,比如M1持仓亏损300*0.85的时候强平,
     * 因此服务命名需要统一,方便系统查找
     * 
     * 底层对Account加载如下几个服务
     * 1.比赛
     * 2.配资
     * 3.秘籍
     * 第一阶段实现比赛和秘籍2大块
     * 比赛记录了每个帐户的比赛情况,同时每日比赛系统整体进行排名统计和相关数据的统计和计算
     * 
     * 秘籍服务 类似配资服务 但是不是通过提高配资额度来提高可用交易保证金来实现的
     * 
     * 秘籍服务修改了传统的保证金的计算方式,以压住的方式来购买合约,客户提交委托时 选择了下注金额,委托成交后,会将该下注金额 附加到持仓上。
     * 秘籍风控会监控每个秘籍服务下的持仓。同时注意风控
     * 
     * 秘籍服务通过构造异化证券来达到区分与普通合约的目的
     * 比如LOTTO-IO1406-C-2150-LT/M1/M2/M3 可以包含多种异化证券类型，其保证金按设定的固定金额保证金进行计算500 1000 2000 3000
     * 然后其风控则按照特定的品种类型进行，如果是LOTTO类型的证券,我们进行单独的风控策略
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * **/
    /// <summary>
    /// 定义了帐户服务类接口
    /// 普通帐户注册了特定的服务后会给该帐户绑定特定的服务
    /// 特定的服务有对应的风控规则和保证金计算规则以及其他特性
    /// 后期根据系统的开发逐步完善
    /// 如何实现
    /// 
    /// </summary>
    public interface IAccountService
    {
        /// <summary>
        /// 返回AccountService的唯一标识
        /// </summary>
        string SN { get; }

        /// <summary>
        /// 该服务所绑定的Account
        /// </summary>
        IAccount Account { get; }

        /// <summary>
        /// 检查是否可以接受委托
        /// 这样就可以绕过保证金检查,比如实现1000元开一手股指
        /// 保证金计算部分
        /// 异化的保证金计算
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        bool CanTakeOrder(Order o,out string msg);


        /// <summary>
        /// 是否可以交易某个合约
        /// 限定合约部分
        /// 比如秘籍级别与衍生证券登记的关系
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        bool CanTradeSymbol(Symbol symbol,out string msg);



        /// <summary>
        /// 返回帐户可某个合约的手数
        /// 逻辑中包含一些特殊的保证金处理
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        int CanOpenSize(Symbol symbol,bool side,QSEnumOffsetFlag flag);


        /// <summary>
        /// 获得某个合约的可用资金
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        decimal GetFundAvabile(Symbol symbol);
       

        /// <summary>
        /// 当前服务是否可用
        /// </summary>
        bool IsAvabile { get; }


        /// <summary>
        /// 获得交易帐户服务 通知
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetNotice();
    }


}
