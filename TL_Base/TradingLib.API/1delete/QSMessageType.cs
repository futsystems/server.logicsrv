//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.ComponentModel;

//namespace TradingLib.API
//{
//    //定义消息的不同显示类别
    
//    /*
//    public enum QSEnumPopMessageType
//    {
//        Info,
//        Waring,
//        Error
//    }
    
//    **/

//    /// <summary>
//    /// 内置消息类型
//    /// </summary>
//    public enum QSEnumMessageLevel
//    {
//        [Description("信息")]
//        Info=1,
//        [Description("警告")]
//        Waring=2,
//        [Description("错误")]
//        Error=3,
        
//    }


//    // #############################################################################系统消息的统一规范##############################################################################
//    // 1.底层消息统一以Message对外发送,消息长度,消息类别,消息内容
//    // 2.具体的消息内容则可以按需求进行定制 只要双边约定好通行协议即可
//    // 系统包含基本消息类型
//    // 1.连接协议消息
//    // 2.交易协议消息
//    // 3.系统级别消息
//    // 
//    //
//    //
//    //

//    /// <summary>
//    /// 委托消息类型
//    /// </summary>
//    public enum QSEnumOrderMessageType
//    {
//        [Description("委托接收")]
//        ACCEPT=100,
//        [Description("委托拒绝")]
//        REJECT=101,
//        [Description("全部成交")]
//        FILLED=102,
//        [Description("部分成交")]
//        PARTFILLED=103,
//        [Description("委托取消")]
//        CANCELED=104,
//        [Description("委托警告")]
//        WARNING=105,
//    }

//    /// <summary>
//    /// 系统消息类型
//    /// </summary>
//    public enum QSEnumSysMessageType
//    {
//        [Description("帐户未登入")]
//        NOTLOGGEDIN=200,
//        [Description("帐户登入错误")]
//        LOGGINFAILED=201,
//        [Description("帐户登入成功")]
//        LOGGINSUCCESS=202,
//        [Description("密码修改成功")]
//        PASSCHANGEFAILED=203,
//        [Description("密码修改错误")]
//        PASSCHANGESUCCESS=204,

//        [Description("操作拒绝")]
//        OPERATIONREJECT=205,
//        [Description("报名成功")]
//        SIGNPRERACESUCCESS=206,
//        [Description("报名失败")]
//        SIGNPRERACEFAILED=207,
//        [Description("操作失败")]
//        MGROPERATIONSUCCESS=208,
//        [Description("操作成功")]
//        MGROPERATIONFAILED=209,
//        [Description("无权进行该操作")]
//        MGROPERATIONPERMISSIONDENIED=210,
        
//    }


//}
