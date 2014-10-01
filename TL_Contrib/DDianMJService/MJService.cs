using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.ComponentModel;


namespace Lottoqq.MJService
{
    /// <summary>
    /// 秘籍服务收费模式
    /// </summary>
    public enum QSEnumMJFeeType
    { 
        /// <summary>
        /// 按每笔手续费加成收费
        /// </summary>
        ByCommission=0,
        /// <summary>
        /// 按月收费比如每月收费多少
        /// </summary>
        ByMonth=1,

        /// <summary>
        /// 按使用次数计费类型
        /// </summary>
        ByCounts=2,
    }

    public enum QSEnumMJStatus
    { 
        /// <summary>
        /// 激活状态
        /// </summary>
        Active=1,
        /// <summary>
        /// 冻结状态
        /// </summary>
        Block=2,
    }

    /// <summary>
    /// 秘籍服务类别
    /// 不同级别异化合约需要对应的等级才可以进行交易
    /// 比如服务为LT则可以交易LT M1 M2 M3 档位
    /// 服务为M1可以交易M1 M2 M3 档位
    /// 衍生合约与等级挂钩，秘籍服务与等级挂钩，同时秘籍等级决定了可以交易的合约等级
    /// 当帐户绑定了某个秘籍等级,如果任意增加了等级则会打乱原有秘籍等级
    /// </summary>
    public enum QSEnumMJServiceLevel
    { 

        L1=1,//L1
        L2=2,//L2
        L3=3,//L3
        L4=4,
        L5=5,
        /// <summary>
        /// 乐透级别代表最高级别
        /// </summary>
        //[Description("乐透")]
        LT = 6,//500

    }



    internal class JsonWrapperMJService
    {
        MJService _mj;
        public JsonWrapperMJService(MJService mj)
        {
            _mj = mj;
        }

        public string Account { get { return _mj.Account.ID; } }

        public QSEnumMJServiceLevel Leve { get { return _mj.Level; } }

        public QSEnumMJFeeType FeeType { get { return _mj.FeeType; } }


        public DateTime ExpiredDate { 
            get 
            {
                if (FeeType == QSEnumMJFeeType.ByCommission)
                {
                    return DateTime.Now.AddYears(10);
                }
                if (FeeType == QSEnumMJFeeType.ByMonth)
                {
                    return _mj.ExpiredDate;
                }
                return DateTime.Now;
            } 
        }

        public bool IsExpired { get { return _mj.IsExpired; } }
        public bool IsValid { get { return _mj.IsValid; } }

    }

    /// <summary>
    /// 秘籍服务
    /// </summary>
    public class MJService:IAccountService
    {

        public string SN { get { return "MJService"; } }

        public MJService(QSEnumMJFeeType feetype, QSEnumMJServiceLevel level)
        {
            _feetype = feetype;
            _level = level;
        }

        public MJService()
        { 
        
        }
        /// <summary>
        /// 获得合约的对应等级
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static QSEnumMJServiceLevel GetSymbolLevel(Symbol symbol)
        {
            string[] symargs = symbol.Symbol.Split('-');
            string levelstr = symargs[symargs.Length - 1].ToUpper();
            QSEnumMJServiceLevel enumlevel = QSEnumMJServiceLevel.L1;

            try
            {
                enumlevel = (QSEnumMJServiceLevel)Enum.Parse(typeof(QSEnumMJServiceLevel), levelstr);
            }
            catch (Exception ex)
            {
                enumlevel = QSEnumMJServiceLevel.L1;
            }
            return enumlevel;
        }


        /// <summary>
        /// 数据库底层编号
        /// </summary>
        public int ID { get; set; }

        IAccount _account = null;
        public IAccount Account { get { return _account; } set { _account = value; } }


        /// <summary>
        /// 是否允许交易某个合约 
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool CanTradeSymbol(Symbol symbol,out string msg)
        {
            msg = string.Empty;
            //return true;
            //1.检查秘籍服务是否有效
            if (!IsValid)
            {
                msg = "秘籍服务过期,请续费!";
                return false;
            }

            //2.如果品种不是异化合约 则直接返回true
            if (symbol.SecurityType != SecurityType.INNOV) return true;

            //3.比较级别
            int nlevel = (int)Level;//秘籍档位
            string[] symargs = symbol.Symbol.Split('-');
            string levelstr = symargs[symargs.Length - 1].ToUpper();
            QSEnumMJServiceLevel enumlevel = QSEnumMJServiceLevel.L1;

            try
            {
                enumlevel = (QSEnumMJServiceLevel)Enum.Parse(typeof(QSEnumMJServiceLevel), levelstr);
            }
            catch (Exception ex)
            {
                enumlevel = QSEnumMJServiceLevel.L1;
            }
            int symlevel = (int)enumlevel;
            //msg = nlevel.ToString() + " " + levelstr;
            if (nlevel < symlevel)
            {
                msg = "秘籍级别:" + nlevel.ToString() + "级,无权交易" + symlevel.ToString() + "级衍生证券";
                return false;
            }

            //通过所有检查
            return true;
        }


        /// <summary>
        /// 是否可以接受某个合约 保证金检查
        /// </summary>
        /// <param name="order"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool CanTakeOrder(Order order,out string msg)
        {
            msg = string.Empty;
            //1.检查秘籍服务是否有效
            if (!IsValid)
            {
                msg = "秘籍服务过期,请续费!";
                return false;
            }

            //2.如果品种不是异化合约 则直接返回true
            if (order.SecurityType != SecurityType.INNOV) return true;

            //3.比较级别
            int nlevel = (int)Level;//秘籍档位
            string[] symargs = order.oSymbol.Symbol.Split('-');
            string levelstr = symargs[symargs.Length - 1].ToUpper();
            QSEnumMJServiceLevel enumlevel = QSEnumMJServiceLevel.L1;

            try
            {
                enumlevel = (QSEnumMJServiceLevel)Enum.Parse(typeof(QSEnumMJServiceLevel), levelstr);
            }
            catch (Exception ex)
            {
                enumlevel = QSEnumMJServiceLevel.L1;
            }
            int symlevel = (int)enumlevel;
            if (nlevel >= symlevel)
            {
                return true;
            }
            return false;
        }

        public int CanOpenSize(Symbol symbol)
        {
            return 0;
        }


        /// <summary>
        /// 获得某个合约的可用资金
        /// 秘籍服务不对可用资金进行调整返回-1
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public decimal GetFundAvabile(Symbol symbol)
        {
            return -1;
        }





        QSEnumMJFeeType _feetype;
        /// <summary>
        /// 收费类别
        /// </summary>
        public QSEnumMJFeeType FeeType { get { return _feetype; } set { _feetype = value; } }


        QSEnumMJServiceLevel _level;
        /// <summary>
        /// 服务级别
        /// </summary>
        public QSEnumMJServiceLevel Level { get { return _level; } set { _level = value; } }


        






        /// <summary>
        /// 修正成交手续费
        /// </summary>
        /// <param name="fill"></param>
        /// <param name="pr"></param>
        /// <returns></returns>
        public decimal AdjustCommission(Trade fill, IPositionRound pr)
        {
            //按手续费加成收手续费则需要按照手续费加成逻辑重新计算手续费
            if (FeeType == QSEnumMJFeeType.ByCommission)
            {
                decimal pct = MJConstant.GetCommissionPect(this.Level);
                return fill.Commission*(1+pct);
            }
            else//
            {
                return fill.Commission;
            }
        }

        DateTime _exipredate;
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpiredDate { get { return _exipredate; } set { _exipredate = value; } }

        /// <summary>
        /// 检查是否过期
        /// </summary>
        public bool IsExpired { get { return DateTime.Now > _exipredate; } }

        /// <summary>
        /// 检查服务是否有效
        /// </summary>
        public bool IsValid {

            get
            {
                //按手续费加成收费 则交易多少收多少
                if (FeeType == QSEnumMJFeeType.ByCommission)
                {
                    return true;
                }
                //按月收费则需要查看过期时间
                if (FeeType == QSEnumMJFeeType.ByMonth)
                {
                    if (IsExpired)
                        return false;
                    return true;
                }
                return false;
            }
        }

        public bool IsAvabile
        {
            get
            {
                return true;
            }
        }

        public override string ToString()
        {
            return "MJService [" + Account.ID + "] " + this.FeeType.ToString()+" " + this.Level.ToString() + " Exp:" + ExpiredDate.ToString() + " Valid:" + IsValid.ToString();
        }


        #region 服务查询和设置
        /// <summary>
        /// 查询服务状态和参数
        /// </summary>
        /// <returns></returns>
        public string QryService()
        {
            return "";
        }

        /// <summary>
        /// 设置服务状态和参数
        /// </summary>
        /// <param name="cfg"></param>
        public void SetService(string cfg)
        {

        }

        #endregion

    }
}
