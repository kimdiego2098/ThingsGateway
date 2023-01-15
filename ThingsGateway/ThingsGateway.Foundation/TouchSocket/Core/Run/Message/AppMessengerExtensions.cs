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
using System.Collections.Generic;
using System.Reflection;

namespace ThingsGateway.Foundation.Core
{
    /// <summary>
    /// AppMessengerExtensions
    /// </summary>
    public static class AppMessengerExtensions
    {
        /// <summary>
        /// 注册类的静态消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Register<T>(this AppMessenger appMessenger) where T : IMessageObject
        {
            Type type = typeof(T);
            Register(appMessenger, type);
        }

        /// <summary>
        /// 注册类的静态消息
        /// </summary>
        /// <param name="appMessenger"></param>
        /// <param name="type"></param>
        /// <exception cref="NotSupportedException"></exception>
        public static void Register(this AppMessenger appMessenger, Type type)
        {
            MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Default);
            foreach (var method in methods)
            {
                IEnumerable<Attribute> attributes = method.GetCustomAttributes();
                foreach (var attribute in attributes)
                {
                    if (attribute is AppMessageAttribute att)
                    {
                        if (string.IsNullOrEmpty(att.Token))
                        {
                            Register(appMessenger, null, method.Name, method);
                        }
                        else
                        {
                            Register(appMessenger, null, att.Token, method);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 注册消息
        /// </summary>
        /// <param name="appMessenger"></param>
        /// <param name="messageObject"></param>
        public static void Register(this AppMessenger appMessenger, IMessageObject messageObject)
        {
            MethodInfo[] methods = messageObject.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Default);
            foreach (var method in methods)
            {
                IEnumerable<Attribute> attributes = method.GetCustomAttributes();
                foreach (var attribute in attributes)
                {
                    if (attribute is AppMessageAttribute att)
                    {
                        if (string.IsNullOrEmpty(att.Token))
                        {
                            Register(appMessenger, messageObject, method.Name, method);
                        }
                        else
                        {
                            Register(appMessenger, messageObject, att.Token, method);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 注册消息
        /// </summary>
        /// <param name="appMessenger"></param>
        /// <param name="messageObject"></param>
        /// <param name="token"></param>
        /// <param name="methodInfo"></param>
        /// <exception cref="MessageRegisteredException"></exception>
        public static void Register(this AppMessenger appMessenger, IMessageObject messageObject, string token, MethodInfo methodInfo)
        {
            appMessenger.Add(token, new MessageInstance(methodInfo, messageObject));
        }

        /// <summary>
        /// 注册消息
        /// </summary>
        /// <param name="appMessenger"></param>
        /// <param name="action"></param>
        /// <param name="token"></param>
        public static void Register(this AppMessenger appMessenger, Action action, string token = default)
        {
            RegisterDelegate(appMessenger, token, action);
        }

        /// <summary>
        /// 注册消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="appMessenger"></param>
        /// <param name="action"></param>
        /// <param name="token"></param>
        public static void Register<T>(this AppMessenger appMessenger, Action<T> action, string token = default)
        {
            RegisterDelegate(appMessenger, token, action);
        }

        /// <summary>
        /// 注册消息
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="appMessenger"></param>
        /// <param name="action"></param>
        /// <param name="token"></param>
        public static void Register<T1, T2>(this AppMessenger appMessenger, Action<T1, T2> action, string token = default)
        {
            RegisterDelegate(appMessenger, token, action);
        }

        /// <summary>
        /// 注册消息
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="appMessenger"></param>
        /// <param name="action"></param>
        /// <param name="token"></param>
        public static void Register<T1, T2, T3>(this AppMessenger appMessenger, Action<T1, T2, T3> action, string token = default)
        {
            RegisterDelegate(appMessenger, token, action);
        }

        /// <summary>
        /// 注册消息
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="appMessenger"></param>
        /// <param name="action"></param>
        /// <param name="token"></param>
        public static void Register<T1, T2, T3, T4>(this AppMessenger appMessenger, Action<T1, T2, T3, T4> action, string token = default)
        {
            RegisterDelegate(appMessenger, token, action);
        }

        /// <summary>
        /// 注册消息
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <param name="appMessenger"></param>
        /// <param name="action"></param>
        /// <param name="token"></param>
        public static void Register<T1, T2, T3, T4, T5>(this AppMessenger appMessenger, Action<T1, T2, T3, T4, T5> action, string token = default)
        {
            RegisterDelegate(appMessenger, token, action);
        }

        /// <summary>
        /// 注册消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="appMessenger"></param>
        /// <param name="action"></param>
        /// <param name="token"></param>
        public static void Register<T, TReturn>(this AppMessenger appMessenger, Func<T, TReturn> action, string token = default)
        {
            RegisterDelegate(appMessenger, token, action);
        }

        /// <summary>
        /// 注册消息
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="appMessenger"></param>
        /// <param name="action"></param>
        /// <param name="token"></param>
        public static void Register<T1, T2, TReturn>(this AppMessenger appMessenger, Func<T1, T2, TReturn> action, string token = default)
        {
            RegisterDelegate(appMessenger, token, action);
        }

        /// <summary>
        /// 注册消息
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="appMessenger"></param>
        /// <param name="action"></param>
        /// <param name="token"></param>
        public static void Register<T1, T2, T3, TReturn>(this AppMessenger appMessenger, Func<T1, T2, T3, TReturn> action, string token = default)
        {
            RegisterDelegate(appMessenger, token, action);
        }

        /// <summary>
        /// 注册消息
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="appMessenger"></param>
        /// <param name="action"></param>
        /// <param name="token"></param>
        public static void Register<T1, T2, T3, T4, TReturn>(this AppMessenger appMessenger, Func<T1, T2, T3, T4, TReturn> action, string token = default)
        {
            RegisterDelegate(appMessenger, token, action);
        }

        /// <summary>
        /// 注册消息
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="appMessenger"></param>
        /// <param name="action"></param>
        /// <param name="token"></param>
        public static void Register<T1, T2, T3, T4, T5, TReturn>(this AppMessenger appMessenger, Func<T1, T2, T3, T4, T5, TReturn> action, string token = default)
        {
            RegisterDelegate(appMessenger, token, action);
        }

        /// <summary>
        /// 注册消息
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="appMessenger"></param>
        /// <param name="action"></param>
        /// <param name="token"></param>
        public static void Register<TReturn>(this AppMessenger appMessenger, Func<TReturn> action, string token = default)
        {
            RegisterDelegate(appMessenger, token, action);
        }

        /// <summary>
        /// 卸载消息
        /// </summary>
        /// <param name="appMessenger"></param>
        /// <param name="messageObject"></param>
        public static void Unregister(this AppMessenger appMessenger, IMessageObject messageObject)
        {
            appMessenger.Remove(messageObject);
        }

        /// <summary>
        /// 移除注册
        /// </summary>
        /// <param name="appMessenger"></param>
        /// <param name="token"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Unregister(this AppMessenger appMessenger, string token)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }
            appMessenger.Remove(token);
        }

        private static void RegisterDelegate(this AppMessenger appMessenger, string token, Delegate dele)
        {
            IEnumerable<Attribute> attributes = dele.Method.GetCustomAttributes();
            foreach (var attribute in attributes)
            {
                if (attribute is AppMessageAttribute att)
                {
                    if (token.IsNullOrEmpty())
                    {
                        if (string.IsNullOrEmpty(att.Token))
                        {
                            token = dele.Method.Name;
                        }
                        else
                        {
                            token = att.Token;
                        }
                    }

                    appMessenger.Add(token, new MessageInstance(dele.Method, dele.Target));
                }
            }
        }
    }
}