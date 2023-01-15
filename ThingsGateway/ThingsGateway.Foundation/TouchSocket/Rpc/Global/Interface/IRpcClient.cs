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

using ThingsGateway.Foundation.Rpc.TouchRpc;

namespace ThingsGateway.Foundation.Rpc
{
    /// <summary>
    /// Rpc接口
    /// </summary>
    public interface IRpcClient : IDisposable
    {
        /// <summary>
        /// 检验能否执行Rpc调用
        /// </summary>
        Func<IRpcClient, bool> TryCanInvoke { get; set; }

        /// <summary>
        /// Rpc调用
        /// <para>如果调用端为客户端，则会调用服务器Rpc服务。</para>
        /// <para>如果调用端为服务器，则会反向调用客户端Rpc服务。</para>
        /// </summary>
        /// <param name="invokeKey">调用键</param>
        /// <param name="parameters">参数</param>
        /// <param name="invokeOption">Rpc调用设置</param>
        /// <exception cref="TimeoutException">调用超时</exception>
        /// <exception cref="RpcSerializationException">序列化异常</exception>
        /// <exception cref="RpcInvokeException">Rpc异常</exception>
        /// <exception cref="RpcNoRegisterException">Rpc服务器未注册</exception>
        /// <exception cref="Exception">其他异常</exception>
        void Invoke(string invokeKey, IInvokeOption invokeOption, params object[] parameters);

        /// <summary>
        /// Rpc调用
        /// <para>如果调用端为客户端，则会调用服务器Rpc服务。</para>
        /// <para>如果调用端为服务器，则会反向调用客户端Rpc服务。</para>
        /// </summary>
        /// <param name="invokeKey">调用键</param>
        /// <param name="parameters">参数</param>
        /// <param name="invokeOption">Rpc调用设置</param>
        /// <exception cref="TimeoutException">调用超时</exception>
        /// <exception cref="RpcSerializationException">序列化异常</exception>
        /// <exception cref="RpcInvokeException">Rpc异常</exception>
        /// <exception cref="RpcNoRegisterException">Rpc服务器未注册</exception>
        /// <exception cref="Exception">其他异常</exception>
        /// <returns>服务器返回结果</returns>
        T Invoke<T>(string invokeKey, IInvokeOption invokeOption, params object[] parameters);

        /// <summary>
        /// Rpc调用
        /// <para>如果调用端为客户端，则会调用服务器Rpc服务。</para>
        /// <para>如果调用端为服务器，则会反向调用客户端Rpc服务。</para>
        /// </summary>
        /// <param name="invokeKey">调用键</param>
        /// <param name="parameters">参数</param>
        /// <param name="types">对应类型集合</param>
        /// <param name="invokeOption">Rpc调用设置</param>
        /// <exception cref="TimeoutException">调用超时</exception>
        /// <exception cref="RpcSerializationException">序列化异常</exception>
        /// <exception cref="RpcInvokeException">Rpc异常</exception>
        /// <exception cref="RpcNoRegisterException">Rpc服务器未注册</exception>
        /// <exception cref="Exception">其他异常</exception>
        /// <returns>返回值</returns>
        T Invoke<T>(string invokeKey, IInvokeOption invokeOption, ref object[] parameters, Type[] types);

        /// <summary>
        /// Rpc调用
        /// </summary>
        /// <param name="invokeKey">调用键</param>
        /// <param name="parameters">参数</param>
        /// <param name="types"></param>
        /// <param name="invokeOption">Rpc调用设置</param>
        /// <exception cref="TimeoutException">调用超时</exception>
        /// <exception cref="RpcSerializationException">序列化异常</exception>
        /// <exception cref="RpcInvokeException">Rpc异常</exception>
        /// <exception cref="RpcNoRegisterException">Rpc服务器未注册</exception>
        /// <exception cref="Exception">其他异常</exception>
        void Invoke(string invokeKey, IInvokeOption invokeOption, ref object[] parameters, Type[] types);

        /// <summary>
        /// Rpc调用
        /// <para>如果调用端为客户端，则会调用服务器Rpc服务。</para>
        /// <para>如果调用端为服务器，则会反向调用客户端Rpc服务。</para>
        /// </summary>
        /// <param name="invokeKey">调用键</param>
        /// <param name="parameters">参数</param>
        /// <param name="invokeOption">Rpc调用设置</param>
        /// <exception cref="TimeoutException">调用超时</exception>
        /// <exception cref="RpcSerializationException">序列化异常</exception>
        /// <exception cref="RpcInvokeException">Rpc异常</exception>
        /// <exception cref="RpcNoRegisterException">Rpc服务器未注册</exception>
        /// <exception cref="Exception">其他异常</exception>
        Task InvokeAsync(string invokeKey, IInvokeOption invokeOption, params object[] parameters);

        /// <summary>
        /// Rpc调用
        /// <para>如果调用端为客户端，则会调用服务器Rpc服务。</para>
        /// <para>如果调用端为服务器，则会反向调用客户端Rpc服务。</para>
        /// </summary>
        /// <param name="invokeKey">调用键</param>
        /// <param name="parameters">参数</param>
        /// <param name="invokeOption">Rpc调用设置</param>
        /// <exception cref="TimeoutException">调用超时</exception>
        /// <exception cref="RpcSerializationException">序列化异常</exception>
        /// <exception cref="RpcInvokeException">Rpc异常</exception>
        /// <exception cref="RpcNoRegisterException">Rpc服务器未注册</exception>
        /// <exception cref="Exception">其他异常</exception>
        /// <returns>服务器返回结果</returns>
        Task<T> InvokeAsync<T>(string invokeKey, IInvokeOption invokeOption, params object[] parameters);
    }
}