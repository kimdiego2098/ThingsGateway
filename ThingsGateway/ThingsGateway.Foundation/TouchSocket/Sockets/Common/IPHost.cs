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
using System.Net;
using System.Net.Sockets;

namespace ThingsGateway.Foundation.Sockets
{
    /// <summary>
    /// IP解析映射
    /// <para>
    /// 支持端口，ip，域名等。具体格式如下：
    /// <list type=" number">
    /// <item>端口：直接按<see cref="int"/>入参，该操作一般在监听时使用。</item>
    /// <item>ip：按127.0.0.1:7789入参。</item>
    /// <item>域名：按tcp://127.0.0.1:7789、或者http://baidu.com入参。</item>
    /// </list>
    /// </para>
    /// </summary>
    public class IPHost
    {
        /// <summary>
        /// IP解析映射
        /// <para>
        /// 支持端口，ip，域名等。具体格式如下：
        /// <list type=" number">
        /// <item>端口：直接按<see cref="int"/>入参，该操作一般在监听时使用。</item>
        /// <item>ip：按127.0.0.1:7789入参。</item>
        /// <item>域名：按tcp://127.0.0.1:7789、或者http://baidu.com入参。</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="host"></param>
        public IPHost(string host)
        {
            if (TouchSocketUtility.IsURL(host))
            {
                IsUri = true;
                Uri = new Uri(host);
                if (Uri.Port > 0)
                {
                    Host = $"{Uri.Host}:{Uri.Port}";
                }
                else
                {
                    Host = Uri.Host;
                }
                if (TouchSocketUtility.IsIPv4(Uri.Host) || TouchSocketUtility.IsIPV6(Uri.Host))
                {
                    Analysis(Uri.Host, Uri.Port.ToString());
                }
                else
                {
                    if (HostNameToIP(Uri.Host, out IPAddress[] addresses))
                    {
                        Analysis(addresses[0].ToString(), Uri.Port.ToString());
                    }
                }
            }
            else
            {
                Host = host;
                int r = host.LastIndexOf(":");
                string ip = host.Substring(0, r);
                Analysis(ip, host.Substring(r + 1, host.Length - (r + 1)));
            }
        }

        /// <summary>
        /// 从IPAddress和端口号
        /// </summary>
        /// <param name="iPAddress"></param>
        /// <param name="port"></param>
        public IPHost(IPAddress iPAddress, int port) : this($"{iPAddress}:{port}")
        {
        }

        /// <summary>
        /// 从端口号创建。
        /// </summary>
        /// <param name="port"></param>
        public IPHost(int port) : this($"0.0.0.0:{port}")
        {
        }

        /// <summary>
        /// 寻址方案
        /// </summary>
        public AddressFamily AddressFamily { get; private set; }

        /// <summary>
        /// 终结点
        /// </summary>
        public IPEndPoint EndPoint { get; private set; }

        /// <summary>
        /// 具有端口信息的host
        /// </summary>
        public string Host { get; private set; }

        /// <summary>
        /// IP
        /// </summary>
        public string IP { get; private set; }

        /// <summary>
        /// 是否为Uri
        /// </summary>
        public bool IsUri { get; private set; }

        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// 统一资源标识
        /// </summary>
        public Uri Uri { get; private set; }

        /// <summary>
        /// 解析一个组的地址。
        /// </summary>
        /// <param name="strs"></param>
        /// <returns></returns>
        public static IPHost[] ParseIPHosts(string[] strs)
        {
            List<IPHost> iPs = new List<IPHost>();
            foreach (var item in strs)
            {
                iPs.Add(new IPHost(item));
            }
            return iPs.ToArray();
        }

        /// <summary>
        /// 获取Url全路径
        /// </summary>
        /// <returns></returns>
        public string GetUrlPath()
        {
            if (IsUri)
            {
                return Uri.PathAndQuery;
            }
            return default;
        }

        /// <summary>
        /// 返回EndPoint字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return EndPoint == null ? null : EndPoint.ToString();
        }

        private static bool HostNameToIP(string hostname, out IPAddress[] address)
        {
            try
            {
                IPHostEntry hostInfo = Dns.GetHostEntry(hostname);
                address = hostInfo.AddressList;
                if (address.Length > 0)
                {
                    return true;
                }

                return false;
            }
            catch
            {
                address = null;
                return false;
            }
        }

        private void Analysis(string ip, string port)
        {
            try
            {
                int portNum = int.Parse(port);
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), portNum);
                if (ip.Contains(":"))
                {
                    AddressFamily = AddressFamily.InterNetworkV6;
                }
                else
                {
                    AddressFamily = AddressFamily.InterNetwork;
                }
                EndPoint = endPoint;
                IP = ip;
                Port = portNum;
            }
            catch (Exception ex)
            {
                throw new Exception($"IPHost初始化失败，信息:{ex.Message}", ex);
            }
        }
    }
}