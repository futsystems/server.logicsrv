﻿using System;
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
                EnumTransactionID x = (EnumTransactionID)32854;
                logger.Info("TID:" + x.ToString());
                //return;

                //CTP登入回报
                string hex = "0200008501030c4ce43001e502e1f5e301e355e4d5fdc8b7efefefefefe21003e1983230313631323031e131323a32303a3337e13838383838e638353632303030383032e654726164696e67486f7374696e67efef04a515665831ec31323a32303a3337e131323a32303a3338e131323a32303a3338e131323a32303a3337e12d2d3a2d2d3a2d2de1";
                //CTP请求查询投资者
                //hex = "0200003201000c4c000400008008000000010001001c0000000207040018383838383800000000000038353632303030383032000000";
                //数据流Topic设定回报
                //hex = "0200002a01030c4ce4f102e504e128e41001e106e101e41001e106e102e41001e106e103e202121001e106e104e4";
                //查询投资者回报
                //hex = "0200008d01030c4ce104e28009e301e101018ce302e106018838353632303030383032e33838383838efe4c7aeb2a8efefefefefe231333230353832313938333034313934353134efefe6013138303531313336313637efefc9ccb3c7c2b7333431bac5d7cfb9e0e2b4f3cfc332323033cad2efefefefefe13230313230393137e13138303531313336313637efefefeb";
                //查询结算确认信息
                //hex = "0200003201000c4c000400008056000000020001001c0000000424580018383838383800000000000038353632303030383032000000";
                //查询结算确认信息回报
                hex = "0200000d01030c4ce104e28057e302e704";//"0200000d01030c4ce104e28057e302e70449";
                //hex = "0200000d01030c4ce104e28057e302e704d7";

                //查询通知回报
                hex = "0200021c01000c430004000080550000000400010206000000112456020238383838380000000000003c703ed7f0beb4b5c4bfcdbba7a3bad6d0b9fabda8c9e8d2f8d0d0bdabd3da32303136c4ea3131d4c23330c8d53234b5e3a3a8bcb43132d4c231c8d5c1e8b3bf30b5e3a3a9d6c132303136c4ea3132d4c231c8d533b5e3cda3d6b9d2b9c5ccb7fecef133d0a1cab1a3bbd6d0b9fac5a9d2b5d2f8d0d0bdabd3da32303136c4ea3131d4c23330c8d53232a3ba35302d3131d4c23330c8d53233a3ba3230d4ddcda3d2f8c6dad7aad5cba1a2b5e7d7d3b3f6c8ebbdf0cfb5cdb3b7fecef1a3ac3131d4c23330c8d53232a3ba35302d3132d4c231c8d53032a3ba3030d4ddcda3d2f8c6dad7aad5cbcdf8d2f8c7feb5c0b7fecef1a3bbbdbbcda8d2f8d0d0d3da32303136c4ea3131d4c23330c8d53231a3ba33302d3132d4c231c8d53033a3ba3030bdf8d0d0cfb5cdb3c9fdbcb6a3bbb9e2b4f3d2f8d0d0bdabd3da32303136c4ea3131d4c23330c8d53233b5e3c6f0d6c1d2b9c5ccbde1caf8bdf8d0d0cfb5cdb3c9fdbcb6a1a33c2f703e3c703ed7f0beb4b5c4bfcdbba7a3ba3c2f703e3c703e266e6273703b266e6273703b20266e6273703b20266e6273703bb7b3c7ebb2e9bfb4b9abcbbecfe0b9d8cdf8d5beb9abb8e6a1a3b5d8d6b7c8e7cfc2a3ba3c6120687265663d2671756f742671756f743b26616d703b71756f74687474703a2f2f7777772e7379776771682e636f6d2f3f703d003000";

                //查询TDNotice
                hex = "0200003201000c4c000400008108000000050001001c00000016248600183838383838000000000000383536323030303830";

                //结算单
                hex = "0200043301030c43e104e2803de309e1020444e317e11b021eed3838383838e638353632303030383032e72020202020202020202020202020202020202020202020202020202020202020202020202020202020202020c9e0ead2f8cdf2b9fac6dabbf520202020202020202020202020202020202020202020202020202020202020202020202020202020202020200d0a2020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020d6c6b1e0edcab1bce0e4204372656174696f6e2044617465a3ba32303136313131350d0a2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d0d0a2020202020202020202020202020202020202020202020202020202020bdbbd2d7bde0e1cbe0e3b5a528b6a2cad02920536574746c656d656e742053746174656d656e74284d544d2920202020202020202020202020202020202020202020202020202020200d0abfcdbba7bac520436c69656e74204944a3ba20203835363230303038303220202020202020202020bfcdbba7c3fbb3c620436c69656e74204e616d65a3bac7aeb2a80d0ac8d5c6da2044617465a3ba32303136313131350d0a0d0a0de21b021eed3838383838e638353632303030383032e70acad0b3a1b7e0e7cfd5c4aab2e0e2a3accef1c7e0ebbdf7c9f7b4d3cac2a1a30d0a0d0a20202020202020202020202020202020202020d7cabdf0d7b4bff62020b1d2d6d6a3bac8cbc3f1b1d220204163636f756e742053756d6d617279202043757272656e6379a3ba434e59200d0a2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d0d0ac6dab3f5bde0e1b4e0e62042616c616e636520622f66a3ba202020202020202020202020202020202020202020383432332e30302020bbf9b4a1b1a3d6a4bdf020496e697469616c204d617267696ea3ba2020202020202020202020202020202020302e30300d0ab3f620c8e0eb20bdf0204465706f7369742f5769746864726177616ca3ba202020202020202020202020202d363530302e30302020c6dac4a9bde0e1b4e0e62042616c616e636520632f66a3ba20202020202020202020202020202020202020313932332e30300d0ac6bdb2d6d3afbff7205265616c697a656420502f4ca3ba2020202020202020202020202020202020202020202020302e30302020d6ca20d1ba20bdf020506c6564676520416d6f756e74a3ba202020202020202020e1";
                hex = "0200043001030c43e104e2803de30ae1020444e317e11b021eed3838383838e638353632303030383032e72020202020202020202020302e30300d0ab3d6b2d6b6a2cad0d3afbff7204d544d20502f4ca3ba202020202020202020202020202020202020202020202020302e30302020bfcdbba7c8a8d2e0e620436c69656e7420457175697479a3baa3ba202020202020202020202020202020313932332e30300d0ac6dac8a8d6b4d0d0d3afbff720457865726369736520502f4ca3ba20202020202020202020202020202020202020302e30302020bbf5b1d2d6cad1bab1a3d6a4bdf0d5bcd3c320465820506c65646765204f63632ea3ba202020202020202020302e30300d0acad620d0f820b7d120436f6d6d697373696f6ea3ba20202020202020202020202020202020202020202020202020302e30302020b1a3d6a4bdf0d5bcd3c3204d617267696e204f63637570696564a3ba20202020202020202020202020202020302e30300d0ad0d0c8a8cad6d0f8b7d120457865726369736520466565a3ba202020202020202020202020202020202020202020302e30302020bdbbb8e0eeb1a3d6a4bdf02044656c6976657279204d617267696ea3ba20202020202020202020202020202020302e30300d0abdbbb8e0eecad6d0f8b7d12044656c697665727920466565a3ba202020202020202020202020202020202020202020302e30302020b6e0e0cdb7c6dac8a8cad0d6b5204d61726b65742076616ce21b021eed3838383838e638353632303030383032e77565286c6f6e6729a3ba2020202020202020202020302e30300d0abbf5b1d2d6cac8e0eb204e657720465820506c65646765a3ba20202020202020202020202020202020202020202020302e30302020bfd5cdb7c6dac8a8cad0d6b5204d61726b65742076616c75652873686f727429a3ba20202020202020202020302e30300d0abbf5b1d2d6cab3f620465820526564656d7074696f6ea3ba20202020202020202020202020202020202020202020302e30302020cad0d6b5c8a8d2e0e6204d61726b65742076616c75652865717569747929a3ba20202020202020202020313932332e30300d0ad6cad1bab1e0e4bbafbdf0b6e0ee2043686720696e20506c6564676520416d74a3ba2020202020202020202020202020302e30302020bfc9d3c3d7cabdf02046756e6420417661696c2ea3ba20202020202020202020202020202020202020313932332e30300d0ac8a8c0fbbdf0cad5c8e0eb205072656d69756d207265636569766564a3ba2020202020202020202020202020202020302e30302020b7e0e720cfd520b6c8205269736b20446567726565a3ba202020202020202020202020202020202020202020302e3030250d0ac8a8c0fbbdf0d6a7b3f6205072656d69756d2070616964a3ba202020202020202020202020202020202020202020302e30302020d3a6d7b7bcd3d7cabdf0204d61e1";
                
                //合约回报
                hex = "0200028201030c43e104e2802fe30de1040468e319e10301165a43363132efeb435a4345e5b6afc1a6c3ba363132ec5a43363132efeb5a43efee31e207e0e0e30ce3c8e301e203e0e8e301e3643fc999999999999a3230313531313235e13230313531323038e13230313631323037e13230313631323037e13230313631323037e131e30132323fc999999999999a3fc999999999999a30efefe17fe0efffffffffffffe13ff0e630e10301164647373031efeb435a4345e5b2a3c1a7373031ee4647373031efeb4647efee31e207e0e1e301e3c8e301e203e0e8e301e3143ff0e63230313531323233e13230313630313138e13230313730313136e13230313730313136e13230313730313136e131e30132323fb1e0eb851eb851e0eb3fb1e0eb851eb851e0eb30efefe17fe0efffffffffffffe17fe0efffffffffffff30e1030116637331373031efea444345e6d3f1c3d7b5e0edb7db31373031e9637331373031efea6373efee31e207e0e1e301e203e0e8e301e203e0e8e301e30a3ff0e63230313531323234e13230313630313138e13230313730313136e13230313730313136e13230313730313136e131e30132323fb1e0eb851eb851e0eb3fb1e0eb851eb851e0eb30efefe17fe0efffffffffffffe17fe0efffffffffffff30e1030116666231373031efea444345e6d6d0c3dcb6c8cfcbceacb0e0e531373031e5666231373031efea6662efee31e207e0e1e301e203e0e8e301e203e0e8e301e201f43fa999999999999a3230313531323234e13230313630313138e13230313730313136e13230313730313136e13230313730313136e131e30132323fc999999999999a3fc999999999999a30efefe17fe0efffffffffffffe17fe0efffffffffffff30";

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
                logger.Info("**** remote raw data size" + dstLen.ToString());
                string rawhex = ByteUtil.ByteToHex(dstData, ' ', dstLen);
                logger.Info(rawhex);


                //从数据包加载数据
                proto_hdr tmp0 = ByteSwapHelp.BytesToStruct<proto_hdr>(dstData, 0);
                
                ftd_hdr tmp1 = ByteSwapHelp.BytesToStruct<ftd_hdr>(dstData, 4);
                EnumTransactionID transid = (EnumTransactionID)tmp1.dTransId;

                ftdc_hdr tmp2 = ByteSwapHelp.BytesToStruct<ftdc_hdr>(dstData, offset + Constanst.PROFTD_HDRLEN);
                EnumFiledID fieldid = (EnumFiledID)tmp2.wFiId;
                int fieldsize = Marshal.SizeOf(typeof(LCThostFtdcInstrumentField));
                LCThostFtdcInstrumentField tmp3 = ByteSwapHelp.BytesToStruct<LCThostFtdcInstrumentField>(dstData, offset + Constanst.PROFTD_HDRLEN + Constanst.FTDC_HDRLEN);

                //offset += Constanst.FTDC_HDRLEN + rspSize;
                //ftdc_hdr tmp4 = ByteSwapHelp.BytesToStruct<ftdc_hdr>(dstData, offset + Constanst.PROFTD_HDRLEN);
                //EnumFiledID fieldid2 = (EnumFiledID)tmp2.wFiId;
                //LCThostFtdcRspUserLoginField tmp5 = ByteSwapHelp.BytesToStruct<LCThostFtdcRspUserLoginField>(dstData, offset + Constanst.PROFTD_HDRLEN + Constanst.FTDC_HDRLEN);

                //logger.Info("**** remote raw data size" + dstLen.ToString());
                //string rawhex = ByteUtil.ByteToHex(dstData, ' ', dstLen);
                //logger.Info(rawhex);

                //将业务数据用本地方法打包
                byte[] data1 = StructHelperV12.PackRsp<LCThostFtdcInstrumentField>(ref tmp3, EnumSeqType.SeqQry, EnumTransactionID.T_RSP_USRINF, (int)tmp1.dReqId, (int)tmp1.dSeqNo);
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
