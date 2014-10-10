using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 查询服务端规则列表
    /// </summary>
    public class MGRQryRuleSetRequest:RequestPacket
    {
        public MGRQryRuleSetRequest()
        {
            _type = MessageTypes.MGRQRYRULECLASS;
            //RuleType = QSEnumRuleType.OrderRule;
        }
        //public QSEnumRuleType RuleType { get; set; }

        public override string ContentSerialize()
        {
            //return RuleType.ToString();
            return string.Empty;
        }

        public override void ContentDeserialize(string contentstr)
        {
            //this.RuleType = (QSEnumRuleType)Enum.Parse(typeof(QSEnumRuleType), contentstr);
        }
    }

    /// <summary>
    /// 查询风控规则回报
    /// </summary>
    public class RspMGRQryRuleSetResponse : RspResponsePacket
    {
        public RspMGRQryRuleSetResponse()
        {
            _type = MessageTypes.MGRRULECLASSRESPONSE;
            this.RuleClassItem = new RuleClassItem();
        }

        public RuleClassItem RuleClassItem { get; set; }

        public override string ResponseSerialize()
        {
            return this.RuleClassItem.Serialize();
        }

        public override void ResponseDeserialize(string content)
        {
            this.RuleClassItem.Deserialize(content);
        }
    }

    /// <summary>
    /// 请求更新规则
    /// </summary>
    public class MGRUpdateRuleRequest : RequestPacket
    {
        public MGRUpdateRuleRequest()
        {
            _type = MessageTypes.MGRUPDATERULEITEM;
            RuleItem = new RuleItem();
        }

        public RuleItem RuleItem { get; set; }

        public override string ContentSerialize()
        {
            return RuleItem.Serialize();
        }

        public override void ContentDeserialize(string contentstr)
        {
            this.RuleItem.Deserialize(contentstr);
        }
    }
    /// <summary>
    /// 更新帐户规则回报
    /// </summary>
    public class RspMGRUpdateRuleResponse : RspResponsePacket
    {
        public RspMGRUpdateRuleResponse()
        {
            _type = MessageTypes.MGRUPDATERULEITEMRESPONSE;
            RuleItem = new RuleItem();
        }

        public RuleItem RuleItem { get; set; }

        public override string ResponseSerialize()
        {
            return RuleItem.Serialize();
        }

        public override void ResponseDeserialize(string content)
        {
            this.RuleItem.Deserialize(content);
        }
    }

    /// <summary>
    /// 查询交易帐户的风控规则
    /// </summary>
    public class MGRQryRuleItemRequest : RequestPacket
    {
        public MGRQryRuleItemRequest()
        {
            _type = MessageTypes.MGRQRYRULEITEM;
        }

        /// <summary>
        /// 查询的交易帐号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 查询的风控规则类别
        /// </summary>
        public QSEnumRuleType RuleType { get; set; }


        public override string ContentSerialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(this.Account);
            sb.Append(d);
            sb.Append(this.RuleType.ToString());
            return sb.ToString();
        }

        public override void ContentDeserialize(string contentstr)
        {
            string[] rec = contentstr.Split(',');
            this.Account = rec[0];
            this.RuleType = (QSEnumRuleType)Enum.Parse(typeof(QSEnumRuleType), rec[1]);
        }
    }

    /// <summary>
    /// 查询帐户风控项目回报
    /// </summary>
    public class RspMGRQryRuleItemResponse : RspResponsePacket
    {
        public RspMGRQryRuleItemResponse()
        {
            _type = MessageTypes.MGRRULEITEMRESPONSE;
            this.RuleItem = new RuleItem();
        }

        public RuleItem RuleItem { get; set; }

        public override string ResponseSerialize()
        {
            return this.RuleItem.Serialize();
        }

        public override void ResponseDeserialize(string content)
        {
            this.RuleItem.Deserialize(content);
        }
    }


    /// <summary>
    /// 删除风控规则
    /// </summary>
    public class MGRDelRuleItemRequest : RequestPacket
    {
        public MGRDelRuleItemRequest()
        {
            _type = MessageTypes.MGRDELRULEITEM;
            RuleItem = new RuleItem();
        }

        public RuleItem RuleItem { get; set; }

        public override string ContentSerialize()
        {
            return RuleItem.Serialize();
        }

        public override void ContentDeserialize(string contentstr)
        {
            this.RuleItem.Deserialize(contentstr);
        }
    }

    /// <summary>
    /// 删除风控规则回报
    /// </summary>
    public class RspMGRDelRuleItemResponse : RspResponsePacket
    {
        public RspMGRDelRuleItemResponse()
        {
            _type = MessageTypes.MGRDELRULEITEMRESPONSE;
            this.RuleItem = new RuleItem();
        }

        public RuleItem RuleItem { get; set; }

        public override string ResponseSerialize()
        {
            return this.RuleItem.Serialize();
        }

        public override void ResponseDeserialize(string content)
        {
            this.RuleItem.Deserialize(content);
        }
    }
}
