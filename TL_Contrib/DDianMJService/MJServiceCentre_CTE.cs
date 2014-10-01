using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.LitJson;


namespace Lottoqq.MJService
{
    public partial class MJServiceCentre
    {
        //[TaskAttr("1秒检查待平持仓", 1, "检查风控")]
        public void CTE_CheckPosition()
        {
            //循环检查所有合约
            foreach (Position pos in GetINNOVLottoPositoins())
            {
                decimal dv = pos.UnRealizedPL * pos.oSymbol.ULSymbol.Multiple;
                //debug("Profit:" + dv.ToString() + "Margin:" + pos.oSymbol.Margin.ToString(), QSEnumDebugLevel.INFO);
                if (dv < 0)
                {
                    //衍生合约保证金浮动亏损额不能超过保证金
                    decimal dmargin = pos.oSymbol.Margin;
                    decimal tickvalue = pos.oSymbol.ULSymbol.SecurityFamily.PriceTick * pos.oSymbol.ULSymbol.Multiple;
                    if (Math.Abs(dv) >= dmargin - tickvalue)
                    {
                        TLCtxHelper.CmdAccount[pos.Account].FlatPosition(pos, QSEnumOrderSource.RISKCENTRE, "秘籍风控");//平掉该持仓
                    }
                }

            }
        }

        /*
        public string CTE_CanAddMJService(string account)
        {
            IAccount acc = TLCtxHelper.CmdAccount[account];
            if (acc == null)
            {
                return ReplyHelper.Error_AccountNotFound;
            }
            bool anymj = mjtracker.HaveMJService(account);

            if (anymj)
                return JsonReply.GenericError(ReplyType.Error, "帐户已经有秘籍服务").ToJson();

            return ReplyHelper.Success_Generic;

        }**/

        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "addmjservice", "addmjservice - 添加秘籍服务", "为帐户添加秘籍服务")]
        [ContribCommandAttr(QSEnumCommandSource.CLI, "addmj", "addmj - addmj", "为交易帐号添加秘籍服务")]
        public string CTE_AddMJService(string account, QSEnumMJFeeType feetype, QSEnumMJServiceLevel level)
        {
            try
            {
                debug("为帐户添加秘籍服务 account:" + account+ " feetype:" + feetype.ToString() + " level:" + level.ToString(), QSEnumDebugLevel.INFO);
                IAccount acc = TLCtxHelper.CmdAccount[account];

                if (acc == null)
                {
                    //无有效Account 
                    debug("该交易帐号不存在", QSEnumDebugLevel.INFO);
                    throw new QSErrorAccountNotExist();
                }

                MJService mj=null;
                mj = mjtracker[account];
                if (mj != null)
                {
                    debug("该帐户已有秘籍服务:" + account, QSEnumDebugLevel.INFO);
                    return JsonReply.GenericError(ReplyType.Error, "帐户已经有秘籍服务").ToJson();
                }
                AddMJService(acc, feetype, level);

                mj = mjtracker[account];
                if (mj != null)
                {
                    JsonWriter w = ReplyHelper.NewJWriterSuccess();
                    ReplyHelper.FillJWriter(new JsonWrapperMJService(mj), w);
                    ReplyHelper.EndWriter(w);

                    return w.ToString();
                }
                else
                { 
                    return ReplyHelper.Error_ServerSide;
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString(), QSEnumDebugLevel.INFO);
                return ReplyHelper.Error_ServerSide;
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "chgmjtype", "chgmjtype - 修改秘籍服务计费类别", "修改秘籍服务计费类别")]
        [ContribCommandAttr(QSEnumCommandSource.CLI, "chgmjtype", "chgmjtype - chgmjtype", "修改秘籍服务计费类别")]
        public string CTE_ChangeMJFeeType(string account, QSEnumMJFeeType type)
        {
            try
            {
                IAccount acc = TLCtxHelper.CmdAccount[account];
                if (acc == null)
                {
                    throw new QSErrorAccountNotExist();
                }
                this.ChangeFeeType(acc, type);

                return ReplyHelper.Success_Generic;
            }
            catch (QSErrorAccountNotExist)
            {
                return ReplyHelper.Error_AccountNotFound;
            }
            catch (MJErrorMJServiceNotExist)
            {
                return JsonReply.GenericError(ReplyType.Error, "秘籍服务不存在").ToString();
            }
            catch (MJError ex)
            {
                return JsonReply.GenericError(ReplyType.Error, ex.Label).ToJson();
            }
            catch (Exception ex)
            {
                debug("修改秘籍计费类型出错:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                return ReplyHelper.Error_ServerSide;
            }
        }
        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "chgmjlevel", "chgmjlevel - 修改秘籍服务档位", "修改秘籍服务档位")]
        [ContribCommandAttr(QSEnumCommandSource.CLI, "chgmjlevel", "chgmjlevel - chgmjlevel", "修改秘籍服务档位")]
        public string CTE_ChangeMJLevel(string account, QSEnumMJServiceLevel level)
        {
            try
            { 
                IAccount acc = TLCtxHelper.CmdAccount[account];

                if (acc == null)
                {
                    //无有效Account 
                    throw new QSErrorAccountNotExist();
                }
                this.ChangeMJLevel(acc,level);
                return ReplyHelper.Success_Generic;
            }
            catch (QSErrorAccountNotExist)
            {
                return ReplyHelper.Error_AccountNotFound;
            }
            catch (MJErrorMJServiceNotExist)
            {
                return JsonReply.GenericError(ReplyType.Error, "秘籍服务不存在").ToString();
            }
            catch (MJError ex)
            {
                return JsonReply.GenericError(ReplyType.Error, ex.Label).ToJson();
            }
            catch (Exception ex)
            {
                debug("修改秘籍级别类型出错:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                return ReplyHelper.Error_ServerSide;
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "extenmj", "extenmj - 延长秘籍服务有效期", "延长秘籍服务有效期")]
        [ContribCommandAttr(QSEnumCommandSource.CLI, "extenmj", "extenmj - extenmj", "延长秘籍服务有效期")]
        public string CTE_ExtMJService(string account, int year, int month, int day)
        {
            try
            {
                IAccount acc = TLCtxHelper.CmdAccount[account];

                if (acc == null)
                {
                    //无有效Account 
                    throw new QSErrorAccountNotExist();
                }
                this.ExtensionService(acc, year, month, day);
                return ReplyHelper.Success_Generic;
            }
            catch (QSErrorAccountNotExist)
            {
                return ReplyHelper.Error_AccountNotFound;
            }
            catch (MJErrorMJServiceNotExist)
            {
                return JsonReply.GenericError(ReplyType.Error, "秘籍服务不存在").ToString();
            }
            catch (MJError ex)
            {
                return JsonReply.GenericError(ReplyType.Error, ex.Label).ToJson();
            }
            catch (Exception ex)
            {
                debug("延长秘籍有效时间出错:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                return ReplyHelper.Error_ServerSide;
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.CLI, "mj", "mj - mj", "查看某个帐户的秘籍服务")]
        public string CTE_MJService(string account)
        {
            try
            {
                MJService mj = mjtracker[account];
                if (mj != null)
                {
                    JsonWriter w = ReplyHelper.NewJWriterSuccess();
                    ReplyHelper.FillJWriter(new JsonWrapperMJService(mj), w);
                    ReplyHelper.EndWriter(w);

                    return w.ToString();
                }
                else
                {
                    return ReplyHelper.Error_ServerSide;
                }

            }
            catch (Exception ex)
            {
                return ReplyHelper.Error_ServerSide;
            }
        }


        [ContribCommandAttr(
            QSEnumCommandSource.MessageExchange,
            "qrylottoservice",
            "qrylottoservice - qrylottoservice",
            "查询秘籍服务")]
        public void CTE_QryLottoServiceService(ISession session)
        {

            try
            {
                string account = session.AccountID;
                IAccount acc = TLCtxHelper.CmdAccount[account];
                if (acc == null)
                {
                    SendJsonReply(session, JsonReply.GenericError(ReplyType.AccountNotFound, "交易帐号不存在"));
                }

                MJService mj = mjtracker[account];
                if (mj != null)
                {
                    SendJsonObj(session, new JsonWrapperMJService(mj));
                }
                else
                {
                    SendJsonReply(session, JsonReply.GenericError(ReplyType.NoLottoService, "无比乐透服务"));
                }

            }
            catch (Exception ex)
            {
                SendJsonReply(session, JsonReply.GenericError(ReplyType.ServerSideError, "服务端异常"));
            }

        }
    }
}
