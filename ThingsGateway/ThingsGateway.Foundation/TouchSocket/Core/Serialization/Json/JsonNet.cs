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

namespace ThingsGateway.Foundation.Core
{
    /// <summary>
    /// 提供Json.net无引用调用。
    /// <para>该代码来源：https://www.cnblogs.com/kewei/p/8228343.html</para>
    /// </summary>
    internal static class JsonNet
    {
        /// <summary>
        /// Json.Net程序集名称
        /// </summary>
        private static readonly string jsonNetAssemblyName = "Newtonsoft.Json";

        /// <summary>
        /// JsonConvert类名
        /// </summary>
        private static readonly string jsonNetJsonConvertTypeName = "Newtonsoft.Json.JsonConvert";

        /// <summary>
        /// 序列化方法的委托
        /// </summary>
        private static Func<object, string> serializeFunc = null;

        /// <summary>
        /// 反序列化方法的委托
        /// </summary>
        private static Func<string, Type, object> deserializeFunc = null;

        /// <summary>
        /// 获取是否得到支持
        /// </summary>
        public static bool IsSupported { get; private set; } = false;

        /// <summary>
        /// Json.net
        /// </summary>
        static JsonNet()
        {
            AppDomain.CurrentDomain.AssemblyLoad += (s, e) => InitJsonNet(e.LoadedAssembly);
            InitJsonNet(AppDomain.CurrentDomain.GetAssemblies());
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static string SerializeObject(object obj)
        {
            return JsonNet.serializeFunc.Invoke(obj);
        }

        /// <summary>
        /// 反序列化为对象
        /// </summary>
        /// <param name="json">json文本</param>
        /// <param name="type">对象类型</param>
        /// <returns></returns>
        public static object DeserializeObject(string json, Type type)
        {
            return JsonNet.deserializeFunc.Invoke(json, type);
        }

        /// <summary>
        /// 初始化json.net
        /// </summary>
        /// <param name="assemblies">查找的程序集</param>
        private static void InitJsonNet(params Assembly[] assemblies)
        {
            if (JsonNet.IsSupported == true)
            {
                return;
            }

            var jsonNetAssembly = assemblies
                .FirstOrDefault(item => item.GetName().Name.Equals(jsonNetAssemblyName, StringComparison.OrdinalIgnoreCase));

            if (jsonNetAssembly == null)
            {
                return;
            }

            var jsonConvertType = jsonNetAssembly.GetType(jsonNetJsonConvertTypeName, false);
            if (jsonConvertType == null)
            {
                return;
            }

            serializeFunc = CreateSerializeObjectFunc(jsonConvertType);
            deserializeFunc = CreateDeserializeObjectFunc(jsonConvertType);
            JsonNet.IsSupported = serializeFunc != null && deserializeFunc != null;
        }

        public static bool InitJsonNet(Type jsonConvertType)
        {
            serializeFunc = CreateSerializeObjectFunc(jsonConvertType);
            deserializeFunc = CreateDeserializeObjectFunc(jsonConvertType);
            return JsonNet.IsSupported = serializeFunc != null && deserializeFunc != null;
        }

        /// <summary>
        /// 创建SerializeObject方法的委托
        /// </summary>
        /// <param name="classType">JsonConvert类型</param>
        /// <returns></returns>
        private static Func<object, string> CreateSerializeObjectFunc(Type classType)
        {
            var method = classType.GetMethod("SerializeObject", new[] { typeof(object) });
            if (method == null)
            {
                return null;
            }
            return (Func<object, string>)method.CreateDelegate(typeof(Func<object, string>));
        }

        /// <summary>
        /// 创建DeserializeObject方法的委托
        /// </summary>
        /// <param name="classType">JsonConvert类型</param>
        /// <returns></returns>
        private static Func<string, Type, object> CreateDeserializeObjectFunc(Type classType)
        {
            var method = classType.GetMethod("DeserializeObject", new[] { typeof(string), typeof(Type) });
            if (method == null)
            {
                return null;
            }
            return (Func<string, Type, object>)method.CreateDelegate(typeof(Func<string, Type, object>));
        }
    }
}