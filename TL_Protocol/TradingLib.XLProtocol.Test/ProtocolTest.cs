using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using TradingLib.XLProtocol;
using NUnit.Framework;

namespace TradingLib.XLProtocol.Test
{
    [TestFixture]
    public class ProtocolTest
    {
        [Test]
        public void PacketDataTest()
        {
            XLPacketData packet = new XLPacketData(XLMessageType.T_REQ_LOGIN);
            V1.XLReqLoginField req = new V1.XLReqLoginField();
            req.UserID = "850001";
            req.Password = "123456";
            req.UserProductInfo = "POBO540";
            req.MacAddress = "XXXXXXX";
            int fieldSize = Marshal.SizeOf(req);
            packet.AddField(req);

            XLEnumSeqType seqType = XLEnumSeqType.SeqReq;
            uint seqNo = 100;
            uint requestID = 101;
            bool isLast = true;
            //将PktData打包成byte数组
            byte[] ret = XLPacketData.PackToBytes(packet, seqType, seqNo, requestID, isLast);

            //解析协议头4个字节
            XLProtocolHeader protoHeader = XLStructHelp.BytesToStruct<XLProtocolHeader>(ret, 0);
            Console.WriteLine(string.Format("MessageType:{0} XLProtoLen:{1}", protoHeader.XLMessageType, protoHeader.XLMessageLength));
            Assert.AreEqual((short)(XLMessageType.T_REQ_LOGIN),protoHeader.XLMessageType);
            Assert.AreEqual(XLConstants.DATA_HEADER_LEN + XLConstants.FIELD_HEADER_LEN + fieldSize, protoHeader.XLMessageLength);//协议头中的数据长度 不包含头长度4个字节

            //通过反序列化 获得PktData 还原数据域
            XLDataHeader dataHeader;
            XLPacketData pktout = XLPacketData.Deserialize(XLMessageType.T_REQ_LOGIN, ret, XLConstants.PROTO_HEADER_LEN,out dataHeader);
            Assert.AreEqual(1, pktout.FieldList.Count);
            Console.WriteLine(string.Format("Enc:{0} Ver:{1} IsLast:{2} SeqType:{3} SeqNo:{4} RequestID:{5} FiCnt:{6} FiLen:{7}", dataHeader.Enctype, dataHeader.Version, dataHeader.IsLast, dataHeader.SeqType, dataHeader.SeqNo, dataHeader.RequestID, dataHeader.FieldCount, dataHeader.FieldLength));
            Assert.AreEqual(XLConstants.XL_ENC_NONE, dataHeader.Enctype);
            Assert.AreEqual(1, dataHeader.FieldCount);
            Assert.AreEqual(XLConstants.FIELD_HEADER_LEN + fieldSize, dataHeader.FieldLength);
            Assert.AreEqual((byte)1, dataHeader.IsLast);
            Assert.AreEqual(requestID, dataHeader.RequestID);
            Assert.AreEqual(seqNo, dataHeader.SeqNo);
            Assert.AreEqual(seqType, (XLEnumSeqType)dataHeader.SeqType);
            Assert.AreEqual(XLConstants.XL_VER_1, dataHeader.Version);

            //域头
            Assert.AreEqual(fieldSize, pktout.FieldList[0].FieldHeader.FieldLength);
            Assert.AreEqual(XLFieldType.F_REQ_LOGIN, (XLFieldType)pktout.FieldList[0].FieldHeader.FieldID);
            //域
            IXLField field = pktout.FieldList[0].FieldData;
            //Console.WriteLine(string.Format("Type:{0}", field.GetType()));
            Assert.AreEqual(true, field is V1.XLReqLoginField);
            V1.XLReqLoginField reqout = (V1.XLReqLoginField)field;
            Assert.AreEqual(req.UserID, reqout.UserID);
            Assert.AreEqual(req.Password, reqout.Password);
            Assert.AreEqual(req.UserProductInfo, reqout.UserProductInfo);
            Assert.AreEqual(req.MacAddress, reqout.MacAddress);

        }
    }
}
