using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using Telerik.WinControls.Primitives;
using TradingLib.API;
using TradingLib.Common;
using System.Threading;

namespace FutSystems.GUI
{
    public partial class ctOrderSender : UserControl,IOrderSender
    {

        const string PROGRAM = "ctOrderSender";
        bool _assumenoordermod = true;
        public bool AssumeNewOrder { get { return _assumenoordermod; } set { _assumenoordermod = value; } }

        string _dispdecpointformat = "N" + ((int)2).ToString();
        //bool touched = false;//鼠标是否在价格或者数量控件里面

        string _account = "";
        public string Account { get { return _account; } set { _account = value; } }
        public event OrderDelegate SendOrderEvent;//发送Order
        public event LongDelegate SendCancelEvent;//发送取消
        public event PositionOffsetArgsDel UpdatePostionOffsetEvent;//对外触发止盈止损更新事件
        
        public event DebugDelegate SendDebugEvent;

        public event QryCanOpenPositionLocalDel QryCanOpenPositionLocalEvent;
        public event QryCanOpenPosition QryCanOpenPositionEvent;
        //public event SymDelegate SendCancelSymbolOrderEvent;//撤掉某个symbol的所有委托
        public event SymDelegate SendReserveSymbolPositionEvent;//反手某个合约持仓
        public event IntStringDelegate SendQryDefaultSizeEvent;

        public event VoidDelegate SendEntrySecEvent;
        public event VoidDelegate SendLeaveSecEvent;
        public event VoidDelegate SendSecuritySelectedEvent;//合约切换事件

        //本控件触发sendOrder事件,买入 卖出按钮通过触发sendOrder事件从而实现对其他组件的调用
        //外部使用该控件时,需要绑定其SendOrder
        //public event OrderDelegate SendOrder;

        public Symbol SecuritySelected { get { return _sec; } }
        private Symbol _sec;

        QSEnumOrderType _ordertype = QSEnumOrderType.Market;
        public QSEnumOrderType OrderType { get { return _ordertype; } set { _ordertype = value; } }

        private Order work;
        SymbolBasket _basket = null;
        public SymbolBasket DefaultBasket { get { return _basket; } set { _basket = value; } }

        PositionTracker _pt;
        public PositionTracker PositionTracker { get { return _pt; } set { _pt = value; } }

        OrderTracker _ot;
        public OrderTracker OrderTracker { get { return _ot; } set { _ot = value; } }

        bool _pricetouch = false;//价格框更新标识
        PriceFollow _pricefollow = PriceFollow.TRADE;

        bool ispostionmode = false;//是否处于持仓平仓模式


        public ctOrderSender()
        {
            InitializeComponent();
            //MessageBox.Show("init");
            
        }

        private void ctOrderSender_Load(object sender, EventArgs e)
        {
            //MessageBox.Show("load");

            //ismarket.IsChecked = true;
            //SetUI();
            InitOrderType();
            InitOffsetType();
        }



        private void debug(string s)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(PROGRAM +":"+s);
        }

        /*
        void SetUI()
        {
            askprice.ForeColor = UIGlobals.ShortSideColor;
            bidprice.ForeColor = UIGlobals.LongSideColor;
        }**/


        //初始化委托类型
        void InitOrderType()
        {
            ArrayList l = new ArrayList();
            ValueObject<QSEnumOrderType> vo1 = new ValueObject<QSEnumOrderType>();
            vo1.Name = "市价";
            vo1.Value = QSEnumOrderType.Market;
            l.Add(vo1);

            ValueObject<QSEnumOrderType> vo2 = new ValueObject<QSEnumOrderType>();
            vo2.Name = "限价";
            vo2.Value = QSEnumOrderType.Limit;
            l.Add(vo2);

            ValueObject<QSEnumOrderType> vo3 = new ValueObject<QSEnumOrderType>();
            vo3.Name = "Stop";
            vo3.Value = QSEnumOrderType.Stop;
            l.Add(vo3);

            orderType.DisplayMember = "Name";
            orderType.ValueMember = "Value";

            Factory.IDataSourceFactory(orderType).BindDataSource(l);
        }


        QSEnumOrderType CurrentOrderType {
            get {
                return (QSEnumOrderType)orderType.SelectedValue;
            }
        }



        #region 设定合约

        //当前合约symbol
        string CurrentSymbol { get { if (_sec != null ) { return _sec.Symbol; } else { return ""; } } }
        //当前合约对象
        public Symbol CurrentSecurity { get { return _sec; } }
        //当前持仓
        Position CurrentPosition { get { if (_sec != null ) { return _pt[_sec.Symbol]; } else { return null; } } }

        /// <summary>
        /// 当默认合约列表发生变化时候,我们触发 用于更新下拉列表
        /// </summary>
        public void OnBasketChange()
        {
            securityList.DataSource = null;
            //securityList.Items.Clear();
            ArrayList l = new ArrayList();

            foreach (Symbol s in DefaultBasket)
            {
                ValueObject<string> vo = new ValueObject<string>();
                vo.Name = s.Symbol + "  " + "" + "";
                vo.Value = s.Symbol;
                l.Add(vo);
                //m.Add(vo.Name);
            }
            //debug("binding security to here");
            //MessageBox.Show("binding security to here:" + DefaultBasket.Count.ToString());
            securityList.DisplayMember = "Name";
            securityList.ValueMember = "Value";

            Factory.IDataSourceFactory(securityList).BindDataSource(l);
            //securityList.DataSource = m;
        }


        

        /// <summary>
        /// 通过合约symbol设定合约
        /// 用于快捷设置合约
        /// </summary>
        /// <param name="symbol"></param>
        public void SetSecurity(string symbol)
        {
            Symbol sec = DefaultBasket[symbol];
            if (sec == null)
            {
                MessageForm.Show("自选合约不存在");
                return;
            }
            SetSecurity(sec);
            //UpdatePosSize();//更新合约持仓
            //updateoffsettype();//更新止损 设置对应的默认值
        }

        /// <summary>
        /// 设定当前交易合约
        /// </summary>
        /// <param name="sec"></param>
        public void SetSecurity(Symbol sec)
        {
            SetSecurity(sec, false);
        }


        /// <summary>
        /// 行情面板双击合约 选定合约,合约列表下拉选择合约,持仓双击 选定合约
        /// 设定合约包含了一些底层的数据的处理
        /// </summary>
        /// <param name="sec"></param>
        void SetSecurity(Symbol sec, bool setposition = false)
        {
            //Reset();
            if (sec == null) return;
            //设置合约,并调整price对应的参数
            _sec = sec;
            _dispdecpointformat = UIUtil.getDisplayFormat(sec.SecurityFamily.PriceTick);
            price.Increment = sec.SecurityFamily.PriceTick;
            price.DecimalPlaces = UIUtil.getDecimalPlace(sec.SecurityFamily.PriceTick);

            lossValue.Increment = sec.SecurityFamily.PriceTick;
            lossValue.DecimalPlaces = UIUtil.getDecimalPlace(sec.SecurityFamily.PriceTick);

            profitValue.Increment = sec.SecurityFamily.PriceTick;
            profitValue.DecimalPlaces = UIUtil.getDecimalPlace(sec.SecurityFamily.PriceTick);

            //生成对应的委托
            work = new OrderImpl(_sec.Symbol, 0);
            //work.Exchange= _sec.DestEx;
            work.LocalSymbol = _sec.Symbol;


            //更新下拉列表的选中合约
            if (securityList.SelectedValue != sec.Symbol)
                securityList.SelectedValue = sec.Symbol;

            debug("Set security to here："+setposition.ToString());
            //如果是平仓设定则不查询可开数量/选择合约，双击报价 则查询可开数量
            if (!setposition)
                UpdateMaxOpenSize();

            //默认手数为1，通过手工调节或者快速调节设定手数 查询默认设置,并设置手数
            int s = GetSize(_sec.Symbol);//获得默认手数
            UpdateSize(s);

            //更新持仓信息
            UpdatePositionSize();
            UpdateUnRealizedPL();
            UpdateRealizedPL();
            UpdateCanFlat();
        }


        


        #endregion



        #region 获得Trade Tick order cancel数据

        public void GotOrder(Order o)
        {
            if (this.InvokeRequired)
            {
                Invoke(new OrderDelegate(GotOrder), new object[] { o });
            }
            else
            {
                if(o.symbol.Equals(CurrentSymbol) && o.Status == QSEnumOrderStatus.Opened)
                {
                    UpdateMaxOpenSize();
                    UpdateCanFlat();
                }
            }
            
        }
        //获得成交数据时候更新对应的持仓信息
        public void GotTrade(Trade t)
        {
            if (this.InvokeRequired)
            {
                Invoke(new FillDelegate(GotTrade), new object[] { t });
            }
            else
            {
                if (t.symbol.Equals(CurrentSymbol))
                {
                    UpdateMaxOpenSize();
                    UpdatePositionSize();
                    if(t.PositionOperation == QSEnumPosOperation.DelPosition || t.PositionOperation == QSEnumPosOperation.ExitPosition || t.PositionOperation == QSEnumPosOperation.UNKNOWN)
                        UpdateRealizedPL();
                    UpdateCanFlat();
                }
            }
        }
        public void GotCancel(long id)
        {
            if (this.InvokeRequired)
            {
                Invoke(new LongDelegate(GotCancel), new object[] { id });
            }
            else
            {
                Order o = _ot.SentOrder(id);
                if (o != null && o.isValid && o.symbol.Equals(CurrentSymbol))
                {
                    UpdateMaxOpenSize();
                    UpdateCanFlat();
                }
            }
        }
        public void GotTick(Tick tick)
        {

            if (this.InvokeRequired)
            {

                Invoke(new TickDelegate(GotTick), new object[] { tick });

            }
            else
            {
                //更新价格
                try
                {
                    if ((tick == null) || (tick.symbol != CurrentSymbol)) return;

                    if (tick.hasAsk) askprice.Text = string.Format(_dispdecpointformat, tick.ask)+" /"+tick.os.ToString();
                    if (tick.hasBid) bidprice.Text = string.Format(_dispdecpointformat, tick.bid)+" /"+tick.bs.ToString();
               
                    if (!_pricetouch)
                    {
                        switch(_pricefollow)
                        {
                            case PriceFollow.TRADE:
                                if(tick.isTrade) price.Value = tick.trade;
                                break;
                            case PriceFollow.ASK:
                                if (tick.hasAsk) price.Value = tick.ask;
                                break;
                            case PriceFollow.BID:
                                if (tick.hasBid) price.Value = tick.bid;
                                break;
                            default:
                                break;
                        }
                    }

                    //更新当前持仓的浮动盈亏
                    UpdateUnRealizedPL();

                }
                catch (Exception e)
                {
                    debug(e.Message);
                }

            }


        }

        #endregion

        #region 更新持仓信息
        void UpdatePositionSize()
        {
            if (InvokeRequired)
                Invoke(new VoidDelegate(UpdatePositionSize), new object[] { });
            else
            {
                try
                {
                    
                    Position pos = CurrentPosition;
                    if (pos == null) return;
                    posSize.Text = pos.UnsignedSize.ToString();
                }
                catch(Exception ex)
                {
                    debug("UpdatePositionSize error:" + ex.ToString());    
                }
            }
        }
        void UpdateRealizedPL()
        {
            if (InvokeRequired)
                Invoke(new VoidDelegate(UpdateRealizedPL), new object[] { });
            else
            {
                try
                {
                    Position pos = CurrentPosition;
                    if (pos == null) return;
                    posRealizedPL.Text = string.Format(_dispdecpointformat,pos.ClosedPL);
                }
                catch (Exception ex)
                {
                    debug("UpdateRealizedPL error:" + ex.ToString());
                }
            }
        }
        void UpdateUnRealizedPL()
        {
            if (InvokeRequired)
                Invoke(new VoidDelegate(UpdateUnRealizedPL), new object[] { });
            else
            {
                try
                {
                    Position pos = CurrentPosition;
                    if (pos == null) return;
                    posUnrealizedPL.Text = string.Format(_dispdecpointformat, pos.UnRealizedPL);
                }
                catch (Exception ex)
                {
                    debug("UpdateUnRealizedPL error:" + ex.ToString());
                }
            }
        }
        void UpdateCanFlat()
        {
            if (InvokeRequired)
                Invoke(new VoidDelegate(UpdateCanFlat), new object[] { });
            else
            {
                try
                {

                    Position pos = CurrentPosition;
                    if (pos == null) return;
                    posCanFlat.Text = (pos.isFlat ? 0 : (pos.UnsignedSize - _ot.getUnfilledSizeExceptStop(pos.Symbol, !pos.isLong))).ToString();
                }
                catch (Exception ex)
                {
                    debug("UpdateUnRealizedPL error:" + ex.ToString());
                }
            }
        }
        #endregion

        #region 手数
        void UpdateSizeCtlMaximum()
        {
            if (InvokeRequired)
                Invoke(new VoidDelegate(UpdateSizeCtlMaximum), new object[] { });
            else
            {
                try
                {
                    maxOpenSize.Text = canopensize.ToString();
                    sizeTrack.Maximum = canopensize;
                    size.Maximum = canopensize;
                }
                catch (Exception ex)
                {
                    debug("UpdateSizeCtlMaximum error:" + ex.ToString());
                }
            }
        }
        void UpdateSizeCtlValue()
        {
            if (InvokeRequired)
                Invoke(new VoidDelegate(UpdateSizeCtlValue), new object[] { });
            else
            {
                try
                {
                    int csize = (int)sizeTrack.Value;
                    csize = csize < canopensize ? csize : canopensize;
                    size.Value = csize;
                }
                catch (Exception ex)
                {
                    debug("UpdateSizeCtlValue error:" + ex.ToString());
                }
            }
        }
        void UpdateSizeTrackValue()
        {
            if (InvokeRequired)
                Invoke(new VoidDelegate(UpdateSizeTrackValue), new object[] { });
            else
            {
                try
                {
                    float fs = (float)size.Value;
                    if (fs > sizeTrack.Maximum)
                        fs = sizeTrack.Maximum;
                    sizeTrack.Value = fs;
                }
                catch (Exception ex)
                {
                    debug("UpdateSizeTrackValue error:" + ex.ToString());
                }
            }
        }
        /// <summary>
        /// 获得合约对应的手数,如果该合约有默认手数就设定该手数
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        int GetSize(string symbol)
        {
            int size = 0;
            if (SendQryDefaultSizeEvent != null)
                size = SendQryDefaultSizeEvent(symbol);
            if (size == 0)
                return 1;
            else
                return size;
        }
        
        /// <summary>
        /// 更新可开数量
        /// 1.服务端查询 
        /// 2.本地计算
        /// </summary>
        public void UpdateMaxOpenSize()
        {
            //本地计算可开手数
            if (QryCanOpenPositionLocalEvent != null && CurrentSecurity != null)
            {
                int r = QryCanOpenPositionLocalEvent(CurrentSecurity.Symbol);
                UpdateMaxOpenSize(r);
                return;
            }
            //服务端查询
            if (QryCanOpenPositionEvent != null && CurrentSecurity !=null)
                QryCanOpenPositionEvent(CurrentSymbol);
        }

        //总可开数量 服务端获得最大可开数量 则通过回调事件调用该函数
        int canopensize = 0;
        public void UpdateMaxOpenSize(int psize)
        {
            canopensize = psize;
            UpdateSizeCtlMaximum();
        }

        //更新当前设定的手数
        void UpdateSize(int nsize)
        {
            int v = nsize <= canopensize ? nsize : canopensize;
            size.Value = v;
        }
        /// <summary>
        /// 设定总可开数量,用于保存可开手数
        /// -1 默认手数
        /// >0 设定对应的手数
        /// </summary>
        /// <param name="psize"></param>
        public void SetOrderSize(int size=-1)
        {
            if (size != -1)
            {
                if (size > canopensize)
                    size = canopensize;
                this.size.Value = size;
            }
            else
            {
                if (this.size.Value > canopensize)
                    this.size.Value = canopensize;
                if (this.size.Value == 0 && canopensize > 0)
                    this.size.Value = 1;
            }
        }

        #endregion

        #region 界面操作 事件
        //合约列表选择事件
        private void securityList_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                string sym = securityList.SelectedValue.ToString();
                debug("select security:" + sym);
                if (sym != CurrentSymbol)
                {
                    SetSecurity(sym);
                    //对外触发合约选择事件
                    if (SendSecuritySelectedEvent != null)
                        SendSecuritySelectedEvent();
                }
            }
            catch (Exception ex)
            {
                debug("选择合约出错:" + ex.ToString());
            }

        }

        //买入按钮
        private void btnBuy_Click(object sender, EventArgs e)
        {
            try
            {
                genOrder(true);
            }
            catch (Exception ex)
            {
                debug("error:" + ex.ToString());
            }
        }
        //卖出按钮
        private void btnSell_Click(object sender, EventArgs e)
        {
            try
            {
                genOrder(false);
            }
            catch (Exception ex)
            {
                debug("error:" + ex.ToString());
            }
        }


        //合约手数
        private void size_ValueChanged(object sender, EventArgs e)
        {
            UpdateSizeTrackValue();
        }
        //合约手数调节器
        private void sizeTrack_ValueChanged(object sender, EventArgs e)
        {
            UpdateSizeCtlValue();
        }

        //委托类别选择触发事件
        private void orderType_SelectedValueChanged(object sender, EventArgs e)
        {
            if (CurrentOrderType == QSEnumOrderType.Market)
            {
                price.Enabled = false;
            }
            else
            {
                price.Enabled = true;
            }
            _pricetouch = false;
        }

        #region 调整跟价设置
        private void bidlabel_DoubleClick(object sender, EventArgs e)
        {
            _pricefollow = PriceFollow.BID;
            _pricetouch = false;
            followtype.Text = "B";
            price.ForeColor = UIGlobals.LongSideColor;

        }

        private void bidlabel_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void asklabel_DoubleClick(object sender, EventArgs e)
        {
            _pricefollow = PriceFollow.ASK;
            _pricetouch = false;
            followtype.Text = "S";
            
        }

        private void asklabel_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void bidlabel_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void asklabel_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void followtype_DoubleClick(object sender, EventArgs e)
        {
            _pricefollow = PriceFollow.TRADE;
            _pricetouch = false;
            followtype.Text = "X";
        }

        private void followtype_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void followtype_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        //价格输入框 操作事件
        private void price_Enter(object sender, EventArgs e)
        {
            //MessageBox.Show("price enter");
            _pricetouch = true;
        }
        #endregion

        //全平按钮
        private void btnFall_Click(object sender, EventArgs e)
        {
            FlatPosition(CurrentPosition);
        }
        //全撤按钮
        private void btnCancelAll_Click(object sender, EventArgs e)
        {
            string sym = CurrentSymbol;
            if (string.IsNullOrEmpty(sym))
            {
                MessageForm.Show("无有效合约");
                return;
            }
            foreach (Order o in OrderTracker)
            {
                if ((o.symbol == sym) && (OrderTracker.isPending(o.id)))
                {
                    CancelOrder(o.id);
                }
            }
        }
        //反手按钮
        private void btnReserve_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region 功能函数

        /// <summary>
        /// 生成对应的买 卖委托并发送出去
        /// </summary>
        /// <param name="f"></param>
        private void genOrder(bool f)
        {
            if (!validSecurity()) return;
            if (!validSize()) return;
            if (!validPrice()) return;


            work.side = f;
            work.size = Math.Abs((int)size.Value);
            if (ismarket)
            {
                work.price = 0;
                work.stopp = 0;
            }
            else
            {
                bool islimit = this.islimit;
                decimal limit = islimit ? (decimal)(price.Value) : 0;
                decimal stop = !islimit ? (decimal)(price.Value) : 0;
                work.price = limit;
                work.stopp = stop;
            }
            if (AssumeNewOrder)
                work.id = 0;
            SendOrder(work);
        }

        /// <summary>
        /// 检查当前设置手数
        /// </summary>
        /// <returns></returns>
        bool validSize()
        {
            if ((int)(size.Value) == 0)
            {
                fmConfirm.Show("请设置手数");
                return false;
            }
            else
            {
                return true;
            }
        }
        bool ismarket { get { return CurrentOrderType == QSEnumOrderType.Market; } }
        bool islimit { get { return CurrentOrderType == QSEnumOrderType.Limit; } }

        bool validPrice()
        {
            if (ismarket|| (decimal)(price.Value)>0)
            {
                return true;
            }
            else
            {
                fmConfirm.Show("请设定价格");
                return false;
            }
        }
        /// <summary>
        /// 检查当前是否选中合约
        /// </summary>
        /// <returns></returns>
        bool validSecurity()
        {
            if (_sec == null)
            {
                fmConfirm.Show("请选择合约！");
                return false;
            }
            else
                return true;
        }


        void SendOrder(Order order)
        {
            debug("Send new order:" + order.ToString());
            //发送委托时,保存当前设置的止损止盈参数,用于持仓列表调用 并自动设置止损 止盈
            //updateSymProfitLossArgs();
            
            if (SendOrderEvent != null)
                SendOrderEvent(order);
            //将附带的止盈止损设置 发送到服务端
            SendOffsetAttached();
            //当重置 或者 在平仓模式下发送过为头后，平仓模式切换
            if (ispostionmode)
            {
                //Reset();
            }
        }

        void CancelOrder(long oid)
        {
            if (SendCancelEvent != null)
                SendCancelEvent(oid);
        }

        /// <summary>
        /// 撤销某个合约的所有委托
        /// </summary>
        /// <param name="symbol"></param>
        void CancelSymbol(string symbol)
        {
            foreach (Order o in _ot)
            {
                if (_ot.isPending(o.id))
                {
                    if (symbol == o.symbol)
                        CancelOrder(o.id);
                }
            }
        }
        void FlatPosition(Position pos)
        {
            if (pos == null || pos.isFlat) return;
            Order o = new MarketOrderFlat(pos);
            SendOrder(o);
            debug("全平仓位:" + pos.Symbol);
        }
        #endregion

        #region 止盈 止损 区域
        //在下单时 如果没有止盈止损则不对服务端的止盈止损参数进行更新
        //如果有止盈止损设置 则对服务端的止盈止损参数进行更新
        //初始化委托类型
        void InitOffsetType()
        {
            ArrayList l1 = new ArrayList();
            ValueObject<QSEnumPositionOffsetType> vo11 = new ValueObject<QSEnumPositionOffsetType>();
            vo11.Name = "价格";
            vo11.Value = QSEnumPositionOffsetType.PRICE;
            l1.Add(vo11);

            ValueObject<QSEnumPositionOffsetType> vo12 = new ValueObject<QSEnumPositionOffsetType>();
            vo12.Name = "点数";
            vo12.Value = QSEnumPositionOffsetType.POINTS;
            l1.Add(vo12);

            lossType.DisplayMember = "Name";
            lossType.ValueMember = "Value";


            ArrayList l2 = new ArrayList();
            ValueObject<QSEnumPositionOffsetType> vo21 = new ValueObject<QSEnumPositionOffsetType>();
            vo21.Name = "价格";
            vo21.Value = QSEnumPositionOffsetType.PRICE;
            l2.Add(vo21);

            ValueObject<QSEnumPositionOffsetType> vo22 = new ValueObject<QSEnumPositionOffsetType>();
            vo22.Name = "点数";
            vo22.Value = QSEnumPositionOffsetType.POINTS;
            l2.Add(vo22);
            profitType.DisplayMember = "Name";
            profitType.ValueMember = "Value";
            

            Factory.IDataSourceFactory(lossType).BindDataSource(l1);
            Factory.IDataSourceFactory(profitType).BindDataSource(l2);
        }
        QSEnumPositionOffsetType LossOffsetType { get { return (QSEnumPositionOffsetType)lossType.SelectedValue; } }
        QSEnumPositionOffsetType ProfitOffsetType { get { return (QSEnumPositionOffsetType)profitType.SelectedValue; } }
        
        void UpdateLossOffsetUI()
        {
            switch (LossOffsetType)
            {
                case QSEnumPositionOffsetType.PRICE:
                    {
                        lossValue.Value = price.Value;
                    }
                    break;
                case QSEnumPositionOffsetType.POINTS:
                    {
                        if (CurrentSecurity != null)
                            lossValue.Value = CurrentSecurity.SecurityFamily.PriceTick * 100;
                    }
                    break;
                default:
                    break;

            }
            
        }

        void UpdateProfitOffsetUI()
        {
            switch (ProfitOffsetType)
            {
                case QSEnumPositionOffsetType.PRICE:
                    {
                        profitValue.Value = price.Value;
                    }
                    break;
                case QSEnumPositionOffsetType.POINTS:
                    {
                        if (CurrentSecurity != null)
                            profitValue.Value = CurrentSecurity.SecurityFamily.PriceTick * 100;
                    }
                    break;
                default:
                    break;
            }
        }

        private void lossType_SelectedValueChanged(object sender, EventArgs e)
        {
            UpdateLossOffsetUI();

        }

        private void profitType_SelectedValueChanged(object sender, EventArgs e)
        {
            UpdateProfitOffsetUI();

        }

        //下单的同时 进行服务端止盈止损参数更新
        //下单时同时设置服务端止盈止损,则我们发送委托时,检查当前的设置 如果有止盈 止损设置 则向服务端提交止盈止损参数
        void SendOffsetAttached()
        {
            debug("下单面板附带止盈止损");
            //如果下单面板没有设定Account参数 则直接返回 止盈止损需要有Account进行标识
            if (string.IsNullOrEmpty(_account)) return;
            //止损有效 发送止损参数
            if (isLossSet.Checked)
            {
                debug("下单面板 附带止损设置");
                PositionOffsetArgs lossarg = new PositionOffsetArgs(_account, CurrentSymbol, QSEnumPositionOffsetDirection.LOSS);
                lossarg.Enable = true;
                lossarg.OffsetType = LossOffsetType;
                lossarg.Value = lossValue.Value;
                lossarg.Size = 0;
                if (UpdatePostionOffsetEvent != null)
                    UpdatePostionOffsetEvent(lossarg);
            }
            //止盈有效  发送止盈参数
            if (isProfitSet.Checked)
            {
                debug("下单面板 附带止盈设置");
                PositionOffsetArgs profitarg = new PositionOffsetArgs(_account, CurrentSymbol, QSEnumPositionOffsetDirection.PROFIT);
                profitarg.Enable = true;
                profitarg.OffsetType = ProfitOffsetType;
                profitarg.Value = profitValue.Value;
                profitarg.Size = 0;
                if (UpdatePostionOffsetEvent != null)
                    UpdatePostionOffsetEvent(profitarg);
            }

        
        }
        #endregion


        #region 快捷 平 撤 反 区域
        private void btnFall_MouseEnter(object sender, EventArgs e)
        {
            FillPrimitive f = ((FillPrimitive)btnFall.ButtonElement.GetChildrenByType(typeof(FillPrimitive))[0]);
            f.BackColor = Color.LightGray;
            //btnFall.ForeColor = Color.Black;
        }

        private void btnFall_MouseLeave(object sender, EventArgs e)
        {
            FillPrimitive f = ((FillPrimitive)btnFall.ButtonElement.GetChildrenByType(typeof(FillPrimitive))[0]);
            f.BackColor = Color.LimeGreen;
        }

        private void btnCancelAll_MouseEnter(object sender, EventArgs e)
        {
            FillPrimitive f = ((FillPrimitive)btnCancelAll.ButtonElement.GetChildrenByType(typeof(FillPrimitive))[0]);
            f.BackColor = Color.LightGray;
        }

        private void btnCancelAll_MouseLeave(object sender, EventArgs e)
        {
            FillPrimitive f = ((FillPrimitive)btnCancelAll.ButtonElement.GetChildrenByType(typeof(FillPrimitive))[0]);
            f.BackColor = Color.Orange;
        }

        private void btnReserve_MouseEnter(object sender, EventArgs e)
        {

            FillPrimitive f = ((FillPrimitive)btnReserve.ButtonElement.GetChildrenByType(typeof(FillPrimitive))[0]);
            f.BackColor = Color.LightGray;
        }

        private void btnReserve_MouseLeave(object sender, EventArgs e)
        {
            FillPrimitive f = ((FillPrimitive)btnReserve.ButtonElement.GetChildrenByType(typeof(FillPrimitive))[0]);
            f.BackColor = Color.Crimson;
        }
        #endregion




        






















    }
    //价格框显示价格类别
    internal enum PriceFollow
    { 
        TRADE,
        ASK,
        BID,
    }
   
}
