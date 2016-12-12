//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace TradingLib.XLProtocol
//{

//    public struct Message
//    {
//        /// <summary>
//        /// 消息类别
//        /// </summary>
//        public int MessageType;

//        /// <summary>
//        /// 消息体
//        /// </summary>
//        public byte[] MessageBody;


//        public Message(int messageType, byte[] messageBody)
//        {
//            this.MessageType = messageType;
//            this.MessageBody = messageBody;
//        }


//        // 4 bytes for length, 4 bytes for type
//        const int HEADERSIZE = 8;
//        const int LENGTHOFFSET = 0;
//        const int TYPEOFFSET = 4;

//        /// <summary>
//        /// 发送消息
//        /// </summary>
//        /// <param name="type"></param>
//        /// <param name="body"></param>
//        /// <returns></returns>
//        public static byte[] SendMessage(int type, byte[] body)
//        {
//            try
//            {
//                int size = HEADERSIZE + body.Length;
//                var data = new byte[size];
//                byte[] sizebyte = BitConverter.GetBytes(size);
//                byte[] typebyte = BitConverter.GetBytes(type);
//                Array.Copy(sizebyte, 0, data, LENGTHOFFSET, sizebyte.Length);
//                Array.Copy(typebyte, 0, data, TYPEOFFSET, typebyte.Length);
//                Array.Copy(body, 0, data, HEADERSIZE, body.Length);
//                return data;
//            }
//            catch (Exception ex)
//            {
//                return new byte[0];
//            }
//        }

//        public static Message[] RecvMessages(ref byte[] data, ref int offset)
//        {

//            // save original length
//            int orglen = data.Length;
//            // prepare to hold a sequence of messages from a buffer
//            List<Message> msgs = new List<Message>();
//            // keep track of length of all read messages
//            int totallen = 0;
//            bool msgok = true;
//            int start = 0;
//            // prepare vars to hold per-message attributes
//            int mt = 0;
//            byte[] md = null;
//            int ml = 0;
//            bool done = false;
//            // fetch all messages we can get
//            while (msgok)
//            {
//                // fetch a message and record success
//                msgok = RecvMessage(start, data, ref mt,ref md, ref ml, ref done);
//                // if we got a message
//                if (msgok)
//                {
//                    // save it
//                    msgs.Add(new Message(mt, md));
//                    // save total length of messages we've read
//                    totallen += ml;
//                }
//                // update next start
//                start += ml;
//            }
//            // set flag if partial message is left
//            bool partial = totallen != data.Length;
//            // set index of any partial message
//            if (!done && partial)
//            {
//                // copy
//                int partialidx = totallen;
//                // save partial length as offset
//                offset = data.Length - partialidx;
//                // move any partial data to front of buffer
//                byte[] pdata = new byte[orglen];
//                Array.Copy(data, partialidx, pdata, 0, offset);
//                data = pdata;
//            }
//            else
//            {
//                //reset buffer and offset
//                data = new byte[data.Length];
//                offset = 0;
//            }
//            // return messages we found
//            return msgs.ToArray();
//        }


//        static bool RecvMessage(int startidx, byte[] data, ref int type,ref byte[] msgdata, ref int msglen, ref bool done)
//        {
//            try
//            {
//                // ensure we have enough room to store header
//                if (startidx + HEADERSIZE >= data.Length)
//                    return false;
//                // get type from message
//                type = (int)BitConverter.ToInt32(data, startidx + TYPEOFFSET);
//                // get length of message
//                msglen = BitConverter.ToInt32(data, startidx + LENGTHOFFSET);
//                // ensure it's valid message
//                if (msglen < HEADERSIZE)
//                {
//                    //done = (msglen == 0) && (type == MessageTypes.OK);
//                    return false;
//                }
//                // ensure we have enough data for message body
//                if (startidx + msglen > data.Length)
//                    return false;

//                msgdata = new byte[msglen - HEADERSIZE];
//                Array.Copy(data, startidx + HEADERSIZE, msgdata, 0, msglen - HEADERSIZE);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                return false;
//            }
//        }

    
//    }
//}
