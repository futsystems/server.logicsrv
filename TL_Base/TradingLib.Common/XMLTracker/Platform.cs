using System;

namespace TradingLib.Common
{
#if ANDRIOD
	public class Platform
	{
		public static  string SecListFN=@"config\secList.xml";
		public static  string SecurityFN = @"config\security.xml";
	}
#else
	public class Platform
	{
		public static  string SecListFN=@"config\secList.xml";
		public static  string SecurityFN = @"config\security.xml";
	}


#endif
}

