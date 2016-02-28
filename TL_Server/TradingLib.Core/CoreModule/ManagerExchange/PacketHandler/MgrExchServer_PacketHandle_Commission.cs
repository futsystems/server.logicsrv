﻿using System;
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
        /// 查询手续费模板
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryCommissionTemplate", "QryCommissionTemplate - qry commission template", "查询手续费模板")]
        public void CTE_QryCommissionTemplate(ISession session)
        {
            Manager manager = session.GetManager();
            UIAccess access = manager.GetAccess();
            if (!access.r_commission)
            {
                throw new FutsRspError("无权查询手续费模板");
            }

            if (manager.IsRoot())
            {
                CommissionTemplateSetting[] templates = manager.Domain.GetCommissionTemplate().ToArray();
                session.ReplyMgr(templates);
            }
            else if (manager.IsAgent())//如果是代理
            {
                CommissionTemplateSetting[] templates = manager.Domain.GetCommissionTemplate().Where(item => item.Manager_ID == manager.BaseMgrID).ToArray();
                session.ReplyMgr(templates);
            }
            else
            {
                throw new FutsRspError("无权查询手续费模板");
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateCommissionTemplate", "UpdateCommissionTemplate - update commission template", "更新手续费模板",QSEnumArgParseType.Json)]
        public void CTE_UpdateCommissionTemplate(ISession session, string json)
        {
            Manager manager = session.GetManager();
            UIAccess access = manager.GetAccess();
            if (!access.r_commission)
            {
                throw new FutsRspError("无权更新手续费模板");
            }

            CommissionTemplateSetting t = Mixins.Json.JsonMapper.ToObject<CommissionTemplateSetting>(json);
            t.Domain_ID = manager.domain_id;
            bool isaddd = t.ID == 0;

            //如果是添加手续费模板 则需要预先将数据写入到数据库
            if (isaddd)
            {
                t.Manager_ID = manager.BaseMgrID;//如果是新添加 则设定管理主域ID        
            }
            else
            {
                if (!manager.IsInRoot())
                {
                    CommissionTemplate template = BasicTracker.CommissionTemplateTracker[t.ID];
                    if (template != null)
                    {
                        if (template.Manager_ID != manager.BaseMgrID)
                        {
                            throw new FutsRspError(string.Format("无权修改手续费模板[{0}]", template.Name));
                        }
                    }
                }
            }

            BasicTracker.CommissionTemplateTracker.UpdateCommissionTemplate(t);
            session.NotifyMgr("NotifyCommissionTemplate",BasicTracker.CommissionTemplateTracker[t.ID]);
            session.OperationSuccess("更新手续费模板成功");

        }


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "DeleteCommissionTemplate", "DeleteCommissionTemplate - delete commission template", "删除手续费模板")]
        public void CTE_DeleteCommissionTemplate(ISession session, int template_id)
        {
            Manager manager = session.GetManager();
            logger.Info(string.Format("管理员:{0} 删除手续费模板 request:{1}", manager.Login, template_id));

            UIAccess access = manager.GetAccess();
            if (!access.r_commission)
            {
                throw new FutsRspError("无权删除手续费模板项目");
            }

            CommissionTemplate template = BasicTracker.CommissionTemplateTracker[template_id];
            if (template == null)
            {
                throw new FutsRspError("指定手续费模板不存在");

            }
            if (template.Domain_ID != manager.domain_id)
            {
                throw new FutsRspError("手续费模板与管理员不属于同一域");
            }
            if (!manager.IsInRoot())
            {
                if (template.Manager_ID != manager.BaseMgrID)
                {
                    throw new FutsRspError(string.Format("无权删除手续费模板[{0}]", template.Name));
                }
            }

            //调用维护器 删除该模板
            BasicTracker.CommissionTemplateTracker.DeleteCommissionTemplate(template_id);
            IAccount[] accounts = manager.Domain.GetAccounts().ToArray();

            for (int i = 0; i < accounts.Length; i++)
            {
                IAccount acc = accounts[i];
                if (acc.Commission_ID == template_id)
                {
                    TLCtxHelper.ModuleAccountManager.UpdateAccountCommissionTemplate(acc.ID, 0);
                }
            }
            session.NotifyMgr("NotifyDeleteCommissionTemplate", template);
            session.OperationSuccess("删除续费模板成功");

        }



        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryCommissionTemplateItem", "QryCommissionTemplateItem - qry commission template item", "查询手续费模板项目")]
        public void CTE_QryCommissionTemplateItem(ISession session, int templateid)
        {
            Manager manager = session.GetManager();
            UIAccess access = manager.GetAccess();
            if (!access.r_commission)
            {
                throw new FutsRspError("无权查询保证金模板项目");
            }

            CommissionTemplate template = BasicTracker.CommissionTemplateTracker[templateid];
            if (template == null)
            {
                throw new FutsRspError("指定手续费模板不存在");

            }
            if (template.Domain_ID != manager.domain_id)
            {
                throw new FutsRspError("手续费模板与管理员不属于同一域");
            }
            if (!manager.IsInRoot())
            {
                if (template.Manager_ID != manager.BaseMgrID)
                {
                    throw new FutsRspError(string.Format("无权查询手续费模板[{0}]", template.Name));
                }
            }

            CommissionTemplateItemSetting[] items = template.CommissionItems.ToArray();
            for (int i = 0; i < items.Length;i++ )
            {
                session.ReplyMgr(items[i], i == items.Length - 1);
            }

        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateCommissionTemplateItem", "UpdateCommissionTemplateItem - update commission template item", "更新手续费模板项目", QSEnumArgParseType.Json)]
        public void CTE_UpdateCommissionTemplateItem(ISession session, string json)
        {
            Manager manager = session.GetManager();
            UIAccess access = manager.GetAccess();
            if (!access.r_commission)
            {
                throw new FutsRspError("无权更新手续费模板项目");
            }

            MGRCommissionTemplateItemSetting item = Mixins.Json.JsonMapper.ToObject<MGRCommissionTemplateItemSetting>(json);
            CommissionTemplate template = BasicTracker.CommissionTemplateTracker[item.Template_ID];
            if (template == null)
            {
                throw new FutsRspError("指定手续费模板不存在");
            }
            if (template.Domain_ID != manager.domain_id)
            {
                throw new FutsRspError("手续费模板与管理员不属于同一域");
            }  
            if (!manager.IsInRoot())
            {
                if (template.Manager_ID != manager.BaseMgrID)
                {
                    throw new FutsRspError(string.Format("无权修改手续费模板[{0}]", template.Name));
                }
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
                    CommissionTemplateItemSetting t = new CommissionTemplateItemSetting();
                    t.OpenByMoney = item.OpenByMoney;
                    t.OpenByVolume = item.OpenByVolume;
                    t.CloseByMoney = item.CloseByMoney;
                    t.CloseByVolume = item.CloseByVolume;
                    t.CloseTodayByMoney = item.CloseTodayByMoney;
                    t.CloseTodayByVolume = item.CloseTodayByVolume;
                    t.ChargeType = item.ChargeType;
                    t.Percent = item.Percent;

                    t.Code = item.Code;
                    t.Month = i;
                    t.Template_ID = item.Template_ID;

                    CommissionTemplateItem t2 = BasicTracker.CommissionTemplateTracker.CommissionTemplateItems.FirstOrDefault(x => x.Code.Equals(item.Code) && x.Month == i);
                    if (t2 != null)
                    {
                        t.ID = t2.ID;
                    }

                    //调用update更新或添加
                    BasicTracker.CommissionTemplateTracker.UpdateCommissionTemplateItem(t);
                    session.NotifyMgr("NotifyCommissionTemplateItem", template[t.CommissionItemKey]);
                }

            }
            else //更新 则便利所有手续费模板项目进行更新
            {
                //更新该品种所有月份
                if (item.SetAllMonth)
                {
                    foreach (CommissionTemplateItemSetting t in BasicTracker.CommissionTemplateTracker[item.Template_ID].CommissionItems.Where(x => x.Code.Equals(item.Code)))
                    {
                        t.OpenByMoney = item.OpenByMoney;
                        t.OpenByVolume = item.OpenByVolume;
                        t.CloseByMoney = item.CloseByMoney;
                        t.CloseByVolume = item.CloseByVolume;
                        t.CloseTodayByMoney = item.CloseTodayByMoney;
                        t.CloseTodayByVolume = item.CloseTodayByVolume;
                        t.ChargeType = item.ChargeType;
                        t.Percent = item.Percent;

                        //调用update更新或添加
                        BasicTracker.CommissionTemplateTracker.UpdateCommissionTemplateItem(t);
                        session.NotifyMgr("NotifyCommissionTemplateItem", template[t.CommissionItemKey]);
                    }
                }
                //更新所有品种所有月份
                else if (item.SetAllCodeMonth)
                {
                    foreach (CommissionTemplateItemSetting t in BasicTracker.CommissionTemplateTracker[item.Template_ID].CommissionItems)
                    {
                        t.OpenByMoney = item.OpenByMoney;
                        t.OpenByVolume = item.OpenByVolume;
                        t.CloseByMoney = item.CloseByMoney;
                        t.CloseByVolume = item.CloseByVolume;
                        t.CloseTodayByMoney = item.CloseTodayByMoney;
                        t.CloseTodayByVolume = item.CloseTodayByVolume;
                        t.ChargeType = item.ChargeType;
                        t.Percent = item.Percent;

                        //调用update更新或添加
                        BasicTracker.CommissionTemplateTracker.UpdateCommissionTemplateItem(t);
                        session.NotifyMgr("NotifyCommissionTemplateItem", template[t.CommissionItemKey]);
                    }
                }
                else //更新某个特定月份
                {
                    if (template[item.CommissionItemKey] == null)
                    {
                        throw new FutsRspError("手续费模板项目不存在");
                    }
                    //调用update更新或添加
                    BasicTracker.CommissionTemplateTracker.UpdateCommissionTemplateItem(item);
                    session.NotifyMgr("NotifyCommissionTemplateItem", template[item.CommissionItemKey]);
                }
            }
            session.OperationSuccess("更新手续费项目功");
        }
    }
}