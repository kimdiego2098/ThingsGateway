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

namespace ThingsGateway.Foundation.Rpc.XmlRpc
{
    /// <summary>
    /// 适用于XmlRpc的标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class XmlRpcAttribute : RpcAttribute
    {
        /// <summary>
        ///  适用于XmlRpc的标记.
        ///  <para>是否仅以函数名调用，当为True是，调用时仅需要传入方法名即可。</para>
        /// </summary>
        /// <param name="methodInvoke"></param>
        public XmlRpcAttribute(bool methodInvoke = false)
        {
            MethodInvoke = methodInvoke;
        }

        /// <summary>
        /// 适用于XmlRpc的标记.
        /// </summary>
        /// <param name="invokenKey"></param>
        public XmlRpcAttribute(string invokenKey)
        {
            InvokenKey = invokenKey;
        }

        /// <summary>
        /// 是否仅以函数名调用，当为True是，调用时仅需要传入方法名即可。
        /// </summary>
        public bool MethodInvoke { get; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override Type[] GetGenericInterfaceTypes()
        {
            return new Type[] { typeof(IXmlRpcClient) };
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="methodInstance"></param>
        /// <returns></returns>
        public override string GetInvokenKey(MethodInstance methodInstance)
        {
            if (MethodInvoke)
            {
                return GetMethodName(methodInstance, false);
            }
            else
            {
                return base.GetInvokenKey(methodInstance);
            }
        }
    }
}