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
using System.Linq;


using System.Reflection;

namespace ThingsGateway.Foundation.Rpc
{
    /// <summary>
    /// Rpc函数实例
    /// </summary>
    public class MethodInstance : Method
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="methodInfo"></param>
        public MethodInstance(MethodInfo methodInfo) : base(methodInfo)
        {
        }

        /// <summary>
        /// 服务实例工厂
        /// </summary>
        public IRpcServerFactory ServerFactory { get; internal set; }

        /// <summary>
        /// 描述属性
        /// </summary>
        public string Description { get; internal set; }

        /// <summary>
        /// 筛选器
        /// </summary>
        public IRpcActionFilter[] Filters { get; internal set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// 是否为单例
        /// </summary>
        public bool IsSingleton { get; internal set; }

        /// <summary>
        /// 函数标识
        /// </summary>
        public MethodFlags MethodFlags { get; internal set; }

        /// <summary>
        /// 参数名集合
        /// </summary>
        public string[] ParameterNames { get; internal set; }

        /// <summary>
        /// 参数集合
        /// </summary>
        public ParameterInfo[] Parameters { get; internal set; }

        /// <summary>
        /// 参数类型集合，已处理out及ref，无参数时为空集合，
        /// </summary>
        public Type[] ParameterTypes { get; internal set; }

        /// <summary>
        /// Rpc属性集合
        /// </summary>
        public RpcAttribute[] RpcAttributes { get; internal set; }

        /// <summary>
        /// 实例类型
        /// </summary>
        public Type ServerType { get; internal set; }

        /// <summary>
        /// 获取指定类型属性标签
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetAttribute<T>()
        {
            object attribute = RpcAttributes.FirstOrDefault((a) => { return typeof(T).IsAssignableFrom(a.GetType()); });
            if (attribute == null)
            {
                return default;
            }
            return (T)attribute;
        }

        /// <summary>
        /// 获取指定类型属性标签
        /// </summary>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        public object GetAttribute(Type attributeType)
        {
            object attribute = RpcAttributes.FirstOrDefault((a) => { return attributeType.IsAssignableFrom(a.GetType()); });
            if (attribute == null)
            {
                return default;
            }
            return attribute;
        }
    }
}