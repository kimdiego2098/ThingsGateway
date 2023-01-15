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
using System.Threading.Tasks;

namespace ThingsGateway.Foundation.Rpc.TouchRpc
{
    /// <summary>
    /// IDRpcActor
    /// </summary>
    public class IDRpcActor : IRpcClient
    {
        private readonly string m_targetId;
        private readonly IRpcActor m_rpcActor;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="rpcActor"></param>
        public IDRpcActor(string targetId, IRpcActor rpcActor)
        {
            m_targetId = targetId;
            m_rpcActor = rpcActor;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Func<IRpcClient, bool> TryCanInvoke { get => m_rpcActor.TryCanInvoke; set => m_rpcActor.TryCanInvoke = value; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Dispose()
        {
            m_rpcActor.Dispose();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="invokeKey"></param>
        /// <param name="invokeOption"></param>
        /// <param name="parameters"></param>
        public void Invoke(string invokeKey, IInvokeOption invokeOption, params object[] parameters)
        {
            m_rpcActor.Invoke(m_targetId, invokeKey, invokeOption, parameters);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="invokeKey"></param>
        /// <param name="invokeOption"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T Invoke<T>(string invokeKey, IInvokeOption invokeOption, params object[] parameters)
        {
            return m_rpcActor.Invoke<T>(m_targetId, invokeKey, invokeOption, parameters);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="invokeKey"></param>
        /// <param name="invokeOption"></param>
        /// <param name="parameters"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public T Invoke<T>(string invokeKey, IInvokeOption invokeOption, ref object[] parameters, Type[] types)
        {
            return m_rpcActor.Invoke<T>(m_targetId, invokeKey, invokeOption, ref parameters, types);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="invokeKey"></param>
        /// <param name="invokeOption"></param>
        /// <param name="parameters"></param>
        /// <param name="types"></param>
        public void Invoke(string invokeKey, IInvokeOption invokeOption, ref object[] parameters, Type[] types)
        {
            m_rpcActor.Invoke(m_targetId, invokeKey, invokeOption, ref parameters, types);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="invokeKey"></param>
        /// <param name="invokeOption"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Task InvokeAsync(string invokeKey, IInvokeOption invokeOption, params object[] parameters)
        {
            return m_rpcActor.InvokeAsync(m_targetId, invokeKey, invokeOption, parameters);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="invokeKey"></param>
        /// <param name="invokeOption"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Task<T> InvokeAsync<T>(string invokeKey, IInvokeOption invokeOption, params object[] parameters)
        {
            return m_rpcActor.InvokeAsync<T>(m_targetId, invokeKey, invokeOption, parameters);
        }
    }
}