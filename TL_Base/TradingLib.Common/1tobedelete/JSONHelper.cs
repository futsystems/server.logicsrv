//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Runtime.Serialization.Json;
//using System.Runtime.Serialization;
//using System.IO;

//namespace TradingLib.Common
//{
//    public  class JSONHelper
//    {
//        /// <summary>
//        /// Json序列化,用于发送到客户端
//        /// </summary>
//        /// <param name="obj"></param>
//        /// <returns></returns>
//        public static string ToJSONString(Object obj)
//        {
//            var serializer = new DataContractJsonSerializer(obj.GetType());
//            var stream = new MemoryStream();
//            serializer.WriteObject(stream, obj);

//            byte[] dataBytes = new byte[stream.Length];

//            stream.Position = 0;

//            stream.Read(dataBytes, 0, (int)stream.Length);

//            string dataString = Encoding.UTF8.GetString(dataBytes);

//            return dataString;
//            /*
//             //序列化object -> json 
//             DataContractJsonSerializer serializer = new DataContractJsonSerializer(item.GetType());

//            using (MemoryStream ms = new MemoryStream())
//            {

//                serializer.WriteObject(ms, item);

//                StringBuilder sb = new StringBuilder();

//                sb.Append(Encoding.UTF8.GetString(ms.ToArray()));

//                return sb.ToString();

//            }**/
//        }



//        /// <summary>
//        /// Json反序列化,用于接收客户端Json后生成对应的对象
//        /// </summary>
//        public static T FromJSONString<T>(string jsonString)
//        {

//            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));

//            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));

//            T jsonObject = (T)ser.ReadObject(ms);

//            ms.Close();

//            return jsonObject;

//        }
       

        
//    }
//}
