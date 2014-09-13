using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if UNIX

#else
using System.Management;
using System.Management.Instrumentation;
using System.Windows.Forms;
#endif

namespace TradingLib.Common
{
    public class HardWareCode
    {
        public static string GetHardwareCode()  
        {
			
#if UNIX
			//unix do not implagte the managementclass to get hardware,so we need to implage another method in linux
			
			return "LINUX";
			
#else
			
            //cpu序列号
            string cpuInfo = "";  
            ManagementClass cimobject = new ManagementClass("Win32_Processor");  
            ManagementObjectCollection moc = cimobject.GetInstances();  
            foreach(ManagementObject mo in moc)  
            {  
             cpuInfo = mo.Properties["ProcessorId"].Value.ToString();  
            }  

            //获取硬盘ID  
            String HDid="";  
            ManagementClass cimobject1 = new ManagementClass("Win32_DiskDrive");  
            ManagementObjectCollection moc1 = cimobject1.GetInstances();  
            foreach(ManagementObject mo in moc1)  
            {  
             HDid = (string)mo.Properties["Model"].Value;  
             //Response.Write ("硬盘序列号："+HDid.ToString ());  
             //MessageBox.Show(HDid);
            }

            return cpuInfo +"+" +HDid;
#endif
    }
   

 
  
    }
}
