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

namespace ThingsGateway.Foundation.Http.WebSockets
{
    /// <summary>
    /// IWSClientBase辅助扩展
    /// </summary>
    public static class WSClientExtensions
    {
        /// <summary>
        /// 是否已经完成握手
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static bool GetHandshaked(this IHttpClientBase client)
        {
            return client.GetValue<bool>(WebSocketServerPlugin.HandshakedProperty);
        }

        /// <summary>
        /// 获取WebSocket版本号。
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static string GetWebSocketVersion(this IHttpClientBase client)
        {
            return client.GetValue<string>(WebSocketServerPlugin.WebSocketVersionProperty);
        }

        /// <summary>
        /// 设置WebSocket版本号。
        /// </summary>
        public static void SetWebSocketVersion(this IHttpClientBase client, string value)
        {
            client.SetValue(WebSocketServerPlugin.WebSocketVersionProperty, value);
        }

        #region 客户端

        /// <summary>
        /// 发送Ping报文。
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static bool Ping(this HttpClientBase client)
        {
            try
            {
                SendWithWS(client, new WSDataFrame() { FIN = true, Opcode = WSDataType.Ping });
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 发送Pong报文。
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static bool Pong(this HttpClientBase client)
        {
            try
            {
                SendWithWS(client, new WSDataFrame() { FIN = true, Opcode = WSDataType.Pong });
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 发送Close报文。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool CloseWithWS(this HttpClientBase client, string msg)
        {
            try
            {
                SendWithWS(client, new WSDataFrame() { FIN = true, Opcode = WSDataType.Close }.AppendText(msg));
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region 同步发送

        /// <summary>
        /// 采用WebSocket协议，发送二进制流数据。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public static void SendWithWS(this HttpClientBase client, byte[] buffer, int offset, int length)
        {
            using (ByteBlock byteBlock = new ByteBlock(length + 1024))
            {
                WSDataFrame dataFrame = new WSDataFrame();
                dataFrame.AppendBinary(buffer, offset, length).BuildRequest(byteBlock);
                client.DefaultSend(byteBlock.Buffer, 0, byteBlock.Len);
            }
        }

        /// <summary>
        /// 采用WebSocket协议，发送二进制流数据。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="byteBlock"></param>
        public static void SendWithWS(this HttpClientBase client, ByteBlock byteBlock)
        {
            SendWithWS(client, byteBlock.Buffer, 0, byteBlock.Len);
        }

        /// <summary>
        /// 采用WebSocket协议，发送二进制流数据。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="buffer"></param>
        public static void SendWithWS(this HttpClientBase client, byte[] buffer)
        {
            SendWithWS(client, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 采用WebSocket协议，发送文本数据。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="text"></param>
        public static void SendWithWS(this HttpClientBase client, string text)
        {
            using (ByteBlock byteBlock = new ByteBlock(text.Length + 1024))
            {
                WSDataFrame dataFrame = new WSDataFrame();
                dataFrame.AppendText(text).BuildRequest(byteBlock);
                client.DefaultSend(byteBlock.Buffer, 0, byteBlock.Len);
            }
        }

        /// <summary>
        /// 采用WebSocket协议，发送WS数据。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="dataFrame"></param>
        public static void SendWithWS(this HttpClientBase client, WSDataFrame dataFrame)
        {
            using (ByteBlock byteBlock = new ByteBlock(dataFrame.PayloadLength + 1024))
            {
                dataFrame.BuildRequest(byteBlock);
                client.DefaultSend(byteBlock.Buffer, 0, byteBlock.Len);
            }
        }

        #endregion 同步发送

        #region 异步发送

        /// <summary>
        /// 采用WebSocket协议，发送二进制流数据。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public static void SendWithWSAsync(this HttpClientBase client, byte[] buffer, int offset, int length)
        {
            using (ByteBlock byteBlock = new ByteBlock(length + 1024))
            {
                WSDataFrame dataFrame = new WSDataFrame();
                dataFrame.AppendBinary(buffer, offset, length).BuildRequest(byteBlock);
                client.DefaultSendAsync(byteBlock.Buffer, 0, byteBlock.Len);
            }
        }

        /// <summary>
        /// 采用WebSocket协议，发送二进制流数据。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="byteBlock"></param>
        public static void SendWithWSAsync(this HttpClientBase client, ByteBlock byteBlock)
        {
            SendWithWSAsync(client, byteBlock.Buffer, 0, byteBlock.Len);
        }

        /// <summary>
        /// 采用WebSocket协议，发送二进制流数据。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="buffer"></param>
        public static void SendWithWSAsync(this HttpClientBase client, byte[] buffer)
        {
            SendWithWSAsync(client, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 采用WebSocket协议，发送文本数据。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="text"></param>
        public static void SendWithWSAsync(this HttpClientBase client, string text)
        {
            using (ByteBlock byteBlock = new ByteBlock(text.Length + 1024))
            {
                WSDataFrame dataFrame = new WSDataFrame();
                dataFrame.AppendText(text).BuildRequest(byteBlock);
                client.DefaultSendAsync(byteBlock.Buffer, 0, byteBlock.Len);
            }
        }

        /// <summary>
        /// 采用WebSocket协议，发送WS数据。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="dataFrame"></param>
        public static void SendWithWSAsync(this HttpClientBase client, WSDataFrame dataFrame)
        {
            using (ByteBlock byteBlock = new ByteBlock(dataFrame.PayloadLength + 1024))
            {
                dataFrame.BuildRequest(byteBlock);
                client.DefaultSendAsync(byteBlock.Buffer, 0, byteBlock.Len);
            }
        }

        #endregion 异步发送

        #region 同步分包发送

        /// <summary>
        /// 分包发送。
        /// <para>
        /// 按packageSize的值，每次发送数据包。
        /// </para>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="packageSize"></param>
        public static void SubSendWithWS(this HttpClientBase client, byte[] buffer, int offset, int length, int packageSize)
        {
            lock (client)
            {
                if (packageSize >= length)
                {
                    SendWithWS(client, buffer, offset, length);
                    return;
                }
                int sentLen = 0;
                WSDataFrame dataFrame = new WSDataFrame();
                dataFrame.SetMaskString("RRQM");
                dataFrame.Mask = true;
                dataFrame.FIN = false;
                dataFrame.Opcode = WSDataType.Binary;

                int count;
                if (length % packageSize == 0)
                {
                    count = length / packageSize;
                }
                else
                {
                    count = (length / packageSize) + 1;
                }

                for (int i = 0; i < count; i++)
                {
                    if (i > 0)
                    {
                        dataFrame.Opcode = WSDataType.Cont;
                    }
                    if (i == count - 1)//最后
                    {
                        dataFrame.FIN = true;
                    }
                    using (ByteBlock byteBlock = new ByteBlock(packageSize + 1024))
                    {
                        int thisLen = Math.Min(packageSize, length - sentLen);
                        WSTools.Build(byteBlock, dataFrame, buffer, offset, thisLen);
                        client.DefaultSend(byteBlock.Buffer, 0, byteBlock.Len);
                        sentLen += thisLen;
                        offset += thisLen;
                    }
                }
            }
        }

        /// <summary>
        /// 分包发送。
        /// <para>
        /// 按packageSize的值，每次发送数据包。
        /// </para>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="buffer"></param>
        /// <param name="packageSize"></param>
        public static void SubSendWithWS(this HttpClientBase client, byte[] buffer, int packageSize)
        {
            SubSendWithWS(client, buffer, 0, buffer.Length, packageSize);
        }

        /// <summary>
        /// 分包发送。
        /// <para>
        /// 按packageSize的值，每次发送数据包。
        /// </para>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="byteBlock"></param>
        /// <param name="packageSize"></param>
        public static void SubSendWithWS(this HttpClientBase client, ByteBlock byteBlock, int packageSize)
        {
            SubSendWithWS(client, byteBlock.Buffer, 0, byteBlock.Len, packageSize);
        }

        #endregion 同步分包发送

        #region 异步分包发送

        /// <summary>
        /// 分包发送。
        /// <para>
        /// 按packageSize的值，每次发送数据包。
        /// </para>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="packageSize"></param>
        public static void SubSendWithWSAsync(this HttpClientBase client, byte[] buffer, int offset, int length, int packageSize)
        {
            lock (client)
            {
                if (packageSize >= length)
                {
                    SendWithWSAsync(client, buffer, offset, length);
                    return;
                }
                int sentLen = 0;
                WSDataFrame dataFrame = new WSDataFrame();
                dataFrame.SetMaskString("RRQM");
                dataFrame.Mask = true;
                dataFrame.FIN = false;
                dataFrame.Opcode = WSDataType.Binary;

                int count;
                if (length % packageSize == 0)
                {
                    count = length / packageSize;
                }
                else
                {
                    count = (length / packageSize) + 1;
                }

                for (int i = 0; i < count; i++)
                {
                    if (i > 0)
                    {
                        dataFrame.Opcode = WSDataType.Cont;
                    }
                    if (i == count - 1)//最后
                    {
                        dataFrame.FIN = true;
                    }
                    using (ByteBlock byteBlock = new ByteBlock(packageSize + 1024))
                    {
                        int thisLen = Math.Min(packageSize, length - sentLen);
                        WSTools.Build(byteBlock, dataFrame, buffer, offset, thisLen);
                        client.DefaultSendAsync(byteBlock.Buffer, 0, byteBlock.Len);
                        sentLen += thisLen;
                        offset += thisLen;
                    }
                }
            }
        }

        /// <summary>
        /// 分包发送。
        /// <para>
        /// 按packageSize的值，每次发送数据包。
        /// </para>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="buffer"></param>
        /// <param name="packageSize"></param>
        public static void SubSendWithWSAsync(this HttpClientBase client, byte[] buffer, int packageSize)
        {
            SubSendWithWSAsync(client, buffer, 0, buffer.Length, packageSize);
        }

        /// <summary>
        /// 分包发送。
        /// <para>
        /// 按packageSize的值，每次发送数据包。
        /// </para>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="byteBlock"></param>
        /// <param name="packageSize"></param>
        public static void SubSendWithWSAsync(this HttpClientBase client, ByteBlock byteBlock, int packageSize)
        {
            SubSendWithWSAsync(client, byteBlock.Buffer, 0, byteBlock.Len, packageSize);
        }

        #endregion 异步分包发送

        #endregion 客户端

        #region 服务器

        /// <summary>
        /// 发送Ping报文。
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static bool Ping(this HttpSocketClient client)
        {
            try
            {
                SendWithWS(client, new WSDataFrame() { FIN = true, Opcode = WSDataType.Ping });
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 发送Pong报文。
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static bool Pong(this HttpSocketClient client)
        {
            try
            {
                SendWithWS(client, new WSDataFrame() { FIN = true, Opcode = WSDataType.Pong });
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 发送Close报文。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool CloseWithWS(this HttpSocketClient client, string msg)
        {
            try
            {
                SendWithWS(client, new WSDataFrame() { FIN = true, Opcode = WSDataType.Close }.AppendText(msg));
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region 同步发送

        /// <summary>
        /// 采用WebSocket协议，发送二进制流数据。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public static void SendWithWS(this HttpSocketClient client, byte[] buffer, int offset, int length)
        {
            using (ByteBlock byteBlock = new ByteBlock(length + 1024))
            {
                WSDataFrame dataFrame = new WSDataFrame();
                dataFrame.AppendBinary(buffer, offset, length).BuildResponse(byteBlock);
                client.DefaultSend(byteBlock.Buffer, 0, byteBlock.Len);
            }
        }

        /// <summary>
        /// 采用WebSocket协议，发送二进制流数据。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="byteBlock"></param>
        public static void SendWithWS(this HttpSocketClient client, ByteBlock byteBlock)
        {
            SendWithWS(client, byteBlock.Buffer, 0, byteBlock.Len);
        }

        /// <summary>
        /// 采用WebSocket协议，发送二进制流数据。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="buffer"></param>
        public static void SendWithWS(this HttpSocketClient client, byte[] buffer)
        {
            SendWithWS(client, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 采用WebSocket协议，发送文本数据。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="text"></param>
        public static void SendWithWS(this HttpSocketClient client, string text)
        {
            using (ByteBlock byteBlock = new ByteBlock(text.Length + 1024))
            {
                WSDataFrame dataFrame = new WSDataFrame();
                dataFrame.AppendText(text).BuildResponse(byteBlock);
                client.DefaultSend(byteBlock.Buffer, 0, byteBlock.Len);
            }
        }

        /// <summary>
        /// 采用WebSocket协议，发送WS数据。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="dataFrame"></param>
        public static void SendWithWS(this HttpSocketClient client, WSDataFrame dataFrame)
        {
            using (ByteBlock byteBlock = new ByteBlock(dataFrame.PayloadLength + 1024))
            {
                dataFrame.BuildResponse(byteBlock);
                client.DefaultSend(byteBlock.Buffer, 0, byteBlock.Len);
            }
        }

        #endregion 同步发送

        #region 异步发送

        /// <summary>
        /// 采用WebSocket协议，发送二进制流数据。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public static void SendWithWSAsync(this HttpSocketClient client, byte[] buffer, int offset, int length)
        {
            using (ByteBlock byteBlock = new ByteBlock(length + 1024))
            {
                WSDataFrame dataFrame = new WSDataFrame();
                dataFrame.AppendBinary(buffer, offset, length).BuildResponse(byteBlock);
                client.DefaultSendAsync(byteBlock.Buffer, 0, byteBlock.Len);
            }
        }

        /// <summary>
        /// 采用WebSocket协议，发送二进制流数据。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="byteBlock"></param>
        public static void SendWithWSAsync(this HttpSocketClient client, ByteBlock byteBlock)
        {
            SendWithWSAsync(client, byteBlock.Buffer, 0, byteBlock.Len);
        }

        /// <summary>
        /// 采用WebSocket协议，发送二进制流数据。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="buffer"></param>
        public static void SendWithWSAsync(this HttpSocketClient client, byte[] buffer)
        {
            SendWithWSAsync(client, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 采用WebSocket协议，发送文本数据。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="text"></param>
        public static void SendWithWSAsync(this HttpSocketClient client, string text)
        {
            using (ByteBlock byteBlock = new ByteBlock(text.Length + 1024))
            {
                WSDataFrame dataFrame = new WSDataFrame();
                dataFrame.AppendText(text).BuildResponse(byteBlock);
                client.DefaultSendAsync(byteBlock.Buffer, 0, byteBlock.Len);
            }
        }

        /// <summary>
        /// 采用WebSocket协议，发送WS数据。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="dataFrame"></param>
        public static void SendWithWSAsync(this HttpSocketClient client, WSDataFrame dataFrame)
        {
            using (ByteBlock byteBlock = new ByteBlock(dataFrame.PayloadLength + 1024))
            {
                dataFrame.BuildResponse(byteBlock);
                client.DefaultSendAsync(byteBlock.Buffer, 0, byteBlock.Len);
            }
        }

        #endregion 异步发送

        #region 同步分包发送

        /// <summary>
        /// 分包发送。
        /// <para>
        /// 按packageSize的值，每次发送数据包。
        /// </para>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="packageSize"></param>
        public static void SubSendWithWS(this HttpSocketClient client, byte[] buffer, int offset, int length, int packageSize)
        {
            lock (client)
            {
                if (packageSize >= length)
                {
                    SendWithWS(client, buffer, offset, length);
                    return;
                }
                int sentLen = 0;
                WSDataFrame dataFrame = new WSDataFrame();
                dataFrame.FIN = false;
                dataFrame.Opcode = WSDataType.Binary;

                int count;
                if (length % packageSize == 0)
                {
                    count = length / packageSize;
                }
                else
                {
                    count = (length / packageSize) + 1;
                }

                for (int i = 0; i < count; i++)
                {
                    if (i > 0)
                    {
                        dataFrame.Opcode = WSDataType.Cont;
                    }
                    if (i == count - 1)//最后
                    {
                        dataFrame.FIN = true;
                    }
                    using (ByteBlock byteBlock = new ByteBlock(packageSize + 1024))
                    {
                        int thisLen = Math.Min(packageSize, length - sentLen);
                        WSTools.Build(byteBlock, dataFrame, buffer, offset, thisLen);
                        client.DefaultSend(byteBlock.Buffer, 0, byteBlock.Len);
                        sentLen += thisLen;
                        offset += thisLen;
                    }
                }
            }
        }

        /// <summary>
        /// 分包发送。
        /// <para>
        /// 按packageSize的值，每次发送数据包。
        /// </para>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="buffer"></param>
        /// <param name="packageSize"></param>
        public static void SubSendWithWS(this HttpSocketClient client, byte[] buffer, int packageSize)
        {
            SubSendWithWS(client, buffer, 0, buffer.Length, packageSize);
        }

        /// <summary>
        /// 分包发送。
        /// <para>
        /// 按packageSize的值，每次发送数据包。
        /// </para>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="byteBlock"></param>
        /// <param name="packageSize"></param>
        public static void SubSendWithWS(this HttpSocketClient client, ByteBlock byteBlock, int packageSize)
        {
            SubSendWithWS(client, byteBlock.Buffer, 0, byteBlock.Len, packageSize);
        }

        #endregion 同步分包发送

        #region 异步分包发送

        /// <summary>
        /// 分包发送。
        /// <para>
        /// 按packageSize的值，每次发送数据包。
        /// </para>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="packageSize"></param>
        public static void SubSendWithWSAsync(this HttpSocketClient client, byte[] buffer, int offset, int length, int packageSize)
        {
            lock (client)
            {
                if (packageSize >= length)
                {
                    SendWithWSAsync(client, buffer, offset, length);
                    return;
                }
                int sentLen = 0;
                WSDataFrame dataFrame = new WSDataFrame();
                dataFrame.FIN = false;
                dataFrame.Opcode = WSDataType.Binary;

                int count;
                if (length % packageSize == 0)
                {
                    count = length / packageSize;
                }
                else
                {
                    count = (length / packageSize) + 1;
                }

                for (int i = 0; i < count; i++)
                {
                    if (i > 0)
                    {
                        dataFrame.Opcode = WSDataType.Cont;
                    }
                    if (i == count - 1)//最后
                    {
                        dataFrame.FIN = true;
                    }
                    using (ByteBlock byteBlock = new ByteBlock(packageSize + 1024))
                    {
                        int thisLen = Math.Min(packageSize, length - sentLen);
                        WSTools.Build(byteBlock, dataFrame, buffer, offset, thisLen);
                        client.DefaultSendAsync(byteBlock.Buffer, 0, byteBlock.Len);
                        sentLen += thisLen;
                        offset += thisLen;
                    }
                }
            }
        }

        /// <summary>
        /// 分包发送。
        /// <para>
        /// 按packageSize的值，每次发送数据包。
        /// </para>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="buffer"></param>
        /// <param name="packageSize"></param>
        public static void SubSendWithWSAsync(this HttpSocketClient client, byte[] buffer, int packageSize)
        {
            SubSendWithWSAsync(client, buffer, 0, buffer.Length, packageSize);
        }

        /// <summary>
        /// 分包发送。
        /// <para>
        /// 按packageSize的值，每次发送数据包。
        /// </para>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="byteBlock"></param>
        /// <param name="packageSize"></param>
        public static void SubSendWithWSAsync(this HttpSocketClient client, ByteBlock byteBlock, int packageSize)
        {
            SubSendWithWSAsync(client, byteBlock.Buffer, 0, byteBlock.Len, packageSize);
        }

        #endregion 异步分包发送

        #endregion 服务器
    }
}