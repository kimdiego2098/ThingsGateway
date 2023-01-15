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
using System.Text;

using System.Threading.Tasks;

namespace ThingsGateway.Foundation.Http
{
    /// <summary>
    /// Http客户端数据处理适配器
    /// </summary>
    public class HttpClientDataHandlingAdapter : NormalDataHandlingAdapter
    {
        /// <summary>
        /// 缓存数据，如果需要手动释放，请先判断，然后到调用<see cref="ByteBlock.Dispose"/>后，再置空；
        /// </summary>
        protected ByteBlock tempByteBlock;

        private static readonly byte[] m_rnCode = Encoding.UTF8.GetBytes("\r\n");
        private HttpResponse m_httpResponse;

        private long m_surLen;

        private Task m_task;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool CanSplicingSend => false;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="byteBlock"></param>
        protected override void PreviewReceived(ByteBlock byteBlock)
        {
            if (tempByteBlock == null)
            {
                byteBlock.Pos = 0;
                Single(byteBlock, false);
            }
            else
            {
                tempByteBlock.Write(byteBlock.Buffer, 0, byteBlock.Len);
                ByteBlock block = tempByteBlock;
                tempByteBlock = null;
                block.Pos = 0;
                Single(block, true);
            }
        }

        private void Cache(ByteBlock byteBlock)
        {
            if (byteBlock.CanReadLen > 0)
            {
                tempByteBlock = new ByteBlock();
                tempByteBlock.Write(byteBlock.Buffer, byteBlock.Pos, byteBlock.CanReadLen);
                if (tempByteBlock.Len > MaxPackageSize)
                {
                    OnError("缓存的数据长度大于设定值的情况下未收到解析信号");
                }
            }
        }

        private FilterResult ReadChunk(ByteBlock byteBlock)
        {
            int position = byteBlock.Pos;
            int index = byteBlock.Buffer.IndexOfFirst(byteBlock.Pos, byteBlock.CanReadLen, m_rnCode);
            if (index > 0)
            {
                int headerLength = index - byteBlock.Pos;
                string hex = Encoding.ASCII.GetString(byteBlock.Buffer, byteBlock.Pos, headerLength - 1);
                int count = hex.ByHexStringToInt32();
                byteBlock.Pos += headerLength + 1;

                if (count >= 0)
                {
                    if (count > byteBlock.CanReadLen)
                    {
                        byteBlock.Pos = position;
                        return FilterResult.Cache;
                    }

                    m_httpResponse.InternalInput(byteBlock.Buffer, byteBlock.Pos, count);
                    byteBlock.Pos += count;
                    byteBlock.Pos += 2;
                    return FilterResult.GoOn;
                }
                else
                {
                    byteBlock.Pos += 2;
                    return FilterResult.Success;
                }
            }
            else
            {
                return FilterResult.Cache;
            }
        }

        private void Single(ByteBlock byteBlock, bool dis)
        {
            try
            {
                while (byteBlock.CanReadLen > 0)
                {
                    if (m_httpResponse == null)
                    {
                        m_httpResponse = new HttpResponse(Client, false);
                        if (m_httpResponse.ParsingHeader(byteBlock, byteBlock.CanReadLen))
                        {
                            byteBlock.Pos++;
                            if (m_httpResponse.IsChunk || m_httpResponse.ContentLength > byteBlock.CanReadLength)
                            {
                                m_surLen = m_httpResponse.ContentLength;
                                m_task = EasyTask.Run(m_httpResponse, (res) =>
                                {
                                    GoReceived(null, res);
                                });
                            }
                            else
                            {
                                byteBlock.Read(out byte[] buffer, (int)m_httpResponse.ContentLength);
                                m_httpResponse.SetContent(buffer);
                                GoReceived(null, m_httpResponse);
                                m_httpResponse = null;
                            }
                        }
                        else
                        {
                            Cache(byteBlock);
                            m_httpResponse = null;
                            m_task?.Wait();
                            m_task = null;
                            return;
                        }
                    }
                    if (m_httpResponse != null)
                    {
                        if (m_httpResponse.IsChunk)
                        {
                            switch (ReadChunk(byteBlock))
                            {
                                case FilterResult.Cache:
                                    Cache(byteBlock);
                                    return;

                                case FilterResult.Success:
                                    m_httpResponse = null;
                                    m_task?.Wait();
                                    m_task = null;
                                    break;

                                case FilterResult.GoOn:
                                default:
                                    break;
                            }
                        }
                        else if (m_surLen > 0)
                        {
                            if (byteBlock.CanRead)
                            {
                                int len = (int)Math.Min(m_surLen, byteBlock.CanReadLength);
                                m_httpResponse.InternalInput(byteBlock.Buffer, byteBlock.Pos, len);
                                m_surLen -= len;
                                byteBlock.Pos += len;
                                if (m_surLen == 0)
                                {
                                    m_httpResponse.InternalInput(null, 0, 0);
                                    m_httpResponse = null;
                                    m_task?.Wait();
                                    m_task = null;
                                }
                            }
                        }
                        else
                        {
                            m_httpResponse = null;
                            m_task?.Wait();
                            m_task = null;
                        }
                    }
                }
            }
            finally
            {
                if (dis)
                {
                    byteBlock.Dispose();
                }
            }
        }
    }
}