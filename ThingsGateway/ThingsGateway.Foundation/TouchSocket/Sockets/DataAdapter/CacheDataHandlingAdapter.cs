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



namespace ThingsGateway.Foundation.Sockets
{
    /// <summary>
    /// CacheDataHandlingAdapter
    /// </summary>
    public abstract class CacheDataHandlingAdapter : DataHandlingAdapter
    {
        /// <summary>
        /// 缓存数据，如果需要手动释放，请先判断，然后到调用<see cref="ByteBlock.Dispose"/>后，再置空；
        /// </summary>
        protected ByteBlock m_cacheByteBlock;

        /// <summary>
        /// 将数据缓存起来
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        protected void Cache(byte[] buffer, int offset, int length)
        {
            m_cacheByteBlock ??= new ByteBlock(length);
            m_cacheByteBlock.Write(buffer, offset, length);
            if (UpdateCacheTimeWhenRev)
            {
                LastCacheTime = DateTime.Now;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void Reset()
        {
            m_cacheByteBlock.SafeDispose();
            m_cacheByteBlock = null;
        }

        /// <summary>
        /// 获取当前缓存，
        /// 如果缓存超时，或者不存在，均会返回false。
        /// 如果获取成功，则会清空内部缓存。
        /// </summary>
        /// <returns></returns>
        protected bool TryGetCache(out byte[] buffer)
        {
            if (m_cacheByteBlock == null)
            {
                buffer = null;
                return false;
            }
            if (DateTime.Now - LastCacheTime > CacheTimeout)
            {
                m_cacheByteBlock.SafeDispose();
                m_cacheByteBlock = null;
                buffer = null;
                return false;
            }
            buffer = m_cacheByteBlock.ToArray();
            m_cacheByteBlock.SafeDispose();
            m_cacheByteBlock = null;
            return true;
        }

        /// <summary>
        /// 获取缓存，注意：获取的ByteBlock需要手动释放。
        /// </summary>
        /// <param name="byteBlock"></param>
        /// <returns></returns>
        protected bool TryGetCache(out ByteBlock byteBlock)
        {
            if (m_cacheByteBlock == null)
            {
                byteBlock = null;
                return false;
            }
            if (DateTime.Now - LastCacheTime > CacheTimeout)
            {
                m_cacheByteBlock.SafeDispose();
                m_cacheByteBlock = null;
                byteBlock = null;
                return false;
            }
            byteBlock = m_cacheByteBlock;
            return true;
        }
    }
}