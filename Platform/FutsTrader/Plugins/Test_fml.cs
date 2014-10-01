using Easychart.Finance;
using Easychart.Finance.DataProvider;
using System.Reflection;
using System.Runtime.CompilerServices;
[assembly: AssemblyVersion("1.0.0.8")]
namespace FML
{
	public class TestScan:FormulaBase
	{
		public double N=0;
		public double M=0;
		public TestScan():base()
		{
			AddParam("N","90","1","1000","",FormulaParamType.Double);
			AddParam("M","20","1","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=C<LLV(C,3) & MA(V,N)>50000 & BETWEEN(C,1,10) & ABS(C-FML(DP,"BB(M,M/10)[LOWER]"))<0.1;
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class TestScan

	public class TestOrgData:FormulaBase
	{
		public string FN="";
		public double XN=0;
		public TestOrgData():base()
		{
			AddParam("FN","Close","0","0","",FormulaParamType.String);
			AddParam("XN","12","1","100","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData MYIV=ORGDATA(FN); MYIV.Name="MYIV";MYIV.SetAttrs("HighQuality");
			FormulaData IVC= SMA(MYIV,XN,1); IVC.Name="IVC";
			return new FormulaPackage(new FormulaData[]{MYIV,IVC},"");
		}
	
		public override string LongName
		{
			get{return "OrgData";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class TestOrgData

	public class TestPartColor:FormulaBase
	{
		public TestPartColor():base()
		{
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=C;
			FormulaData NONAME1=MA(C,20);NONAME1.SetAttrs("UpColorRed,DownColorGreen,Width2,HighQuality");
			return new FormulaPackage(new FormulaData[]{NONAME0,NONAME1},"");
		}
	
		public override string LongName
		{
			get{return "Support up color and download of line";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class TestPartColor

	public class TwoDiff:FormulaBase
	{
		public string SYMBOL2="";
		public TwoDiff():base()
		{
			AddParam("Symbol2","MSFT","0","0","",FormulaParamType.String);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData C1=FML(SYMBOL2,"C")-C; C1.Name="C1";
			FormulaData H1=FML(SYMBOL2,"H")-H; H1.Name="H1";
			FormulaData L1=FML(SYMBOL2,"L")-L; L1.Name="L1";
			FormulaData O1=FML(SYMBOL2,"O")-O; O1.Name="O1";
			FormulaData NONAME0=GETSTOCK(O1,C1,MAX(C1,O1,H1,L1),MIN(C1,O1,H1,L1));
			
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Difference of Two Symbol";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class TwoDiff

	public class CustomTrade:FormulaBase
	{
		public double N=0;
		public CustomTrade():base()
		{
			AddParam("N","1","1","3","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData M=ORGDATA("M"+N); M.Name="M";
			FormulaData NONAME0=DRAWICON(M==1,L,"EnterLong.GIF");NONAME0.SetAttrs("BOTTOM");
			FormulaData NONAME1=DRAWICON(M==2,H,"EnterShort.GIF");NONAME1.SetAttrs("GOP");
			FormulaData NONAME2=DRAWICON(M==3,H,"ExitLong.GIF");NONAME2.SetAttrs("TOP");
			FormulaData NONAME3=DRAWICON(M==4,L,"ExitShort.GIF");NONAME3.SetAttrs("BOTTOM");
			SETTEXTVISIBLE(FALSE);
			return new FormulaPackage(new FormulaData[]{NONAME0,NONAME1,NONAME2,NONAME3},"");
		}
	
		public override string LongName
		{
			get{return "Trade sign from database";}
		}
	
		public override string Description
		{
			get{return "Load EnterLong,EnterShort,ExitLong,ExitShort from database";}
		}
	} //class CustomTrade

	public class VolChange:FormulaBase
	{
		public double N=0;
		public VolChange():base()
		{
			AddParam("N","50","1","1000","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData NONAME0=V/MA(C,N)-1;NONAME0.SetAttrs("ColorRed");
			SETNAME("Vol%Change");
			return new FormulaPackage(new FormulaData[]{NONAME0},"");
		}
	
		public override string LongName
		{
			get{return "Vol Change";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class VolChange

	public class PartStockColor:FormulaBase
	{
		public double N=0;
		public PartStockColor():base()
		{
			AddParam("N","13","0","1000","",FormulaParamType.Double);
		}
	
		public override FormulaPackage Run(IDataProvider DP)
		{
			this.DataProvider = DP;
			FormulaData M=MA(C,N); M.Name="M";
			SETNAME(M,"MA"+N);
			FormulaData A1=STOCK; A1.Name="A1";A1.SetAttrs("MonoStock,BrushGreen,ColorGreen");
			FormulaData A2=IF(C>M,STOCK,NAN); A2.Name="A2";A2.SetAttrs("MonoStock,BrushRed,ColorRed");
			SETTEXTVISIBLE(A2,FALSE);
			SETNAME(A1,"C");
			SETTEXTVISIBLE(FALSE);
			return new FormulaPackage(new FormulaData[]{M,A1,A2},"");
		}
	
		public override string LongName
		{
			get{return "";}
		}
	
		public override string Description
		{
			get{return "";}
		}
	} //class PartStockColor

} // namespace FML
