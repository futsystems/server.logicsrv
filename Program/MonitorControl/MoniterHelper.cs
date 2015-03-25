using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using System.Windows.Forms;
using System.Collections;
using ComponentFactory.Krypton.Toolkit;
using TradingLib.Mixins.Json;

namespace TradingLib.MoniterControl
{
    public class MoniterHelper
    {
        static ICTX _ctx = null;

        /// <summary>
        /// 注册全局调用接口
        /// </summary>
        /// <param name="ctx"></param>
        public static void RegisterCTX(ICTX ctx)
        {
            _ctx = ctx;
        }

        public static ICTX CTX { get { return _ctx; } }



        #region 对象与回调函数注册
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public static void RegIEventHandler(IEventBinder obj)
        {
            if (_ctx != null)
            {
                _ctx.RegIEventHandler(obj);
            }
        }

        /**
         * 在一些基础组件上 我们需要动态的响应回调函数，因此需要在控件内部进行注册和注销回调
         * 这个过程需要用到注册和注销回调函数的入口
         * 
         * 
         * 
         * **/
        /// <summary>
        /// 注册回调函数
        /// </summary>
        /// <param name="module"></param>
        /// <param name="cmd"></param>
        /// <param name="handler"></param>
        public static void RegisterCallback(string module, string cmd, Action<string,bool> handler)
        {
            if (_ctx != null)
            {
                _ctx.RegisterCallback(module, cmd, handler);
            }
        }

        /// <summary>
        /// 注销回调函数
        /// </summary>
        /// <param name="module"></param>
        /// <param name="cmd"></param>
        /// <param name="handler"></param>
        public static void UnRegisterCallback(string module, string cmd, Action<string, bool> handler)
        {
            if (_ctx != null)
            {
                _ctx.UnRegisterCallback(module, cmd, handler);
            }
        }

        /// <summary>
        /// 注册通知类回调
        /// </summary>
        /// <param name="module"></param>
        /// <param name="cmd"></param>
        /// <param name="handler"></param>
        public static void RegisterNotifyCallback(string module, string cmd, Action<string> handler)
        {
            if (_ctx != null)
            {
                _ctx.RegisterNotifyCallback(module, cmd, handler);
            }
        }


        /// <summary>
        /// 注销通知类回调
        /// </summary>
        /// <param name="module"></param>
        /// <param name="cmd"></param>
        /// <param name="handler"></param>
        public static void UnRegisterNotifyCallback(string module, string cmd, Action<string> handler)
        {
            if (_ctx != null)
            {
                _ctx.UnRegisterNotifyCallback(module, cmd, handler);
            }
        }
        #endregion

        /// <summary>
        /// 通过底层Client发送一个请求
        /// </summary>
        /// <param name="module"></param>
        /// <param name="cmd"></param>
        /// <param name="args"></param>
        public static void Request(string module, string cmd, string args)
        {
            if (_ctx != null)
            {
                _ctx.Request(module, cmd, args);
            }
        }



        /// <summary>
        /// 解析返回的Json数据到对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T ParseJsonResponse<T>(string json)
        {
            JsonReply<T> reply = JsonReply.ParseReply<T>(json);
            if (reply.Code == 0)
            {
                return reply.Payload;
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// 将控件适配到IDataSource用于数据的统一绑定
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IDataSource AdapterToIDataSource(object obj)
        {
            if (obj is KryptonComboBox)
                return new KryptonComboBox2IDataSource(obj as KryptonComboBox);
            else if (obj is KryptonListBox)
                return new KryptonListBox2IDataSource(obj as KryptonListBox);
            else if (obj is ListBox)
                return new ListBox2IDataSource(obj as ListBox);
            else if (obj is ComboBox)
                return new ComboBox2IDataSource(obj as ComboBox);
            else if (obj is CheckedListBox)
                return new CheckedListBox2IDataSource(obj as CheckedListBox);
            return new Invalid2IDataSource(); ;
        }

    }
}
