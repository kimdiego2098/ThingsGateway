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
namespace ThingsGateway.Foundation.Core
{
    /// <summary>
    /// FastBinary转换器
    /// </summary>
    public interface IFastBinaryConverter
    {
        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public object Read(byte[] buffer, int offset, int len);

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="byteBlock"></param>
        /// <param name="obj"></param>
        public int Write(ByteBlock byteBlock, object obj);
    }

    /// <summary>
    /// FastBinary转换器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class FastBinaryConverter<T> : IFastBinaryConverter
    {
        int IFastBinaryConverter.Write(ByteBlock byteBlock, object obj)
        {
            return Write(byteBlock, (T)obj);
        }

        object IFastBinaryConverter.Read(byte[] buffer, int offset, int len)
        {
            return Read(buffer, offset, len);
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="byteBlock"></param>
        /// <param name="obj"></param>
        protected abstract int Write(ByteBlock byteBlock, T obj);

        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        protected abstract T Read(byte[] buffer, int offset, int len);
    }
}