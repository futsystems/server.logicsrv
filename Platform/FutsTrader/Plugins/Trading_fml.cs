using Easychart.Finance;
using Easychart.Finance.DataProvider;
namespace FML
{
	namespace Trading
	{
		public class RSI:FormulaBase
		{
			public double N=0;
			public RSI():base()
			{
				AddParam("N","14","1","100","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData ENTERLONG=CROSS(FML(DP,"RSI(N)"),30); ENTERLONG.Name="EnterLong";
				FormulaData EXITLONG=CROSS(70,FML(DP,"RSI(N)")); EXITLONG.Name="ExitLong";
				return new FormulaPackage(new FormulaData[]{ENTERLONG,EXITLONG},"");
			}
		
			public override string LongName
			{
				get{return "RSI Trading System";}
			}
		
			public override string Description
			{
				get{return "RSI Trading system";}
			}
		} //class RSI

		public class MACD:FormulaBase
		{
			public double N1=0;
			public double N2=0;
			public double N3=0;
			public MACD():base()
			{
				AddParam("N1","26","1","100","",FormulaParamType.Double);
				AddParam("N2","12","1","100","",FormulaParamType.Double);
				AddParam("N3","9","1","100","",FormulaParamType.Double);
			}
		
			public override FormulaPackage Run(IDataProvider DP)
			{
				this.DataProvider = DP;
				FormulaData ENTERLONG=LONGCROSS(FML(DP,"MACD(N1,N2,N3)"),0.2,3); ENTERLONG.Name="EnterLong";
				FormulaData EXITLONG=LONGCROSS(-0.2,FML(DP,"MACD(N1,N2,N3)"),3); EXITLONG.Name="ExitLong";
				return new FormulaPackage(new FormulaData[]{ENTERLONG,EXITLONG},"");
			}
		
			public override string LongName
			{
				get{return "MACD Trading System";}
			}
		
			public override string Description
			{
				get{return "MACD Trading System";}
			}
		} //class MACD

	} // namespace Trading

} // namespace FML
