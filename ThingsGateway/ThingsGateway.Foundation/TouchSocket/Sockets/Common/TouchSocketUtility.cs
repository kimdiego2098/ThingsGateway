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
using System.Text.RegularExpressions;

namespace ThingsGateway.Foundation.Sockets
{
    /// <summary>
    /// TouchSocketUtility
    /// </summary>
    public class TouchSocketUtility
    {
        /// <summary>
        /// 判断输入的字符串是否是一个超链接
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsURL(string input)
        {
            string pattern = @"^[a-zA-Z]+://(\w+(-\w+)*)(\.(\w+(-\w+)*))*(\?\S*)?$?";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }

        /// <summary>
        /// 判断输入的字符串是否是表示一个IP地址
        /// </summary>
        /// <param name="input">被比较的字符串</param>
        /// <returns>是IP地址则为True</returns>
        public static bool IsIPv4(string input)
        {
            try
            {
                string[] IPs = input.Split('.');
                Regex regex = new Regex(@"^\d+$");
                for (int i = 0; i < IPs.Length; i++)
                {
                    if (!regex.IsMatch(IPs[i]))
                    {
                        return false;
                    }
                    if (Convert.ToUInt16(IPs[i]) > 255)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 判断输入的字符串是否是合法的IPV6 地址
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsIPV6(string input)
        {
            string pattern = "";
            string temp = input;
            string[] strs = temp.Split(':');
            if (strs.Length > 8)
            {
                return false;
            }
            int count = StringExtension.HitStringCount(input, "::");
            if (count > 1)
            {
                return false;
            }
            else if (count == 0)
            {
                pattern = @"^([\da-f]{1,4}:){7}[\da-f]{1,4}$";

                Regex regex = new Regex(pattern);
                return regex.IsMatch(input);
            }
            else
            {
                pattern = @"^([\da-f]{1,4}:){0,5}::([\da-f]{1,4}:){0,5}[\da-f]{1,4}$";
                Regex regex1 = new Regex(pattern);
                return regex1.IsMatch(input);
            }
        }

        /// <summary>
        /// 大数据边界
        /// </summary>
        public const int BigDataBoundary = 1024 * 64;
    }
}