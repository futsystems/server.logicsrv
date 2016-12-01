using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using Common.Logging;
using CTPService;
using CTPService.Struct.V12;
using CTPService.Struct;


namespace FrontServer
{
    class Program
    {

        static void Main(string[] args)
        {
            ILog logger = LogManager.GetLogger("Main");

            if (false)
            {
                //CTP登入回报
                string hex = "0200008501030c4ce43001e502e1f5e301e355e4d5fdc8b7efefefefefe21003e1983230313631323031e131323a32303a3337e13838383838e638353632303030383032e654726164696e67486f7374696e67efef04a515665831ec31323a32303a3337e131323a32303a3338e131323a32303a3338e131323a32303a3337e12d2d3a2d2d3a2d2de1";
                //CTP请求查询投资者
                hex = "0200003201000c4c000400008008000000010001001c0000000207040018383838383800000000000038353632303030383032000000";
                //数据流Topic设定回报
                hex = "0200002a01030c4ce4f102e504e128e41001e106e101e41001e106e102e41001e106e103e202121001e106e104e4";
                //查询投资者回报
                hex = "0200008d01030c4ce104e28009e301e101018ce302e106018838353632303030383032e33838383838efe4c7aeb2a8efefefefefe231333230353832313938333034313934353134efefe6013138303531313336313637efefc9ccb3c7c2b7333431bac5d7cfb9e0e2b4f3cfc332323033cad2efefefefefe13230313230393137e13138303531313336313637efefefeb";
                byte[] srcData = ByteUtil.HexToByte(hex);
                logger.Info("**** remtoe compressed data size" + srcData.Length.ToString());
                string rawhexcompressed = ByteUtil.ByteToHex(srcData, ' ');
                logger.Info(rawhexcompressed);

                int dstLen = srcData.Length * 3;
                byte[] dstData = new byte[dstLen];

                Type rspType = typeof(LCThostFtdcRspInfoField);
                int rspSize = Marshal.SizeOf(rspType);
                byte enc = srcData[5];

                int offset = 0;
                if (enc == 3)
                {
                    //解码数据包
                    StructHelperV12.LZ_Uncompress(ref dstData, ref dstLen, srcData, srcData.Length);
                }
                else
                {
                    dstData = srcData;

                }

                //从数据包加载数据
                proto_hdr tmp0 = ByteSwapHelp.BytesToStruct<proto_hdr>(dstData, 0);
                
                ftd_hdr tmp1 = ByteSwapHelp.BytesToStruct<ftd_hdr>(dstData, 4);
                EnumTransactionID transid = (EnumTransactionID)tmp1.dTransId;

                ftdc_hdr tmp2 = ByteSwapHelp.BytesToStruct<ftdc_hdr>(dstData, offset + Constanst.PROFTD_HDRLEN);
                EnumFiledID fieldid = (EnumFiledID)tmp2.wFiId;
                LCThostFtdcInvestorField tmp3 = ByteSwapHelp.BytesToStruct<LCThostFtdcInvestorField>(dstData, offset + Constanst.PROFTD_HDRLEN + Constanst.FTDC_HDRLEN);

                //offset += Constanst.FTDC_HDRLEN + rspSize;
                //ftdc_hdr tmp4 = ByteSwapHelp.BytesToStruct<ftdc_hdr>(dstData, offset + Constanst.PROFTD_HDRLEN);
                //EnumFiledID fieldid2 = (EnumFiledID)tmp2.wFiId;
                //LCThostFtdcRspUserLoginField tmp5 = ByteSwapHelp.BytesToStruct<LCThostFtdcRspUserLoginField>(dstData, offset + Constanst.PROFTD_HDRLEN + Constanst.FTDC_HDRLEN);

                logger.Info("**** remote raw data size" + dstLen.ToString());
                string rawhex = ByteUtil.ByteToHex(dstData, ' ', dstLen);
                logger.Info(rawhex);

                //将业务数据用本地方法打包
                byte[] data1 = StructHelperV12.PackRsp<LCThostFtdcInvestorField>(ref tmp3, EnumSeqType.SeqQry, EnumTransactionID.T_RSP_USRINF, (int)tmp1.dReqId,(int)tmp1.dSeqNo);
                logger.Info("**** local raw data size:" + data1.Length.ToString());
                logger.Info(ByteUtil.ByteToHex(data1, ' '));

                //将数据进行编码
                int dstLen2 = 0;
                byte[] data2 = StructHelperV12.EncPkt(data1, out dstLen2);
                logger.Info("**** local compressed data size:" + dstLen2.ToString());
                string localhexcompressed = ByteUtil.ByteToHex(data2, ' ', dstLen2);
                logger.Info(localhexcompressed);


                int dstLen3 = srcData.Length * 3;
                byte[] dstData3 = new byte[dstLen];
                StructHelperV12.LZ_Uncompress(ref dstData3, ref dstLen3, data2, dstLen2);

                proto_hdr tmp01 = ByteSwapHelp.BytesToStruct<proto_hdr>(dstData3, 0);
                ftd_hdr tmp11 = ByteSwapHelp.BytesToStruct<ftd_hdr>(dstData3, 4);
                string localrawhex = ByteUtil.ByteToHex(dstData3, ' ', dstLen3);

                logger.Info("Compressed     OK:" + (rawhexcompressed == localhexcompressed).ToString());
                logger.Info("Row            OK:" + (rawhex == localrawhex).ToString());
                return;
            }
            /*
            byte[] demo = new byte[] { 0x02, 00, 00, 08, 04, 0xec, 00, 00, 00, 00 };
            int pktLen = demo.Length*2;
            byte[] encData = new byte[pktLen];
            StructHelperV12.LZ_Compress(ref encData,ref pktLen, demo,demo.Length);

            

            logger.Info("Raw Bytes:" + ByteUtil.ByteToHex(demo, ' '));
            logger.Info("Enc Bytes:" + ByteUtil.ByteToHex(encData, ' ',pktLen));

            byte b = 0xe0;
            logger.Info("Byte:" + ByteUtil.ByteToHex(b));

            byte r = 0xe1 & 0xf0;
            logger.Info("result:" + ByteUtil.ByteToHex(r));

            byte c = 224 + 5;
            logger.Info("c:" + ByteUtil.ByteToHex(c));

            byte d = 239;
            logger.Info("d=239 " + ((int)d == 239));


            byte[] dst = new byte[demo.Length];
            int dstLen = 0;
            StructHelperV12.LZ_Compress(ref dst, ref dstLen, demo, demo.Length);

            logger.Info("result:" + ByteUtil.ByteToHex(dst, ' ',dstLen));

            //int size = Marshal.SizeOf(CTPService.FTDC_Header);
             * */
            MQServer mqServer = new MQServer();

            CTPServiceHost host = new CTPServiceHost(mqServer);
            mqServer.Start();
            host.Start();

            System.Threading.Thread.Sleep(int.MaxValue);
        }
    }
}
