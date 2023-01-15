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

namespace ThingsGateway.Foundation.Rpc
{
    /// <summary>
    /// RPC行为过滤器。
    /// </summary>
    public interface IRpcActionFilter
    {
        /// <summary>
        /// 成功执行Rpc后。
        /// <para>如果修改<paramref name="invokeResult"/>的InvokeStatus，或Result。则会影响RPC最终结果</para>
        /// </summary>
        /// <param name="callContext"></param>
        /// <param name="parameters"></param>
        /// <param name="invokeResult"></param>
        void Executed(ICallContext callContext, object[] parameters, ref InvokeResult invokeResult);

        /// <summary>
        /// 成功执行Rpc后。
        /// <para>如果修改<paramref name="invokeResult"/>的InvokeStatus，或Result。则会影响RPC最终结果</para>
        /// </summary>
        /// <param name="callContext"></param>
        /// <param name="parameters"></param>
        /// <param name="invokeResult"></param>
        Task ExecutedAsync(ICallContext callContext, object[] parameters, ref InvokeResult invokeResult);

        /// <summary>
        /// 执行Rpc遇见异常。
        /// <para>如果修改<paramref name="invokeResult"/>的InvokeStatus，或Result。则会影响RPC最终结果</para>
        /// </summary>
        /// <param name="callContext"></param>
        /// <param name="parameters"></param>
        /// <param name="invokeResult"></param>
        /// <param name="exception"></param>
        void ExecutException(ICallContext callContext, object[] parameters, ref InvokeResult invokeResult, Exception exception);

        /// <summary>
        /// 执行Rpc遇见异常。
        /// <para>如果修改<paramref name="invokeResult"/>的InvokeStatus，或Result。则会影响RPC最终结果</para>
        /// </summary>
        /// <param name="callContext"></param>
        /// <param name="parameters"></param>
        /// <param name="invokeResult"></param>
        /// <param name="exception"></param>
        Task ExecutExceptionAsync(ICallContext callContext, object[] parameters, ref InvokeResult invokeResult, Exception exception);

        /// <summary>
        /// 在执行Rpc之前。
        /// <para>当<paramref name="invokeResult"/>的InvokeStatus不为<see cref="InvokeStatus.Ready"/>。则不会执行RPC</para>
        /// <para>同时，当<paramref name="invokeResult"/>的InvokeStatus为<see cref="InvokeStatus.Success"/>。会直接返回结果</para>
        /// </summary>
        /// <param name="callContext"></param>
        /// <param name="parameters"></param>
        /// <param name="invokeResult"></param>
        void Executing(ICallContext callContext, object[] parameters, ref InvokeResult invokeResult);

        /// <summary>
        /// 在执行Rpc之前。
        /// <para>当<paramref name="invokeResult"/>的InvokeStatus不为<see cref="InvokeStatus.Ready"/>。则不会执行RPC</para>
        /// <para>同时，当<paramref name="invokeResult"/>的InvokeStatus为<see cref="InvokeStatus.Success"/>。会直接返回结果</para>
        /// </summary>
        /// <param name="callContext"></param>
        /// <param name="parameters"></param>
        /// <param name="invokeResult"></param>
        Task ExecutingAsync(ICallContext callContext, object[] parameters, ref InvokeResult invokeResult);
    }
}