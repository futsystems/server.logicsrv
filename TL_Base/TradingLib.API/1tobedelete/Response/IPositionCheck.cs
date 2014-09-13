//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;


//namespace TradingLib.API
//{
//    public interface IPositionCheck
//    {
//        /// <summary>
//        /// 策略对外输出窗口
//        /// </summary>
//        Object DisplayForm { get; set; }
//        /// <summary>
//        /// 名称
//        /// </summary>
//        string Name { get; set; }

//        /// <summary>
//        /// 所对应的仓位
//        /// </summary>
//        Position myPosition { get; set; }
       
//        /// <summary>
//        /// 所对应的合约
//        /// </summary>
//        Symbol Security { get; set; }

//        /// <summary>
//        /// 检查持仓主题函数
//        /// </summary>
//        /// <param name="msg"></param>
//        void checkPosition(out string msg);
        
//        /// <summary>
//        /// 生成配置文本
//        /// </summary>
//        /// <returns></returns>
//        string ToText();
//        /// <summary>
//        /// 从文本加载配置信息
//        /// </summary>
//        /// <param name="msg"></param>
//        /// <returns></returns>
//        IPositionCheck FromText(string msg);

//        /// <summary>
//        /// 得到容易理解的positioncheck的中文描述
//        /// </summary>
//        /// <returns></returns>
//        string PositionCheckDescription();


//    }
//}
