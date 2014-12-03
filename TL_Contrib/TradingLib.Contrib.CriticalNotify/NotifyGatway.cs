using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;



namespace TradingLib.Contrib.NotifyCentre
{
    [ContribAttr(NotifyGatway.ContribName, "重要消息通知扩展", "用于采集系统重要信息然后通过邮件,短消息,微信等方式对外发送")]
    public partial class NotifyGatway : ContribSrvObject, IContrib
    {
        const string ContribName = "NotifyGatway";

        System.Net.Mail.SmtpClient client;

        string _user;
        string _pass;
        string _smtp;
        string _from;
        string _noticelsit;
        int _port;

        ConfigDB _cfgdb;



        public NotifyGatway()
            : base(NotifyGatway.ContribName)
        {

            
        }





        /// <summary>
        /// 加载
        /// </summary>
        public void OnLoad()
        {
            debug("");
            
            //1.加载配置文件
            _cfgdb = new ConfigDB(NotifyGatway.ContribName);
            if (!_cfgdb.HaveConfig("smtp"))
            {
                _cfgdb.UpdateConfig("smtp", QSEnumCfgType.String, "smtp.qq.com", "SMTP邮件服务器");
            }
            if (!_cfgdb.HaveConfig("sendfrom"))
            {
                _cfgdb.UpdateConfig("sendfrom", QSEnumCfgType.String, "468669351@qq.com", "发件地址");
            }
            if (!_cfgdb.HaveConfig("username"))
            {
                _cfgdb.UpdateConfig("username", QSEnumCfgType.String, "468669351@qq.com", "邮箱用户名");
            }

            if (!_cfgdb.HaveConfig("pass"))
            {
                _cfgdb.UpdateConfig("pass", QSEnumCfgType.String, "lemtone2005", "邮箱密码");
            }

            if (!_cfgdb.HaveConfig("port"))
            {
                _cfgdb.UpdateConfig("port", QSEnumCfgType.Int, 25, "邮件发送端口");
            }

            _smtp = "smtp.163.com"; ;// _cfgdb["smtp"].AsString();
            _from = "quantshop@163.com";// _cfgdb["sendfrom"].AsString();
            _user = "quantshop@163.com";// _cfgdb["username"].AsString();
            _pass = "lemtone2005";//_cfgdb["pass"].AsString();
            _port = 25;//_cfgdb["port"].AsInt();

            debug("smtp:" + _smtp + " from:" + _from + " user:" + _user + " pass:" + _pass + " port:" + _port.ToString(), QSEnumDebugLevel.INFO);
            client = new System.Net.Mail.SmtpClient();
            client.Host = _smtp;
            client.Port = _port;
            client.EnableSsl = true;
            client.UseDefaultCredentials = true;
            client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            client.Credentials = new System.Net.NetworkCredential(_user, _pass);

            TLCtxHelper.CashOperationEvent.CashOperationRequest += new EventHandler<CashOperationEventArgs>(CashOperationEvent_CashOperationRequest);


        }

        /// <summary>
        /// 响应出入金操作事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CashOperationEvent_CashOperationRequest(object sender, CashOperationEventArgs e)
        {
            EmailDrop emaildrop = new CashOperationEmailDrop(e.CashOperation);

            foreach (IEmail email in emaildrop.GenerateEmails())
            {
                SendEmail(email);
            }
        }

        public void SendEmail(IEmail email)
        {
            emailcache.Write(email);
        }
        RingBuffer<IEmail> emailcache = new RingBuffer<IEmail>(1000);

        [TaskAttr("每5秒检查一次邮件发送",5,"定时发送邮件")]
        public void Task_SendEmail()
        {
            try
            {
                //debug("check and send emial ------------", QSEnumDebugLevel.INFO);
                while (emailcache.hasItems)
                {
                    sendmail(emailcache.Read());
                }
            }
            catch (Exception ex)
            {
                debug("Send email error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }

        /// <summary>
        /// 通过client发送电子邮件
        /// </summary>
        /// <param name="email"></param>
        void sendmail(IEmail email)
        {
            if (email.Receivers.Length == 0)
                email.Receivers = _noticelsit.Split(',');
            debug("send email to:" + string.Join(",", email.Receivers), QSEnumDebugLevel.INFO);
            System.Net.Mail.MailMessage Message = new System.Net.Mail.MailMessage();
            Message.From = new System.Net.Mail.MailAddress(_from);//这里需要注
            foreach (string add in email.Receivers)
            {
                Message.To.Add(add);
            }

            Message.Subject = email.Subject;
            Message.Body = email.Body;
            Message.SubjectEncoding = System.Text.Encoding.UTF8;
            Message.BodyEncoding = System.Text.Encoding.UTF8;
            Message.Priority = System.Net.Mail.MailPriority.High;
            
            client.Send(Message);
        }


        /// <summary>
        /// 销毁
        /// </summary>
        public void OnDestory() { }
        /// <summary>
        /// 启动
        /// </summary>
        public void Start() { }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop() { }
    }
}
