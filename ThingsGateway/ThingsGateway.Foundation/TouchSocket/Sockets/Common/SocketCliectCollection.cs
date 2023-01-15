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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace ThingsGateway.Foundation.Sockets
{
    /// <summary>
    /// 客户端集合
    /// </summary>
    [DebuggerDisplay("Count={Count}")]
    public sealed class SocketClientCollection
    {
        private readonly ConcurrentDictionary<string, ISocketClient> m_tokenDic = new ConcurrentDictionary<string, ISocketClient>();

        /// <summary>
        /// 数量
        /// </summary>
        public int Count => m_tokenDic.Count;

        /// <summary>
        /// 获取SocketClient
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ISocketClient this[string id]
        {
            get
            {
                TryGetSocketClient(id, out ISocketClient t);
                return t;
            }
        }

        /// <summary>
        /// 获取所有的客户端
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ISocketClient> GetClients()
        {
            return m_tokenDic.Values;
        }

        /// <summary>
        /// 获取ID集合
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetIDs()
        {
            return m_tokenDic.Keys;
        }

        /// <summary>
        /// 根据ID判断SocketClient是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool SocketClientExist(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }

            if (m_tokenDic.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 尝试获取实例
        /// </summary>
        /// <param name="id"></param>
        /// <param name="socketClient"></param>
        /// <returns></returns>
        public bool TryGetSocketClient(string id, out ISocketClient socketClient)
        {
            if (string.IsNullOrEmpty(id))
            {
                socketClient = null;
                return false;
            }

            return m_tokenDic.TryGetValue(id, out socketClient);
        }

        /// <summary>
        /// 尝试获取实例
        /// </summary>
        /// <typeparam name="TClient"></typeparam>
        /// <param name="id"></param>
        /// <param name="socketClient"></param>
        /// <returns></returns>
        public bool TryGetSocketClient<TClient>(string id, out TClient socketClient) where TClient : ISocketClient
        {
            if (string.IsNullOrEmpty(id))
            {
                socketClient = default;
                return false;
            }

            if (m_tokenDic.TryGetValue(id, out ISocketClient client))
            {
                socketClient = (TClient)client;
                return true;
            }
            socketClient = default;
            return false;
        }

        internal bool TryAdd(ISocketClient socketClient)
        {
            return m_tokenDic.TryAdd(socketClient.ID, socketClient);
        }

        internal bool TryRemove(string id, out ISocketClient socketClient)
        {
            if (string.IsNullOrEmpty(id))
            {
                socketClient = null;
                return false;
            }
            return m_tokenDic.TryRemove(id, out socketClient);
        }

        internal bool TryRemove<TClient>(string id, out TClient socketClient) where TClient : ISocketClient
        {
            if (string.IsNullOrEmpty(id))
            {
                socketClient = default;
                return false;
            }

            if (m_tokenDic.TryRemove(id, out ISocketClient client))
            {
                socketClient = (TClient)client;
                return true;
            }
            socketClient = default;
            return false;
        }
    }
}