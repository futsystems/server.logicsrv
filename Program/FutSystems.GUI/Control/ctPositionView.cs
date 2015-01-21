using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using TradingLib.API;
using TradingLib.Common;


namespace FutSystems.GUI
{
    public partial class ctPositionView : UserControl,IPositionView
    {
        string _defaultformat = "{0:F1}";


        BindingSource datasource = new BindingSource();
        //持仓记录器 客户端是通过TradingTracker组件来记录order position记录的,服务端我们需要自己内建一个positiontracker用于记录相关信息
        LSPositionTracker pt;
        public LSPositionTracker PositionTracker { get { return pt; } set { pt = value; } }
        
        //委托记录器
        OrderTracker _ot;
        public OrderTracker OrderTracker { get { return _ot; } set { _ot = value; } }

        //委托事务辅助,用于执行反手等功能
        OrderTransactionHelper _ordTransHelper;


        //事件
        public event DebugDelegate SendDebugEvent;//日志对外输出时间
        public event OrderDelegate SendOrderEvent;//发送委托
        public event LongDelegate SendCancelEvent;//取消委托
        //public event FindSymbolDel FindSecurityEvent;//获得对应的合约信息
        public event PositionOffsetArgsDel UpdatePostionOffsetEvent;//对外触发止盈止损更新事件

        public event PositionDelegate PositionSelectedEvent;//选择了某个持仓
        //public event LookUpLossArgs LookUpLossArgsEvent;//获得sendorder中的某个合约的止损参数
        //public event LookUpProfitArgs LookUpProfitArgsEvent;//获得sendorder中的某个合约的止盈参数

        //通过symbol查找到对应的security
        Symbol findSecurity(string symbol)
        {
            return UIGlobals.FindSymbol(symbol);
        }


        //属性获得和设置
        [DefaultValue(true)]
        bool _enableoperation = true;
        public bool EnableOperation
        {
            get
            {
                return _enableoperation;
            }
            set
            {
                _enableoperation = value;
                btnCancel.Visible = _enableoperation;
                btnFlat.Visible = _enableoperation;
                btnFlatAll.Visible = _enableoperation;
                btnReserve.Visible = _enableoperation;
                btnCancel.Visible = _enableoperation;
                isDoubleFlat.Visible = _enableoperation;
            }
        }


        Dictionary<string, string> secFromatMap = new Dictionary<string, string>();
        //通过symbil获得对应的价格显示格式
        string getDisplayFormat(Symbol sym)
        {
            if (sym == null)
                return _defaultformat;
            if (secFromatMap.Keys.Contains(sym.Symbol))
                return secFromatMap[sym.Symbol];
           
            else
            {
                string f = UIUtil.getDisplayFormat(sym.SecurityFamily.PriceTick);
                secFromatMap.Add(sym.Symbol, f);
                return f;
            }
        }

        string getDisplayFormat(string sym)
        {
            if (secFromatMap.Keys.Contains(sym))
                return secFromatMap[sym];
            return _defaultformat; 
        }

        int getMultiple(Symbol sym)
        {
            if (sym == null)
                return 1;
            else
                return sym.Multiple;
        }

        ////获得止盈参数
        //ProfitArgs getDefaultprofitargs(string symbol)
        //{
        //    if (LookUpProfitArgsEvent != null)
        //        return LookUpProfitArgsEvent(symbol);
        //    return null;
        //}
        ////获得止损参数
        //StopLossArgs getDefaultlossargs(string symbol)
        //{
        //    if (LookUpLossArgsEvent != null)
        //        return LookUpLossArgsEvent(symbol);
        //    return null;
        //}

        void SendOrder(Order o)
        {
            if (SendOrderEvent != null)
                SendOrderEvent(o);
        }

        void CancelOrder(long oid)
        {
            if (SendCancelEvent != null)
                SendCancelEvent(oid);
        }

        /// <summary>
        /// 平仓
        /// </summary>
        /// <param name="pos"></param>
        void FlatPosition(Position pos)
        {
            if (pos == null || pos.isFlat) return;
            bool side = pos.isLong ? true : false;

            //上期所需要区分平今和平昨 需要按照昨仓和今仓分开提交委托
            if (pos.oSymbol.SecurityFamily.Exchange.EXCode.Equals("SHFE"))
            {
                int voltd = pos.PositionDetailTodayNew.Sum(p => p.Volume);//今日持仓
                int volyd = pos.PositionDetailYdNew.Sum(p => p.Volume);//昨日持仓
                //MessageBox.Show("posyd:" + volyd.ToString() + " voltd:" + voltd.ToString());
                if (volyd != 0)
                {
                    Order oyd = new OrderImpl(pos.Symbol, volyd *(side?1:-1)* -1);
                    oyd.OffsetFlag = QSEnumOffsetFlag.CLOSE;
                    SendOrder(oyd);
                }
                if (voltd != 0)
                {
                    Order otd = new OrderImpl(pos.Symbol, voltd * (side ? 1 : -1) * -1);
                    otd.OffsetFlag = QSEnumOffsetFlag.CLOSETODAY;
                    SendOrder(otd);
                }
            }
            else
            {
                Order o = new MarketOrderFlat(pos);
                o.OffsetFlag = QSEnumOffsetFlag.CLOSE;
                SendOrder(o);
                debug("全平仓位:" + pos.Symbol);
            }
        }
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }




        const string SYMBOL = "合约";
        const string SIDE = "方向";
        const string DIRECTION = "多空";
        const string SIZE = "总持仓";
        const string YDSIZE = "昨仓";
        const string CANFLATSIZE = "可平量";//用于计算当前限价委托可以挂单数量
        const string LASTPRICE = "最新";//最新成交价
        const string AVGPRICE = "持仓均价";
        const string UNREALIZEDPL = "浮盈";
        const string REALIZEDPL = "平盈";
        const string UNREALIZEDPLPOINT = "浮盈点数";
        const string REALIZEDPLPOINT = "平盈点数";
        const string ACCOUNT = "账户";
        const string PROFITTARGET = "止盈";
        const string STOPLOSS = "止损";
        const string KEY = "编号";
        //const string ACCOUNT = "帐户";
        //const string STRATEGY = "出场策略";
        DataTable gt = new DataTable();


        public ctPositionView()
        {
            InitializeComponent();
            SetPreferences();
            InitTable();
            BindToTable();

            WireEvent();
            _ordTransHelper = new OrderTransactionHelper("Moniter");
            _ordTransHelper.SendOrderEvent += new OrderDelegate(SendOrder);
            
            _ordTransHelper.Start();
            //初始化止盈止损设置窗口
            //frmPosOffset = new frmPositionOffset();
            //frmPosOffset.TopMost = true;
            //frmPosOffset.UpdatePostionOffsetEvent += (PositionOffsetArgs args) =>
            //{
            //    if (UpdatePostionOffsetEvent != null)
            //        UpdatePostionOffsetEvent(args);
            //};
            
        }


        #region 止盈止损参数

        //frmPositionOffset frmPosOffset;
        //止损参数映射
        ConcurrentDictionary<string, PositionOffsetArgs> lossOffsetMap = new ConcurrentDictionary<string, PositionOffsetArgs>();
        //止盈参数映射
        ConcurrentDictionary<string, PositionOffsetArgs> profitOffsetMap = new ConcurrentDictionary<string, PositionOffsetArgs>();


        PositionOffsetArgs GetLossArgs(string key)
        {
            if (lossOffsetMap.Keys.Contains(key))
                return lossOffsetMap[key];
            else
                return null;
        }

        PositionOffsetArgs GetProfitArgs(string key)
        {
            if (profitOffsetMap.Keys.Contains(key))
                return profitOffsetMap[key];
            else
                return null;
        }

        //设置止盈止损参数时,弹出窗体,然后按相应操作触发 参数提交的操作
        //服务端获得参数设置成功后,回传对应的参数

        void ResetOffset(string key)
        {
            PositionOffsetArgs p = GetProfitArgs(key);
            if (p != null)
                p.Enable = false;
            PositionOffsetArgs l = GetLossArgs(key);
            if (l != null)
                l.Enable = false;
        }

        //获得显示的数值
        string GetGridOffsetText(Position pos,bool side, QSEnumPositionOffsetDirection direction)
        {
            string key = pos.GetKey(side);
            decimal price = direction == QSEnumPositionOffsetDirection.LOSS ? GetLossArgs(key).TargetPrice(pos) : GetProfitArgs(key).TargetPrice(pos);
            if (price == -1)
            {
                return "停止";
            }
            if (price == 0)
            {
                return "无持仓";
            }
            else
            {
                return string.Format(getDisplayFormat(pos.Symbol), price);
            }

        }

        #endregion


        //合约所对应的table id key为 account - symbol
        ConcurrentDictionary<string, int> symRowMap = new ConcurrentDictionary<string, int>();
        //通过account-symbol获得对应的tableid
        int positionidx(string acc_sym)
        {
            if (symRowMap.Keys.Contains(acc_sym))
                return symRowMap[acc_sym];
            return -1;
        }
        //获得持昂方向
        //string getDirection(int size)
        //{
        //    return size == 0 ? "无持仓" : (size > 0 ? "看多" : "看空");
        //}

        //获得某个持仓的可平数量
        int getCanFlatSize(Position pos)
        {
            return pos.isFlat ? 0 : (pos.UnsignedSize - _ot.GetPendingExitSize(pos.Symbol,pos.DirectionType== QSEnumPositionDirectionType.Long?true:false));
        }

        //往datatable中插入一行记录
        int InsertNewRow(Position pos,bool positionside)
        {
            string account = pos.Account;
            string symbol = pos.Symbol;
            gt.Rows.Add(symbol);
            int i = gt.Rows.Count - 1;//得到新建的Row号
            //如果不存在,则我们将该account-symbol对插入映射列表我们的键用的是account_symbol配对
            string key = pos.GetKey(positionside);
            gt.Rows[i][SIDE] = positionside;
            gt.Rows[i][DIRECTION] = positionside ? "多" : "空";
            gt.Rows[i][KEY] = key;
            gt.Rows[i][ACCOUNT] = pos.Account;

            debug("new row inserted,account-symbol-side:" + key);
            if (!symRowMap.ContainsKey(key))
                symRowMap.TryAdd(key, i);
            //同时为该key准备positoinoffsetarg
            lossOffsetMap[key] = new PositionOffsetArgs(account, symbol,positionside, QSEnumPositionOffsetDirection.LOSS);
            profitOffsetMap[key] = new PositionOffsetArgs(account, symbol, positionside, QSEnumPositionOffsetDirection.PROFIT);

            return i;
        }

        #region 辅助功能函数
        //获得当前选中持仓
        Position CurrentPositoin
        {
            get
            {

                if (positiongrid.SelectedRows.Count > 0)
                {
                    int row = positiongrid.SelectedRows[0].Index;
                    string account = positiongrid[ACCOUNT, row].Value.ToString();
                    string sym = positiongrid[SYMBOL, row].Value.ToString();
                    bool side = bool.Parse(positiongrid[SIDE, row].Value.ToString());
                    debug("sym:" + sym + " account:" + account + " side:" + side.ToString());
                    Position pos = pt[sym, account, side];
                    debug("Pos:" + pos.ToString());
                    return pos;
                }
                else
                {
                    return null;
                }
            }
        }

        //获得当前选中合约
        string CurrentSymbol
        {
            get
            {
                if (positiongrid.SelectedRows.Count > 0)
                {
                    int row = positiongrid.SelectedRows[0].Index;
                    string sym = positiongrid[SYMBOL, row].Value.ToString();
                    return sym;
                }
                else
                {
                    return null;
                }
            }
        }

        //获得当前key account-symbol
        string CurrentKey
        {
            get
            {
                if (positiongrid.SelectedRows.Count > 0)
                {
                    string key = positiongrid.CurrentRow.Cells[KEY].Value.ToString();
                    return key;
                }
                else
                {
                    return "";
                }
            }
        }

        
        #endregion


        #region 相应服务端数据回报
        //获得昨日隔夜持仓，作为基数累加后得到当前持仓数据
        public void GotPosition(Position pos)
        {

            if (InvokeRequired)
            {
                try
                {
                    Invoke(new PositionDelegate(GotPosition), new object[] { pos });
                }
                catch (Exception ex)
                { }
            }
            else
            {

                int posidx = positionidx(pos.GetKey(pos.isLong));//通过position key 获得对应的idx
                string _fromat = getDisplayFormat(pos.oSymbol);
                if ((posidx > -1) && (posidx < gt.Rows.Count))//idx存在
                {
                    int size = pos.Size;
                    gt.Rows[posidx][SIZE] = Math.Abs(size);
                    gt.Rows[posidx][YDSIZE] = pos.PositionDetailYdNew.Sum(p => p.Volume); 
                    gt.Rows[posidx][CANFLATSIZE] = getCanFlatSize(pos);
                    gt.Rows[posidx][AVGPRICE] = string.Format(getDisplayFormat(pos.oSymbol), pos.AvgPrice);
                    gt.Rows[posidx][REALIZEDPL] = string.Format(getDisplayFormat(pos.oSymbol), pos.ClosedPL * getMultiple(pos.oSymbol));
                    gt.Rows[posidx][REALIZEDPLPOINT] = string.Format(getDisplayFormat(pos.oSymbol), pos.ClosedPL);
                    num.Text = positiongrid.RowCount.ToString();
                }
                else//idx不存在
                {
                    //如果不存在,则我们将该account-symbol对插入映射列表我们的键用的是account_symbol配对
                    int i = InsertNewRow(pos, pos.isLong);
                    int size = pos.Size;
                    gt.Rows[i][SIZE] = Math.Abs(size);
                    gt.Rows[i][YDSIZE] = pos.PositionDetailYdNew.Sum(p => p.Volume); 
                    gt.Rows[i][CANFLATSIZE] = getCanFlatSize(pos);
                    gt.Rows[i][AVGPRICE] = string.Format(getDisplayFormat(pos.oSymbol), pos.AvgPrice);
                    gt.Rows[i][REALIZEDPL] = string.Format(getDisplayFormat(pos.oSymbol), pos.ClosedPL * getMultiple(pos.oSymbol));
                    gt.Rows[i][REALIZEDPLPOINT] = string.Format(getDisplayFormat(pos.oSymbol), pos.ClosedPL);
                    num.Text = positiongrid.RowCount.ToString();
                    
                }
            }

        }

        public void GotTick(Tick t)
        {
            //debug("position view got tick:" + t.ToString());
            if (InvokeRequired)
            {
                
                    Invoke(new TickDelegate(GotTick), new object[] { t });
               
            }
            else
            {
                try
                {
                    string _fromat = getDisplayFormat(t.Symbol);
                    //数据列中如果是该symbol则必须全比更新
                    for (int i = 0; i < gt.Rows.Count; i++)
                    {
                        //便利所有合约与tick.symbol相同的行
                        //debug("Ticktime"+t.time.ToString()+"symbol:" + gt.Rows[i][SYMBOL].ToString() + " side:" + gt.Rows[i][SIDE].ToString());
                        //debug("row idx:" + i.ToString() + " acc:" + gt.Rows[i][ACCOUNT].ToString() + " symbol:" + gt.Rows[i][SYMBOL].ToString() + " side:" + gt.Rows[i][SIDE].ToString() + " rowsnum:" + gt.Rows.Count.ToString());
                        if (gt.Rows[i][SYMBOL].ToString() == t.Symbol)
                        {
                            //记录该仓位所属账户
                            string acc = gt.Rows[i][ACCOUNT].ToString();
                            bool posside = bool.Parse(gt.Rows[i][SIDE].ToString());
                            Position pos = pt[t.Symbol, acc, posside];
                            string key = pos.GetKey(posside);
                            decimal unrealizedpl = pos.UnRealizedPL;
                            //debug("accc-symbol-side:" + key);
                            //更新最新成交价
                            if (t.isTrade)
                            {
                                gt.Rows[i][LASTPRICE] = string.Format(getDisplayFormat(t.Symbol), t.Trade);
                            }
                            //空仓 未平仓合约与 最新价格
                            if (pos.isFlat)
                            {
                                gt.Rows[i][UNREALIZEDPL] = 0;
                                gt.Rows[i][UNREALIZEDPLPOINT] = 0;
                            }
                            else
                            {
                                //更新unrealizedpl
                                gt.Rows[i][UNREALIZEDPL] = string.Format(getDisplayFormat(pos.oSymbol), unrealizedpl * getMultiple(pos.oSymbol));
                                gt.Rows[i][UNREALIZEDPLPOINT] = string.Format(getDisplayFormat(pos.oSymbol), unrealizedpl);
                            }

                            //gt.Rows[i][STOPLOSS] = GetGridOffsetText(pos, QSEnumPositionOffsetDirection.LOSS);
                            //gt.Rows[i][PROFITTARGET] = GetGridOffsetText(pos, QSEnumPositionOffsetDirection.PROFIT);
                        }
                    }
                }
                catch (Exception ex)
                {
                    debug("error:" + ex.ToString());
                }
        
            }
        }


        /// <summary>
        /// 获得委托数据回报 用于更新可平数量
        /// </summary>
        /// <param name="o"></param>
        public void GotOrder(Order o)
        {
            //当有委托近来时候,我们需要重新计算我们所对应的可以平仓数量
            if (InvokeRequired)
            {
                try
                {
                    Invoke(new OrderDelegate(GotOrder), new object[] { o });
                }
                catch (Exception ex)
                { }
            }
            else
            {
                bool posside = o.PositionSide;
                Position pos = pt[o.Symbol, o.Account, posside];
                //通过account_symbol键对找到对应的行
                int posidx = positionidx(pos.GetKey(posside));
                string _fromat = getDisplayFormat(pos.Symbol);
                //如果持仓条目已经存在 更新可平数量 委托回报只会更新可平数量
                if ((posidx > -1) && (posidx < gt.Rows.Count))
                {
                    gt.Rows[posidx][CANFLATSIZE] = getCanFlatSize(pos);
                }

            }
        }

        /// <summary>
        /// 获得委托取消回报 获得委托取消回报 用于更新可平数量
        /// </summary>
        /// <param name="oid"></param>
        public void GotCancel(long oid)
        {
            //当有委托近来时候,我们需要重新计算我们所对应的可以平仓数量
            if (InvokeRequired)
            {
                try
                {
                    Invoke(new LongDelegate(GotCancel), new object[] { oid });
                }
                catch (Exception ex)
                { 
                    
                }
            }
            else
            {
                Order o = _ot.SentOrder(oid);
                if (o == null || !o.isValid) return;
                bool posside = o.PositionSide;
                Position pos = pt[o.Symbol, o.Account,posside];
                //通过account_symbol键对找到对应的行
                int posidx = positionidx(o.Account + "_" + o.Symbol);
                string _fromat = getDisplayFormat(pos.Symbol);
                if ((posidx > -1) && (posidx < gt.Rows.Count))
                {
                    gt.Rows[posidx][CANFLATSIZE] = getCanFlatSize(pos);
                    //updateCurrentRowPositionNum();
                }
            }
        }

        public void GotFill(Trade t)
        {
            if (InvokeRequired)
            {
                try
                {
                    Invoke(new FillDelegate(GotFill), new object[] { t });
                }
                catch (Exception ex)
                { }
            }
            else
            {
                
                bool posside = t.PositionSide;//每个成交可以确定仓位操作方向比如是多头操作(买入1手开仓 卖出1手平仓) 还是空头操作(卖出1手开仓 买入1手平仓)
                Position pos = pt[t.Symbol, t.Account, posside];//获得对应持仓数据
                //通过account_symbol键对找到对应的行
                string key = pos.GetKey(posside);
                int posidx = positionidx(key);
                if ((posidx > -1) && (posidx <gt.Rows.Count))
                {
                    int size = pos.Size;
                    gt.Rows[posidx][SIZE] = Math.Abs(size);
                    gt.Rows[posidx][YDSIZE] = pos.PositionDetailYdNew.Sum(p => p.Volume); 
                    gt.Rows[posidx][CANFLATSIZE] = getCanFlatSize(pos);
                    gt.Rows[posidx][AVGPRICE] = string.Format(getDisplayFormat(pos.oSymbol), pos.AvgPrice);
                    gt.Rows[posidx][REALIZEDPL] = string.Format(getDisplayFormat(pos.oSymbol), pos.ClosedPL * getMultiple(pos.oSymbol));
                    gt.Rows[posidx][REALIZEDPLPOINT] = string.Format(getDisplayFormat(pos.oSymbol), pos.ClosedPL);

                    if (pos.isFlat)
                    {
                        ResetOffset(key);
                    }
                    //gt.Rows[posidx][STOPLOSS] = GetGridOffsetText(pos, posside,QSEnumPositionOffsetDirection.LOSS);
                    //gt.Rows[posidx][PROFITTARGET] = GetGridOffsetText(pos,posside, QSEnumPositionOffsetDirection.PROFIT);
                    num.Text = positiongrid.RowCount.ToString();
                }
                else
                {
                    //如果不存在,则我们将该account-symbol对插入映射列表我们的键用的是account_symbol配对
                    int i = InsertNewRow(pos,posside);
                    int size = pos.Size;
                    gt.Rows[i][SIZE] = Math.Abs(size);
                    gt.Rows[i][YDSIZE] = pos.PositionDetailYdNew.Sum(p => p.Volume); 
                    gt.Rows[i][CANFLATSIZE] = getCanFlatSize(pos);
                    gt.Rows[i][AVGPRICE] = string.Format(getDisplayFormat(pos.Symbol), pos.AvgPrice);
                    gt.Rows[i][REALIZEDPL] = string.Format(getDisplayFormat(pos.Symbol), pos.ClosedPL * getMultiple(pos.oSymbol));
                    gt.Rows[i][REALIZEDPLPOINT] = string.Format(getDisplayFormat(pos.Symbol), pos.ClosedPL);
                }
                _ordTransHelper.GotFill(t);
            }
        }

        #endregion


        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = positiongrid;

            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ColumnHeadersHeight = 25;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ReadOnly = true;
            grid.RowHeadersVisible = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke;

            grid.StateCommon.Background.Color1 = Color.WhiteSmoke;
            grid.StateCommon.Background.Color2 = Color.WhiteSmoke;
        }
        /// <summary>
        /// 初始化数据表格
        /// </summary>
        private void InitTable()
        {
            gt.Columns.Add(SYMBOL);//0
            gt.Columns.Add(SIDE);//1
            
            gt.Columns.Add(DIRECTION);//2
            gt.Columns.Add(SIZE, typeof(int));//3
            gt.Columns.Add(YDSIZE,typeof(int));//1
            gt.Columns.Add(CANFLATSIZE, typeof(int));//4
            gt.Columns.Add(LASTPRICE);//5
            gt.Columns.Add(AVGPRICE);//6
            gt.Columns.Add(UNREALIZEDPL);//7
            gt.Columns.Add(REALIZEDPL);//8
            gt.Columns.Add(UNREALIZEDPLPOINT);//9
            gt.Columns.Add(REALIZEDPLPOINT);//10
            
            gt.Columns.Add(PROFITTARGET);
            gt.Columns.Add(STOPLOSS);
            gt.Columns.Add(ACCOUNT);
            gt.Columns.Add(KEY);
            //gt.Columns.Add(STRATEGY, typeof(Image));

        }
        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = positiongrid;
            //grid.TableElement.BeginUpdate();             
            //grid.MasterTemplate.Columns.Clear(); 
            datasource.DataSource = gt;
            grid.DataSource = datasource;
            //grid.Columns[ACCOUNT].IsVisible = false;
            grid.Columns[KEY].Visible = false;
            grid.Columns[PROFITTARGET].Visible = false;
            grid.Columns[STOPLOSS].Visible = false;
            grid.Columns[SIDE].Visible = false;

            //set width
            grid.Columns[SYMBOL].Width = 45;
            //grid.Columns[SYMBOL].TextAlignment = ContentAlignment.MiddleCenter;
            grid.Columns[DIRECTION].Width = 45;
            //grid.Columns[DIRECTION].TextAlignment = ContentAlignment.MiddleCenter;
            grid.Columns[SIZE].Width = 45;
            //grid.Columns[SIZE].TextAlignment = ContentAlignment.MiddleCenter;
            grid.Columns[CANFLATSIZE].Width = 45;
            //grid.Columns[CANFLATSIZE].TextAlignment = ContentAlignment.MiddleCenter;
            grid.Columns[LASTPRICE].Width = 65;
            //grid.Columns[LASTPRICE].TextAlignment = ContentAlignment.MiddleRight;
            grid.Columns[AVGPRICE].Width = 65;
            //grid.Columns[AVGPRICE].TextAlignment = ContentAlignment.MiddleRight;
            grid.Columns[UNREALIZEDPL].Width = 65;
            //grid.Columns[UNREALIZEDPL].TextAlignment = ContentAlignment.MiddleRight;
            grid.Columns[REALIZEDPL].Width = 65;
            //grid.Columns[REALIZEDPL].TextAlignment = ContentAlignment.MiddleRight;
            grid.Columns[PROFITTARGET].Width = 75;
            //grid.Columns[PROFITTARGET].TextAlignment = ContentAlignment.MiddleCenter;
            grid.Columns[STOPLOSS].Width = 75;
            //grid.Columns[STOPLOSS].TextAlignment = ContentAlignment.MiddleCenter;

            for (int i = 0; i < gt.Columns.Count; i++)
            {
                grid.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

        }

        private void ctPositionView_Load(object sender, EventArgs e)
        {
            //SetPreferences();
            //InitTable();
            //BindToTable();
        }




        void WireEvent()
        {
            btnShowAll.CheckedChanged += new EventHandler(btnShowAll_CheckedChanged);
            btnShowHold.CheckedChanged += new EventHandler(btnShowHold_CheckedChanged);
            btnFlat.Click +=new EventHandler(btnFlat_Click);
            btnFlatAll.Click +=new EventHandler(btnFlatAll_Click);
            btnCancel.Click +=new EventHandler(btnCancel_Click);
            btnReserve.Click +=new EventHandler(btnReserve_Click);

            positiongrid.DoubleClick +=new EventHandler(positiongrid_DoubleClick);
            positiongrid.CellFormatting += new DataGridViewCellFormattingEventHandler(positiongrid_CellFormatting);
            positiongrid.RowPrePaint += new DataGridViewRowPrePaintEventHandler(positiongrid_RowPrePaint);
        }

        void positiongrid_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            e.PaintParts = e.PaintParts ^ DataGridViewPaintParts.Focus;
        }

        void positiongrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 2)
                {
                    string direction = positiongrid[DIRECTION, e.RowIndex].Value.ToString();
                    if (direction.Equals("多"))
                    {
                        e.CellStyle.ForeColor = UIGlobals.LongSideColor;
                        e.CellStyle.Font = UIGlobals.BoldFont;
                    }
                    else if (direction.Equals("空"))
                    {
                        e.CellStyle.ForeColor = UIGlobals.ShortSideColor;
                        e.CellStyle.Font = UIGlobals.BoldFont;
                    }
                    else
                    {
                        e.CellStyle.ForeColor = Color.Black;
                        e.CellStyle.Font = UIGlobals.BoldFont;
                    }
                }

                if (e.ColumnIndex == 8 || e.ColumnIndex == 9 || e.ColumnIndex == 10 || e.ColumnIndex == 11)
                {

                    decimal v = 0;
                    decimal.TryParse(positiongrid[e.ColumnIndex, e.RowIndex].Value.ToString(), out v);
                    if (v > 0)
                    {
                        e.CellStyle.ForeColor = UIGlobals.LongSideColor;
                        e.CellStyle.Font = UIGlobals.BoldFont;
                    }
                    else if (v < 0)
                    {
                        e.CellStyle.ForeColor = UIGlobals.ShortSideColor;
                        e.CellStyle.Font = UIGlobals.BoldFont;
                    }
                    else
                    {
                        e.CellStyle.ForeColor = Color.Black;
                        e.CellStyle.Font = UIGlobals.BoldFont;
                    }
                }
            }
            catch (Exception ex)
            {
                debug("cell format error:" + ex.ToString());
            }

        }


        #region 界面操作事件
        private void btnShowAll_CheckedChanged(object sender, EventArgs e)
        {
            string strFilter = "";
            datasource.Filter = strFilter;
        }

        private void btnShowHold_CheckedChanged(object sender, EventArgs e)
        {
            string strFilter;
            strFilter = String.Format(SIZE + " > '{0}'","0");
            datasource.Filter = strFilter;
        }
        //平掉当前选中持仓
        private void btnFlat_Click(object sender, EventArgs e)
        {
            Position pos = CurrentPositoin;
            if (pos == null)
            {
                ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("请选择持仓");
                return;
            }
            if (pos.isFlat)
            {
                ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("该合约没有持仓");
                return;
            }
            FlatPosition(CurrentPositoin);
            
        }
        //平调所有持仓
        private void btnFlatAll_Click(object sender, EventArgs e)
        {
            foreach (Position pos in pt)
            {
                FlatPosition(pos);
            }
        }
        //撤单
        private void btnCancel_Click(object sender, EventArgs e)
        {
            string sym = CurrentSymbol;
            if (string.IsNullOrEmpty(sym))
            {
                ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("请选中持仓");
                return;
            }
            foreach (Order o in OrderTracker)
            {
                if ((o.Symbol == sym) && (OrderTracker.isPending(o.id)))
                {
                    CancelOrder(o.id);
                }
            }
        }
        //反手
        private void btnReserve_Click(object sender, EventArgs e)
        {
            Position pos = CurrentPositoin;
            if (pos != null && !pos.isFlat)
            {
                Order o = new MarketOrderFlat(pos);
                debug("提交反手事务");
                _ordTransHelper.AddFlatPositionBeforeInsert(pos, o);
                Order no = new OrderImpl(o);
                SendOrder(no);
            }
            else
            {
                ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("请选择有效持仓!");
            }
        }

        //双击持仓某行
        //1.修改止盈止损参数
        private void positiongrid_DoubleClick(object sender, EventArgs e)
        {
            
            int rownum = positiongrid.CurrentRow.Index+1;
            //止盈 止损设置
            //if (positiongrid.CurrentColumn.Name == STOPLOSS)
            //{
            //    //MessageBox.Show("stoploss edit");
            //    //frmPositionOffset fm = new frmPositionOffset();
            //    //fm.TopMost = true;
            //    frmPosOffset.ParseOffset(GetLossArgs(CurrentKey),CurrentPositoin,findSecurity(CurrentSymbol));
            //    Point p = this.PointToScreen(positiongrid.Location);
            //    p.X = p.X + 250;
            //    p.Y = p.Y + UIGlobals.HeaderHeight + UIGlobals.RowHeight * rownum;
            //    frmPosOffset.Location = p;
            //    frmPosOffset.Show();
            //    return;
            //}
            //if (positiongrid.CurrentColumn.Name == PROFITTARGET)
            //{
            //    //frmPositionOffset fm = new frmPositionOffset();
            //    //fm.TopMost = true;
            //    frmPosOffset.ParseOffset(GetProfitArgs(CurrentKey), CurrentPositoin, findSecurity(CurrentSymbol));
            //    Point p = this.PointToScreen(positiongrid.Location);
            //    p.X = p.X + 250;
            //    p.Y = p.Y + UIGlobals.HeaderHeight + UIGlobals.RowHeight * rownum;
            //    frmPosOffset.Location = p;
            //    frmPosOffset.Show();
            //    return;
            //}

            //非止盈止损列的双击并且设定为双击平仓,平调所选持仓
            if (isDoubleFlat.Checked)
            {
                FlatPosition(CurrentPositoin);
            }
            else
            { 
            
            }

        }
        #endregion



        // Fires when the editor changes its value. The value is stored only inside the editor. 
        //private void positiongrid_ValueChanged(object sender, EventArgs e)
        //{
        //    MessageBox.Show("eidtor value changed");
        //    StopLossEditor edit = sender as StopLossEditor;
        //    if (edit != null)
        //    {
        //        GridCellElement cellstop = positiongrid.TableElement.GetCellElement(positiongrid.CurrentRow, positiongrid.Columns[STOPLOSS]);
        //        if (cellstop != null)
        //        {
        //            cellstop.Text = "i am here";
        //        }
        //    }
        //}




        public void Clear()
        {
            positiongrid.DataSource = null;
            symRowMap.Clear();
            gt.Rows.Clear();
            BindToTable();
        }

        
        


    }
}
