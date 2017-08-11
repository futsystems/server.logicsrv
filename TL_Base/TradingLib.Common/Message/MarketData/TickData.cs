using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class TickDataNotify : NotifyResponsePacket
    {
        public TickDataNotify()
        {
            _type = MessageTypes.XTICKNOTIFY2;
            this.TickData = new List<TickData>();
        }

        public List<TickData> TickData { get; set; }
        public override byte[] Data { get { return this.SerializeBin(); } }

        public override string Content { get { return "TickData Notify"; } }

          /// <summary>
        /// 二进制数据反序列化
        /// </summary>
        /// <param name="data"></param>
        public override void DeserializeBin(byte[] data)
        {
            byte[] rawData = data;// ZlibNet.Decompress(data);
            using (MemoryStream ms = new MemoryStream(rawData))
            {
                using (BinaryReader reader = new BinaryReader(ms))
                {
                    while (ms.Position < ms.Length)
                    {
                        TickDataImpl k = TickDataImpl.Read(reader);
                        this.TickData.Add(k);
                    }
                }
            }
        }


        
        
        
        /// <summary>
        /// 序列化成二进制
        /// </summary>
        /// <returns></returns>
        public override byte[] SerializeBin()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter b = new BinaryWriter(ms))
                {

                    for (int i = 0; i < this.TickData.Count; i++)
                    {
                        TickDataImpl.Write(b, this.TickData[i]);
                    }
                   
                    byte[] rawData = ms.ToArray();
                    int size = (int)rawData.Length + Message.HEADERSIZE;
                    byte[] buffer = new byte[size];

                    byte[] sizebyte = BitConverter.GetBytes(size);
                    byte[] typebyte = BitConverter.GetBytes((int)this.Type);

                    Array.Copy(sizebyte, 0, buffer, Message.LENGTHOFFSET, sizebyte.Length);
                    Array.Copy(typebyte, 0, buffer, Message.TYPEOFFSET, typebyte.Length);
                    Array.Copy(rawData, 0, buffer, Message.HEADERSIZE, rawData.Length);
                    return buffer;
                }
            }
        }


        /// <summary>
        /// 序列化成字符串 由子类提供序列化函数
        /// </summary>
        /// <returns></returns>
        public override string Serialize()
        {
            throw new NotImplementedException();
        }

        

        /// <summary>
        /// 反序列化成对象
        /// </summary>
        /// <param name="reqstr"></param>
        /// <returns></returns>
        public override  void Deserialize(string reqstr)
        {
            throw new NotImplementedException();
        }


       
    }
}
