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
using System.Collections.Specialized;

namespace ThingsGateway.Foundation.Http
{
    /// <summary>
    /// 表单文件
    /// </summary>
    public interface IFormFile
    {
        /// <summary>
        /// 获取Content-Disposition
        /// </summary>
        string ContentDisposition { get; }

        /// <summary>
        /// 获取Content-Type
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// 实际的数据
        /// </summary>
        public byte[] Data { get; }

        /// <summary>
        /// 数据对
        /// </summary>
        public NameValueCollection DataPair { get; }

        /// <summary>
        /// 获取file name
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// 文件长度。在数据接收完成之前，该值为-1;
        /// </summary>
        long Length { get; }

        /// <summary>
        ///  获取name字段
        /// </summary>
        string Name { get; }

        ///// <summary>
        ///// 读取文件数据  //太麻烦先不实现
        ///// </summary>
        //public int Read(byte[] buffer, int offset, int count);
    }

    internal class InternalFormFile : IFormFile
    {
        public string ContentDisposition => DataPair["Content-Disposition"];

        public string ContentType => DataPair["Content-Type"];

        public byte[] Data { get; set; }
        public NameValueCollection DataPair { get; set; }
        public string FileName => DataPair["filename"];
        public long Length => Data == null ? 0 : Data.Length;

        public string Name => DataPair["name"];
        //public int Read(byte[] buffer, int offset, int count)
        //{
        //    return this.ReadAction(buffer, offset, count);
        //}

        //public Func<byte[], int, int, int> ReadAction { get; set; }
    }
}