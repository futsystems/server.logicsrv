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
        const string PROGRAME = "FrontSrv";
        static ILog logger = LogManager.GetLogger(PROGRAME);
        static void Main(string[] args)
        {
            
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            #region 调试
            ////System.Globalization.CultureInfo info = new System.Globalization.CultureInfo("zh-CN");
            
            //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("zh-CN");
            //logger.Info(string.Format("Encoding1:{0}", Encoding.Default));//936
            //logger.Info(string.Format("{0}-{1}", Encoding.Default.CodePage, Encoding.Default.WindowsCodePage));

            //Encoding c = Encoding.GetEncoding(936);
            //logger.Info(ByteUtil.ByteToHex(Encoding.Default.GetBytes("测试"))) ;//936
            //return;
            if (false)
            {
                
                //Type t = typeof(LCThostFtdcOrderField);
                //int ts = Marshal.SizeOf(t);
                //string b = "测试";
                //logger.Info(b + " " + ts.ToString());
                //return;
                EnumTransactionID x = (EnumTransactionID)32854;
                logger.Info("TID:" + x.ToString());
                //return;

                //3001 T_RSP_LOGIN      0   0
                string hex = "0200008501030c4ce43001e502e1f5e301e355e4d5fdc8b7efefefefefe21003e1983230313631323031e131323a32303a3337e13838383838e638353632303030383032e654726164696e67486f7374696e67efef04a515665831ec31323a32303a3337e131323a32303a3338e131323a32303a3338e131323a32303a3337e12d2d3a2d2d3a2d2de1";
                //CTP请求查询投资者
                //hex = "0200003201000c4c000400008008000000010001001c0000000207040018383838383800000000000038353632303030383032000000";

                //f102 T_RSP_SEQSN      0   0
                //hex = "0200002a01030c4ce4f102e504e128e41001e106e101e41001e106e102e41001e106e103e202121001e106e104e4";

                //8009 T_RSP_USRINF     4   1
                //hex = "0200008d01030c4ce104e28009e301e101018ce302e106018838353632303030383032e33838383838efe4c7aeb2a8efefefefefe231333230353832313938333034313934353134efefe6013138303531313336313637efefc9ccb3c7c2b7333431bac5d7cfb9e0e2b4f3cfc332323033cad2efefefefefe13230313230393137e13138303531313336313637efefefeb";
                //
                //hex = "0200003201000c4c000400008056000000020001001c0000000424580018383838383800000000000038353632303030383032000000";
                //8057 T_RSP_SETCONFIRM 4   2
                //hex = "0200000d01030c4ce104e28057e302e704";//"0200000d01030c4ce104e28057e302e70449";
                //hex = "0200000d01030c4ce104e28057e302e704d7";

                //8078 T_RSP_CFMMCKEY   4   3
                //hex = "0200000d01030c4ce104e28078e303e70f61";

                //811c T_RSP_TDTOK      1   1
                //hex = "0200003301030c4ce101e2811ce301e102e175e310e355e4d5fdc8b7efefefefefe2250fe1183838383838e638353632303030383032e3";

                //查询通知回报
                //hex = "0200021c01000c430004000080550000000400010206000000112456020238383838380000000000003c703ed7f0beb4b5c4bfcdbba7a3bad6d0b9fabda8c9e8d2f8d0d0bdabd3da32303136c4ea3131d4c23330c8d53234b5e3a3a8bcb43132d4c231c8d5c1e8b3bf30b5e3a3a9d6c132303136c4ea3132d4c231c8d533b5e3cda3d6b9d2b9c5ccb7fecef133d0a1cab1a3bbd6d0b9fac5a9d2b5d2f8d0d0bdabd3da32303136c4ea3131d4c23330c8d53232a3ba35302d3131d4c23330c8d53233a3ba3230d4ddcda3d2f8c6dad7aad5cba1a2b5e7d7d3b3f6c8ebbdf0cfb5cdb3b7fecef1a3ac3131d4c23330c8d53232a3ba35302d3132d4c231c8d53032a3ba3030d4ddcda3d2f8c6dad7aad5cbcdf8d2f8c7feb5c0b7fecef1a3bbbdbbcda8d2f8d0d0d3da32303136c4ea3131d4c23330c8d53231a3ba33302d3132d4c231c8d53033a3ba3030bdf8d0d0cfb5cdb3c9fdbcb6a3bbb9e2b4f3d2f8d0d0bdabd3da32303136c4ea3131d4c23330c8d53233b5e3c6f0d6c1d2b9c5ccbde1caf8bdf8d0d0cfb5cdb3c9fdbcb6a1a33c2f703e3c703ed7f0beb4b5c4bfcdbba7a3ba3c2f703e3c703e266e6273703b266e6273703b20266e6273703b20266e6273703bb7b3c7ebb2e9bfb4b9abcbbecfe0b9d8cdf8d5beb9abb8e6a1a3b5d8d6b7c8e7cfc2a3ba3c6120687265663d2671756f742671756f743b26616d703b71756f74687474703a2f2f7777772e7379776771682e636f6d2f3f703d003000";

                //查询TDNotice
                //hex = "0200003201000c4c000400008108000000050001001c00000016248600183838383838000000000000383536323030303830";

                //结算单
                //hex = "0200043301030c43e104e2803de309e1020444e317e11b021eed3838383838e638353632303030383032e72020202020202020202020202020202020202020202020202020202020202020202020202020202020202020c9e0ead2f8cdf2b9fac6dabbf520202020202020202020202020202020202020202020202020202020202020202020202020202020202020200d0a2020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020d6c6b1e0edcab1bce0e4204372656174696f6e2044617465a3ba32303136313131350d0a2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d0d0a2020202020202020202020202020202020202020202020202020202020bdbbd2d7bde0e1cbe0e3b5a528b6a2cad02920536574746c656d656e742053746174656d656e74284d544d2920202020202020202020202020202020202020202020202020202020200d0abfcdbba7bac520436c69656e74204944a3ba20203835363230303038303220202020202020202020bfcdbba7c3fbb3c620436c69656e74204e616d65a3bac7aeb2a80d0ac8d5c6da2044617465a3ba32303136313131350d0a0d0a0de21b021eed3838383838e638353632303030383032e70acad0b3a1b7e0e7cfd5c4aab2e0e2a3accef1c7e0ebbdf7c9f7b4d3cac2a1a30d0a0d0a20202020202020202020202020202020202020d7cabdf0d7b4bff62020b1d2d6d6a3bac8cbc3f1b1d220204163636f756e742053756d6d617279202043757272656e6379a3ba434e59200d0a2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d2d0d0ac6dab3f5bde0e1b4e0e62042616c616e636520622f66a3ba202020202020202020202020202020202020202020383432332e30302020bbf9b4a1b1a3d6a4bdf020496e697469616c204d617267696ea3ba2020202020202020202020202020202020302e30300d0ab3f620c8e0eb20bdf0204465706f7369742f5769746864726177616ca3ba202020202020202020202020202d363530302e30302020c6dac4a9bde0e1b4e0e62042616c616e636520632f66a3ba20202020202020202020202020202020202020313932332e30300d0ac6bdb2d6d3afbff7205265616c697a656420502f4ca3ba2020202020202020202020202020202020202020202020302e30302020d6ca20d1ba20bdf020506c6564676520416d6f756e74a3ba202020202020202020e1";
                //hex = "0200043001030c43e104e2803de30ae1020444e317e11b021eed3838383838e638353632303030383032e72020202020202020202020302e30300d0ab3d6b2d6b6a2cad0d3afbff7204d544d20502f4ca3ba202020202020202020202020202020202020202020202020302e30302020bfcdbba7c8a8d2e0e620436c69656e7420457175697479a3baa3ba202020202020202020202020202020313932332e30300d0ac6dac8a8d6b4d0d0d3afbff720457865726369736520502f4ca3ba20202020202020202020202020202020202020302e30302020bbf5b1d2d6cad1bab1a3d6a4bdf0d5bcd3c320465820506c65646765204f63632ea3ba202020202020202020302e30300d0acad620d0f820b7d120436f6d6d697373696f6ea3ba20202020202020202020202020202020202020202020202020302e30302020b1a3d6a4bdf0d5bcd3c3204d617267696e204f63637570696564a3ba20202020202020202020202020202020302e30300d0ad0d0c8a8cad6d0f8b7d120457865726369736520466565a3ba202020202020202020202020202020202020202020302e30302020bdbbb8e0eeb1a3d6a4bdf02044656c6976657279204d617267696ea3ba20202020202020202020202020202020302e30300d0abdbbb8e0eecad6d0f8b7d12044656c697665727920466565a3ba202020202020202020202020202020202020202020302e30302020b6e0e0cdb7c6dac8a8cad0d6b5204d61726b65742076616ce21b021eed3838383838e638353632303030383032e77565286c6f6e6729a3ba2020202020202020202020302e30300d0abbf5b1d2d6cac8e0eb204e657720465820506c65646765a3ba20202020202020202020202020202020202020202020302e30302020bfd5cdb7c6dac8a8cad0d6b5204d61726b65742076616c75652873686f727429a3ba20202020202020202020302e30300d0abbf5b1d2d6cab3f620465820526564656d7074696f6ea3ba20202020202020202020202020202020202020202020302e30302020cad0d6b5c8a8d2e0e6204d61726b65742076616c75652865717569747929a3ba20202020202020202020313932332e30300d0ad6cad1bab1e0e4bbafbdf0b6e0ee2043686720696e20506c6564676520416d74a3ba2020202020202020202020202020302e30302020bfc9d3c3d7cabdf02046756e6420417661696c2ea3ba20202020202020202020202020202020202020313932332e30300d0ac8a8c0fbbdf0cad5c8e0eb205072656d69756d207265636569766564a3ba2020202020202020202020202020202020302e30302020b7e0e720cfd520b6c8205269736b20446567726565a3ba202020202020202020202020202020202020202020302e3030250d0ac8a8c0fbbdf0d6a7b3f6205072656d69756d2070616964a3ba202020202020202020202020202020202020202020302e30302020d3a6d7b7bcd3d7cabdf0204d61e1";
                
                //合约回报
                //hex = "0200028201030c43e104e2802fe30de1040468e319e10301165a43363132efeb435a4345e5b6afc1a6c3ba363132ec5a43363132efeb5a43efee31e207e0e0e30ce3c8e301e203e0e8e301e3643fc999999999999a3230313531313235e13230313531323038e13230313631323037e13230313631323037e13230313631323037e131e30132323fc999999999999a3fc999999999999a30efefe17fe0efffffffffffffe13ff0e630e10301164647373031efeb435a4345e5b2a3c1a7373031ee4647373031efeb4647efee31e207e0e1e301e3c8e301e203e0e8e301e3143ff0e63230313531323233e13230313630313138e13230313730313136e13230313730313136e13230313730313136e131e30132323fb1e0eb851eb851e0eb3fb1e0eb851eb851e0eb30efefe17fe0efffffffffffffe17fe0efffffffffffff30e1030116637331373031efea444345e6d3f1c3d7b5e0edb7db31373031e9637331373031efea6373efee31e207e0e1e301e203e0e8e301e203e0e8e301e30a3ff0e63230313531323234e13230313630313138e13230313730313136e13230313730313136e13230313730313136e131e30132323fb1e0eb851eb851e0eb3fb1e0eb851eb851e0eb30efefe17fe0efffffffffffffe17fe0efffffffffffff30e1030116666231373031efea444345e6d6d0c3dcb6c8cfcbceacb0e0e531373031e5666231373031efea6662efee31e207e0e1e301e203e0e8e301e203e0e8e301e201f43fa999999999999a3230313531323234e13230313630313138e13230313730313136e13230313730313136e13230313730313136e131e30132323fc999999999999a3fc999999999999a30efefe17fe0efffffffffffffe17fe0efffffffffffff30";

                //委托回报
                //hex = "0200000e01030c4ce104e28001e20178e71a";

                //签约银行回报
                //hex = "0200015801030c43e104e28104e2017ce10a04e0e2e33a2470e1793838383838e631e330303030e1b9a4c9ccd2f8d0d0efefefefefefe32470e1793838383838e63130e230303030e1d5d0c9ccd2f8d0d0efefefefefefe32470e1793838383838e63131e230303030e1d6d0d0c5d2f8d0d0efefefefefefe32470e1793838383838e63132e230303030e1c3f1c9fad2f8d0d0efefefefefefe32470e1793838383838e63133e230303030e1c6bdb0b2d2f8d0d0efefefefefefe32470e1793838383838e632e330303030e1c5a9d2b5d2f8d0d0efefefefefefe32470e1793838383838e633e330303030e1d6d0b9fad2f8d0d0efefefefefefe32470e1793838383838e634e330303030e1bda8c9e0e8d2f8d0d0efefefefefefe32470e1793838383838e635e330303030e1bdbbcda8d2f8d0d0efefefefefefe32470e1793838383838e636e330303030e1c6d6b7a2d2f8d0d0efefefefefefe3";
                //hex = "0200007501030c4ce104e28104e2017de1030177e33a2470e1793838383838e637e330303030e1d0cbd2b5d2f8d0d0efefefefefefe32470e1793838383838e638e330303030e1bbe0e3b7e0e1d2f8d0d0efefefefefefe32470e1793838383838e639e330303030e1b9e0e2b4f3d2f8d0d0efefefefefefe3";

                //查询TDTOK
                //hex = "0200003201000c4c00010000811b000000010001001c00000010250f00183838383838000000000000383536323030303830320000";
                //T_RSP_TDTOK
                //hex = "0200003301030c4ce101e2811ce301e102e175e310e355e4d5fdc8b7efefefefefe2250fe1183838383838e638353632303030383032e3";
                //T_RTN_TDTOK
                //hex = "0200004101030c4ce102e2f019e301e101e140e42510e13c3838383838e630313737e738353632303030383032e514df49435532413431414c31434d3336575634335142e1";
                //可开回报
                //hex = "0200003f01030c4ce101e24012e303e102e19be35ae355e4d5fdc8b7efefefefefe2040ee13e3838383838e638353632303030383032e3617531373031efea303031e4";
                //hex = "0200003f01030c4ce101e24012e304e102e19be35be355e4d5fdc8b7efefefefefe2040ee13e3838383838e638353632303030383032e3617531373031efea313031e4";
                //hex = "0200003f01030c4ce101e24012e305e102e19be35ce355e4d5fdc8b7efefefefefe2040ee13e3838383838e638353632303030383032e3617531373031efea303031e4";

                //委托提交
                //hex = "0200011f01000c4c00010000400000000017000101090000005f040001053838383838000000000000383536323030303830320000006331373031000000000000000000000000000000000000000000000000000020202020202020202020393500383536323030303830320000000000003230300000000031000000004098ec00000000000000000133000000000000000000310000000031000000000000000030000000000000000000000000000000000000000000000000000000005f00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000003139322e3136382e302e313032000000334334364438423641313831000000000000000000";
                //委托回报
                //hex = "0200010401030c4ce102e2f001e302e101027fe40401027b3838383838e638353632303030383032e36331373031efeb202020202020202020203935e138353632303030383032e6323030e431e44098e0ece80133e931e431e830e43030353632303035efe15f202020202020203131323237e1444345e630303536e73031323234313832e36331373031efeb3030353632303035efe10130e43230313631323036e401efe6306130e7013230313631323036e130393a34393a3437efefefefefe10518ba1ac750425344535635343030e1b1a8b5a5d2d1cce0e1bdbbefefefefefefe201e0e238efefefefec3139322e3136382e302e313032e3334334364438423641313831e9";
                //hex = "0200011801030c4ce102e2f001e303e101027fe40401027b3838383838e638353632303030383032e36331373031efeb202020202020202020203935e138353632303030383032e6323030e431e44098e0ece80133e931e431e830e42020202020202020202020203130393239333738e45f202020202020203131323237e1444345e630303536e73031323234313832e36331373031efeb3030353632303035efe10133e3013230313631323036e401202020203130393239333738e9303330e7013230313631323036e130393a34393a3438efefefefeb50f8e30518ba1ac750425344535635343030e1ceb4b3c9bdbbefefefefefefe601e0e238efefefefec3139322e3136382e302e313032e3334334364438423641313831e9";
                
                //秘密修改回报 300b
                //hex = "0200004d01030c4ce101e2300be303e102e1cae351e355e30e4354503ad4adbfdac1e0eeb2bbc6a5c5e0e4efefefefe50304e16d3838383838e638353632303030383032e6313131efefe8313131efefe8";
                //交易账户修改 300e
                //hex = "0200008801000c4c00010000300e0000000100010072000000a12460006e38383838380000000000003835363230303038303200000031313100000000000000000000000000000000000000000000000000000000000000000000000000003131310000000000000000000000000000000000000000000000000000000000000000000000000000434e5900";
                
                //查询银行流水
                //hex = "0200003a01000c4c0004000082000000000c00010024000000ed300d00203838383838000000000000383536323030303830320000000000000000000000";

                //银行流水记录
                //hex = "020000b201030c4ce104e28201e20185e1010197e3f3300c0193e39e3230313631323036e13230313631323037e132323a30363a3438e1323032303031e531e330303030e13039353538383031303031313438313637373138efe73238383537303531e53838383838efefe838353632303030383032e338353632303030383032e501e0ee31333230353832313938333034313934353134efefe3434e59e13ff0efe731efefefefe2bdbbd2d7b3c9b9a6efefefefed";
                //hex = "020000ea01030c4ce104e2401ce20182e1020134e2013f100ee1963230313631323037e132323a30363a3438e1323032303031e301e0ee3838383838e638353632303030383032ec01b852db31e330303030e139353538383031303031313438313637373138efe7333230353832313938333034313934353134e3434e59e13ff0e631100ee1963230313631323037e130393a31303a3331e1323032303032e304f63838383838e638353632303030383032ec01c787fb31e330303030e139353538383031303031313438313637373138efe7333230353832313938333034313934353134e3434e59e13ff0e631";
                
                //编码测试 
                //hex = "0200021C01000C4C000400008055000000010001020600000002245602023838383838380000000000B2E2CAD400000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
                //hex = "0200021C01000C4C000400008055000000010001020600000002245602023838383838000000000000E6B58BE8AF950000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
                
                //authinfo
                //hex = "020000c501000c4c00010000301000000001000100af00000001101200ab303138370000000000000038353030303031000000000000000000504253445356353430300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";

                //T_RSP_SEQSN
                hex = "0200002a01030c4ce4f102e504e128e41001e106e101e41001e106e102e41001e106e103e202121001e106e104e4";
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
                int fieldsize = Marshal.SizeOf(typeof(TopicData));
                TopicData rsp = ByteSwapHelp.BytesToStruct<TopicData>(dstData, offset + Constanst.PROFTD_HDRLEN + Constanst.FTDC_HDRLEN);
                //string msg = Encoding.GetEncoding("GB2312").GetString(rsp.ErrorMsg);

                ftdc_hdr hd02 = ByteSwapHelp.BytesToStruct<ftdc_hdr>(dstData, offset + Constanst.PROFTD_HDRLEN + Constanst.FTDC_HDRLEN + fieldsize);
                EnumFiledID hd02type = (EnumFiledID)hd02.wFiId;


                LCThostFtdcNoticeField tmp3 = ByteSwapHelp.BytesToStruct<LCThostFtdcNoticeField>(dstData, offset + Constanst.PROFTD_HDRLEN + Constanst.FTDC_HDRLEN);


                //LCThostFtdcInputOrderField tmp3 = ByteSwapHelp.BytesToStruct<LCThostFtdcInputOrderField>(dstData, offset + Constanst.PROFTD_HDRLEN + Constanst.FTDC_HDRLEN);

                offset += Constanst.FTDC_HDRLEN + fieldsize;
                ftdc_hdr tmp4 = ByteSwapHelp.BytesToStruct<ftdc_hdr>(dstData, offset + Constanst.PROFTD_HDRLEN);
                EnumFiledID fieldid2 = (EnumFiledID)tmp2.wFiId;
                LCThostFtdcTransferSerialField tmp5 = ByteSwapHelp.BytesToStruct<LCThostFtdcTransferSerialField>(dstData, offset + Constanst.PROFTD_HDRLEN + Constanst.FTDC_HDRLEN);

                //logger.Info("**** remote raw data size" + dstLen.ToString());
                //string rawhex = ByteUtil.ByteToHex(dstData, ' ', dstLen);
                //logger.Info(rawhex);

                //将业务数据用本地方法打包
                byte[] data1 = StructHelperV12.PackRsp<LCThostFtdcNoticeField>(ref tmp3, EnumSeqType.SeqQry, EnumTransactionID.T_RSP_USRINF, (int)tmp1.dReqId, (int)tmp1.dSeqNo);
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
            #endregion

            //创建MQServer
            MQServer mqServer = new MQServer();
            //创建TL ServiceHost
            TLServiceHost.TLServiceHost tlhost = new TLServiceHost.TLServiceHost(mqServer);

            //创建CTP ServiceHost
            CTPServiceHost ctphost = new CTPServiceHost(mqServer);

            //创建XL ServiceHost
            XLServiceHost.XLServiceHost xlhost = new XLServiceHost.XLServiceHost(mqServer);

            //创建WebSocket ServiceHost
            WSServiceHost.WSServiceHost wshost = new WSServiceHost.WSServiceHost(mqServer);

            //创建WatchDog
            WatchDog watchDog = new WatchDog(mqServer, ctphost);

            //依次启动服务
            mqServer.Start();
            tlhost.Start();
            ctphost.Start();
            xlhost.Start();
            wshost.Start();
            watchDog.Join();

            
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            logger.Error("UnhandledException:" + ex.ToString());
        }
    }
}
