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
    /// 响应数据。
    /// </summary>
    public struct ResponsedData
    {
        private readonly byte[] m_data;
        private readonly IRequestInfo m_requestInfo;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="data"></param>
        /// <param name="requestInfo"></param>
        public ResponsedData(byte[] data, IRequestInfo requestInfo)
        {
            m_data = data;
            m_requestInfo = requestInfo;
        }

        /// <summary>
        /// 数据
        /// </summary>
        public byte[] Data => m_data;

        /// <summary>
        /// RequestInfo
        /// </summary>
        public IRequestInfo RequestInfo => m_requestInfo;
    }
}