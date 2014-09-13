//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TradingLib.Common;
//using TradingLib.API;
//using System.ComponentModel;
////using System.Windows.Forms;

//namespace TradingLib.Common
//{
//    //PositionCheckTemplate在ResponseTemplate基础上定义了 positioncheck的基础工作流程
//    //positionCheck实现的操作有平仓,加减仓,反手等操作 positioncheck并不作数据管理
//    //positioncheck可以在template的基础上用简单的逻辑实现较为复杂工作
//    //positioncheck不需要进行数据管理 order管理 以及 position管理 这些工作统一由tradingtracker完成
//    //positioncheck可以tick驱动,或者bar驱动此外的数据position并不处理与关心
//    public abstract class PositionCheckTemplate:ResponseTemplate,IPositionCheck
//    {
//        Object _displayfrom = null;
//        public Object DisplayForm { get { return _displayfrom; } set { _displayfrom = value; } }

//        //记录positionCheck所跟踪的Position
//        private Position _pos;
//        [Description("Positon数据"), Category("PositionCheck基本属性")]
//        public Position myPosition { get { return _pos; } set { _pos = value; } }

//        private Symbol _sec;
//        [Description("Security数据"), Category("PositionCheck基本属性")]
//        public Symbol Security { get { return _sec; } set { _sec = value; } }




//        private PositionDataDriver _pdatadriver = PositionDataDriver.Tick;
//        [Description("策略驱动数据的类型"), Category("数据")]
//        public PositionDataDriver DataDriver { get { return _pdatadriver; } set { _pdatadriver = value; } }


//        #region response input 的函数
//        //当有新的tick数据进来后执行的操作 注意positioncheck通过上层tick检查与symbol的配对，到本positioncheck
//        //的tick都是myposition的tick
//        //注意这里被子类覆盖我们需要用virtual 否则子类对其进行覆写后会产生运行时nullreference exception的问题
//        //策略的数据驱动入口就是有这里进去,所有策略通过对应Tick来驱动
//        protected Tick Quote;
//        public  override void GotTick(Tick k)
//        {
//            //D("Got tick:"+k.ToString());
//            Quote = k;
//            //1.检查position
//            if (k.isValid)
//            {
//                string msg;
//                checkPosition(out msg);
//            }
//        }

        
//        //public abstract void onTick(Tick k);
//        public override void GotOrder(Order o)
//        {
//            base.GotOrder(o);
//        }

//        int lastPositionSize = 0;
//        protected int PositionSizeBeforeTrade = 0;
//        public override void GotFill(Trade f)
//        {
//            base.GotFill(f);
//            //如果这个成交以前 仓位为0,并且当前有仓位 则表示开仓
//            if (lastPositionSize == 0 && !myPosition.isFlat)//建仓事件
//            {
//                //D("lastposition:" + lastPositionSize.ToString() + " nowposition:" + myPosition.Size.ToString());
//                onPositionEntry(f);
//                PositionSizeBeforeTrade = lastPositionSize;
//                lastPositionSize = myPosition.Size;
//                return;
//            }
//            if (lastPositionSize != 0 && myPosition.isFlat)//平仓事件
//            {
//                //D("lastposition:" + lastPositionSize.ToString() + " nowposition:" + myPosition.Size.ToString());
//                onPositionExit(f);
//                PositionSizeBeforeTrade = lastPositionSize;
//                lastPositionSize = myPosition.Size;
//                return;
//            }
//            if (lastPositionSize != 0 && lastPositionSize != myPosition.Size)
//            {
//                //D("lastposition:"+lastPositionSize.ToString()+" nowposition:"+myPosition.Size.ToString());
//                if (Math.Abs(lastPositionSize) < Math.Abs(myPosition.Size))//仓位增加
//                    onPositionAdd(f);
//                else
//                    onPositionCut(f);
//                //onPositionChanged(f);
//                PositionSizeBeforeTrade = lastPositionSize;
//                lastPositionSize = myPosition.Size;
//                return;
//            }
            
//        }
//        //加仓事件
//        protected virtual void onPositionAdd(Trade f)
//        { 
            
//        }
//        //减仓事件
//        protected virtual void onPositionCut(Trade f)
//        {

//        }

//        //平掉一个仓位时候进行的调用
//        protected virtual void onPositionExit(Trade f)
//        { 
            
//        }
//        //当打开一个仓位时候进行的调用
//        protected virtual void onPositionEntry(Trade f)
//        { 
            
//        }

//        #endregion

//        public abstract void checkPosition(out string msg);
//        public override void Reset()
//        {
//            base.Reset();
//            lastPositionSize = myPosition.Size;
//        }

//        public override void Shutdown()
//        {

//        }
//        //配置文本化
//        public abstract string ToText();
//        //从文本读取配置信息,通过给positioncheck配置不同的参数实现不同的效果
//        public abstract IPositionCheck FromText(string msg);
//        //得到容易理解的positioncheck的中文描述
//        public virtual string PositionCheckDescription(){return "";}


//        #region position基础操作
//        //需要用到的常规仓位操作，方便具体的position操作调用
//        protected void SellMarket(int size,string comment="")
//        {
//            Order o = new MarketOrder(myPosition.Symbol, false, size,comment);
//            sendorder(o); 
//        }
//        protected void SellLimit(int size,decimal price,string comment="")
//        {
//            Order o = new LimitOrder(myPosition.Symbol, false, size, price);
//            o.comment = comment;
//            sendorder(o);
//        }
//        protected void SellLimit(int size, decimal price,string tif,string comment="")
//        {
            
//            Order o = new LimitOrder(myPosition.Symbol, false, size, price);
//            o.TIF = tif;
//            o.comment = comment;
//            sendorder(o);
//        }
//        private void SellStop(int size, decimal price,string comment="")
//        {
//            Order o = new SellStop(myPosition.Symbol, size, price);
//            o.comment = comment;
//            sendorder(o);
//        }

//        protected void BuyMarket(int size,string comment="")
//        {
//            Order o = new MarketOrder(myPosition.Symbol, true, size);
//            o.comment = comment;
//            sendorder(o);
//        }
//        protected void BuyLimit(int size, decimal price,string comment="")
//        {
//            Order o = new LimitOrder(myPosition.Symbol, true, size, price);
//            o.comment = comment;
//            sendorder(o);
//        }
//        protected void BuyLimit(int size, decimal price,string tif,string comment="")
//        {
//            Order o = new LimitOrder(myPosition.Symbol, true, size, price);
//            o.TIF = tif;
//            o.comment = comment;
//            sendorder(o);
//        }
//        protected void BuyStop(int size, decimal price,string comment="")
//        {
//            Order o = new SellStop(myPosition.Symbol, size, price);
//            o.comment = comment;
//            sendorder(o);
//        }
//        protected bool FlatPosition(string comment="")
//        {
//            if (myPosition.isFlat) return false;
//            Order o = new MarketOrderFlat(myPosition);
//            o.comment = comment;
//            sendorder(o);
//            return true;
//        }


//        #endregion
//    }

//}
