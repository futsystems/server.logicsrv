//Copyright 2013 by FutSystems,Inc.
//20170112 整理无用操作

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Protocol;


namespace TradingLib.Core
{
    /// <summary>
    /// 交易账户参数模板
    /// </summary>
    public partial class MgrExchServer
    {
        #region 手续费模板
        /// <summary>
        /// 查询手续费模板
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryCommissionTemplate", "QryCommissionTemplate - qry commission template", "查询手续费模板")]
        public void CTE_QryCommissionTemplate(ISession session)
        {
            Manager manager = session.GetManager();
            if (manager.IsInRoot())
            {
                CommissionTemplateSetting[] items = manager.Domain.GetCommissionTemplate().ToArray();
                if (items.Length == 0)
                {
                    session.ReplyMgr(null, true);
                }
                for (int i = 0; i < items.Length;i++ )
                {
                    session.ReplyMgr(items[i], i == items.Length - 1);
                }
            }
            else if (manager.IsInAgent())
            {
                CommissionTemplateSetting[] items = manager.Domain.GetCommissionTemplate().Where(item => item.Manager_ID == manager.BaseMgrID).ToArray();
                if (items.Length == 0)
                {
                    session.ReplyMgr(null, true);
                }
                for (int i = 0; i < items.Length; i++)
                {
                    session.ReplyMgr(items[i], i == items.Length - 1);
                }
            }
        }

        [PermissionRequiredAttr("r_template_edit")]
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateCommissionTemplate", "UpdateCommissionTemplate - update commission template", "更新手续费模板",QSEnumArgParseType.Json)]
        public void CTE_UpdateCommissionTemplate(ISession session, string json)
        {
            Manager manager = session.GetManager();
            CommissionTemplateSetting t = json.DeserializeObject<CommissionTemplateSetting>();
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
            session.RspMessage("更新手续费模板成功");

        }

        [PermissionRequiredAttr("r_template_edit")]
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "DeleteCommissionTemplate", "DeleteCommissionTemplate - delete commission template", "删除手续费模板")]
        public void CTE_DeleteCommissionTemplate(ISession session, int template_id)
        {
            Manager manager = session.GetManager();
            logger.Info(string.Format("管理员:{0} 删除手续费模板 request:{1}", manager.Login, template_id));

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
            //如果交易账户试用的该模板则将账户模板重置
            foreach (var acc in manager.Domain.GetAccounts())
            {
                if (acc.Commission_ID == template_id)
                {
                    TLCtxHelper.ModuleAccountManager.UpdateAccountCommissionTemplate(acc.ID, 0);
                }
            }
            session.NotifyMgr("NotifyDeleteCommissionTemplate", template);
            session.RspMessage("删除续费模板成功");

        }



        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryCommissionTemplateItem", "QryCommissionTemplateItem - qry commission template item", "查询手续费模板项目")]
        public void CTE_QryCommissionTemplateItem(ISession session, int templateid)
        {
            Manager manager = session.GetManager();
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
                //由代理创建或者代理自己当前模板则有权查询
                if (template.Manager_ID == manager.BaseMgrID || (manager.AgentAccount != null && manager.AgentAccount.Commission_ID == templateid))
                {

                }
                else
                {
                    throw new FutsRspError(string.Format("无权查询手续费模板[{0}]", template.Name));
                }
            }

            CommissionTemplateItemSetting[] items = template.CommissionItems.ToArray();
            //for (int i = 0; i < items.Length;i++ )
            //{
            //    session.ReplyMgr(items[i], i == items.Length - 1);
            //}
            session.ReplyMgrArray(items);

        }

        [PermissionRequiredAttr("r_template_edit")]
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateCommissionTemplateItem", "UpdateCommissionTemplateItem - update commission template item", "更新手续费模板项目", QSEnumArgParseType.Json)]
        public void CTE_UpdateCommissionTemplateItem(ISession session, string json)
        {
            Manager manager = session.GetManager();

            MGRCommissionTemplateItemSetting item = json.DeserializeObject<MGRCommissionTemplateItemSetting>();
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
            session.RspMessage("更新手续费项目功");
        }
        #endregion

        #region 保证金模板
        /// <summary>
        /// 查询保证金模板
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryMarginTemplate", "QryMarginTemplate - qry margin template", "查询保证金模板")]
        public void CTE_QryMarginTemplate(ISession session)
        {
            Manager manager = session.GetManager();
           

            if (manager.IsInRoot())
            {
                MarginTemplateSetting[] items = manager.Domain.GetMarginTemplate().ToArray();
                //if (items.Length == 0)
                //{
                //    session.ReplyMgr(null, true);
                //}
                //for (int i = 0; i < items.Length; i++)
                //{
                //    session.ReplyMgr(items[i], i == items.Length - 1);
                //}
                session.ReplyMgrArray(items);
            }
            else if (manager.IsInAgent())
            {
                MarginTemplateSetting[] items = manager.Domain.GetMarginTemplate().Where(item => item.Manager_ID == manager.BaseMgrID).ToArray();
                //if (items.Length == 0)
                //{
                //    session.ReplyMgr(null, true);
                //}
                //for (int i = 0; i < items.Length; i++)
                //{
                //    session.ReplyMgr(items[i], i == items.Length - 1);
                //}
                session.ReplyMgrArray(items);
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
        [PermissionRequiredAttr("r_template_edit")]
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateMarginTemplate", "UpdateMarginTemplate - update margin template", "更新保证金模板", QSEnumArgParseType.Json)]
        public void CTE_UpdateMarginTemplate(ISession session, string json)
        {

            Manager manager = session.GetManager();
           

            MarginTemplateSetting t = json.DeserializeObject<MarginTemplateSetting>();// Mixins.Json.JsonMapper.ToObject<MarginTemplateSetting>(json);
            t.Domain_ID = manager.domain_id;
            bool isaddd = t.ID == 0;
            if (isaddd)
            {
                t.Manager_ID = manager.BaseMgrID;//如果是新添加 则设定管理主域ID
            }
            else
            {
                if (!manager.IsInRoot())
                {
                    MarginTemplate template = BasicTracker.MarginTemplateTracker[t.ID];
                    if (template != null)
                    {
                        if (template.Manager_ID != manager.BaseMgrID)
                        {
                            throw new FutsRspError(string.Format("无权修改保证金模板[{0}]", template.Name));
                        }
                    }
                }
            }
            BasicTracker.MarginTemplateTracker.UpdateMarginTemplate(t);
            session.NotifyMgr("NotifyMarginTemplate", BasicTracker.MarginTemplateTracker[t.ID]);
            session.RspMessage("更新保证金模板成功");

        }

        [PermissionRequiredAttr("r_template_edit")]
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "DeleteMarginTemplate", "DeleteMarginTemplate - delete margin template", "删除保证金模板")]
        public void CTE_DeleteMarginTemplate(ISession session, int template_id)
        {
            Manager manager = session.GetManager();
            logger.Info(string.Format("管理员:{0} 删除保证金模板 request:{1}", manager.Login, template_id));

          

            MarginTemplate template = BasicTracker.MarginTemplateTracker[template_id];
            if (template == null)
            {
                throw new FutsRspError("指定保证金模板不存在");
            }
            if (template.Domain_ID != manager.domain_id)
            {
                throw new FutsRspError("保证金模板与管理员不属于同一域");
            }
            if (!manager.IsInRoot())
            {
                if (template.Manager_ID != manager.BaseMgrID)
                {
                    throw new FutsRspError(string.Format("无权修改保证金模板[{0}]", template.Name));
                }
            }


            //调用维护器 删除该模板
            BasicTracker.MarginTemplateTracker.DeleteMarginTemplate(template_id);

            IAccount[] accounts = manager.Domain.GetAccounts().ToArray();

            for (int i = 0; i < accounts.Length; i++)
            {
                IAccount acc = accounts[i];
                if (acc.Commission_ID == template_id)
                {
                    TLCtxHelper.ModuleAccountManager.UpdateAccountMarginTemplate(acc.ID, 0);
                }
            }

            session.NotifyMgr("NotifyDeleteMarginTemplate", template);
            session.RspMessage("删除保证金模板成功");

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

            MarginTemplate template = BasicTracker.MarginTemplateTracker[templateid];
            if (template == null)
            {
                throw new FutsRspError("指定保证金模板不存在");
            }
            if (template.Domain_ID != manager.domain_id)
            {
                throw new FutsRspError("保证金模板与管理员不属于同一域");
            }
            if (!manager.IsInRoot())
            {
                if (template.Manager_ID != manager.BaseMgrID)
                {
                    throw new FutsRspError(string.Format("无权修改保证金模板[{0}]", template.Name));
                }
            }

            MarginTemplateItemSetting[] items = template.MarginTemplateItems.ToArray();
            //for (int i = 0; i < items.Length; i++)
            //{
            //    session.ReplyMgr(items[i], i != items.Length - 1);
            //}
            session.ReplyMgrArray(items);
        }

        [PermissionRequiredAttr("r_template_edit")]
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateMarginTemplateItem", "UpdateMarginTemplateItem - update margin template item", "更新保证金模板项目", QSEnumArgParseType.Json)]
        public void CTE_UpdateMarginTemplateItem(ISession session, string json)
        {
            Manager manager = session.GetManager();
            

            MGRMarginTemplateItemSetting item = json.DeserializeObject<MGRMarginTemplateItemSetting>();// Mixins.Json.JsonMapper.ToObject<MGRMarginTemplateItemSetting>(json);
            MarginTemplate template = BasicTracker.MarginTemplateTracker[item.Template_ID];
            if (template == null)
            {
                throw new FutsRspError("指定保证金模板不存在");
            }
            if (template.Domain_ID != manager.domain_id)
            {
                throw new FutsRspError("保证金模板与管理员不属于同一域");
            }
            if (!manager.IsInRoot())
            {
                if (template.Manager_ID != manager.BaseMgrID)
                {
                    throw new FutsRspError(string.Format("无权修改保证金模板[{0}]", template.Name));
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
                    foreach (MarginTemplateItemSetting t in BasicTracker.MarginTemplateTracker[item.Template_ID].MarginTemplateItems.Where(x => x.Code.Equals(item.Code)))
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
                    foreach (MarginTemplateItemSetting t in BasicTracker.MarginTemplateTracker[item.Template_ID].MarginTemplateItems)
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
            session.RspMessage("更新手续费项目功");
        }

        #endregion

        #region 策略模板
        /// <summary>
        /// 查询计算策略模板
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryExStrategyTemplate", "QryExStrategyTemplate - qry exstrategy template", "查询计算策略模板")]
        public void CTE_QryExStrategyTemplate(ISession session)
        {
            Manager manager = session.GetManager();
            if (manager.IsInRoot())
            {
                ExStrategyTemplate[] items = manager.Domain.GetExStrategyTemplate().ToArray();
                //if (items.Length == 0)
                //{
                //    session.ReplyMgr(null, true);
                //}
                //for (int i = 0; i < items.Length; i++)
                //{
                //    session.ReplyMgr(items[i], i == items.Length - 1);
                //}
                session.ReplyMgrArray(items);
            }
            else if (manager.IsInAgent())
            {
                ExStrategyTemplate[] items = manager.Domain.GetExStrategyTemplate().Where(item => item.Manager_ID == manager.BaseMgrID).ToArray();
                //if (items.Length == 0)
                //{
                //    session.ReplyMgr(null, true);
                //}
                //for (int i = 0; i < items.Length; i++)
                //{
                //    session.ReplyMgr(items[i], i == items.Length - 1);
                //}
                session.ReplyMgrArray(items);
            }
            else
            {
                throw new FutsRspError("无权查询计算策略模板");
            }
        }

        [PermissionRequiredAttr("r_template_edit")]
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateExStrategyTemplate", "UpdateExStrategyTemplate - update exstrategy template", "更新计算策略模板", QSEnumArgParseType.Json)]
        public void CTE_UpdateExStrategyTemplate(ISession session, string json)
        {
            Manager manager = session.GetManager();
            ExStrategyTemplateSetting t = json.DeserializeObject<ExStrategyTemplateSetting>();// Mixins.Json.JsonMapper.ToObject<ExStrategyTemplateSetting>(json);
            t.Domain_ID = manager.domain_id;
            bool isaddd = t.ID == 0;

            //如果是添加手续费模板 则需要预先将数据写入到数据库
            if (isaddd)
            {
                t.Manager_ID = manager.BaseMgrID;//如果是新添加 则设定管理主域ID    
            }
            else
            {
                if (!manager.IsRoot())
                {
                    ExStrategyTemplate template = BasicTracker.ExStrategyTemplateTracker[t.ID];
                    if (template != null)
                    {
                        if (template.Manager_ID != manager.BaseMgrID)
                        {
                            throw new FutsRspError(string.Format("无权修改交易参数模板[{0}]", template.Name));
                        }
                    }
                }
            }

            BasicTracker.ExStrategyTemplateTracker.UpdateExStrategyTemplate(t);
            session.NotifyMgr("NotifyExStrategyTemplate", BasicTracker.ExStrategyTemplateTracker[t.ID]);
            session.RspMessage("更新计算策略模板成功");

        }

        [PermissionRequiredAttr("r_template_edit")]
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "DeleteExStrategyTemplate", "DeleteExStrategyTemplate - delete ex strategy template", "删除交易参数模板")]
        public void CTE_DelExStrategyTemplate(ISession session, int template_id)
        {
            Manager manager = session.GetManager();
            ExStrategyTemplate template = BasicTracker.ExStrategyTemplateTracker[template_id];
            if (template == null)
            {
                throw new FutsRspError("指定交易参数模板不存在");
            }
            if (template.Domain_ID != manager.domain_id)
            {
                throw new FutsRspError("交易参数模板与管理员不属于同一域");
            }
            if (!manager.IsInRoot())
            {
                if (template.Manager_ID != manager.BaseMgrID)
                {
                    throw new FutsRspError(string.Format("无权删除交易参数模板[{0}]", template.Name));
                }
            }


            //调用维护器 删除该模板
            BasicTracker.ExStrategyTemplateTracker.DeleteExStrategyTemplate(template_id);
            IAccount[] accounts = manager.Domain.GetAccounts().ToArray();

            //遍历所有交易帐户 如果交易参数模板为删掉的模板则将模板id设置为0
            for (int i = 0; i < accounts.Length; i++)
            {
                IAccount acc = accounts[i];
                if (acc.ExStrategy_ID == template_id)
                {
                    TLCtxHelper.ModuleAccountManager.UpdateAccountExStrategyTemplate(acc.ID, 0);
                }
            }
            session.NotifyMgr("NotifyDeleteExStrategyTemplate", template);
            session.RspMessage("删除交易参数模板成功");
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryExStrategyTemplateItem", "QryExStrategyTemplateItem - qry commission template item", "查询手续费模板项目")]
        public void CTE_QryExStrategyTemplateItem(ISession session, int templateid)
        {
            Manager manager = session.GetManager();
            ExStrategyTemplate template = BasicTracker.ExStrategyTemplateTracker[templateid];
            if (template == null)
            {
                throw new FutsRspError("指定交易参数模板不存在");
            }
            if (template.Domain_ID != manager.domain_id)
            {
                throw new FutsRspError("交易参数模板与管理员不属于同一域");
            }
            if (!manager.IsInRoot())
            {
                if (template.Manager_ID != manager.BaseMgrID)
                {
                    throw new FutsRspError(string.Format("无权查询交易参数模板[{0}]", template.Name));
                }
            }
            session.ReplyMgr(template.ExStrategy);

        }



        [PermissionRequiredAttr("r_template_edit")]
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateExStrategyTemplateItem", "UpdateExStrategyTemplateItem - update exstrategy template item", "更新交易参数模板项目", QSEnumArgParseType.Json)]
        public void CTE_UpdateExStrategyTemplateItem(ISession session, string json)
        {
            Manager manager = session.GetManager();
            
            ExStrategy item = json.DeserializeObject<ExStrategy>();// Mixins.Json.JsonMapper.ToObject<ExStrategy>(json);
            ExStrategyTemplate template = BasicTracker.ExStrategyTemplateTracker[item.Template_ID];
            if (template == null)
            {
                throw new FutsRspError("指定交易参数模板不存在");
            }
            if (template.Domain_ID != manager.domain_id)
            {
                throw new FutsRspError("交易参数模板与管理员不属于同一域");
            }
            if (!manager.IsInRoot())
            {
                if (template.Manager_ID != manager.BaseMgrID)
                {
                    throw new FutsRspError(string.Format("无权查询交易参数模板[{0}]", template.Name));
                }
            }

            BasicTracker.ExStrategyTemplateTracker.UpdateExStrategy(item);
            session.NotifyMgr("NotifyExStrategyTemplateItem", BasicTracker.ExStrategyTemplateTracker[item.Template_ID].ExStrategy);
            session.RspMessage("更新交易参数模板成功");

        }
        #endregion

        #region 配置模板
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryConfigTemplate", "QryConfigTemplate - qry config template", "查询配置模板")]
        public void CTE_QryConfigTemplate(ISession session)
        {
            Manager manager = session.GetManager();
            if (manager.IsInRoot())
            {
                ConfigTemplate[] items = manager.Domain.GetConfigTemplate().ToArray();
                if (items.Length == 0)
                {
                    session.ReplyMgr(null, true);
                }
                for (int i = 0; i < items.Length; i++)
                {
                    session.ReplyMgr(items[i], i == items.Length - 1);
                }
            }
            else if (manager.IsInAgent())
            {
                ConfigTemplate[] items = manager.Domain.GetConfigTemplate().Where(item => item.Manager_ID == manager.BaseMgrID).ToArray();
                if (items.Length == 0)
                {
                    session.ReplyMgr(null, true);
                }
                for (int i = 0; i < items.Length; i++)
                {
                    session.ReplyMgr(items[i], i == items.Length - 1);
                }
            }
            else
            {
                throw new FutsRspError("无权查询计算策略模板");
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateConfigTemplate", "UpdateConfigTemplate - update config template", "更新配置模板", QSEnumArgParseType.Json)]
        public void CTE_UpdateConfigTemplate(ISession session, string json)
        {
            Manager manager = session.GetManager();
            ConfigTemplate t = json.DeserializeObject<ConfigTemplate>();
            t.Domain_ID = manager.domain_id;
            bool isaddd = t.ID == 0;

            //如果是添加手续费模板 则需要预先将数据写入到数据库
            if (isaddd)
            {
                t.Manager_ID = manager.BaseMgrID;//如果是新添加 则设定管理主域ID    
            }
            else
            {
                if (!manager.IsRoot())
                {
                    ExStrategyTemplate template = BasicTracker.ExStrategyTemplateTracker[t.ID];
                    if (template != null)
                    {
                        if (template.Manager_ID != manager.BaseMgrID)
                        {
                            throw new FutsRspError(string.Format("无权修改配置模板[{0}]", template.Name));
                        }
                    }
                }
            }

            BasicTracker.ConfigTemplateTracker.UpdateConfigTemplate(t);
            session.NotifyMgr("NotifyConfigTemplate", BasicTracker.ConfigTemplateTracker[t.ID]);
            session.RspMessage("更新配置模板成功");



        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "DeleteConfigTemplate", "DeleteConfigTemplate - delete config template", "删除配置模板")]
        public void CTE_DeleteConfigTemplate(ISession session, int template_id)
        {
            Manager manager = session.GetManager();
            ConfigTemplate template = BasicTracker.ConfigTemplateTracker[template_id];
            if (template == null)
            {
                throw new FutsRspError("指定配置模板不存在");
            }
            if (template.Domain_ID != manager.domain_id)
            {
                throw new FutsRspError("配置模板与管理员不属于同一域");
            }
            if (!manager.IsInRoot())
            {
                if (template.Manager_ID != manager.BaseMgrID)
                {
                    throw new FutsRspError(string.Format("无权删除配置模板[{0}]", template.Name));
                }
            }

            //delete config template
            BasicTracker.ConfigTemplateTracker.DeleteConfigTemplate(template_id);

            session.NotifyMgr("NotifyDeleteConfigTemplate", template);
            session.RspMessage("删除配置模板成功");
        }

        #endregion

    }


}
