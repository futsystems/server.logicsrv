using Easychart.Finance;
using Easychart.Finance.DataProvider;
using System.Reflection;
using System.Runtime.CompilerServices;
[assembly: AssemblyVersion("1.0.0.27")]
namespace FML
{
	namespace Scan
	{
		#region Formula Group Indicator
		public class UP:FormulaBase
		{
			public UP():base()
			{
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=ISUP;
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "ISUP";}
			}
		
			public override string Description
			{
				get{return "The stock is going up";}
			}
		} //class UP

		public class Range:FormulaBase
		{
			public double N1=0;
			public double N2=0;
			public double N3=0;
			public Range():base()
			{
				AddParam("N1","10","0.01","1000","",FormulaParamType.Double);
				AddParam("N2","12","0.01","1000","",FormulaParamType.Double);
				AddParam("N3","100000","0.01","5000000000000","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=BETWEEN(C,N1,N2) & (V>N3);
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Range";}
			}
		
			public override string Description
			{
				get{return "Scan the quotes between N1 and N2";}
			}
		} //class Range

		public class CrossMA:FormulaBase
		{
			public double N1=0;
			public double N2=0;
			public double N3=0;
			public CrossMA():base()
			{
				AddParam("N1","13","1","10000000","",FormulaParamType.Double);
				AddParam("N2","50","1","10000000","",FormulaParamType.Double);
				AddParam("N3","5","1","100","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=EXIST(CROSS(MA(C,N1),MA(C,N2)),N3);
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Cross MA";}
			}
		
			public override string Description
			{
				get{return "MA1 cross MA2 from below within N3 days";}
			}
		} //class CrossMA

		public class Rising:FormulaBase
		{
			public Rising():base()
			{
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=C>REF(C,1);
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Rising";}
			}
		
			public override string Description
			{
				get{return "Rising stocks";}
			}
		} //class Rising

		public class MACD:FormulaBase
		{
			public double LONG=0;
			public double SHORT=0;
			public double M=0;
			public MACD():base()
			{
				AddParam("LONG","26","20","100","",FormulaParamType.Double);
				AddParam("SHORT","12","5","50","",FormulaParamType.Double);
				AddParam("M","9","2","40","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData DIFF= EMA(CLOSE,SHORT) - EMA(CLOSE,LONG); DIFF.Name="DIFF";
				FormulaData DEA= EMA(DIFF,M); DEA.Name="DEA";
				FormulaData NONAME0=CROSS(DIFF,DEA);
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Scan by MACD";}
			}
		
			public override string Description
			{
				get{return "DIFF line cross DEA from below.";}
			}
		} //class MACD

		public class RSI:FormulaBase
		{
			public double N=0;
			public double LL=0;
			public RSI():base()
			{
				AddParam("N","6","2","100","",FormulaParamType.Double);
				AddParam("LL","20","0","40","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=CROSS(FML(DP,"RSI(N)[RSI]"),LL);
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "RSI scanning";}
			}
		
			public override string Description
			{
				get{return "";}
			}
		} //class RSI

		public class NewHigh:FormulaBase
		{
			public double N=0;
			public NewHigh():base()
			{
				AddParam("N","3","1","100","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=HHV(H,N)==H & BARSCOUNT(C)>=N;
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Newest high within N days";}
			}
		
			public override string Description
			{
				get{return "Newest high within N days";}
			}
		} //class NewHigh

		public class BOLL:FormulaBase
		{
			public double N=0;
			public double P=0;
			public BOLL():base()
			{
				AddParam("N","26","5","100","",FormulaParamType.Double);
				AddParam("P","2","0.1","10","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=CROSS(C,FML(DP,"BB(N,P)[LOWER]"));
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Boll scanning";}
			}
		
			public override string Description
			{
				get{return "";}
			}
		} //class BOLL

		public class SAR:FormulaBase
		{
			public double N=0;
			public double STEP=0;
			public double MAXP=0;
			public SAR():base()
			{
				AddParam("N","10","1","50","",FormulaParamType.Double);
				AddParam("STEP","2","1","5","",FormulaParamType.Double);
				AddParam("MAXP","20","5","80","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=SARTURN(N,STEP,MAXP)==1;
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Sar scan";}
			}
		
			public override string Description
			{
				get{return "";}
			}
		} //class SAR

		public class NewLow:FormulaBase
		{
			public double N=0;
			public NewLow():base()
			{
				AddParam("N","3","1","100","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=LLV(L,N)==L & BARSCOUNT(C)>=N;
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Newest low within N days";}
			}
		
			public override string Description
			{
				get{return "";}
			}
		} //class NewLow

		public class SVG:FormulaBase
		{
			public double N1=0;
			public double N2=0;
			public SVG():base()
			{
				AddParam("N1","4","1","100","",FormulaParamType.Double);
				AddParam("N2","20","1","100","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=V>(MA(V,N2)*N1) & ISUP;
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Strong Volume Gainers";}
			}
		
			public override string Description
			{
				get{return "";}
			}
		} //class SVG

		public class SVD:FormulaBase
		{
			public double N1=0;
			public double N2=0;
			public SVD():base()
			{
				AddParam("N1","4","1","100","",FormulaParamType.Double);
				AddParam("N2","20","1","100","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=V>(MA(V,N2)*N1) & ISDOWN;
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Strong Volume Decliners";}
			}
		
			public override string Description
			{
				get{return "";}
			}
		} //class SVD

		public class MCU:FormulaBase
		{
			public double N1=0;
			public double N2=0;
			public MCU():base()
			{
				AddParam("N1","13","1","100","",FormulaParamType.Double);
				AddParam("N2","50","1","100","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=CROSS(MA(C,N1),MA(C,N2)) & ISUP;
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Bullish MA Crossover";}
			}
		
			public override string Description
			{
				get{return "Stocks that had the simple moving average of the last N1 closing prices move above the simple moving average of the last N2 closing prices.\n\n";}
			}
		} //class MCU

		public class MCD:FormulaBase
		{
			public double N1=0;
			public double N2=0;
			public MCD():base()
			{
				AddParam("N1","13","1","100","",FormulaParamType.Double);
				AddParam("N2","50","1","100","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=CROSS(MA(C,N1),MA(C,N2)) & ISDOWN;
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Bearish MA Crossover";}
			}
		
			public override string Description
			{
				get{return "Stocks that had the simple moving average of the last N1 closing prices move below the simple moving average of the last N2 closing prices.\n";}
			}
		} //class MCD

		public class AUBB:FormulaBase
		{
			public double N=0;
			public AUBB():base()
			{
				AddParam("N","20","1","100","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=CROSS(C,FML(DP,"BB(N,2)[UPPER]"));
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Moved Above Upper Bollinger Band";}
			}
		
			public override string Description
			{
				get{return "Stocks which closed above the upper line of their N-day Bollinger Band and which were below that same band after the previous trading session.\n";}
			}
		} //class AUBB

		public class BLBB:FormulaBase
		{
			public double N=0;
			public BLBB():base()
			{
				AddParam("N","20","1","100","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=CROSS(FML(DP,"BB(N,2)[LOWER]"),C);
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Moved Below Lower Bollinger Band";}
			}
		
			public override string Description
			{
				get{return "Stocks which closed below the lower line of their N-day Bollinger Band and which were above that same band after the previous trading session.\n";}
			}
		} //class BLBB

		public class GU:FormulaBase
		{
			public double P=0;
			public GU():base()
			{
				AddParam("P","2.5","1","100","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData LH= REF(H,1); LH.Name="LH";
				FormulaData NONAME0=(L-LH)/LH>(P/100);
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Gap Ups";}
			}
		
			public override string Description
			{
				get{return "Stocks whose current low was at least P percent higher than the previous day's high.";}
			}
		} //class GU

		public class GD:FormulaBase
		{
			public double P=0;
			public GD():base()
			{
				AddParam("P","2.5","1","100","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData LL= REF(L,1); LL.Name="LL";
				FormulaData NONAME0=(LL-H)/LL>(P/100);
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Gap Downs";}
			}
		
			public override string Description
			{
				get{return "Stocks whose current high was at least P percent lower than the previous day's low.";}
			}
		} //class GD

		public class IT:FormulaBase
		{
			public double P=0;
			public IT():base()
			{
				AddParam("P","2.5","1","100","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=REF(FML(DP,"GU(P)"),1) & FML(DP,"GD(P)");
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Island Tops";}
			}
		
			public override string Description
			{
				get{return "Stocks that gapped up at least P percent yesterday and gapped down at least 2.5 percent today.\n";}
			}
		} //class IT

		public class IB:FormulaBase
		{
			public double P=0;
			public IB():base()
			{
				AddParam("P","2.5","1","100","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=REF(FML(DP,"GD(P)"),1) & FML(DP,"GU(P)");
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Island Bottoms";}
			}
		
			public override string Description
			{
				get{return "";}
			}
		} //class IB

		public class ADXU:FormulaBase
		{
			public double N=0;
			public double M=0;
			public ADXU():base()
			{
				AddParam("N","14","1","100","",FormulaParamType.Double);
				AddParam("M","20","1","100","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=CROSS(FML(DP,"ADX(N)"),M) & FML(DP,"ADX(N)[+DI]")>FML(DP,"ADX(N)[-DI]");
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Stocks in a New Uptrend(ADX)";}
			}
		
			public override string Description
			{
				get{return "Stocks for which the N-day ADX Line just moved above the +M level (signaling a new trend) and the +DI line is above the -DI line (signaling that the new trend is upwards).";}
			}
		} //class ADXU

		public class ADXD:FormulaBase
		{
			public double N=0;
			public double M=0;
			public ADXD():base()
			{
				AddParam("N","14","1","100","",FormulaParamType.Double);
				AddParam("M","20","1","100","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=CROSS(FML(DP,"ADX(N)"),M) & FML(DP,"ADX(N)[-DI]")>FML(DP,"ADX(N)[+DI]");
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Stocks in a New Downtrend (ADX)";}
			}
		
			public override string Description
			{
				get{return "Stocks for which the N-day ADX Line just moved above the +M level (signaling a new trend) and the -DI line is above the +DI line (signaling that the new trend is downwards).\n";}
			}
		} //class ADXD

		public class MACDU:FormulaBase
		{
			public double LONG=0;
			public double SHORT=0;
			public double M=0;
			public MACDU():base()
			{
				AddParam("Long","26","1","100","",FormulaParamType.Double);
				AddParam("Short","12","1","100","",FormulaParamType.Double);
				AddParam("M","9","1","100","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData DIFF= EMA(CLOSE,SHORT) - EMA(CLOSE,LONG); DIFF.Name="DIFF";
				FormulaData DEA= EMA(DIFF,M); DEA.Name="DEA";
				FormulaData NONAME0=LONGCROSS(DIFF,DEA,3);
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Bullish MACD Crossovers";}
			}
		
			public override string Description
			{
				get{return "Stocks whose MACD line crossed above the signal line today after being below the signal line for the previous three days. The MACD parameters used are 26 and 12 and the signal line is a 9-day EMA of the MACD line.\n";}
			}
		} //class MACDU

		public class MACDD:FormulaBase
		{
			public double LONG=0;
			public double SHORT=0;
			public double M=0;
			public MACDD():base()
			{
				AddParam("Long","26","1","100","",FormulaParamType.Double);
				AddParam("Short","12","1","100","",FormulaParamType.Double);
				AddParam("M","9","1","100","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData DIFF= EMA(CLOSE,SHORT) - EMA(CLOSE,LONG); DIFF.Name="DIFF";
				FormulaData DEA= EMA(DIFF,M); DEA.Name="DEA";
				FormulaData NONAME0=LONGCROSS(DEA,DIFF,3);
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Bearish MACD Crossovers";}
			}
		
			public override string Description
			{
				get{return "Stocks whose MACD line crossed below the signal line today after being above the signal line for the previous three days. The MACD parameters used are 26 and 12 and the signal line is a 9-day EMA of the MACD line.";}
			}
		} //class MACDD

		public class RSID:FormulaBase
		{
			public double N=0;
			public double LL=0;
			public RSID():base()
			{
				AddParam("N","14","1","100","",FormulaParamType.Double);
				AddParam("LL","70","1","100","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=LONGCROSS(LL,FML(DP,"RSI(N)[RSI]"),3);
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Overbought with a Declining RSI";}
			}
		
			public override string Description
			{
				get{return "Stocks whose RSI line moved below 70 today after being above 70 for the previous three days. The RSI period used is 14.";}
			}
		} //class RSID

		public class RSII:FormulaBase
		{
			public double N=0;
			public double LL=0;
			public RSII():base()
			{
				AddParam("N","14","1","100","",FormulaParamType.Double);
				AddParam("LL","30","1","100","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=LONGCROSS(FML(DP,"RSI(N)[RSI]"),LL,3);
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Oversold with an Improving RSI";}
			}
		
			public override string Description
			{
				get{return "Stocks whose RSI line moved above 30 today after being below 30 for the previous three days. The RSI period used is 14.\n";}
			}
		} //class RSII

		public class CMFI:FormulaBase
		{
			public double N=0;
			public CMFI():base()
			{
				AddParam("N","21","1","100","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=CROSS(FML(DP,"CMF(N)"), 0.2);
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Improving Chaikin Money Flow";}
			}
		
			public override string Description
			{
				get{return "Stocks for which the 21-day Chaikin Money Flow oscillator has just moved above the +20% level.\n";}
			}
		} //class CMFI

		public class CMFD:FormulaBase
		{
			public double N=0;
			public CMFD():base()
			{
				AddParam("N","21","1","100","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=CROSS(-0.2,FML(DP,"CMF(N)"));
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Declining Chaikin Money Flow";}
			}
		
			public override string Description
			{
				get{return "Stocks for which the 21-day Chaikin Money Flow oscillator has just moved below the -20% level.\n";}
			}
		} //class CMFD

		public class CCIB:FormulaBase
		{
			public double N=0;
			public CCIB():base()
			{
				AddParam("N","20","1","100","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData TYP= (HIGH + LOW + CLOSE)/3; TYP.Name="TYP";
				FormulaData NONAME0=CROSS((TYP-MA(TYP,N))/(0.015*AVEDEV(TYP,N)),100);
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "CCI Buy Signals";}
			}
		
			public override string Description
			{
				get{return "Stocks for which the 20-day Commodity Channel Index (CCI) has just moved above the +100 level.";}
			}
		} //class CCIB

		public class CCIS:FormulaBase
		{
			public double N=0;
			public CCIS():base()
			{
				AddParam("N","20","1","100","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData TYP= (HIGH + LOW + CLOSE)/3; TYP.Name="TYP";
				FormulaData NONAME0=CROSS(-100,(TYP-MA(TYP,N))/(0.015*AVEDEV(TYP,N)));
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "CCI Sell Signals";}
			}
		
			public override string Description
			{
				get{return "Stocks for which the 20-day Commodity Channel Index (CCI) has just moved below the -100 level.";}
			}
		} //class CCIS

		public class SARB:FormulaBase
		{
			public double N=0;
			public double STEP=0;
			public double MAXP=0;
			public SARB():base()
			{
				AddParam("N","10","1","100","",FormulaParamType.Double);
				AddParam("STEP","2","0","10","",FormulaParamType.Double);
				AddParam("MAXP","20","0","100","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=SARTURN(N,STEP,MAXP)==1;
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Parabolic SAR Buy Signals";}
			}
		
			public override string Description
			{
				get{return "Stocks whose Parabolic SAR just \"flipped\" from above the price bars to below the price bars. The parameters used are 0.02 and 0.20.\n";}
			}
		} //class SARB

		public class SARS:FormulaBase
		{
			public double N=0;
			public double STEP=0;
			public double MAXP=0;
			public SARS():base()
			{
				AddParam("N","10","1","100","",FormulaParamType.Double);
				AddParam("STEP","2","1","100","",FormulaParamType.Double);
				AddParam("MAXP","20","1","100","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=SARTURN(N,STEP,MAXP)==-1;
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Parabolic SAR Sell Signals";}
			}
		
			public override string Description
			{
				get{return "";}
			}
		} //class SARS

		public class Fibonnaci:FormulaBase
		{
			public double N=0;
			public Fibonnaci():base()
			{
				AddParam("N","40","1","100","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=BETWEEN((FML(DP,"Fibonnaci(N)[A3]")-C)/C,-0.01,0.01);
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Fibonnaci";}
			}
		
			public override string Description
			{
				get{return "";}
			}
		} //class Fibonnaci

		public class DownT:FormulaBase
		{
			public double N=0;
			public double P=0;
			public double M=0;
			public DownT():base()
			{
				AddParam("N","10","0","1000","",FormulaParamType.Double);
				AddParam("P","90","0","100","",FormulaParamType.Double);
				AddParam("M","52","0","100","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData A=HHV(C,M*5); A.Name="A";
				FormulaData NONAME0=C<N & BETWEEN(C,A*P/100,A);
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Down Trend";}
			}
		
			public override string Description
			{
				get{return "DownT will show all stocks under N which closed less than P% below their M week high. So for example, if the 52 week high is $10 and the stock closed at $9.00 it would be in the screen.  If it closed at $8.90 it would not show up.";}
			}
		} //class DownT

		public class DTWLL:FormulaBase
		{
			public double N=0;
			public double M=0;
			public double P1=0;
			public double P2=0;
			public DTWLL():base()
			{
				AddParam("N","2","0","100","",FormulaParamType.Double);
				AddParam("M","2","0","100","",FormulaParamType.Double);
				AddParam("P1","5","0","100000","",FormulaParamType.Double);
				AddParam("P2","10000","0","100000","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData PC= REF(C,1); PC.Name="PC";
				FormulaData NONAME0=(C-PC)/PC>N/100 & (H-C)/(H-L)<=M/100 & C>=P1 & C<=P2;
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "up at least N% and closed in the upper M% range of the days range";}
			}
		
			public override string Description
			{
				get{return "up at least N% and closed in the upper M% range of the days range";}
			}
		} //class DTWLL

		public class DTWLS:FormulaBase
		{
			public double N=0;
			public double M=0;
			public double P1=0;
			public double P2=0;
			public DTWLS():base()
			{
				AddParam("N","2","0","100","",FormulaParamType.Double);
				AddParam("M","2","0","100","",FormulaParamType.Double);
				AddParam("P1","5","0","1000000","",FormulaParamType.Double);
				AddParam("P2","10000","0","100000","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData PC= REF(C,1); PC.Name="PC";
				FormulaData NONAME0=(C-PC)/PC<-N/100 & (C-L)/(H-L)<=M/100 & C>=P1 & C<=P2;
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "down at least N% and closed in the lower M% range of the days range";}
			}
		
			public override string Description
			{
				get{return "down at least N% and closed in the lower M% range of the days range";}
			}
		} //class DTWLS

		public class Muddy:FormulaBase
		{
			public double N1=0;
			public double N2=0;
			public double N3=0;
			public Muddy():base()
			{
				AddParam("N1","1","0.001","5000000","",FormulaParamType.Double);
				AddParam("N2","1000","0.001","5000000","",FormulaParamType.Double);
				AddParam("N3","50","1","5000000","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData R1= REF(C,1); R1.Name="R1";
				FormulaData R2= REF(R1,1); R2.Name="R2";
				FormulaData NONAME0=C<R1 & R1<R2 & MA(V,N3)>100000 & BETWEEN(C,N1,N2) & FML(DP,"BB(M,2)[LOWER]")/C>0.99;
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Muddy Original";}
			}
		
			public override string Description
			{
				get{return "This locates stocks where the price has been decreasing for the past 3 days, has touched the lower Bollinger Band and the volume is above the 90 day average of N3 volume\nN1=Enter Min Price\nN2=Enter Max Price\nN3=Enter the average 90 day volume";}
			}
		} //class Muddy

		public class Muddy2:FormulaBase
		{
			public double MINPRICE=0;
			public double MAXPRICE=0;
			public double MINVOLUME=0;
			public Muddy2():base()
			{
				AddParam("MinPrice","0.01","0","1000000","",FormulaParamType.Double);
				AddParam("MaxPrice","10","0","1000000","",FormulaParamType.Double);
				AddParam("MinVolume","100000","1","10000000000","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData X=(V>REF(V,1))*2; X.Name="X";
				FormulaData AVGV= MA(V,90); AVGV.Name="AvgV";
				FormulaData Y=COUNT(V>AVGV,3); Y.Name="Y";
				FormulaData Z=X+Y; Z.Name="Z";
				FormulaData C1=Z>1; C1.Name="C1";
				FormulaData C2=C<REF(C,1) & REF(C,1)<REF(C,2) & COUNT(ISUP,3)==0; C2.Name="C2";
				FormulaData C3=AVGV>MINVOLUME; C3.Name="C3";
				FormulaData C4=BETWEEN(C,MINPRICE,MAXPRICE); C4.Name="C4";
				FormulaData C5=C<FML(DP,"BB(20,2)[LOWER]"); C5.Name="C5";
				FormulaData NONAME0=C1 & C2 & C3 & C4 & C5;
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Muddy 2";}
			}
		
			public override string Description
			{
				get{return "This locates stocks where the price has been decreasing for the past 3 days, has touched the lower Bollinger Band and the volume is above the 90 day average of N3 volume";}
			}
		} //class Muddy2

		public class Muddy3:FormulaBase
		{
			public double MINPRICE=0;
			public double MAXPRICE=0;
			public double MINVOLUME=0;
			public Muddy3():base()
			{
				AddParam("MinPrice","0.01","0","1000000","",FormulaParamType.Double);
				AddParam("MaxPrice","10","0","1000000","",FormulaParamType.Double);
				AddParam("MinVolume","100000","0","10000000000","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData URSI= COUNT(FML(DP,"RSI(2)")>90, 5); URSI.Name="uRSI";
				FormulaData C1= C < EMA(C,13); C1.Name="C1";
				FormulaData C2= C > MA(C,20); C2.Name="C2";
				FormulaData C3= URSI > 0; C3.Name="C3";
				FormulaData C4= BETWEEN(C,MINPRICE,MAXPRICE); C4.Name="C4";
				FormulaData C5= V > MINVOLUME; C5.Name="C5";
				FormulaData C6= EXIST(CROSS(FML(DP,"BB(20,2)[UPPER]"),C),7); C6.Name="C6";
				FormulaData NONAME0=C1 & C2 & C3 & C4 & C5 & C6;
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Avery Zone";}
			}
		
			public override string Description
			{
				get{return "This locates stocks which broke the upper bollinger\nband and pulled back into a 'zone' between EMA13 and\nMA 20.\n";}
			}
		} //class Muddy3

		public class Muddy4:FormulaBase
		{
			public double MINPRICE=0;
			public double MAXPRICE=0;
			public double MINVOLUME=0;
			public double N=0;
			public Muddy4():base()
			{
				AddParam("MinPrice","0.01","0","1000000","",FormulaParamType.Double);
				AddParam("MaxPrice","10","0","1000000","",FormulaParamType.Double);
				AddParam("MinVolume","250000","0","10000000000","",FormulaParamType.Double);
				AddParam("N","60","0","10000","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData C1=C<REF(C,1) & REF(C,1)<REF(C,2) & COUNT(ISUP,3)==0; C1.Name="C1";
				FormulaData C2=MA(V,90)>MINVOLUME & BETWEEN(C,MINPRICE,MAXPRICE) & FML(DP,"BB(20,2)[LOWER]")/C>0.99; C2.Name="C2";
				FormulaData C3=FML(DP,"RSI(2)")<1; C3.Name="C3";
				FormulaData C4=SLOPE(C,N)>0; C4.Name="C4";
				FormulaData C5=NEAR(FML(DP,"LinRegr(N)[LOWER]"),L,0.05); C5.Name="C5";
				FormulaData NONAME0=C1 & C2 & C3 & C4 & C5;
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Muddy Triple";}
			}
		
			public override string Description
			{
				get{return "";}
			}
		} //class Muddy4

		public class Muddy5:FormulaBase
		{
			public double MINPRICE=0;
			public double MAXPRICE=0;
			public double MINVOLUME=0;
			public double N=0;
			public Muddy5():base()
			{
				AddParam("MinPrice","0.01","0","1000000","",FormulaParamType.Double);
				AddParam("MaxPrice","10","0","1000000","",FormulaParamType.Double);
				AddParam("MinVolume","250000","0","10000000000","",FormulaParamType.Double);
				AddParam("N","60","0","10000","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData C1=C<REF(C,1) & REF(C,1)<REF(C,2) & COUNT(ISUP,3)==0; C1.Name="C1";
				FormulaData C2=MA(V,90)>MINVOLUME & BETWEEN(C,MINPRICE,MAXPRICE) & FML(DP,"BB(20,2)[LOWER]")/C>0.99; C2.Name="C2";
				FormulaData C3=FML(DP,"RSI(2)")<1; C3.Name="C3";
				FormulaData C4=SLOPE(C,N)>0; C4.Name="C4";
				FormulaData C5=NEAR(FML(DP,"LinRegr(N)[LOWER]"),L,0.05); C5.Name="C5";
				FormulaData NONAME0=C1 & C2 & ((C4 & C5) | C3);
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Muddy Double";}
			}
		
			public override string Description
			{
				get{return "";}
			}
		} //class Muddy5

		public class IGLN:FormulaBase
		{
			public string INDI="";
			public double N=0;
			public double ISLESS=0;
			public IGLN():base()
			{
				AddParam("Indi","RSI","0","0","",FormulaParamType.Indicator);
				AddParam("N","0","0","1000000000","",FormulaParamType.Double);
				AddParam("IsLess","0","0","2","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=(FML(INDI) > N) ^ ISLESS;
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Indicator greater or less than a number";}
			}
		
			public override string Description
			{
				get{return "";}
			}
		} //class IGLN

		public class SName:FormulaBase
		{
			public string KEY="";
			public SName():base()
			{
				AddParam("Key","","","","",FormulaParamType.String);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=CONTAIN(STKNAME,KEY) | CONTAIN(CODE,KEY);
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Search key from stock name";}
			}
		
			public override string Description
			{
				get{return "";}
			}
		} //class SName

		#endregion

		#region Formula Group Pattern
		public class BullE:FormulaBase
		{
			public BullE():base()
			{
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData LO=REF(O,1); LO.Name="LO";
				FormulaData LC=REF(C,1); LC.Name="LC";
				FormulaData NONAME0=C>LO & O<LC & LO>LC;
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Bullish Engulfing";}
			}
		
			public override string Description
			{
				get{return "";}
			}
		} //class BullE

		public class PL:FormulaBase
		{
			public PL():base()
			{
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData LC=REF(C,1); LC.Name="LC";
				FormulaData LO=REF(O,1); LO.Name="LO";
				FormulaData NONAME0=(LO-LC)/LO>0.05 & O<LC & C>(LC+LO)/2;
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Piercing Line";}
			}
		
			public override string Description
			{
				get{return "1)The first is a relatively long black candlestick.\n2)The second is a white candlestick that opens below the previous period's close and closes above the mid-point of the black candlestick's body.";}
			}
		} //class PL

		public class BullH:FormulaBase
		{
			public BullH():base()
			{
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData LC=REF(C,1); LC.Name="LC";
				FormulaData LO=REF(O,1); LO.Name="LO";
				FormulaData NONAME0=ABS(LO-LC)/LO>0.05 & MIN(LC,LO)<MIN(C,O) & MAX(LC,LO)>MAX(C,O);
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Bullish Harami";}
			}
		
			public override string Description
			{
				get{return "The bullish harami is made up of two candlesticks. The first has a large body and the second a small body that is totally encompassed by the first.";}
			}
		} //class BullH

		public class BearE:FormulaBase
		{
			public BearE():base()
			{
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData LO=REF(O,1); LO.Name="LO";
				FormulaData LC=REF(C,1); LC.Name="LC";
				FormulaData NONAME0=O>LC & C<LO & LC>LO;
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Bearish Engulfing";}
			}
		
			public override string Description
			{
				get{return "The open must be higher than the previous close.\nThe close must be lower than the previous open.";}
			}
		} //class BearE

		public class BearH:FormulaBase
		{
			public BearH():base()
			{
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData LC=REF(C,1); LC.Name="LC";
				FormulaData LO=REF(O,1); LO.Name="LO";
				FormulaData NONAME0=ABS(LO-LC)/LO>0.05 & MIN(LC,LO)<MIN(C,O) & MAX(LC,LO)>MAX(C,O);
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Bearish Harami";}
			}
		
			public override string Description
			{
				get{return "The first candlestick has a long body that is white or black.\n\nThe second candlestick has a short body that is white or black and is nestled within the body of the first candlestick.";}
			}
		} //class BearH

		public class TBC:FormulaBase
		{
			public TBC():base()
			{
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=ISDOWN & REF(ISDOWN,1) & REF(ISDOWN,2);
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Three Black Crows";}
			}
		
			public override string Description
			{
				get{return "Three black crows is a bearish reversal pattern that forms with three consecutive long black candlesticks. After an advance, the three black crows pattern signals a change in sentiment and reversal of trend from bullish to bearish. Further bearish confirmation is not required, but there is sometimes a test of resistance established by the reversal.";}
			}
		} //class TBC

		public class RTM:FormulaBase
		{
			public RTM():base()
			{
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=C>REF(C,1) & REF(C,1)>REF(C,2);
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Rising Three Methods";}
			}
		
			public override string Description
			{
				get{return "Stocks that have a Rising Three Methods pattern at the end of their daily chart.\n";}
			}
		} //class RTM

		public class FTM:FormulaBase
		{
			public FTM():base()
			{
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=C<REF(C,1) & REF(C,1)<REF(C,2);
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Falling Three Methods";}
			}
		
			public override string Description
			{
				get{return "Stocks that have a Falling Three Methods pattern at the end of their daily chart";}
			}
		} //class FTM

		public class DDoji:FormulaBase
		{
			public DDoji():base()
			{
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=BETWEEN((C-O)/O,0,0.005) & (H-C)<(O-L);
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Dragonfly Doji";}
			}
		
			public override string Description
			{
				get{return "Stocks that have a Dragonfly Doji pattern at the end of their daily chart.\n";}
			}
		} //class DDoji

		public class GDoji:FormulaBase
		{
			public GDoji():base()
			{
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=BETWEEN((O-C)/C,0,0.005) & (H-O)>(C-L);
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Gravestone Doji";}
			}
		
			public override string Description
			{
				get{return "Stocks that have a Gravestone Doji pattern at the end of their daily chart.\n";}
			}
		} //class GDoji

		public class Hammer:FormulaBase
		{
			public Hammer():base()
			{
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=BETWEEN(ABS(O-C)/C,0,0.02) & (H-MAX(O,C))<(MIN(C,O)-L);
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Hammer, Hanging Man";}
			}
		
			public override string Description
			{
				get{return "The resulting candlestick looks like a square lollipop with a long stick. The long stick below represents the long lower shadow that forms from the intraday low. The high for the day is near the open or the close, depending on which of the two is higher.";}
			}
		} //class Hammer

		public class MDS:FormulaBase
		{
			public MDS():base()
			{
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=REF(ISDOWN,2) & REF(ISEQUAL,1) & ISUP;
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Morning Doji Star";}
			}
		
			public override string Description
			{
				get{return "A Morning Doji Star is a 3-candlestick pattern that forms after a decline and marks a potential reversal of trend.\n1.The first candlestick is black and should have a relatively long body.\n2.The middle candlestick is a doji that forms after a gap down on the open.\n3.The third candlestick is white and has a relatively long white body.\n";}
			}
		} //class MDS

		public class TWS:FormulaBase
		{
			public TWS():base()
			{
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=REF(ISUP,2) & REF(ISUP,1) & ISUP;
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Three White Soldiers";}
			}
		
			public override string Description
			{
				get{return "Three white soldiers is a bullish reversal pattern that forms with three consecutive long white candlesticks. After a decline, the three white soldiers pattern signals a change in sentiment and reversal of trend from bearish to bullish. Further bullish confirmation is not required, but there is sometimes a test of support established by the reversal.\n\n";}
			}
		} //class TWS

		public class DCC:FormulaBase
		{
			public DCC():base()
			{
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=REF(ISUP,1) & O>REF(H,1) & C<REF((O+C)/2,1);
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Dark Cloud Cover Pattern (bearish)";}
			}
		
			public override string Description
			{
				get{return "A dark cloud cover pattern is formed by two candlesticks where:\n\n1.The first is a relatively long white candlestick.\n2.The second is a black candlestick that opens above the previous period's high and closes below the mid-point of the long white candlestick's body";}
			}
		} //class DCC

		public class ES:FormulaBase
		{
			public ES():base()
			{
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=REF(ISUP,2) & REF(ISEQUAL,1) & ISDOWN;
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Evening Star (bearish)";}
			}
		
			public override string Description
			{
				get{return "An evening star is a bearish pattern that forms after an advance and marks a reversal of trend. The evening star formation is made up of three candlesticks:\n\n1.The first candlestick should have a relatively long white body.\n2.The middle candlestick can be either black or white and has a relatively small body. It should also form with a gap up on the open.\n3.The third candlestick has a relatively long black body and should form with a gap down on the open.\n\n\n";}
			}
		} //class ES

		public class Shooting:FormulaBase
		{
			public Shooting():base()
			{
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=BETWEEN(ABS(O-C)/C,0,0.02) & (H-MAX(O,C))>(MIN(C,O)-L);
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Shooting Star";}
			}
		
			public override string Description
			{
				get{return "A shooting star candlestick forms when a security advances significantly higher after the open, but gives up most or all of its intraday gain to close well off of its high. As opposed to the hanging man or the hammer, a shooting star looks like an upside down square lollipop with a long stick. Sometimes there will be a short lower shadow, but not always.\n";}
			}
		} //class Shooting

		public class FBC:FormulaBase
		{
			public FBC():base()
			{
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=O==H & L==C & (O-C)/O>0.05;
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Filled Black Candles";}
			}
		
			public override string Description
			{
				get{return "Stocks that have a black, filled-in candlestick at the end of their daily chart.\n";}
			}
		} //class FBC

		public class HRC:FormulaBase
		{
			public HRC():base()
			{
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=O==L & H==C & (C-O)/O>0.05;
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Hollow Red Candles";}
			}
		
			public override string Description
			{
				get{return "Stocks that have a red, hollow candlestick at the end of their daily chart.\n";}
			}
		} //class HRC

		#endregion

		#region Formula Group Others
		public class EMAC:FormulaBase
		{
			public double N=0;
			public double ISLESS=0;
			public EMAC():base()
			{
				AddParam("N","14","0","100000","",FormulaParamType.Double);
				AddParam("IsLess","0","0","2","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=(C > EMA(C,N)) ^ ISLESS;
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "EMA is greater or less than Close";}
			}
		
			public override string Description
			{
				get{return "";}
			}
		} //class EMAC

		public class MAC:FormulaBase
		{
			public double N=0;
			public double ISLESS=0;
			public MAC():base()
			{
				AddParam("N","14","0","100000","",FormulaParamType.Double);
				AddParam("IsLess","0","0","2","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData NONAME0=(C > MA(C,N)) ^ ISLESS;
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "Moving average is greater or less than closs";}
			}
		
			public override string Description
			{
				get{return "";}
			}
		} //class MAC

		public class AtrStat:FormulaBase
		{
			public double NATR=0;
			public double NEMA=0;
			public double FACTOR=0;
			public AtrStat():base()
			{
				AddParam("NAtr","10","0","1000","",FormulaParamType.Double);
				AddParam("NEma","10","0","1000","",FormulaParamType.Double);
				AddParam("Factor","1","0","100","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData M= ATRSAR(NATR,NEMA,FACTOR); M.Name="M";
				FormulaData NONAME0=M>C;
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "ATR sar status";}
			}
		
			public override string Description
			{
				get{return "";}
			}
		} //class AtrStat

		public class AtrDist:FormulaBase
		{
			public double NATR=0;
			public double NEMA=0;
			public double FACTOR=0;
			public AtrDist():base()
			{
				AddParam("NAtr","10","0","1000","",FormulaParamType.Double);
				AddParam("NEma","10","0","1000","",FormulaParamType.Double);
				AddParam("Factor","1","0","100","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData M= ATRSAR(NATR,NEMA,FACTOR); M.Name="M";
				FormulaData NONAME0=(M-C)/C;
				return new FormulaPackage(new FormulaData[]{NONAME0},"");
			}
		
			public override string LongName
			{
				get{return "The distance between atr sar and closing price";}
			}
		
			public override string Description
			{
				get{return "";}
			}
		} //class AtrDist

		#endregion

	} // namespace Scan

} // namespace FML
