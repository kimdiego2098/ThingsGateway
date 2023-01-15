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
using System.Text;

namespace ThingsGateway.Foundation.Core
{
    /// <summary>
    /// MD5相关操作类
    /// </summary>
    public static class MD5
    {
        /// <summary>
        /// 从字符串获取MD5值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetMD5Hash(string str)
        {
            StringBuilder sb = new StringBuilder();
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
                int length = data.Length;
                for (int i = 0; i < length; i++)
                    sb.Append(data[i].ToString("X2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 从字节获取MD5值
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetMD5Hash(byte[] buffer, int offset, int length)
        {
            StringBuilder sb = new StringBuilder();
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] data = md5.ComputeHash(buffer, offset, length);
                int count = data.Length;
                for (int i = 0; i < count; i++)
                    sb.Append(data[i].ToString("X2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 从字节获取MD5值
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string GetMD5Hash(byte[] buffer)
        {
            return GetMD5Hash(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 验证MD5值。
        /// </summary>
        /// <param name="str"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static bool VerifyMD5Hash(string str, string hash)
        {
            string hashOfInput = GetMD5Hash(str);
            if (hashOfInput.CompareTo(hash) == 0)
                return true;
            else
                return false;
        }
    }
}