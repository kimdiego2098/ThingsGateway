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
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace ThingsGateway.Foundation.Core
{
    /// <summary>
    /// Task类型
    /// </summary>
    public enum TaskReturnType
    {
        /// <summary>
        /// 没有Task
        /// </summary>
        None,

        /// <summary>
        /// 仅返回Task
        /// </summary>
        Task,

        /// <summary>
        /// 返回Task的值
        /// </summary>
        TaskObject
    }

    /// <summary>
    /// 表示方法
    /// </summary>
    public class Method
    {
        private readonly MethodInfo m_info;

        /// <summary>
        /// 方法执行委托
        /// </summary>
        private readonly Func<object, object[], object> m_invoker;

        private readonly bool m_isByRef;

        /// <summary>
        /// 方法
        /// </summary>
        /// <param name="method">方法信息</param>
        public Method(MethodInfo method)
        {
            m_info = method ?? throw new ArgumentNullException(nameof(method));
            Name = method.Name;
            Static = method.IsStatic;
            foreach (var item in method.GetParameters())
            {
                if (item.ParameterType.IsByRef)
                {
                    m_isByRef = true;
                    if (method.ReturnType == typeof(Task))
                    {
                        HasReturn = false;
                        TaskType = TaskReturnType.Task;
                    }
                    else if (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                    {
                        HasReturn = true;
                        ReturnType = method.ReturnType.GetGenericArguments()[0];
                        TaskType = TaskReturnType.TaskObject;
                    }
                    else if (method.ReturnType == typeof(void))
                    {
                        HasReturn = false;
                        TaskType = TaskReturnType.None;
                    }
                    else
                    {
                        HasReturn = true;
                        TaskType = TaskReturnType.None;
                        ReturnType = method.ReturnType;
                    }
                    return;
                }
            }

            if (!ReflectionFirst)
            {
                m_invoker = CreateInvoker(method);
            }
        }

        /// <summary>
        /// 反射优先。实测在unity中执行il2cpp时，需要设置该值为true，其余时候为false即可。
        /// </summary>
        public static bool ReflectionFirst { get; set; }

        /// <summary>
        /// 是否具有返回值
        /// </summary>
        public bool HasReturn { get; private set; }

        /// <summary>
        /// 方法信息
        /// </summary>
        public MethodInfo Info => m_info;

        /// <summary>
        /// 是否有引用类型
        /// </summary>
        public bool IsByRef => m_isByRef;

        /// <summary>
        /// 获取方法名
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// 返回值类型。
        /// <para>当方法为void或task时，为null</para>
        /// <para>当方法为task泛型时，为泛型元素类型</para>
        /// </summary>
        public Type ReturnType { get; private set; }

        /// <summary>
        /// 是否为静态函数
        /// </summary>
        public bool Static { get; private set; }

        /// <summary>
        /// 返回值的Task类型。
        /// </summary>
        public TaskReturnType TaskType { get; private set; }

        /// <summary>
        /// 执行方法。
        /// <para>当方法为void或task时，会返回null</para>
        /// <para>当方法为task泛型时，会wait后的值</para>
        /// <para>注意：当调用方为UI主线程时，调用异步方法，则极有可能发生死锁。</para>
        /// </summary>
        /// <param name="instance">实例</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public object Invoke(object instance, params object[] parameters)
        {
            switch (TaskType)
            {
                case TaskReturnType.None:
                    {
                        object re;
                        if (m_isByRef || ReflectionFirst)
                        {
                            re = m_info.Invoke(instance, parameters);
                        }
                        else
                        {
                            re = m_invoker.Invoke(instance, parameters);
                        }
                        return re;
                    }
                case TaskReturnType.Task:
                    {
                        object re;
                        if (m_isByRef || ReflectionFirst)
                        {
                            re = m_info.Invoke(instance, parameters);
                        }
                        else
                        {
                            re = m_invoker.Invoke(instance, parameters);
                        }
                        Task task = (Task)re;
                        task.Wait();
                        return default;
                    }
                case TaskReturnType.TaskObject:
                    {
                        object re;
                        if (m_isByRef || ReflectionFirst)
                        {
                            re = m_info.Invoke(instance, parameters);
                        }
                        else
                        {
                            re = m_invoker.Invoke(instance, parameters);
                        }
                        Task task = (Task)re;
                        task.Wait();
                        return task.GetType().GetProperty("Result").GetValue(task);
                    }
                default:
                    return default;
            }
        }

        /// <summary>
        /// 异步调用
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Task InvokeAsync(object instance, params object[] parameters)
        {
            switch (TaskType)
            {
                case TaskReturnType.None:
                    {
                        throw new Exception("该方法不包含Task。");
                    }
                case TaskReturnType.Task:
                    {
                        object re;
                        if (m_isByRef || ReflectionFirst)
                        {
                            re = m_info.Invoke(instance, parameters);
                        }
                        else
                        {
                            re = m_invoker.Invoke(instance, parameters);
                        }
                        return (Task)re;
                    }
                case TaskReturnType.TaskObject:
                    {
                        object re;
                        if (m_isByRef || ReflectionFirst)
                        {
                            re = m_info.Invoke(instance, parameters);
                        }
                        else
                        {
                            re = m_invoker.Invoke(instance, parameters);
                        }
                        return (Task)re;
                    }
                default:
                    return default;
            }
        }

        /// <summary>
        /// 调用异步结果
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<object> InvokeObjectAsync(object instance, params object[] parameters)
        {
            switch (TaskType)
            {
                case TaskReturnType.None:
                    {
                        object re;
                        if (m_isByRef || ReflectionFirst)
                        {
                            re = m_info.Invoke(instance, parameters);
                        }
                        else
                        {
                            re = m_invoker.Invoke(instance, parameters);
                        }
                        return re;
                    }
                case TaskReturnType.Task:
                    {
                        object re;
                        if (m_isByRef || ReflectionFirst)
                        {
                            re = m_info.Invoke(instance, parameters);
                        }
                        else
                        {
                            re = m_invoker.Invoke(instance, parameters);
                        }
                        Task task = (Task)re;
                        await task;
                        return default;
                    }
                case TaskReturnType.TaskObject:
                    {
                        object re;
                        if (m_isByRef || ReflectionFirst)
                        {
                            re = m_info.Invoke(instance, parameters);
                        }
                        else
                        {
                            re = m_invoker.Invoke(instance, parameters);
                        }
                        Task task = (Task)re;
                        await task;
                        return task.GetType().GetProperty("Result").GetValue(task);
                    }
                default:
                    return default;
            }
        }

        /// <summary>
        /// 生成方法的调用委托
        /// </summary>
        /// <param name="method">方法成员信息</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        private Func<object, object[], object> CreateInvoker(MethodInfo method)
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var parameters = Expression.Parameter(typeof(object[]), "parameters");

            var instanceCast = method.IsStatic ? null : Expression.Convert(instance, method.DeclaringType);
            var parametersCast = method.GetParameters().Select((p, i) =>
            {
                var parameter = Expression.ArrayIndex(parameters, Expression.Constant(i));
                return Expression.Convert(parameter, p.ParameterType);
            });

            var body = Expression.Call(instanceCast, method, parametersCast);

            if (method.ReturnType == typeof(Task))
            {
                HasReturn = false;
                TaskType = TaskReturnType.Task;
                var bodyCast = Expression.Convert(body, typeof(object));
                return Expression.Lambda<Func<object, object[], object>>(bodyCast, instance, parameters).Compile();
            }
            else if (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                TaskType = TaskReturnType.TaskObject;
                HasReturn = true;
                ReturnType = method.ReturnType.GetGenericArguments()[0];
                var bodyCast = Expression.Convert(body, typeof(object));
                return Expression.Lambda<Func<object, object[], object>>(bodyCast, instance, parameters).Compile();
            }
            else if (method.ReturnType == typeof(void))
            {
                HasReturn = false;
                TaskType = TaskReturnType.None;
                var action = Expression.Lambda<Action<object, object[]>>(body, instance, parameters).Compile();
                return (_instance, _parameters) =>
                {
                    action.Invoke(_instance, _parameters);
                    return null;
                };
            }
            else
            {
                HasReturn = true;
                TaskType = TaskReturnType.None;
                ReturnType = method.ReturnType;
                var bodyCast = Expression.Convert(body, typeof(object));
                return Expression.Lambda<Func<object, object[], object>>(bodyCast, instance, parameters).Compile();
            }
        }
    }
}