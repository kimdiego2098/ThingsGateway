//------------------------------------------------------------------------------
//  此代码版权（除特别声明或在XREF结尾的命名空间的代码）归作者本人若汝棋茗所有
//  源代码使用协议遵循本仓库的开源协议及附加协议，若本仓库没有设置，则按MIT开源协议授权
//  CSDN博客：https://blog.csdn.net/qq_40374647
//  哔哩哔哩视频：https://space.bilibili.com/94253567
//  Gitee源代码仓库：https://gitee.com/RRQM_Home
//  Github源代码仓库：https://github.com/RRQM
//  API首页：https://www.yuque.com/rrqm/touchsocket/index
//  交流QQ群：234762506
//  感谢您的下载和使用
//------------------------------------------------------------------------------
//------------------------------------------------------------------------------

//using ThingsGateway.Foundation;
//using System.Collections.Generic;
//using System.Text;
//using System.Text.RegularExpressions;

//namespace ThingsGateway.Foundation.Sockets
//{
//    /// <summary>
//    /// Json字符串数据处理解析器（该解析器由网友"明月"提供）
//    /// </summary>
//    public class JsonStringDataHandlingAdapter : CustomDataHandlingAdapter<JsonRequestInfo>
//    {
//        private ByteBlock Temp;

//        /// <summary>
//        /// <inheritdoc/>
//        /// </summary>
//        public override bool CanSplicingSend => false;

//        /// <summary>
//        /// 预解析
//        /// </summary>
//        /// <param name="byteBlock"></param>
//        protected override void PreviewReceived(ByteBlock byteBlock)
//        {
//            byte[] buffer = byteBlock.Buffer;
//            int length = byteBlock.Len;

//            //Console.WriteLine("----------------接收的新数据-------------------");
//            if (Temp != null)
//            {
//                Temp.Write(byteBlock.Buffer, 0, length);
//                buffer = Temp.Buffer;
//                length = (int)Temp.Length;
//            }

//            string msg = Encoding.UTF8.GetString(buffer, 0, length);
//            if (msg.Contains("}{"))
//            {
//                //Console.WriteLine("----------------发生粘包-------------------");
//                string[] mes = Regex.Split(msg, "}{");
//                for (int i = 0; i < mes.Length; i++)
//                {
//                    string str = mes[i];
//                    if (i == 0)
//                    {
//                        str += "}";
//                    }
//                    else if (i == mes.Length - 1)
//                    {
//                        str = "{" + str;
//                        int start = StringCount(str, "{");
//                        int end = StringCount(str, "}");
//                        if (start == end)
//                        {
//                            if (Temp != null)
//                            {
//                                Temp = null;
//                            }
//                        }
//                        else
//                        {
//                            byte[] surPlus = Encoding.UTF8.GetBytes(str);

//                            if (Temp != null)
//                            {
//                                Temp = null;
//                            }
//                            //Temp = BytePool.GetByteBlock(1024*1024*10);
//                            Temp = BytePool.GetByteBlock(length);

//                            Temp.Write(surPlus);
//                            //Console.WriteLine("----------------数据不完整-------------------");
//                            break;
//                        }
//                    }
//                    else
//                    {
//                        str = "{" + str + "}";
//                    }
//                    //Console.WriteLine(str);
//                    PreviewHandle(str);
//                }
//            }
//            else if (msg[0] == '{' && msg[1] == '}')
//            {
//                Temp = null;
//                PreviewHandle(msg);
//            }
//            else
//            {
//                if (Temp == null)
//                {
//                    Temp = BytePool.GetByteBlock(length);
//                    Temp.Write(byteBlock.Buffer, 0, length);
//                }

//                Temp.Write(byteBlock.Buffer, 0, length);
//            }
//        }

//        /// <summary>
//        /// 预发送封装
//        /// </summary>
//        /// <param name="buffer"></param>
//        /// <param name="offset"></param>
//        /// <param name="length"></param>
//        /// <param name="isAsync"></param>
//        protected override void PreviewSend(byte[] buffer, int offset, int length, bool isAsync)
//        {
//            this.GoSend(buffer, offset, length, isAsync);
//        }

//        private int StringCount(string source, string match)
//        {
//            int count = 0;
//            if (source.Contains(match))
//            {
//                string temp = source.Replace(match, "");
//                count = (source.Length - temp.Length) / match.Length;
//            }
//            return count;
//        }

//        private void PreviewHandle(string msg)
//        {
//            GoReceived(null, msg);
//        }

//        protected override FilterResult Filter(ByteBlock byteBlock, bool beCached, ref JsonRequestInfo request, ref int tempCapacity)
//        {
//        }
//    }

//    /// <summary>
//    /// json解析
//    /// </summary>
//    public class JsonRequestInfo:IRequestInfo
//    {
//    }
//}