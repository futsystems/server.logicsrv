using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using System.Xml.Serialization;

namespace TradingLib.Quant.Base
{
    public class SymbolItemManager
    {
        // Fields
        private WatchListItem rootitem;

        // Methods
        public WatchListItem LoadSymbolItems()
        {
            TextReader textReader = null;
            try
            {
                string path = Path.Combine(BaseGlobals.CONFIGPATH, BaseGlobals.SYMBOLCONFIGFILENAME);
                while (!File.Exists(path))
                {
                    File.Copy(Path.Combine(BaseGlobals.CONFIGPATH, "SymbolDefaults.xml"), path);
                    break;
                }
                textReader = new StreamReader(path);
                XmlSerializer serializer = new XmlSerializer(typeof(WatchListItem));
                this.rootitem = (WatchListItem)serializer.Deserialize(textReader);
            }
            catch (FileNotFoundException exception)
            {
                string message = exception.Message;
                this.rootitem = new WatchListItem(true);
                this.rootitem.Folder.FolderName = "代码列表";
                this.SaveSymbolItems();
            }
            finally
            {
                if (textReader != null)
                {
                    textReader.Close();
                }
            }
            return this.rootitem;
        }

        public void SaveSymbolItems()
        {
            TextWriter textWriter = null;
            try
            {
                textWriter = new StreamWriter(Path.Combine(BaseGlobals.CONFIGPATH, BaseGlobals.SYMBOLCONFIGFILENAME));
                new XmlSerializer(typeof(WatchListItem)).Serialize(textWriter, this.rootitem);
            }
            finally
            {
                if (textWriter != null)
                {
                    textWriter.Close();
                }
            }
        }

        // Properties
        public WatchListItem RootItem
        {
            get
            {
                return this.rootitem;
            }
            set
            {
                this.rootitem = value;
            }
        }
    }


}
