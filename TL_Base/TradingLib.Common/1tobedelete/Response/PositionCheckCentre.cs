//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TradingLib.API;
//using System.Reflection;


//namespace TradingLib.Common
//{
//    //关于数据驱动
//    //策略如果没有生效,那么我们tick数据就没有必要由策略去运算.只有当策略被激活的时候 策略才可以相应数据
//    //仓位策略运行的容器,仓位策略激活，修改的内存数据均在这里进行保存并应用
//    //当某个symbol有仓位时,右键出现的可用策略也是从这里获取
//    //
//    public class PositionCheckCentre 
//    {
//        public event DebugDelegate SendDebugEvent;
//        public event SwitchPositionCheckDel SwitchPositionCheckEvent;
//        public event OrderDelegate SendOrderEvent;
//        public event LongDelegate CancelOrderEvent;

//        void out_sendorder(Order o)
//        {
//            if(SendOrderEvent!=null)
//            {
//                SendOrderEvent(o);
//            }
//        }
//        void out_cancelorder(long oid)
//        {
//            if (CancelOrderEvent != null)
//            {
//                CancelOrderEvent(oid);
//            }
//        }

//        void SwitchPositionCheck(IPositionCheck check, bool run)
//        {
//            if (SwitchPositionCheckEvent != null)
//                SwitchPositionCheckEvent(check, run);
//        }
//        private SymbolBasket _defaultBasket;
//        //private ITradingCore _coreCentre;
//        private ITradingTrackerCentre _ttc;

//        private bool handleresponseexception=true;
//        public bool HandleResponseException { get { return handleresponseexception; } set { handleresponseexception = value; } }
//        public bool disableresponseonexception = false;
//        public bool DisableResponseOnException { get { return disableresponseonexception; } set { disableresponseonexception = value; } }
        
//        //strategyTemple数据结构
//        //名称(类的全名 命名空间+类名)与策略类型的对应关系
//        private Dictionary<string, Type> poscheckTypeMap = new Dictionary<string, Type>();
//        //类型到某个类型positioncheck的中文名 
//        private Dictionary<Type, string> poscheckTypeTitleMap = new Dictionary<Type, string>();
//        //中文名称->类型的对应 用于菜单调用时候的策略索引
//        private Dictionary<string, Type> poscheckTitleTypeMap = new Dictionary<string, Type>();

//        //response实例数据结构
//        //用于储存response列表,其他对response的调用均通过列表与ID进行
//        private List<Response> _reslist = new List<Response>();
//        //从配置文件加载上来的string - >仓位策略Response之间的映射关系
//        private Dictionary<string, List<int>> symPositionCheckMap = new Dictionary<string, List<int>>();
//        //每个response中文名(参数名) - response的映射关系 用于从菜单标题缩影到该Response然后进行操作
//        private Dictionary<string,int> _menuTitle2Idx = new Dictionary<string,int>();
//        private Dictionary<int, string> _Idx2menuTitle = new Dictionary<int, string>();

//        //idx与symbol对应关系
//        private Dictionary<int, string> _Idx2Symbol = new Dictionary<int, string>();
//        //本地list index与response ID的映射关系
//        private Dictionary<int,int> _rid2LocalIdx = new Dictionary<int,int>();
        

        

//        const int MAXRESPONSEPERASP = 100;
//        const int MAXASPINSTANCE = 1;
//        int _ASPINSTANCE = 0;
//        int _INITIALRESPONSEID = 0;
//        int _NEXTRESPONSEID = 0;

//        //仓位检查策略 构造时需要提供CoreCentre TradingTrackerCentere 用于获得当前的交易状态与交易信息
//        public PositionCheckCentre(ITradingTrackerCentre ttc)
//        {
//            //_coreCentre = cc;
//            _ttc = ttc;
//            // count instances of program
//            _ASPINSTANCE =  1;
//            // set next response id
//            _NEXTRESPONSEID = _ASPINSTANCE * MAXRESPONSEPERASP;
//            _INITIALRESPONSEID = _NEXTRESPONSEID;
//            InitPositionCheckCentre();
//        }
//        private void debug(string msg)
//        {
//            if (SendDebugEvent != null)
//                SendDebugEvent(msg);

//        }

//        /// <summary>
//        /// 查询某个symbol是否有策略在运行
//        /// </summary>
//        /// <param name="symbol"></param>
//        /// <returns></returns>
//        public bool AnyStrategyRuning(string symbol)
//        {
//            foreach (int id in symPositionCheckMap[symbol])
//            {
//                if (_reslist[id].isValid)
//                    return true;
//            }
//            return false;
//        }

//        /// <summary>
//        /// 打开或者关闭某个策略
//        /// </summary>
//        /// <param name="menutitle"></param>
//        public void switchResponse(string menutitle)
//        {
//            switchResponse(menuTitle2LocalIdx(menutitle));
//        }
//        /// <summary>
//        /// 打开或者关闭某个策略
//        /// </summary>
//        /// <param name="menutitle"></param>
//        public void switchResponse(int idx)
//        {
//            if (!isBadResponse(idx))
//            {
//                bool valid = !_reslist[idx].isValid;
//                if (valid)
//                {
//                    //将最新的position信息以及bar信息绑定给positioncheck
//                    //if (((IPositionCheck)_reslist[idx]).BarListTracker==null)
//                    //((IPositionCheck)_reslist[idx]).BarListTracker = null;// _ttc.BarListTracker as IBarListTracker;
//                    //debug("why why");
//                    //将品种信息传递给positioncheck
//                    ((IPositionCheck)_reslist[idx]).Security = null;// _ttc.getMasterSecurity(Idx2Symbol(idx));
//                    //将持仓信息传递给positioncheck,由于每次断开连接重新清理数据获得当天信息,因此在传统止损 止盈里面需要重新启动止损，否则会造成止损不触发
//                    ((IPositionCheck)_reslist[idx]).myPosition = _ttc.getPosition(Idx2Symbol(idx));//这里会触发tradingtracker got a position需要查看下函数是如何运行的
//                    //((PositionCheckTemplate)_reslist[idx]).DataManager = _ttc.DataManager;
//                    //先初始化策略 然后再设置为valid
//                    _reslist[idx].Reset();
//                    _reslist[idx].isValid = valid;
//                    debug("打开策略:"+((IPositionCheck)_reslist[idx]).Security.Symbol);
//                    SwitchPositionCheck(_reslist[idx] as IPositionCheck, true);
//                }
//                else
//                {
//                    //((IPositionCheck)_reslist[idx]).BarListTracker = null;
//                    //((IPositionCheck)_reslist[idx]).myPosition = null;
//                    _reslist[idx].Shutdown();
//                    _reslist[idx].isValid = valid;
//                    SwitchPositionCheck(_reslist[idx] as IPositionCheck,false);
//                }
//                debug(_reslist[idx].Name +" ["+LocalIdx2MenuTitle(idx) +"] " + (valid ? "激活." : "关闭."));
//            }
//        }
//        //response分配的ID转换到对应的symbol
//        public string rid2Symbol(int rid)
//        {
//            return Idx2Symbol(rid2localIdx(rid));
//        }
//        //response index转换到对应的symbol
//        private string Idx2Symbol(int idx)
//        {
//            string s = string.Empty;
//            if (_Idx2Symbol.TryGetValue(idx, out s))
//                return s;
//            return string.Empty;
//        }
//        //response分配的ID到本地index的转换
//        private int rid2localIdx(long responseid)
//        {
//            int idx = -1;
//            if (_rid2LocalIdx.TryGetValue((int)responseid, out idx))
//                return idx;
//            return -1;
//        }
//        //菜单标题项到本地index的转换
//        private int menuTitle2LocalIdx(string menutitle)
//        {
//            int idx = -1;
//            if (_menuTitle2Idx.TryGetValue(menutitle, out idx))
//                return idx;
//            return -1;
//        }
//        //本地index项到菜单项的转换
//        private string LocalIdx2MenuTitle(int idx)
//        {
//            string s = string.Empty;
//            if (_Idx2menuTitle.TryGetValue(idx, out s))
//                return s;
//            return string.Empty;
//        }
//        //检查某个response index是否有效
//        private bool isBadResponse(int idx)
//        {
//            return ((idx < 0) || (idx >= _reslist.Count) || (_reslist[idx] == null));// || !_reslist[idx].isValid);
//        }
//        //当策略出现异常的时候 我们执行通知
//        void notifyresponseexception(int idx, int time, string ondata, Exception ex)
//        {
//            string ridn = _reslist[idx].ID + " " + _reslist[idx].FullName + " at: " + time + " on: " + ondata;
//            debug(ridn + " had a user code error: " + ex.Message + ex.StackTrace + ".  Purchase a support contract at http://www.pracplay.com or ask community at http://community.tradelink.org if you need help resolving your error.");
//            if (disableresponseonexception)
//            {
//                _reslist[idx].isValid = false;
//                debug(ridn + " was marked invalid because of user code error.");
//            }
//        }

//        #region response 方法 事件 处理
//        //往本地reslist增加一个response实例,并分配virtural ID
//        private int addResponse(Response r)
//        {
//            int id = _NEXTRESPONSEID;
//            try
//            {
//                // set the id
//                r.ID = id;
//                // ensure it has a full name
//                r.Name = r.GetType().Name;
//                r.FullName = r.GetType().FullName;
//                // get local response index
//                int idx = _reslist.Count;//获得本地reslist的idx序列 默认将response加到list列表中
//                //绑定response事件 从而response可以触发事件
//                bindResponsEevents(r);
//                //将response加入到本地response列表
//                lock (_reslist)
//                {
//                    _reslist.Add(r);
//                }
//                // save id to local relationship
//                _rid2LocalIdx.Add(r.ID, idx);
//                //递增responseid
//                _NEXTRESPONSEID++;
//                return idx;//返回本地indx用于绑定其他映射
//            }
//            catch (Exception ex)
//            {
//                debug(ex.ToString());
//                return -1;
//            }
            


//        }
//        //绑定response事件
//        private void bindResponsEevents(Response tmp)
//        {
//            // handle all the outgoing events from the response
//            tmp.SendOrderEvent += new OrderSourceDelegate(r_SendOrder);
//            tmp.SendDebugEvent += new DebugDelegate(r_GotDebug);
//            tmp.SendCancelEvent += new LongSourceDelegate(r_CancelOrderSource);
//            tmp.SendBasketEvent += new BasketDelegate(r_SendBasket);
//            tmp.SendMessageEvent += new MessageDelegate(r_SendMessage);
//            //tmp.SendChartLabelEvent += new ChartLabelDelegate(r_SendChartLabel);
//            tmp.SendIndicatorsEvent += new ResponseStringDel(r_SendIndicators);
//            //tmp.SendTicketEvent += new TicketDelegate(r_SendTicketEvent);
//        }

//        #region Response输入操作
//        //当服务器返回fill回报时进行的操作
//        public void GotFill(Trade t)
//        {
//            try
//            {
//                //获得与trade symbol 对应的response id然后用于执行驱动
//                List<int> l;
//                //查找策略缓存中是否有对应symbol的策略
//                if (!symPositionCheckMap.TryGetValue(t.symbol, out l)) return;//没有对应的策略列表
//                if (l.Count == 0) return;//有对应的策略列表 列表中没有具体的策略 返回
//                foreach (int i in l) //遍历每个index对应的策略response并用Tick进行驱动
//                {
//                    //只有当response有些并且状态为valid的时候我们才用数据区驱动该response
//                    if (!isBadResponse(i) && _reslist[i].isValid)
//                    {
//                        if (handleresponseexception)
//                        {
//                            try
//                            {
//                                _reslist[i].GotFill(t);
//                            }
//                            catch (Exception ex)
//                            {
//                                notifyresponseexception(i, Util.ToTLTime(DateTime.Now), "Trade:" + t.ToString(), ex);
//                            }
//                        }
//                        else
//                        {
//                            _reslist[i].GotFill(t);
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                debug(ex.ToString());
//            }

//        }
//        //当服务端返回order取消回报
//        public void GotCancel(long number)
//        {
//            try
//            {
//                //获得与tick symbol 对应的response id然后用于执行驱动
//                List<int> l;
//                //查找策略缓存中是否有对应symbol的策略
//                string sym = _ttc.SentOrder(number).symbol;
//                if (!symPositionCheckMap.TryGetValue(sym, out l)) return;//没有对应的策略列表
//                if (l.Count == 0) return;//有对应的策略列表 列表中没有具体的策略 返回
//                foreach (int i in l) //遍历每个index对应的策略response并用Tick进行驱动
//                {
//                    if (handleresponseexception)
//                    {
//                        try
//                        {
//                            _reslist[i].GotOrderCancel(number);
//                        }
//                        catch (Exception ex)
//                        {
//                            notifyresponseexception(i, Util.ToTLTime(DateTime.Now), "Cancel:" + number.ToString(), ex);
//                        }
//                    }
//                    else
//                    {
//                        _reslist[i].GotOrderCancel(number);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                debug(ex.ToString());
//            }
//        }
//        //当服务端返回Tick信息
//        public void GotTick(Tick k)
//        {
//            try
//            {
//                //获得与tick symbol 对应的response id然后用于执行驱动
//                List<int> l;
//                //查找策略缓存中是否有对应symbol的策略
//                if (!symPositionCheckMap.TryGetValue(k.symbol, out l)) return;//没有对应的策略列表
//                if (l.Count == 0) return;//有对应的策略列表 列表中没有具体的策略 返回
//                foreach (int i in l) //遍历每个index对应的策略response并用Tick进行驱动
//                {
//                    //只有当response有些并且状态为valid的时候我们才用数据区驱动该response
//                    //if (handleresponseexception)
//                    //{
//                        try
//                        {
//                            //debug("strategycentre got tick:" + k.ToString());
//                            if(_reslist[i].isValid)
//                                _reslist[i].GotTick(k);
//                        }
//                        catch (Exception ex)
//                        {
//                            //notifyresponseexception(i, Util.ToTLTime(DateTime.Now), "Tick:" + k.ToString(), ex);
//                        }
//                    //}
//                    //else
//                    //{
//                    //    try
//                    //    {
//                    //        _reslist[i].GotTick(k);
//                    //    }
//                    //    catch (Exception ex)
//                    //    { 
                        
//                    //    }
//                    //}
//                }
//            }
//            catch (Exception ex)
//            {
//                //debug(ex.ToString());
//            }
//        }


//        #endregion

//        #region Response输出操作
//        void r_SendOrder(Order o, int id)
//        {
//            int rid = rid2localIdx(id);
//            if (rid < 0)
//            {
//                debug("Ignoring order from response with invalid id: " + id + " index not found. order: " + o.ToString());
//                return;
//            }
//            if (!_reslist[rid].isValid)
//            {
//                debug("Ignoring order from disabled response: " + _reslist[rid].Name + " order: " + o.ToString());
//                return;
//            }
//            //if (_enableoversellprotect)
//            //    _ost.sendorder(o);
//            else
//                out_sendorder(o);
//                //_coreCentre.SendOrder(o);
               
//        }
//        //response对外取消Order
//        void r_CancelOrderSource(long number, int id)
//        {
//            int rid = rid2localIdx(id);
//            if (!_reslist[rid].isValid)
//            {
//                debug("Ignoring cancel from disabled response: " + _reslist[rid].Name + " orderid: " + number);
//                return;
//            }
//            // pass cancels along to tradelink
//            out_cancelorder((long)number);
//            //_coreCentre.CancelOrder((long)number);
            
//        }
//        //response发送debug信息输出
//        void r_GotDebug(string msg)
//        {
//            // display to screen
//            debug(msg);
//        }
//        //发送basket信息
//        void r_SendBasket(SymbolBasket b, int id)
//        {
//            // get storage index of response from response id
//            int idx = rid2localIdx(id);
//            // update symbols for response
//            //newsyms(b.ToString().Split(','), idx);
//        }

//        //void r_SendTicketEvent(string space, string user, string password, string summary, string description, Priority pri, TicketStatus stat)
//        //{
//            //_rt.Track(space, user, password, summary, description, pri, stat);
//        //}

//        void r_SendMessage(MessageTypes type, long source, long dest, long msgid, string request, ref string response)
//        {
//            //_bf.TLSend(type, source, dest, msgid, request, ref response);
//        }

//        //bool _inderror = false;
//        //RingBuffer<inddata> bufind = new RingBuffer<inddata>(5000);
//        //从response得到输出的调式结果
//        void r_SendIndicators(int idx, string param)
//        {
//            //if (!_ao._saveinds.Checked)
//            //    return;
//            //bufind.Write(new inddata(idx, param));
//            debug(idx.ToString() + ":" + param);
//        }

//        bool _charterror = false;
//        void r_SendChartLabel(decimal price, int bar, string label, System.Drawing.Color c)
//        {
//            if (!_charterror)
//            {
//                //debug(PROGRAM + " does not support sendchart.");
//                _charterror = true;
//            }
//        }
//        #endregion

//        #endregion
//        /// <summary>
//        /// 通过type全名来得到response instance
//        /// </summary>
//        /// <param name="typefullname"></param>
//        /// <returns></returns>
//        public Response getPositionStrategyInstanceByFullName(string typefullname)
//        { 
//            Type t =null;
//            if(poscheckTypeMap.TryGetValue(typefullname,out t))
//                return (Response)Activator.CreateInstance(t,new object[] { });
//            return null;
//        }
//        /// <summary>
//        /// 通过positioncheck的中文标题名称来获得对应的response instance
//        /// </summary>
//        /// <param name="cnname"></param>
//        /// <returns></returns>
//        public Response getPositionStrategyInstanceByTitle(string cnname)
//        {
//            Type t = null;
//            if(poscheckTitleTypeMap.TryGetValue(cnname,out t))
//                return (Response)Activator.CreateInstance(t, new object[] { });
//            return null;
//        }

//        /// <summary>
//        /// 获得仓位策略模板
//        /// </summary>
//        /// <returns></returns>
//        public List<Type> getPositionStrategyTemple()
//        {   
//            return poscheckTypeMap.Values.ToList();
//        }

//        /*
//        /// <summary>
//        /// 更新某个sym对应的response列表
//        /// </summary>
//        /// <param name="sym"></param>
//        /// <param name="rlist"></param>
//        public void updateSymbolPositionStrategy(string sym,List<Response> rlist)
//        {
//            foreach (Response r in rlist)
//            {
//                addResponseIntoCache(sym, r);
//            } 
//        }**/

//        /// <summary>
//        /// 根据response返回其中文标题
//        /// </summary>
//        /// <param name="r"></param>
//        /// <returns></returns>
//        public string getPositionStrategyTitle(Response r)
//        {
//            return getPositionStrategyTitle(r.GetType());
//        }
//        /// <summary>
//        /// 通过类型返回该策略的中文标题名称
//        /// </summary>
//        /// <param name="t"></param>
//        /// <returns></returns>
//        public string getPositionStrategyTitle(Type t)
//        {
//            string s = string.Empty;
//            if (poscheckTypeTitleMap.TryGetValue(t, out s))
//                return s;
//            return s;
//        }

//        /// <summary>
//        /// 通过symbol 返回策略id list.或者null无策略集
//        /// </summary>
//        /// <param name="sym"></param>
//        /// <returns></returns>
//        public List<int> getResponseIdxViaSymbol(string sym)
//        {
           
//            List<int> a;
//            if (symPositionCheckMap.TryGetValue(sym, out a))
//                return a;
//            return null;
            
//        }
//        /// <summary>
//        /// 获得某个symbol正在运行的ipositioncheck
//        /// </summary>
//        /// <param name="sym"></param>
//        /// <returns></returns>
//        public List<IPositionCheck> getIPositionCheckRuningListViaSymbol(string sym)
//        {

//            List<IPositionCheck> res = new List<IPositionCheck>();
//            List<int> a = new List<int>();
//            if (!symPositionCheckMap.TryGetValue(sym, out a)) return res;
//            //MessageBox.Show(sym +"have "+a.Count.ToString() + " posstartegy");    
//            foreach (int i in a)
//            {
//                if(_reslist[i].isValid)
//                    res.Add(_reslist[i] as IPositionCheck);
//            }
//            return res;
//        }
//        //返回某个symbol所存在的positioncheck strategy
//        public List<Response> getPositionStrategy(string sym)
//        {
//            List<Response> res = new List<Response>();
//            List<int> a  = new List<int>();
//            if (!symPositionCheckMap.TryGetValue(sym, out a)) return res;
//            //MessageBox.Show(sym +"have "+a.Count.ToString() + " posstartegy");    
//            foreach (int i in a)
//            {
//                res.Add(_reslist[i]);
//            }
//            return res;
//        }

//        void InitPositionCheckCentre()
//        {
//            //加载策略模板(用于通过全名得到Type进而实例化成可运行的策略)
//            LoadPositionCheckStrategyTemple();
//            //加载symbol对应的配置好的可运行策略,通过加载配置文件将默认策略配置成该Symbol特有的策略
//            LoadPositionStrategyForSymbols();
//        }

//        /// <summary>
//        /// 从策略文件中加载 IPositionCheck类型的策略
//        /// </summary>
//        private void LoadPositionCheckStrategyTemple()
//        {
//            foreach (Type t in StrategyHelper.GetResponseListViaType<IPositionCheck>())
//            {
//                string rsname = (string)t.InvokeMember("Title",
//                    BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetProperty,
//                    null, null, null);
//                string rsdescription = (string)t.InvokeMember("Description",
//                    BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetProperty,
//                    null, null, null);
//                //建立双向索引从类型-名称
//                poscheckTypeMap.Add(t.FullName, t);//类型全名-类型
//                poscheckTypeTitleMap.Add(t, rsname);//类型-类型中文名
//                poscheckTitleTypeMap.Add(rsname, t);//类型中文名-类型

//            }
//        }

//        /// <summary>
//        /// 加载默认合约列表中合约的预配出场策略
//        /// </summary>
//        private void LoadPositionStrategyForSymbols()
//        {
//            //_defaultBasket = BasketTracker.getBasket("Default");
//            foreach (string sym in _defaultBasket.ToSymArray())
//            {
//                //MessageBox.Show("加载合约:" + sym+" 的预配策略");
//                LoadPositionStrategyForSymbol(sym);
//            }
//        }

//        /// <summary>
//        /// 将测量从内存中安全删除
//        /// </summary>
//        /// <param name="sym"></param>
//        /// <param name="r"></param>
//        public void delResponseFromCache(string sym, Response r)
//        {
//            //得到response的中文标题(参数列表) 该标识可以用于区分不同的response
//            string s = sym+":"+getPositionStrategyTitle(r) + "(" + ((IPositionCheck)r).ToText() + ")";
//            int idx = menuTitle2LocalIdx(s);
//            //表明系统中没有该response我们不去去除
//            if (idx == -1)
//                return;
//            lock (_reslist)
//            {
//                _reslist[idx] = new InvalidResponse();//将原有序号对应的response设定为无效response
//            }
//            //从symbol list<int>中将该response删除
//            lock (symPositionCheckMap[sym])
//            {
//                symPositionCheckMap[sym].Remove(idx);
//            }
//            lock (_menuTitle2Idx)
//            {
//                _menuTitle2Idx.Remove(s);
//            }
//            lock (_Idx2Symbol)
//            {
//                _Idx2menuTitle.Remove(idx);
//            }
//            lock (_Idx2Symbol)
//            {
//                _Idx2Symbol.Remove(idx);
//            }


//        }

//        /// <summary>
//        /// 将某个response加入到内存中 并将其打开或者关闭
//        /// </summary>
//        /// <param name="sym"></param>
//        /// <param name="r"></param>
//        /// <param name="valid"></param>
//        public void addResponseIntoCache(string sym, Response r, bool valid)
//        {
//            if (valid)
//            {
               
//                int idx = addResponseIntoCache(sym, r);
//                switchResponse(idx);
//            }
//            else
//            {
//                addResponseIntoCache(sym, r);
//            }

//        }
//        /// <summary>
//        /// 查询是否存在某个策略r,如果不存在返回false,如果存在则将策略缓存中的r实例返回
//        /// </summary>
//        /// <param name="sym"></param>
//        /// <param name="r"></param>
//        /// <returns></returns>
//        public bool HasResponse(string sym, ref Response r,out int index)
//        { 
//             r.isValid = false;
//            //得到response的中文标题(参数列表) 该标识可以用于区分不同的response
//            //菜单名由于参数可能重复,因此需要外加sym做唯一标识
//            string s = sym +":"+getPositionStrategyTitle(r) + "(" + ((IPositionCheck)r).ToText() + ")";
//            int idx = menuTitle2LocalIdx(s);
//            //表明系统中symbol已经存在了该response不用重复添加
//            index = idx;
//            if (idx >= 0)
//            {
//                r = _reslist[idx];
//                return true;
//            }
//            return false;
//        }
//        /// <summary>
//        /// 将某个symbol的策略加入到缓存中
//        /// </summary>
//        /// <param name="sym"></param>
//        /// <param name="r"></param>
//        /// <returns></returns>
//        public int  addResponseIntoCache(string sym, Response r)
//        {
//            //debug("symbol:"+((IPositionCheck)r).Security.Symbol.ToString() +"price tick:" + ((IPositionCheck)r).Security.PriceTick.ToString());
//            r.isValid = false;
//            //得到response的中文标题(参数列表) 该标识可以用于区分不同的response
//            //菜单名由于参数可能重复,因此需要外加sym做唯一标识
//            string s = sym +":"+getPositionStrategyTitle(r) + "(" + ((IPositionCheck)r).ToText() + ")";
//            int idx = menuTitle2LocalIdx(s);
//            //表明系统中symbol已经存在了该response不用重复添加
//            if (idx >= 0)
//                return idx;
//            //将该idx对应的策略加载到symbol对应的缓存中
//            idx = addResponse(r);
            
//            //将该idx与对应的symbol进行绑定
//            if (!symPositionCheckMap.ContainsKey(sym))//如果映射列表中不存在该sym对应的list我们先增加该list
//                symPositionCheckMap.Add(sym, new List<int>());
//            symPositionCheckMap[sym].Add(idx);
//            try
//            {
//                _menuTitle2Idx.Add(s, idx);//菜单标题-->idx映射
//                _Idx2menuTitle.Add(idx, s);//idx-->菜单标题映射
//                _Idx2Symbol.Add(idx, sym);//idx-->合约symbol映射
//            }
//            catch (Exception ex)
//            {
//                debug(ex.ToString());
//            }
//            //MessageBox.Show(s + "added");
//            //debug("symbol:" + ((IPositionCheck)r).Security.Symbol.ToString() + "price tick:" + ((IPositionCheck)r).Security.PriceTick.ToString());
            
//            return idx;
//        }
//        /// <summary>
//        /// 加载单个symbol下的positionstrategy
//        /// </summary>
//        /// <param name="sym"></param>
//        private void LoadPositionStrategyForSymbol(string sym)
//        { 
//            List<string> l = PositionCheckTracker.getPositionCheckFromSymbol(sym);
//            for (int i = 0; i < l.Count; i++)
//            {
//                object[] args;
//                args = new object[] { };
//                //分解参数
//                string[] re = l[i].Split(':');
//                string rname = re[0];
//                string cfgtest = re[1];
//                Type t;
//                //MessageBox.Show(sym + "load template |" + rname + "|" + poscheckTypeMap.ContainsKey(rname));
//                if (poscheckTypeMap.TryGetValue(rname, out t))//如果策略类型在策略模板集中不存在，则不创建该策略
//                {
//                    //MessageBox.Show(sym +"add |"+rname);
//                    Response rpc = ((IPositionCheck)Activator.CreateInstance(t, args)).FromText(cfgtest) as Response;
//                    //将从配置文件实例化得到的response加载到系统,初始情况均为不激活
//                    this.addResponseIntoCache(sym, rpc);
//                }
//            }
//        }
    
//    }
//}
