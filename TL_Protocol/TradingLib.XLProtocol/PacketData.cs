using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TradingLib.XLProtocol
{
    /// <summary>
    /// 业务结构体数据包
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class XLFieldData<T>
        where T:IXLField
    {
        /// <summary>
        /// 业务数据域头
        /// </summary>
        public XLFieldHeader FieldHeader;

        /// <summary>
        /// 业务数据域
        /// </summary>
        public T FieldData;
    }


    public class XLPacketData
    {

        public XLPacketData(XLMessageType msgType)
        {
            _messageType = msgType;
        }

        public XLPacketData(XLMessageType msgType, List<IXLField> fields)
        {
            _messageType = msgType;
            _fieldList.AddRange(fields);
        }

        XLMessageType _messageType = XLMessageType.T_HEARTBEEAT;
        /// <summary>
        /// 消息类别
        /// </summary>
        public XLMessageType MessageType { get { return _messageType; } }



        List<IXLField> _fieldList = new List<IXLField>();
        /// <summary>
        /// 业务数据结
        /// </summary>
        public List<IXLField> FieldList { get { return _fieldList; } }


        /// <summary>
        /// 添加一个业务数据域
        /// </summary>
        /// <param name="field"></param>
        public void AddField(IXLField field)
        {
            //XLFieldHeader header = new XLFieldHeader();
            //int fieldLen = Marshal.SizeOf(field);
            //FillFieldHeader(ref header, field.FieldID, (ushort)fieldLen);

            _fieldList.Add(field);// (new XLFieldData<IXLField>() { FieldHeader = header, FieldData = field });
        }

       


        #region 二进制 序列化与反序列化
        /// <summary>
        /// 将业务数据包打包成byte数组
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public static byte[] PackToBytes(XLPacketData packet,XLEnumSeqType seqType,uint seqNo,uint requestID,bool isLast)
        {
            XLProtocolHeader protoHeader = new XLProtocolHeader();
            XLDataHeader dataHeader = new XLDataHeader();
            List<XLFieldHeader> hdList = new List<XLFieldHeader>();
            ushort fieldLength = 0;
            foreach (var field in packet.FieldList)
            {  
                XLFieldHeader header = new XLFieldHeader();
                int fieldLen = Marshal.SizeOf(field);
                FillFieldHeader(ref header, field.FieldID, (ushort)fieldLen);
                hdList.Add(header);
                fieldLength += (ushort)(header.FieldLength + XLConstants.FIELD_HEADER_LEN);
            }

            //ushort fieldLength = (ushort)packet.FieldList.Sum(d => (d.FieldHeader.FieldLength + XLConstants.FIELD_HEADER_LEN));
            ushort pktLen = (ushort)(XLConstants.PROTO_HEADER_LEN + XLConstants.DATA_HEADER_LEN + fieldLength);
            ushort fieldCount = (ushort)packet.FieldList.Count;
            FillProtoHeader(ref protoHeader, packet.MessageType, pktLen);

            FillDataHeader(ref dataHeader, seqType, seqNo, fieldCount, pktLen, requestID, isLast);

            Byte[] data = new Byte[pktLen];

            int offset = 0;
            Array.Copy(XLStructHelp.StructToBytes<XLProtocolHeader>(protoHeader), 0, data, 0, XLConstants.PROTO_HEADER_LEN);
            offset += XLConstants.PROTO_HEADER_LEN;
            Array.Copy(XLStructHelp.StructToBytes<XLDataHeader>(dataHeader), 0, data, offset, XLConstants.DATA_HEADER_LEN);
            offset += XLConstants.DATA_HEADER_LEN;

            int i = 0;
            //遍历所有业务数据域 转换成byte数组
            foreach (var field in packet.FieldList)
            {
                Array.Copy(XLStructHelp.StructToBytes<XLFieldHeader>(hdList[i]), 0, data, offset, XLConstants.FIELD_HEADER_LEN);
                Array.Copy(XLStructHelp.StructToBytes(field), 0, data, offset + XLConstants.FIELD_HEADER_LEN, hdList[i].FieldLength);
                offset += (XLConstants.FIELD_HEADER_LEN + hdList[i].FieldLength);
                i++;
              }

            return data;
        }

        /// <summary>
        /// 从byte数组中解析出PacketData
        /// </summary>
        /// <param name="type">协议头所获得的消息类型</param>
        /// <param name="data">数据</param>
        /// <param name="offset">数据偏移量 此处数据从数据正文开始(不包含协议头4个字节数据)</param>
        /// <returns></returns>
        public static XLPacketData Deserialize(XLMessageType type,byte[] data,int offset,out XLDataHeader dataHeader)
        { 
            int _offset = offset;
            dataHeader = XLStructHelp.BytesToStruct<XLDataHeader>(data, _offset);
            _offset += XLConstants.DATA_HEADER_LEN;

            List<IXLField> list = new List<IXLField>();
            for (int i = 0; i < dataHeader.FieldCount; i++)
            {
                XLFieldHeader fieldHeader = XLStructHelp.BytesToStruct<XLFieldHeader>(data, _offset);
                XLFieldType fieldType = (XLFieldType)fieldHeader.FieldID;
                IXLField fieldData = null;
                switch (dataHeader.Version)
                {
                    case XLConstants.XL_VER_1:
                        fieldData = V1.StructHelp.BytesToStruct(data, _offset + XLConstants.FIELD_HEADER_LEN, fieldType);
                        break;
                    default:
                        throw new Exception(string.Format("Version:{0} not supported", dataHeader.Version));
                }
                list.Add(fieldData);// (new XLFieldData<IXLField> { FieldHeader = fieldHeader, FieldData = fieldData });
                _offset += XLConstants.FIELD_HEADER_LEN + fieldHeader.FieldLength;
            }

            return new XLPacketData(type, list);
        }
        #endregion




        /// <summary>
        /// 将业务数据包打包成Json字符串
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public static string PackJsonRequest(object request)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(request);
        }



        public static string PackJsonNotify(XLPacketData pkt)
        { 
            object notify = null;
            switch (pkt.MessageType)
            {
                case XLMessageType.T_RTN_ORDER:
                    {
                        notify = new JsonNotify(pkt.MessageType, (V1.XLOrderField)pkt.FieldList[0]);
                        break;
                    }
                case XLMessageType.T_RTN_TRADE:
                    {
                        notify = new JsonNotify(pkt.MessageType, (V1.XLTradeField)pkt.FieldList[0]);
                        break;
                    }
                case XLMessageType.T_RTN_POSITIONUPDATE:
                    {
                        notify = new JsonNotify(pkt.MessageType, (V1.XLPositionField)pkt.FieldList[0]);
                        break;
                    }
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(notify);
        }
        /// <summary>
        /// 将回报消息打包成Json字符串
        /// </summary>
        /// <param name="pkt"></param>
        /// <param name="requestID"></param>
        /// <param name="isLast"></param>
        /// <returns></returns>
        public static string PackJsonResponse(XLPacketData pkt,int requestID,bool isLast)
        {
            object response = null;
            switch (pkt.MessageType)
            {
                case XLMessageType.T_RSP_ERROR:
                    {
                        response = new JsonResponse(pkt.MessageType, (ErrorField)pkt.FieldList[0], null , requestID, isLast);
                        break;
                    }
                case XLMessageType.T_RSP_LOGIN://登入回报
                    {
                        response = new JsonResponse(pkt.MessageType, (ErrorField)pkt.FieldList[0], (V1.XLRspLoginField)pkt.FieldList[1], requestID, isLast);
                        break;
                    }
                case XLMessageType.T_RSP_SYMBOL://查询合约回报
                    {
                        response = new JsonResponse(pkt.MessageType,null, (V1.XLSymbolField)pkt.FieldList[0], requestID, isLast);
                        break;
                    }
                case XLMessageType.T_RSP_ORDER://查询委托回报
                    {
                        response = new JsonResponse(pkt.MessageType, null, (V1.XLOrderField)pkt.FieldList[0], requestID, isLast);
                        break;
                    }
                case XLMessageType.T_RSP_TRADE://查询成交回报
                    {
                        response = new JsonResponse(pkt.MessageType, null, (V1.XLTradeField)pkt.FieldList[0], requestID, isLast);
                        break;
                    }
                case XLMessageType.T_RSP_POSITION://查询持仓回报
                    {
                        response = new JsonResponse(pkt.MessageType, null, (V1.XLPositionField)pkt.FieldList[0], requestID, isLast);
                        break;
                    }
                case XLMessageType.T_RSP_ACCOUNT://查询账户回报
                    {
                        response = new JsonResponse(pkt.MessageType, null, (V1.XLTradingAccountField)pkt.FieldList[0], requestID, isLast);
                        break;
                    }
                case XLMessageType.T_RSP_UPDATEPASS://更新密码回报
                    {
                        response = new JsonResponse(pkt.MessageType, null, (V1.XLRspUserPasswordUpdateField)pkt.FieldList[1], requestID, isLast);
                        break;
                    }
                case XLMessageType.T_RSP_MAXORDVOL://查询最大下单手数回报
                    {
                        response = new JsonResponse(pkt.MessageType, null, (V1.XLQryMaxOrderVolumeField)pkt.FieldList[0], requestID, isLast);
                        break;
                    }
                case XLMessageType.T_RSP_INSERTORDER://提交委托异常回报
                    {
                        response = new JsonResponse(pkt.MessageType, (ErrorField)pkt.FieldList[0], (V1.XLInputOrderField)pkt.FieldList[1], requestID, isLast);
                        break;
                    }
                case XLMessageType.T_RSP_ORDERACTION://提交委托操作异常回报
                    {
                        response = new JsonResponse(pkt.MessageType, (ErrorField)pkt.FieldList[0], (V1.XLInputOrderActionField)pkt.FieldList[1], requestID, isLast);
                        break;
                    }
                default:
                    break;

            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(response);
        
        }
         
        /// <summary>
        /// 从Json字符串 反序列化出来XLPacketData
        /// </summary>
        /// <param name="type"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public static XLPacketData DeserializeJsonRequest(XLMessageType msgType, string json,out int requestID)
        {
            requestID = 0;
            switch (msgType)
            {
                case XLMessageType.T_REQ_LOGIN:
                    {
                        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonRequest<V1.XLReqLoginField>>(json);
                        requestID = data.RequestID;
                        return new XLPacketData(msgType, new List<IXLField>() { data.Request });
                    }
                case XLMessageType.T_QRY_SYMBOL://查合约
                    {
                        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonRequest<V1.XLQrySymbolField>>(json);
                        requestID = data.RequestID;
                        return new XLPacketData(msgType, new List<IXLField>() { data.Request });
                    }
                case XLMessageType.T_QRY_ORDER://查委托
                    {
                        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonRequest<V1.XLQryOrderField>>(json);
                        requestID = data.RequestID;
                        return new XLPacketData(msgType, new List<IXLField>() { data.Request });
                    }
                case XLMessageType.T_QRY_TRADE://查成交
                    {
                        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonRequest<V1.XLQryTradeField>>(json);
                        requestID = data.RequestID;
                        return new XLPacketData(msgType, new List<IXLField>() { data.Request });
                    }
                case XLMessageType.T_QRY_POSITION://查持仓
                    {
                        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonRequest<V1.XLQryPositionField>>(json);
                        requestID = data.RequestID;
                        return new XLPacketData(msgType, new List<IXLField>() { data.Request });
                    }
                case XLMessageType.T_QRY_ACCOUNT://查账户
                    {
                        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonRequest<V1.XLQryTradingAccountField>>(json);
                        requestID = data.RequestID;
                        return new XLPacketData(msgType, new List<IXLField>() { data.Request });
                    }
                case XLMessageType.T_REQ_UPDATEPASS://修改账户密码
                    {
                        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonRequest<V1.XLReqUserPasswordUpdateField>>(json);
                        requestID = data.RequestID;
                        return new XLPacketData(msgType, new List<IXLField>() { data.Request });
                    }
                case XLMessageType.T_QRY_MAXORDVOL://查询最大报单
                    {
                        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonRequest<V1.XLQryMaxOrderVolumeField>>(json);
                        requestID = data.RequestID;
                        return new XLPacketData(msgType, new List<IXLField>() { data.Request });
                    }
                case XLMessageType.T_REQ_INSERTORDER://提交委托
                    {
                        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonRequest<V1.XLInputOrderField>>(json);
                        requestID = data.RequestID;
                        return new XLPacketData(msgType, new List<IXLField>() { data.Request });
                    }
                case XLMessageType.T_REQ_ORDERACTION://提交委托操作
                    {
                        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonRequest<V1.XLInputOrderActionField>>(json);
                        requestID = data.RequestID;
                        return new XLPacketData(msgType, new List<IXLField>() { data.Request });
                    }
            }
            return null;
        }


        #region 填充 头字段
        /// <summary>
        /// 填充协议头
        /// </summary>
        /// <param name="header"></param>
        /// <param name="messageType"></param>
        /// <param name="pktLen"></param>
        static void FillProtoHeader(ref XLProtocolHeader header, XLMessageType messageType, ushort pktLen)
        {
            header.XLMessageType = (short)messageType;
            header.XLMessageLength = (ushort)(pktLen - XLConstants.PROTO_HEADER_LEN);//数据长度 = 总长度 - 协议头长度
        }

        /// <summary>
        /// 填充正文头
        /// </summary>
        /// <param name="header"></param>
        /// <param name="isLast"></param>
        /// <param name="seqType"></param>
        /// <param name="seqNo"></param>
        /// <param name="requestId"></param>
        /// <param name="fieldCount"></param>
        /// <param name="pktlen"></param>
        static void FillDataHeader(ref XLDataHeader header, XLEnumSeqType seqType, uint seqNo, ushort fieldCount, ushort pktlen, uint requestId, bool isLast)
        {
            header.Enctype = XLConstants.XL_ENC_NONE;
            header.Version = XLConstants.XL_VER_1;
            header.IsLast = (byte)(isLast?1:0);
            header.SeqType = (byte)seqType;
            header.SeqNo = seqNo;
            header.RequestID = requestId;
            header.FieldCount = fieldCount;
            header.FieldLength = (ushort)(pktlen - XLConstants.PROTO_HEADER_LEN - XLConstants.DATA_HEADER_LEN);
            
        }
        /// <summary>
        /// 填充域头
        /// </summary>
        /// <param name="header"></param>
        /// <param name="fieldType"></param>
        /// <param name="fieldLen"></param>
        static void FillFieldHeader(ref XLFieldHeader header,ushort fieldID,ushort fieldLen)
        {
            header.FieldID = fieldID;
            header.FieldLength = fieldLen;
        }
        #endregion

    }

    


}
