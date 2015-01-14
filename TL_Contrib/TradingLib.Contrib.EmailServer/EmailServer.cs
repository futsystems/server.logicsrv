using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;



namespace TradingLib.Contrib.EmailSrv
{

    
    /// <summary>
    /// 邮件服务器,用于接收系统过来的信息,然后发送到管理员的邮件
    /// </summary>
    [ContribAttr(EmailServer.ContribName, "邮件发送服务", "邮件发送服务为系统扩展邮件发送功能,使得系统内组件可以通过全局邮件发送入口向系统管理员发送通知邮件")]
    public class EmailServer:BaseSrvObject,IContrib
    {

        const string ContribName = "EmailServer";

        System.Net.Mail.SmtpClient client;

        string _user;
        string _pass;
        string _smtp;
        string _from;
        string _noticelsit;
        int _port;

        ConfigDB _cfgdb;
        public EmailServer()
            : base(EmailServer.ContribName)
        {
            //1.加载配置文件
            _cfgdb = new ConfigDB(EmailServer.ContribName);
            if (!_cfgdb.HaveConfig("smtp"))
            {
                _cfgdb.UpdateConfig("smtp", QSEnumCfgType.String, "smtp.exmail.qq.com", "SMTP邮件服务器");
            }
            if (!_cfgdb.HaveConfig("sendfrom"))
            {
                _cfgdb.UpdateConfig("sendfrom", QSEnumCfgType.String, "tradingsrv@if888.com.cn", "发件地址");
            }
            if (!_cfgdb.HaveConfig("username"))
            {
                _cfgdb.UpdateConfig("username", QSEnumCfgType.String, "tradingsrv@if888.com", "邮箱用户名");
            }

            if (!_cfgdb.HaveConfig("pass"))
            {
                _cfgdb.UpdateConfig("pass", QSEnumCfgType.String, "hisense9", "邮箱密码");
            }

            if (!_cfgdb.HaveConfig("port"))
            {
                _cfgdb.UpdateConfig("port", QSEnumCfgType.Int, 25, "邮件发送端口");
            }

            if (!_cfgdb.HaveConfig("notifylist"))
            {
                _cfgdb.UpdateConfig("notifylist", QSEnumCfgType.String, "vincent@if888.com.cn", "邮件发送端口");
            }

            _smtp = _cfgdb["smtp"].AsString();
            _from = _cfgdb["sendfrom"].AsString();
            _user = _cfgdb["username"].AsString();
            _pass = _cfgdb["pass"].AsString();
            _port = _cfgdb["port"].AsInt();

            _noticelsit = _cfgdb["notifylist"].AsString();


            client = new System.Net.Mail.SmtpClient();
            client.Host = _smtp;
            client.Port = _port;
//            client.UseDefaultCredentials = true;
//            client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
//            client.Credentials = new System.Net.NetworkCredential(_user,_pass);

        }



        /// <summary>
        /// 加载
        /// </summary>
        public void OnLoad()
        {
            TLCtxHelper.SendEmailEvent +=new EmailDel(SendEmail);
            TLCtxHelper.EventSystem.PositionFlatEvent += new EventHandler<PositionFlatEventArgs>(EventSystem_PositionFlatEvent);
            //TLCtxHelper.ExContribEvent.FlatSuccessEvent += new PositionDelegate(ExContribEvent_FlatSuccessEvent);
        }

        void EventSystem_PositionFlatEvent(object sender, PositionFlatEventArgs e)
        {
            debug("强平:"+(e.FlatSuccess?"成功":"失败") + e.Position.ToString(), QSEnumDebugLevel.INFO);
        }

      
        /// <summary>
        /// 销毁
        /// </summary>
        public void OnDestory()
        {
            TLCtxHelper.SendEmailEvent -= new EmailDel(SendEmail);
            base.Dispose();

        }
        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
        
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            
        }
        static TimeSpan ts1 = new TimeSpan(0,0,2);

        //[TaskAttr("测试定时任务",2)]
        //[TaskAttr("测试定时任务2",0,0,15)]
        public void DemoTask()
        {
            Util.Debug("i am here....");
        }

        /*
        public void SendEmail(string subject, string body)
        {
            emailcache.Write(new Email(subject, body, new string[] { }));
        }**/
        /// <summary>
        /// 发送邮件，放入缓存队列进行队列发送
        /// </summary>
        /// <param name="email"></param>
        public void SendEmail(IEmail email)
        {
            emailcache.Write(email);
        }
        RingBuffer<IEmail> emailcache = new RingBuffer<IEmail>(1000);

        void Task_SendEmail()
        {
            try
            {
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
            debug(PROGRAME + ":发送邮件到:" + string.Join(",", email.Receivers), QSEnumDebugLevel.INFO);
            System.Net.Mail.MailMessage Message = new System.Net.Mail.MailMessage();
            Message.From = new System.Net.Mail.MailAddress(_from);//这里需要注
            foreach(string add in email.Receivers)
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

        public static bool SendTestMail(string emailserver,string emailsendfrom,string emaillogin,string emailpass,string adminemail,int port =25)
        {
            try
            {
                System.Net.Mail.SmtpClient d = new System.Net.Mail.SmtpClient();
                d.Host = emailserver;
                d.Port = port;
                d.UseDefaultCredentials = true;
                d.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                d.Credentials = new System.Net.NetworkCredential(emaillogin, emailpass);

                System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage();

                m.From = new System.Net.Mail.MailAddress(emailsendfrom);
                m.To.Add(adminemail);
                m.Subject = "设置测试邮件";
                m.Body = "测试成功";
                m.SubjectEncoding = System.Text.Encoding.UTF8;
                m.BodyEncoding = System.Text.Encoding.UTF8;
                m.Priority = System.Net.Mail.MailPriority.High;
                d.Send(m);
                return true;

            }
            catch (Exception ex)
            {
                return true;
            }
        
        }
    }
}
