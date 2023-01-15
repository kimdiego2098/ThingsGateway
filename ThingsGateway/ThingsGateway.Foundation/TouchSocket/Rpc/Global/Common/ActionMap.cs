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
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace ThingsGateway.Foundation.Rpc
{
    /// <summary>
    /// 服务映射图
    /// </summary>
    public class ActionMap : IEnumerable<KeyValuePair<string, MethodInstance>>
    {
        private readonly ConcurrentDictionary<string, MethodInstance> m_actionMap = new ConcurrentDictionary<string, MethodInstance>();

        /// <summary>
        /// 服务键集合
        /// </summary>
        public IEnumerable<string> ActionKeys => m_actionMap.Keys;

        /// <summary>
        /// 添加调用
        /// </summary>
        /// <param name="actionKey"></param>
        /// <param name="methodInstance"></param>
        public void Add(string actionKey, MethodInstance methodInstance)
        {
            if (m_actionMap.ContainsKey(actionKey))
            {
                throw new System.Exception($"调用键为{actionKey}的函数已存在。");
            }
            m_actionMap.TryAdd(actionKey, methodInstance);
        }

        /// <summary>
        /// 获取所有服务函数实例
        /// </summary>
        /// <returns></returns>
        public MethodInstance[] GetAllMethodInstances()
        {
            return m_actionMap.Values.ToArray();
        }

        /// <summary>
        /// 返回迭代器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, MethodInstance>> GetEnumerator()
        {
            return m_actionMap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_actionMap.GetEnumerator();
        }

        /// <summary>
        /// 通过actionKey获取函数实例
        /// </summary>
        /// <param name="actionKey"></param>
        /// <returns></returns>
        public MethodInstance GetMethodInstance(string actionKey)
        {
            m_actionMap.TryGetValue(actionKey, out MethodInstance methodInstance);
            return methodInstance;
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="actionKey"></param>
        /// <param name="methodInstance"></param>
        public bool Remove(string actionKey, out MethodInstance methodInstance)
        {
            return m_actionMap.TryRemove(actionKey, out methodInstance);
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="actionKey"></param>
        /// <returns></returns>
        public bool Remove(string actionKey)
        {
            return m_actionMap.TryRemove(actionKey, out _);
        }

        /// <summary>
        /// 通过actionKey获取函数实例
        /// </summary>
        /// <param name="actionKey"></param>
        /// <param name="methodInstance"></param>
        /// <returns></returns>
        public bool TryGetMethodInstance(string actionKey, out MethodInstance methodInstance)
        {
            return m_actionMap.TryGetValue(actionKey, out methodInstance);
        }
    }
}