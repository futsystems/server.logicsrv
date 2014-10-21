using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;
using System.Drawing;

namespace FutSystems.GUI
{
    public class UIUtil
    {
        public  const string ANY = "<Any>";
        public static void genSecTreeView(ref TreeView tv)
        {
            string[] baskets = null;// BasketTracker.getBaskets();
            for (int i = 0; i < baskets.Length; i++)
            {
                string[] secs = null;//BasketTracker.getSymbolList(baskets[i]);
                TreeNode[] t = new TreeNode[secs.Length];
                for (int j = 0; j < secs.Length; j++)
                {
                    t[j] = new TreeNode(secs[j]);
                }
                tv.Nodes.Add(new TreeNode(baskets[i], t));
            }
        }

        //将枚举类型形成下拉惨淡栏输出，用于用户UI
        public static void genComboBoxList<T>(ref ComboBox cb)
        {
            genComboBoxList<T>(ref cb,false);
        }
        
        public static void genComboBoxList<T>(ref ComboBox cb,bool isany)
        {
            ArrayList list = new ArrayList();
            if (isany)
            {
                ValueObject<T> vo = new ValueObject<T>();
                vo.Name = ANY;
                vo.Value = (T)(Enum.GetValues(typeof(T)).GetValue(0));
                list.Add(vo);
            }
           
            foreach (T c in Enum.GetValues(typeof(T)))
            {
                ValueObject<T> vo = new ValueObject<T>();
                vo.Name = Util.GetEnumDescription(c);
                vo.Value = c;
                list.Add(vo);
            }
            cb.DataSource = list;
            cb.DisplayMember = "Name";
            cb.ValueMember = "Value";
            
            //cb.Items.AddRange((ComboBox.ObjectCollection[])list.ToArray());
        }

        public static ArrayList GetEnumValueObjects<T>(bool isany=false)
        {
            ArrayList list = new ArrayList();
            if (isany)
            {
                ValueObject<T> vo = new ValueObject<T>();
                vo.Name = ANY;
                vo.Value = (T)(Enum.GetValues(typeof(T)).GetValue(0));
                list.Add(vo);
            }
            foreach (T c in Enum.GetValues(typeof(T)))
            {
                ValueObject<T> vo = new ValueObject<T>();
                vo.Name = Util.GetEnumDescription(c);
                vo.Value = c;
                list.Add(vo);
            }
            return list;
        }
        //获得positioncheck列表选择框
        public static void genCheckedListBoxPositoinCheck(ref CheckedListBox ck, Dictionary<string, Type> data)
        {
            ArrayList list = new ArrayList();
            foreach (string name in data.Keys)
            {
                ValueObject<Type> vo = new ValueObject<Type>();
                vo.Name = name;
                vo.Value = data[name];
                list.Add(vo);
            }
            ck.DataSource = list;
            ck.DisplayMember = "Name";
            ck.ValueMember = "Value";

        }
        public static void getListBoxStrategy(ref ListBox ck, Dictionary<string, Type> data)
        {
            ArrayList list = new ArrayList();
            foreach (string name in data.Keys)
            {
                ValueObject<Type> vo = new ValueObject<Type>();
                vo.Name = name;
                vo.Value = data[name];
                list.Add(vo);
            }
            ck.DataSource = list;
            ck.DisplayMember = "Name";
            ck.ValueMember = "Value";
        }

        public static void genCheckedListBoxRuleSet(ref CheckedListBox ck, Dictionary<string, Type> data)
        {
            ArrayList list = new ArrayList();
            foreach (string name in data.Keys)
            {
                ValueObject<Type> vo = new ValueObject<Type>();
                vo.Name = name;
                vo.Value = data[name];
                list.Add(vo);
            }
            ck.DataSource = list;
            ck.DisplayMember = "Name";
            ck.ValueMember = "Value";
        
        }
        public static void genListBoxBasket(ref ListBox lb, string basket)
        {
            ArrayList list = new ArrayList();
            //string[] secs = BasketTracker.getSymbolList(basket);
            SymbolBasket b = null;// BasketTracker.getBasket(basket);
            foreach(Symbol sec in b.ToArray())
            {
                ValueObject<Symbol> vo = new ValueObject<Symbol>();
                vo.Name = sec.Symbol;
                vo.Value = sec;
                list.Add(vo);
            }
            /*for (int i = 0; i < secs.Length; i++)
            {
                ValueObject<string> vo = new ValueObject<string>();
                vo.Name = secs[i];
                vo.Value = secs[i];
                list.Add(vo); 
            }
             * */
            lb.DataSource = list;
            lb.DisplayMember = "Name";
            lb.ValueMember = "Value";
            
        }
        #region 生成对应的list列表

        public static ArrayList genTypeList(Dictionary<string, Type> data)
        {
            ArrayList list = new ArrayList();
            foreach (string name in data.Keys)
            {
                ValueObject<Type> vo = new ValueObject<Type>();
                vo.Name = name;
                vo.Value = data[name];
                list.Add(vo);
            }
            return list;
        }

        public static ArrayList genPositoinExitStrategyList(Dictionary<string, Type> data)
        {
            ArrayList list = new ArrayList();
            foreach (string name in data.Keys)
            {
                ValueObject<Type> vo = new ValueObject<Type>();
                vo.Name = name;
                vo.Value = data[name];
                list.Add(vo);
            }
            return list;
        }
        public static ArrayList genBasektList()
        {
            return genBasektList(false);
        }
        public static ArrayList genBasektList(bool isany)
        {
            ArrayList list = new ArrayList();
            if (isany)
            {
                ValueObject<string> vo = new ValueObject<string>();
                vo.Name = ANY;
                vo.Value = ANY;
                list.Add(vo);
            }
            string[] baskets = null;// BasketTracker.getBaskets();
            for (int i = 0; i < baskets.Length; i++)
            {
                ValueObject<string> vo = new ValueObject<string>();
                vo.Name = baskets[i];
                vo.Value = baskets[i];
                list.Add(vo);
            }
            return list;
        }

        public static ArrayList genSymbolListForBasket(string basket)
        {
            ArrayList list = new ArrayList();
            //string[] secs = BasketTracker.getSymbolList(basket);
            SymbolBasket b = null;//BasketTracker.getBasket(basket);
            foreach (Symbol sec in b.ToArray())
            {
                ValueObject<Symbol> vo = new ValueObject<Symbol>();
                vo.Name = sec.Symbol;
                vo.Value = sec;
                list.Add(vo);
            }
            return list;
        }

        public static ArrayList genExchangeList()
        {
            return genExchangeList(false);
        }
        public static ArrayList genExchangeList(bool isany)
        {
            ArrayList list = new ArrayList();
            if (isany)
            {
                ValueObject<Exchange> vo = new ValueObject<Exchange>();
                vo.Name = ANY;
                vo.Value = new Exchange(); ;
                list.Add(vo);

            }
            foreach (Exchange e in ExchangeTracker.getExchList())
            {
                ValueObject<Exchange> vo = new ValueObject<Exchange>();
                vo.Name = e.Name;
                vo.Value = e;
                list.Add(vo);
            }
            return list;
        }

        public static ArrayList genEnumList<T>()
        {
            return genEnumList<T>(false);
        }

        public static ArrayList genEnumList<T>(bool isany)
        {
            ArrayList list = new ArrayList();
            if (isany)
            {
                ValueObject<T> vo = new ValueObject<T>();
                vo.Name = ANY;
                vo.Value = (T)(Enum.GetValues(typeof(T)).GetValue(0));
                list.Add(vo);
            }

            foreach (T c in Enum.GetValues(typeof(T)))
            {
                ValueObject<T> vo = new ValueObject<T>();
                vo.Name = Util.GetEnumDescription(c);
                vo.Value = c;
                list.Add(vo);
            }
            return list;
        }

        public static ArrayList genExpireList()
        {
            ArrayList list = new ArrayList();
            DateTime lastDay = Convert.ToDateTime(DateTime.Now.AddMonths(1).ToString("yyyy-MM-01")).AddDays(-1);
            for (int i = 0; i < 12; i++)
            {
                ValueObject<int> vo = new ValueObject<int>();

                vo.Name = lastDay.AddMonths(i).ToString("yyyyMM");
                vo.Value = Convert.ToInt32(vo.Name);//Util.DT2FT(lastDay.AddMonths(i));
                list.Add(vo);
            }
            return list;
        }
        #endregion 
        //生成basket选择列表
        public static void genComboBoxBasekt(ref ListControl cb)
        {
            genComboBoxBasekt(ref cb, false);
        }
        public static void genComboBoxBasekt(ref ListControl cb, bool isany)
        {
            
            ArrayList list = new ArrayList();
            if (isany)
            {
                ValueObject<string> vo = new ValueObject<string>();
                vo.Name = ANY;
                vo.Value = ANY;
                list.Add(vo); 
            }
            string[] baskets = null;// BasketTracker.getBaskets();
            for (int i = 0; i < baskets.Length; i++)
            {
                ValueObject<string> vo = new ValueObject<string>();
                vo.Name = baskets[i];
                vo.Value = baskets[i];
                list.Add(vo); 
            }
            cb.DataSource = list;
            cb.DisplayMember = "Name";
            cb.ValueMember = "Value";
            
        }

        public static void genComboBoxHistory(ref ToolStripComboBox cb)
        {
            ArrayList list = new ArrayList();
            DateTime today = DateTime.Now;//Convert.ToDateTime(DateTime.Now.AddMonths(1).ToString("yyyy-MM-01")).AddDays(-1);
            //1周
            ValueObject<int> vo1 = new ValueObject<int>();
            vo1.Name = "1周";
            vo1.Value = Util.ToTLDate(today.AddDays(-7));
            list.Add(vo1);
            //1月
            ValueObject<int> vo2 = new ValueObject<int>();
            vo2.Name = "1月";
            vo2.Value = Util.ToTLDate(today.AddDays(-30));
            list.Add(vo2);
            //半年
            ValueObject<int> vo3 = new ValueObject<int>();
            vo3.Name = "半年";
            vo3.Value = Util.ToTLDate(today.AddDays(-180));
            list.Add(vo3);
            //1年
            ValueObject<int> vo4 = new ValueObject<int>();
            vo4.Name = "1年";
            vo4.Value = Util.ToTLDate(today.AddYears(-1));
            list.Add(vo4);
            //2年
            ValueObject<int> vo5 = new ValueObject<int>();
            vo5.Name = "2年";
            vo5.Value = Util.ToTLDate(today.AddYears(-2));
            list.Add(vo4);
            //5年


            //cb.Items.Add
            cb.ComboBox.DataSource = list;
            //cb.DataSource = list;
            cb.ComboBox.DisplayMember = "Name";
            cb.ComboBox.ValueMember = "Value";
        }

        public static void genComboBoxExpire(ref ComboBox cb)
        {
            ArrayList list = new ArrayList();
            DateTime lastDay = Convert.ToDateTime(DateTime.Now.AddMonths(1).ToString("yyyy-MM-01")).AddDays(-1);
            for (int i = 0; i < 12; i++)
            {
                ValueObject<int> vo = new ValueObject<int>();

                vo.Name = lastDay.AddMonths(i).ToString("yyyyMM");
                vo.Value = Convert.ToInt32(vo.Name);//Util.DT2FT(lastDay.AddMonths(i));
                list.Add(vo);
            }
            cb.DataSource = list;
            cb.DisplayMember = "Name";
            cb.ValueMember = "Value";
        }

        public static void genComboBoxExchange(ref ComboBox cb)
        {
            genComboBoxExchange(ref cb, false);
        }
        public static void genComboBoxExchange(ref ComboBox cb,bool isany)
        {
            ArrayList list = new ArrayList();
            if (isany)
            {
                ValueObject<Exchange> vo = new ValueObject<Exchange>();
                vo.Name = ANY;
                vo.Value = new Exchange(); ;
                list.Add(vo);
                
            }
            foreach (Exchange e in ExchangeTracker.getExchList())
            {
                ValueObject<Exchange> vo = new ValueObject<Exchange>();
                vo.Name = e.Name;
                vo.Value = e;
                list.Add(vo);
            }
            cb.DataSource = list;
            cb.DisplayMember = "Name";
            cb.ValueMember = "Value";
        }

        public static void genExchangeList(ref CheckedListBox ck)
        {
            ArrayList list = new ArrayList();
             foreach (Exchange e in ExchangeTracker.getExchList())
            {
                ValueObject<Exchange> vo = new ValueObject<Exchange>();
                vo.Name = e.Name;
                vo.Value = e;
                list.Add(vo);
            }
            ck.DataSource = list;
            ck.DisplayMember = "Name";
            ck.ValueMember = "Value";
        }
        public static void genExchangeList(ref ListBox ck)
        {
            ArrayList list = new ArrayList();
            foreach (Exchange e in ExchangeTracker.getExchList())
            {
                ValueObject<Exchange> vo = new ValueObject<Exchange>();
                vo.Name = e.Name;
                vo.Value = e;
                list.Add(vo);
            }
            ck.DataSource = list;
            ck.DisplayMember = "Name";
            ck.ValueMember = "Value";
        }


        public static void genTickList(ref ComboBox cb)
        {
            decimal[] ticklist = { (decimal)0.0000001, (decimal)0.000001, (decimal)0.00001,(decimal)000025,(decimal)0.00005,(decimal)0.0001,(decimal)0.00025,
                                     (decimal)0.0005,(decimal)0.001,(decimal)0.00125,(decimal)0.0020,(decimal)0.0025,(decimal)0.005,(decimal)0.01,(decimal)0.0125,
                                 (decimal)0.020,(decimal)0.025,(decimal)0.05,(decimal)0.1,(decimal)0.125,(decimal)0.20,(decimal)0.25,(decimal)0.5,(decimal)1,(decimal)2,
                                 (decimal)5,(decimal)10,(decimal)25,(decimal)100,(decimal)1000,(decimal)10000,(decimal)100000};
            ArrayList list = new ArrayList();
            foreach (decimal t in ticklist)
            {
                ValueObject<decimal> vo = new ValueObject<decimal>();
                vo.Name = t.ToString();
                vo.Value = t;
                list.Add(vo);
            }
            cb.DataSource = list;
            cb.DisplayMember = "Name";
            cb.ValueMember = "Value";
        }

        private static ValueObject<int> anyItem()
        {
            ValueObject<int> a = new ValueObject<int>();
            a.Name = "<Any>";
            a.Value = -1;
            return a;
        }

        
        /// <summary>
        /// 通过PriceTick得到数字显示格式
        /// </summary>
        /// <param name="pricetick"></param>
        /// <returns></returns>
       
        public static string getDisplayFormat(decimal pricetick)
        { 
            //1 0.2
            string[] p = pricetick.ToString().Split('.');
            if (p.Length <= 1)
                return "{0:F0}";
            else
                return "{0:F" + p[1].ToCharArray().Length.ToString() + "}";
            
        }
        public static int getDecimalPlace(decimal pricetick)
        {
            
            //1 0.2
            string[] p = pricetick.ToString().Split('.');
            if (p.Length <= 1)
                return 0;
            else
                return p[1].ToCharArray().Length;

        }

        public static void genListBoxServerAddress(ref ListBox lb,bool isdev=true,bool isreal=true)
        {
            ArrayList list = new ArrayList();
           
            ValueObject<string> s0 = new ValueObject<string>();
            s0.Name = "电信通道1";
            s0.Value = "DX1";

            ValueObject<string> s1 = new ValueObject<string>();
            s1.Name = "电信通道2";
            s1.Value = "DX2";

            ValueObject<string> s2 = new ValueObject<string>();
            s2.Name = "联通通道";
            s2.Value = "LT";

            list.Add(s0);
            list.Add(s1);
            list.Add(s2);


            if (isreal)
            {
                ValueObject<string> s3 = new ValueObject<string>();
                s3.Name = "实盘通道(主)";
                s3.Value = "SP1";

                ValueObject<string> s4 = new ValueObject<string>();
                s4.Name = "实盘通道(备)";
                s4.Value = "SP2";


                list.Add(s3);
                list.Add(s4);
            }

            if (isdev)
            {
                ValueObject<string> s5 = new ValueObject<string>();
                s5.Name = "本机连接";
                s5.Value = "LOCAL";

                ValueObject<string> s6 = new ValueObject<string>();
                s6.Name = "VPN连接";
                s6.Value = "VPN";


                list.Add(s5);
                list.Add(s6);
            }
            
            
            lb.DataSource = list;
            lb.DisplayMember = "Name";
            lb.ValueMember = "Value";

        }

        //public static Image GetImageForPositionExit(IPositionCheck check)
        //{
        //    return GetImageForPositionExit(check.GetType());
        //}
        //public static Image GetImageForPositionExit(Type checktype)
        //{
        //    switch (checktype.FullName)
        //    {
        //        case "Strategy.ExitPosition.Traditional":
                    
        //            return null;//(Image)Resources.stopprofitok;
        //        case "Strategy.ExitPosition.TargetProfitExit":
        //            return null;//(Image)Resources.profit_take;
        //        case "Strategy.ExitPosition.ScalperManual":
        //            return null;//(Image)Resources.scalper;
        //        case "Strategy.ExitPosition.StopLoss":
        //            return null;//(Image)Resources.loss_take;
        //        case "Strategy.ExitPosition.TwoStepPoint":
        //            return null;//(Image)Resources._2step;
        //        default:
        //            return null;//(Image)Resources.strategy;

        //    }
        //}

        //public static Image GetImageListForPositionChecks(List<IPositionCheck> l)
        //{
        //    //如果仓位由运行的策略,则返回图标,没有则返回null
        //    if (l != null && l.Count > 0)
        //    {
        //        Bitmap resultImg;    //存放最终拼接结果的图片
        //        Graphics resultGraphics;    //用来绘图的实例
        //        resultImg = new Bitmap(16 * l.Count, 16);    //每单个数字图片的宽度是45像素，高度是60像素，这里显示4位长度的整数
        //        resultGraphics = Graphics.FromImage(resultImg);
        //        //int num = l.Count;
        //        int i = 0;
        //        foreach (IPositionCheck check in l)
        //        {
        //            resultGraphics.DrawImage(GetImageForPositionExit(check), 16 * i, 0);
        //            i++;
        //        }
        //        return (Image)resultImg;
        //    }
        //    else
        //        return null;

        //}


        /// <summary>
        /// 按钮点击 失效 延迟激活
        /// </summary>
        /// <param name="btn"></param>
        /// <param name="func"></param>
        /// <param name="timedelay"></param>
        //public static void ButtonActionDelay(object btn, VoidDelegate func, decimal timedelay = 1.5M)
        //{

        //    if (btn is Telerik.WinControls.UI.RadButton)
        //    {
        //        Telerik.WinControls.UI.RadButton b = btn as Telerik.WinControls.UI.RadButton;

        //        b.Enabled = false;
        //        Thread t = new Thread(() =>
        //        {
        //            //平掉当前仓位
        //            func();
        //            Thread.Sleep((int)(1000 * timedelay));
        //            b.Invoke(new Action(() =>
        //            {
        //                //btnFlatPostions.Text = i.ToString();
        //                b.Enabled = true;
        //                //updatePositionButton();
        //            }));
        //        });
        //        t.IsBackground = true;
        //        t.Start();
        //    }
        //}


        
    }



    //下拉菜单所使用的vlaueobject,用于形成名称与对应的value
    
    public class ValueObject<T>
    {
        
        private string _name;
        private T _value;
        public T Value { get { return _value; } set { _value = value; } }
        public string Name { get { return _name; } set { _name = value; } }

    }



         
}
