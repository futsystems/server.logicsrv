using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Contrib.CLICommand
{

    //internal class SecSide
    //{
    //    string _key = string.Empty;
    //    public SecSide(Position pos)
    //    {
    //        this.SecCode = pos.oSymbol.SecurityFamily.Code;
    //        this.Side = pos.isLong;
    //        _key = string.Format("{0}-{1}", this.SecCode, this.Side);
    //    }

    //    public SecSide(Order o)
    //    {
    //        this.SecCode = o.oSymbol.SecurityFamily.Code;
    //        this.Side = o.PositionSide;
    //        _key = string.Format("{0}-{1}", this.SecCode, this.Side);
    //    }
    //    /// <summary>
    //    /// 品种代码
    //    /// </summary>
    //    public string SecCode { get; set; }

    //    /// <summary>
    //    /// 多空方向
    //    /// </summary>
    //    public bool Side { get; set; }

    //    public decimal Margin { get; set; }

    //    public int HoldSize { get; set; }

    //    public decimal MarginFrozen { get; set; }

    //    public int PendingOpenSize { get; set; }

    //    /// <summary>
    //    /// 所有保证金
    //    /// </summary>
    //    public decimal TotalMargin { get { return this.Margin + this.MarginFrozen; } }


    //    public string Key { get { return _key; } }


    //    public override string ToString()
    //    {
    //        return string.Format("Key:{0} Total:{1} Margin:{2} MarginFrozen:{3}",_key,this.TotalMargin,this.Margin,this.MarginFrozen);
    //    }

    //    public override bool Equals(object obj)
    //    {
    //        if (obj is SecSide)
    //        { 
    //            SecSide other = obj as SecSide;
    //            return this.GetHashCode() == other.GetHashCode();
    //        }
    //        return false;
    //    }
    //    public override int GetHashCode()
    //    {
    //        return _key.GetHashCode();
    //    }
    //}


    //internal class MarginSet
    //{

    //    public MarginSet(string code, decimal margin, decimal frozen)
    //    {
    //        this.Code = code;
    //        this.Margin = margin;
    //        this.MarginFrozen = frozen;
    //    }
    //    /// <summary>
    //    /// 键值 品种-方向
    //    /// </summary>
    //    public string Code { get; set; }

    //    /// <summary>
    //    /// 占用保证金
    //    /// </summary>
    //    public decimal Margin { get; set; }

    //    /// <summary>
    //    /// 冻结保证金
    //    /// </summary>
    //    public decimal MarginFrozen { get; set; }

    //    public override string ToString()
    //    {
    //        return string.Format("Code:{0} Margin:{1} MarginFrozen:{2}", this.Code, this.Margin, this.MarginFrozen);
    //    }
    //}


    public partial class CommandCabinet : ContribSrvObject, IContrib
    {
        [CoreCommandAttr(QSEnumCommandSource.CLI, "demo1", "demo1 - ", "")]
        public string CTE_FlatPosition()
        {
            IAccount account = TLCtxHelper.CmdAccount["4444"];

            StringBuilder ret = new StringBuilder();
            foreach (MarginSet ms in account.CalFutMarginSet())
            {
                ret.AppendLine(ms.ToString());
            }
            return ret.ToString();
            //foreach (Position pos in account.GetPositionsHold())
            //{
            //    //ret.AppendLine("pos:" + pos.ToString());
            //}

            ////按品种分组

            ////按多空分组

            ////按某个键分组
            //Dictionary<string, SecSide> map = new Dictionary<string, SecSide>();

            //var gposresult = account.GetPositionsHold().GroupBy(p => new SecSide(p));
            //foreach (var g in gposresult)
            //{ 
            //    //分组完毕后 遍历组内对象
            //    //foreach(Position pos in g)
            //    //{
            //    //    ret.AppendLine("group:" + g.Key.ToString() + " pos:" + pos.ToString());
            //    //}
            //    g.Key.Margin = g.Sum(p => p.CalcPositionMargin());
            //    g.Key.HoldSize = g.Sum(p=>p.UnsignedSize);

            //    map.Add(g.Key.Key, g.Key);
            //    //按分总做累加
            //    ret.AppendLine("group:" + g.Key + " Margin:" + g.Sum(p => p.CalcPositionMargin()));

            //}

            //var gorderresult = account.GetPendingOrders().Where(o => o.IsEntryPosition).GroupBy(o => new SecSide(o));
            //foreach (var g in gorderresult)
            //{

            //    if (map.Keys.Contains(g.Key.Key))
            //    {
            //        SecSide s = map[g.Key.Key];
            //        s.MarginFrozen = g.Sum(o => o.CalFundRequired(TLCtxHelper.CmdUtils.GetAvabilePrice(o.Symbol)));
            //        s.PendingOpenSize = g.Sum(o => o.UnsignedSize);
            //    }
            //    else
            //    {
            //        g.Key.MarginFrozen = g.Sum(o => o.CalFundRequired(TLCtxHelper.CmdUtils.GetAvabilePrice(o.Symbol)));
            //        g.Key.PendingOpenSize = g.Sum(o => o.UnsignedSize);

            //        map.Add(g.Key.Key, g.Key);
            //    }
            //    ret.AppendLine("group:" + g.Key + " MarginFrozen:" + g.Sum(o => o.CalFundRequired(TLCtxHelper.CmdUtils.GetAvabilePrice(o.Symbol))));
            //}

            //ret.AppendLine("------------------------------------------");
            //foreach (SecSide s in map.Values)
            //{
            //    ret.AppendLine(s.ToString());
            //}

            //var maginlist = map.Values.GroupBy(s => s.SecCode).Select(g =>
            //{
            //    //如果只有1边持仓
            //    if (g.Count() == 1)
            //    {
            //        return new MarginSet(g.Key, g.ElementAt(0).Margin, g.ElementAt(0).MarginFrozen);
            //    }
            //    //双边持仓
            //    else
            //    { 
            //        int maxidx = g.ElementAt(0).Margin > g.ElementAt(1).Margin?0:1;
            //        SecSide big = g.ElementAt(maxidx);//大边
            //        SecSide small = g.ElementAt(maxidx == 0 ? 1 : 0);//小边
            //        //
            //        decimal marginfrozen=0;
            //        //大边挂单大于小边挂单 则必然使用大边挂单作为冻结保证金
            //        if(big.PendingOpenSize>=small.PendingOpenSize)
            //        {
            //            marginfrozen = big.MarginFrozen;
            //        }
            //        else//大边挂单小于小边挂单 
            //        {
            //            //
            //            int holddiff = big.HoldSize - small.HoldSize;//大边与小边的持仓差，这个差需要从小边挂单中扣除。小边挂单要超过大边持仓才占用保证金
            //            //小边挂单数量如果小于持仓仓差，则小边不计算冻结保证金
            //            if (small.PendingOpenSize <= holddiff)
            //                marginfrozen = big.MarginFrozen;
            //            else //小边挂单数量大于持仓仓差，则小边需要计算冻结保证金
            //            {
            //                //小边净挂单量
            //                int netsize = small.PendingOpenSize - holddiff;
            //                if (netsize <= big.PendingOpenSize)//小边净挂单量小于等于 大边挂单量，则按大边挂单量计算
            //                    marginfrozen = big.MarginFrozen;
            //                else
            //                    marginfrozen = (small.MarginFrozen / small.PendingOpenSize) * netsize;
            //            }
                        
            //        }
            //        return new MarginSet(g.Key, big.Margin, marginfrozen);
            //    }
            //});

            //ret.AppendLine("------------------------------------------");
            //foreach (MarginSet ms in maginlist)
            //{
            //    ret.AppendLine("marginset:" + ms.ToString());
            //}
            ////计算出不同品种 多空方向上的 冻结保证金与占用保证金 ，然后判断大边状态，最后返回对应的保证金
            //return ret.ToString();
        }
    }
}
