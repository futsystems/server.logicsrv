using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.LitJson;

namespace Lottoqq.UCenter
{
    [ContribAttr(UCenterAccess.ContribName, "UCenterAccess接入扩展", "用于交易平台介入UCenter进行认证,查询以及对应货币类的操作")]
    public partial class UCenterAccess : ContribSrvObject, IContrib
    {


        const string ContribName = "UCenterAccess";

        ConfigDB _cfgdb;
        UCenterCli _ucli = null;
        int _port = 9000;
        string _address = "uc_dev.huiky.com";
        public UCenterAccess()
            : base(UCenterAccess.ContribName)
        { 
            
        }

        
        /// <summary>
        /// 加载
        /// </summary>
        public void OnLoad()
        {
            //TLCtxHelper.EventSession.AuthUserEvent += new AuthUserDel(AuthUser);

            TLCtxHelper.EventSession.AuthUserEvent += new LoginRequestDel<TrdClientInfo>(AuthUser);

            _cfgdb = new ConfigDB(UCenterAccess.ContribName);
            if (!_cfgdb.HaveConfig("ucaddress"))
            {
                _cfgdb.UpdateConfig("ucaddress", QSEnumCfgType.String, "uc_dev.huiky.com", "UCenter API地址");
            }
            if (!_cfgdb.HaveConfig("ucport"))
            {
                _cfgdb.UpdateConfig("ucport", QSEnumCfgType.Int, 9000, "UCenter API访问端口");
            }
            _address = _cfgdb["ucaddress"].AsString();
            _port = _cfgdb["ucport"].AsInt();

            debug("UCenter Server :" + _address + ":" + _port.ToString(), QSEnumDebugLevel.INFO);
            _ucli = new UCenterCli(_address, _port);

        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void OnDestory()
        {
            TLCtxHelper.EventSession.AuthUserEvent -= new LoginRequestDel<TrdClientInfo>(AuthUser);
            base.Dispose();
        }
        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            debug("UCenterAccess Starting.......",QSEnumDebugLevel.INFO);
            _ucli.Init();

        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            
        }

        /// 外部用户中心认证,通过login pass向外部授权服务进行认证,如果认证通过则返回0,并返回全局uid,认证失败则返回错误代码,并同时返回messsage
        /// 0:验证成功
        /// 1:验证函数未绑定/没有绑定到实际的验证函数
        /// 2:用户不存在
        /// 3:验证失败
        void AuthUser(TrdClientInfo cinfo,LoginRequest request,ref LoginResponse response)
        { 
            if (_ucli.IsLive)
            {
                string ret = _ucli.AuthUser(request.LoginID, request.Passwd);
                debug("got auth rep:" + ret, QSEnumDebugLevel.INFO);
                JsonData data = JsonMapper.ToObject(ret);
                //认证失败
                if (Convert.ToInt32(data["Result"].ToString()) != 0)
                {
                    //认证失败
                    response.RspInfo.FillError("INVALID_LOGIN");
                    response.Authorized = false;
                   
                }
                //认证成功
                else
                {
                    //认证成功
                    response.LoginID = request.LoginID;
                    response.Authorized = true;
                    response.UserID = Convert.ToInt32(data["UID"].ToString());
                    response.Email = data["Email"] != null ? data["Email"].ToString() : "";
                    response.NickName = data["NickName"] != null ? data["NickName"].ToString() : "";
                    response.Mobile = data["Mobile"] != null ? data["Mobile"].ToString() : "";

                }
            }
            else
            {
                response.Authorized = false;
            }
        }
    }
}
