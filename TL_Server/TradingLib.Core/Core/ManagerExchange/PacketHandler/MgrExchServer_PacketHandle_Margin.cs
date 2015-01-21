using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Protocol;


namespace TradingLib.Core
{
    public partial class MgrExchServer
    {
        /// <summary>
        /// 查询保证金模板
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryMarginTemplate", "QryMarginTemplate - qry margin template", "查询保证金模板")]
        public void CTE_QryMarginTemplate(ISession session)
        {
            Manager manager = session.GetManager();
            if (manager.IsRoot())
            {
                MarginTemplateSetting[] templates = manager.Domain.GetMarginTemplate().ToArray();
                session.ReplyMgr(templates);
            }
            else
            {
                throw new FutsRspError("无权查询手续费模板");
            }
        }

        /// <summary>
        /// 更新保证金模板
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateMarginTemplate", "UpdateMarginTemplate - update margin template", "更新保证金模板", QSEnumArgParseType.Json)]
        public void CTE_UpdateMarginTemplate(ISession session, string json)
        {
            Manager manager = session.GetManager();
            if (manager.IsRoot())
            {
                MarginTemplateSetting t = Mixins.Json.JsonMapper.ToObject<MarginTemplateSetting>(json);
                t.Domain_ID = manager.domain_id;
                bool isaddd = t.ID == 0;
                BasicTracker.MarginTemplateTracker.UpdateMarginTemplate(t);

                //如果是添加手续费模板 则需要预先将数据写入到数据库
                if (isaddd)
                {

                }
                session.NotifyMgr("NotifyMarginTemplate", BasicTracker.MarginTemplateTracker[t.ID]);
                session.OperationSuccess("更新保证金模板成功");
            }
            else
            {
                throw new FutsRspError("无权修改保证金模板");
            }
        }

        /// <summary>
        /// 查询保证金项目
        /// </summary>
        /// <param name="session"></param>
        /// <param name="templateid"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryMarginTemplateItem", "QryMarginTemplateItem - qry margin template item", "查询保证金模板项目")]
        public void CTE_QryMarginTemplateItem(ISession session, int templateid)
        {
            Manager manager = session.GetManager();
            if (manager.IsRoot())
            {
                MarginTemplate template = BasicTracker.MarginTemplateTracker[templateid];

                MarginTemplateItemSetting[] items = template.MarginTemplateItems.ToArray();
                session.ReplyMgr(items);
            }
            else
            {
                throw new FutsRspError("无权查询保证金模板项目");
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateMarginTemplateItem", "UpdateMarginTemplateItem - update margin template item", "更新保证金模板项目", QSEnumArgParseType.Json)]
        public void CTE_UpdateMarginTemplateItem(ISession session, string json)
        {
            Manager manager = session.GetManager();
            if (manager.IsRoot())
            {
                MGRMarginTemplateItemSetting item = Mixins.Json.JsonMapper.ToObject<MGRMarginTemplateItemSetting>(json);
                MarginTemplate template = BasicTracker.MarginTemplateTracker[item.Template_ID];
                if (template == null)
                {
                    throw new FutsRspError("指定保证金模板不存在");
                }

                if (!manager.Domain.GetSecurityFamilies().Any(sec => sec.Code.Equals(item.Code)))
                {
                    throw new FutsRspError("不存在对应的品种");
                }

                bool isadd = item.ID == 0;
                if (isadd)//如果是添加 则没有更新所有品种所有月份的选项
                {
                    for (int i = 1; i <= 12; i++)
                    {
                        MarginTemplateItemSetting t = new MarginTemplateItemSetting();
                        t.MarginByMoney = item.MarginByMoney;
                        t.MarginByVolume = item.MarginByVolume;
                        t.ChargeType = item.ChargeType;
                        t.Percent = item.Percent;

                        t.Code = item.Code;
                        t.Month = i;
                        t.Template_ID = item.Template_ID;

                        MarginTemplateItemSetting t2 = BasicTracker.MarginTemplateTracker.MarginTemplateItems.FirstOrDefault(x => x.Code.Equals(item.Code) && x.Month == i);
                        if (t2 != null)
                        {
                            t.ID = t2.ID;
                        }

                        //调用update更新或添加
                        BasicTracker.MarginTemplateTracker.UpdateMarginTemplateItem(t);
                        session.NotifyMgr("NotifyMarginTemplateItem", template[t.Code, t.Month]);
                    }

                }
                else //更新 则便利所有手续费模板项目进行更新
                {
                    //更新该品种所有月份
                    if (item.SetAllMonth)
                    {
                        foreach (MarginTemplateItemSetting t in BasicTracker.MarginTemplateTracker.MarginTemplateItems.Where(x => x.Code.Equals(item.Code)))
                        {
                            t.MarginByMoney = item.MarginByMoney;
                            t.MarginByVolume = item.MarginByVolume;
                            t.ChargeType = item.ChargeType;
                            t.Percent = item.Percent;

                            //调用update更新或添加
                            BasicTracker.MarginTemplateTracker.UpdateMarginTemplateItem(t);
                            session.NotifyMgr("NotifyMarginTemplateItem", template[t.Code, t.Month]);
                        }
                    }
                    //更新所有品种所有月份
                    else if (item.SetAllCodeMonth)
                    {
                        foreach (MarginTemplateItemSetting t in BasicTracker.MarginTemplateTracker.MarginTemplateItems)
                        {
                            t.MarginByMoney = item.MarginByMoney;
                            t.MarginByVolume = item.MarginByVolume;
                            t.ChargeType = item.ChargeType;
                            t.Percent = item.Percent;

                            //调用update更新或添加
                            BasicTracker.MarginTemplateTracker.UpdateMarginTemplateItem(t);
                            session.NotifyMgr("NotifyMarginTemplateItem", template[t.Code, t.Month]);
                        }
                    }
                    else //更新某个特定月份
                    {
                        if (template[item.Code, item.Month] == null)
                        {
                            throw new FutsRspError("保证金模板项目不存在");
                        }
                        //调用update更新或添加
                        BasicTracker.MarginTemplateTracker.UpdateMarginTemplateItem(item);
                        session.NotifyMgr("NotifyMarginTemplateItem", template[item.Code, item.Month]);

                    }
                }
                //}
                session.OperationSuccess("更新手续费项目功");
            }
            else
            {
                throw new FutsRspError("无权修改手续费模板");
            }
        }
    }
}
