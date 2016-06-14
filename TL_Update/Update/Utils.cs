using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Update
{
    class Utils
    {
        const string CONFIG_FILE = "update_config.ini";
        const string SELECTION_CONFIG = "CONFIG";
        const string KEY_IPADDRESS = "_ADDRESS";
        const string KEY_UNIT = "_UNIT";
        const string KEY_APPNAMEUPDATE = "_APPNAMEUPDATE";
        const string KEY_PORT = "_PORT";
        const string KEY_APPNAME = "_APPNAME";
        const string KEY_AUTOCLOSE="_AUTOCLOSE";
        public const string UPDATE_FILE = "update_info.xml";
        public static Ant.Component.Progress FileProgress = new Ant.Component.Progress(298, 24);
        public static Ant.Component.Progress TotalProgress = new Ant.Component.Progress(298, 24);
        public static Ant.Component.INIClass INI;

        public static string Unit
        {
            get
            {
                return INI.IniReadValue(SELECTION_CONFIG, KEY_UNIT);
            }
            set
            {
                INI.IniWriteValue(SELECTION_CONFIG, KEY_UNIT, value);
            }
        }
        public static string AppNameUpdate
        {
            get
            {
                return INI.IniReadValue(SELECTION_CONFIG, KEY_APPNAMEUPDATE);
            }
            set
            {
                INI.IniWriteValue(SELECTION_CONFIG, KEY_APPNAMEUPDATE, value);
            }
        }
        public static string IPAddress
        {
            get
            {
                return INI.IniReadValue(SELECTION_CONFIG, KEY_IPADDRESS);
            }
            set
            {
                INI.IniWriteValue(SELECTION_CONFIG, KEY_IPADDRESS, value);
            }
        }
        public static string Port
        {
            get
            {
                return INI.IniReadValue(SELECTION_CONFIG, KEY_PORT);
            }
            set
            {
                INI.IniWriteValue(SELECTION_CONFIG, KEY_PORT, value);
            }
        }
        public static string AppName
        {
            get
            {
                return INI.IniReadValue(SELECTION_CONFIG, KEY_APPNAME);
            }
            set
            {
                INI.IniWriteValue(SELECTION_CONFIG, KEY_APPNAME, value);
            }
        }
        public static string AutoClose
        {
            get
            {
                return INI.IniReadValue(SELECTION_CONFIG, KEY_AUTOCLOSE);
            }
            set
            {
                INI.IniWriteValue(SELECTION_CONFIG, KEY_AUTOCLOSE, value);
            }
        }
        public static string GetFileFullName(string name)
        {
            return Application.StartupPath + "\\" + name;
        }
        
        public static void LoadINI()
        {
            INI = new Ant.Component.INIClass(GetFileFullName(CONFIG_FILE));
        }
    }
}
