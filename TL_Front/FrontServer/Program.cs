using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using Common.Logging;
using CTPService;
using CTPService.Struct.V12;


namespace FrontServer
{
    class Program
    {

        static void Main(string[] args)
        {
            ILog logger = LogManager.GetLogger("Main");
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
            CTPServiceHost host = new CTPServiceHost();
            host.Start();

            System.Threading.Thread.Sleep(int.MaxValue);
        }
    }
}
