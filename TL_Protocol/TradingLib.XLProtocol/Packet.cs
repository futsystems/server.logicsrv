//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Reflection;
//using System.Runtime.InteropServices;


//namespace TradingLib.XLProtocol
//{

//    public class demo
//    {
//        /// <summary>
//        /// 客户端发送Packet 将请求结构体与请求编号打包成Message
//        /// </summary>
//        /// <param name="requestField"></param>
//        /// <param name="requestID"></param>
//        /// <returns></returns>
//        public static Message CliSendPacket(object requestField, int requestID)
//        {
//            if(requestField is XLReqLoginField) return new Message((int)XLMessageType.T_RSP_LOGIN,SerializeRequest(requestField,requestID));

//            return new Message();
//        }

//        /// <summary>
//        /// 服务端接受Packet 将数据解析成请求结构体与请求编号
//        /// </summary>
//        /// <param name="message"></param>
//        /// <param name="requestField"></param>
//        /// <param name="requestID"></param>
//        public static void SrvRecvPacket(Message message, out object requestField, out int requestID)
//        {
//            switch (message.MessageType)
//            {
//                case (int)XLMessageType.T_REQ_LOGIN:
//                    {
//                        XLReqLoginField reqField;
//                        DeserializeRequest<XLReqLoginField>(message.MessageBody, out reqField, out requestID);
//                        requestField = reqField;
//                        break;
//                    }
//                default:
//                    break;
//            }
//            requestField = null;
//            requestID = 0;
//        }








        
//        #region Request
//        /// <summary>
//        /// 客户端接口提交请求
//        /// 按Request(RequestField,RequestID)的格式进行
//        /// </summary>
//        /// <param name="requestField"></param>
//        /// <param name="requestID"></param>
//        /// <returns></returns>
//        public static byte[] SerializeRequest(object requestField, int requestID)
//        {
//            //得到结构体的大小  
//            int size = Marshal.SizeOf(requestField);
//            byte[] reqIDByte = BitConverter.GetBytes(requestID);

//            //创建byte数组  
//            byte[] bytes = new byte[size + reqIDByte.Length];
//            //分配结构体大小的内存空间  
//            IntPtr structPtr = Marshal.AllocHGlobal(size);
//            //将结构体拷到分配好的内存空间  
//            Marshal.StructureToPtr(requestField, structPtr, false);
//            //从内存空间拷到byte数组  
//            Marshal.Copy(structPtr, bytes, 0, size);
//            //设定RequestID
//            Array.Copy(reqIDByte, 0, bytes, size, reqIDByte.Length);
//            //释放内存空间  
//            Marshal.FreeHGlobal(structPtr);
//            //返回byte数组  
//            return bytes;
//        }

//        /// <summary>
//        /// 请求消息内容解析成对应的请求结构体
//        /// </summary>
//        /// <param name="requestData"></param>
//        /// <param name="requestField"></param>
//        /// <param name="requestId"></param>
//        public static void DeserializeRequest<T>(byte[] requestData, out T requestField, out int requestId)
//        {
//            Type type = typeof(T);
//            //得到结构体的大小  
//            int size = Marshal.SizeOf(type);
//            //byte数组长度小于结构体的大小  
//            if (size > requestData.Length)
//            {
//                //返回空  
//                requestField = default(T);
//                requestId = 0;
//            }
//            //分配结构体大小的内存空间  
//            IntPtr structPtr = Marshal.AllocHGlobal(size);
//            //将byte数组拷到分配好的内存空间  
//            Marshal.Copy(requestData, 0, structPtr, size);
//            //将内存空间转换为目标结构体  
//            requestField = (T)Marshal.PtrToStructure(structPtr, type);
//            //
//            requestId = (int)BitConverter.ToInt32(requestData,size);
//            //释放内存空间  
//            Marshal.FreeHGlobal(structPtr);
//        }
//        #endregion



//        #region Response
//        /// <summary>
//        /// 客户端接口请求应答
//        /// 按Response(ResponseField,ErrorField,RequestID)的格式进行
//        /// </summary>
//        /// <param name="responseField"></param>
//        /// <param name="errorField"></param>
//        /// <param name="requestID"></param>
//        /// <returns></returns>
//        public static byte[] SerializeResponse(object responseField, ErrorField errorField, int requestID)
//        {
//            //得到结构体的大小  
//            int size = Marshal.SizeOf(responseField);
//            int errorSize = Marshal.SizeOf(errorField);
//            byte[] reqIDByte = BitConverter.GetBytes(requestID);

//            //创建byte数组  
//            byte[] bytes = new byte[size + errorSize + reqIDByte.Length];
//            //序列化Response
//            //分配结构体大小的内存空间  
//            IntPtr structPtr = Marshal.AllocHGlobal(size);
//            //将结构体拷到分配好的内存空间  
//            Marshal.StructureToPtr(responseField, structPtr, false);
//            //从内存空间拷到byte数组  
//            Marshal.Copy(structPtr, bytes, 0, size);

//            //序列化ErrorField
//            //分配结构体大小的内存空间  
//            IntPtr errStructPtr = Marshal.AllocHGlobal(errorSize);
//            //将结构体拷到分配好的内存空间  
//            Marshal.StructureToPtr(errorField, errStructPtr, false);
//            //从内存空间拷到byte数组  
//            Marshal.Copy(errStructPtr, bytes, size, errorSize);


//            //设定RequestID
//            Array.Copy(reqIDByte, 0, bytes, size + errorSize, reqIDByte.Length);
//            //释放内存空间  
//            Marshal.FreeHGlobal(structPtr);
//            Marshal.FreeHGlobal(errStructPtr);
//            //返回byte数组  
//            return bytes;
//        }




//        /// <summary>
//        /// 回报消息内容解析成对应的回报结构体
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="responseData"></param>
//        /// <param name="responseField"></param>
//        /// <param name="errorField"></param>
//        /// <param name="requestId"></param>
//        public static void DeserializeResponse<T>(byte[] responseData, out T responseField, out ErrorField errorField, out int requestId)
//        {
//            Type type = typeof(T);
//            Type errType = typeof(ErrorField);
//            //得到结构体的大小  
//            int size = Marshal.SizeOf(type);
//            int errorSize = Marshal.SizeOf(errType);
//            //byte数组长度小于结构体的大小  
//            if (size + errorSize  + 4 > responseData.Length)
//            {
//                //返回空  
//                responseField = default(T);
//                errorField = new ErrorField();
//                requestId = 0;
//            }
//            //分配结构体大小的内存空间  
//            IntPtr structPtr = Marshal.AllocHGlobal(size);
//            //将byte数组拷到分配好的内存空间  
//            Marshal.Copy(responseData, 0, structPtr, size);

//            //分配结构体大小的内存空间  
//            IntPtr errStructPtr = Marshal.AllocHGlobal(errorSize);
//            //将byte数组拷到分配好的内存空间  
//            Marshal.Copy(responseData,size, errStructPtr, errorSize);

//            //将内存空间转换为目标结构体  
//            responseField = (T)Marshal.PtrToStructure(structPtr, type);
//            //
//            errorField = (ErrorField)Marshal.PtrToStructure(errStructPtr, errType);
//            //
//            requestId = (int)BitConverter.ToInt32(responseData, size + errorSize);
//            //释放内存空间  
//            Marshal.FreeHGlobal(structPtr);
//            Marshal.FreeHGlobal(errStructPtr);
//        }
//        #endregion

//        #region Notify
//        /// <summary>
//        /// 结构体转换成Byte
//        /// </summary>
//        /// <param name="structObj"></param>
//        /// <returns></returns>
//        public static byte[] SerializeNotify(object notify)
//        {
//            //得到结构体的大小  
//            int size = Marshal.SizeOf(notify);
//            //创建byte数组  
//            byte[] bytes = new byte[size];
//            //分配结构体大小的内存空间  
//            IntPtr structPtr = Marshal.AllocHGlobal(size);
//            //将结构体拷到分配好的内存空间  
//            Marshal.StructureToPtr(notify, structPtr, false);
//            //从内存空间拷到byte数组  
//            Marshal.Copy(structPtr, bytes, 0, size);
//            //释放内存空间  
//            Marshal.FreeHGlobal(structPtr);
//            //返回byte数组  
//            return bytes;
//        }



//        /// <summary>
//        /// Byte转换成结构体
//        /// </summary>
//        /// <param name="bytes"></param>
//        /// <param name="type"></param>
//        /// <returns></returns>
//        public static void DeserializeNotify<T>(byte[] bytes,out T notify)
//        {
//            Type type = typeof(T);
//            //得到结构体的大小  
//            int size = Marshal.SizeOf(type);
//            //byte数组长度小于结构体的大小  
//            if (size > bytes.Length)
//            {
//                //返回空  
//                notify =  default(T);
//            }
//            //分配结构体大小的内存空间  
//            IntPtr structPtr = Marshal.AllocHGlobal(size);
//            //将byte数组拷到分配好的内存空间  
//            Marshal.Copy(bytes, 0, structPtr, size);
//            //将内存空间转换为目标结构体  
//            notify = (T)Marshal.PtrToStructure(structPtr, type);
//            //释放内存空间  
//            Marshal.FreeHGlobal(structPtr);
//        }
//        #endregion


//    }
//}
