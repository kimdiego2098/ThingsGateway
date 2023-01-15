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
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ThingsGateway.Foundation.Core
{
    /// <summary>
    /// 数据安全加密
    /// </summary>
    public static class DataSecurity
    {
        /// <summary>
        /// 自定义加密密钥。
        /// </summary>
        private static byte[] Keys { get; set; } = { 0x12, 0x34, 4, 0x78, 0x90, 255, 0xCD, 0xEF };//自定义密匙

        /// <summary>
        /// 使用3DES加密
        /// </summary>
        /// <param name="data">待加密字节</param>
        /// <param name="encryptKey">加密口令（长度为8）</param>
        /// <returns></returns>
        public static byte[] EncryptDES(byte[] data, string encryptKey)
        {
            if (encryptKey.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(encryptKey));
            }
            if (encryptKey.Length < 8)
            {
                throw new Exception("密钥长度不足8位。");
            }
            byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
            using (var des = DES.Create())
            {
                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream cStream = new CryptoStream(mStream, des.CreateEncryptor(rgbKey, Keys), CryptoStreamMode.Write))
                    {
                        cStream.Write(data, 0, data.Length);
                        cStream.FlushFinalBlock();
                        return mStream.ToArray();
                    }
                }
            }
        }

        /// <summary>
        /// 使用3DES解密
        /// </summary>
        /// <param name="data">待解密字节</param>
        /// <param name="encryptKey">解密口令（长度为8）</param>
        /// <returns></returns>
        public static byte[] DecryptDES(byte[] data, string encryptKey)
        {
            if (encryptKey.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(encryptKey));
            }
            if (encryptKey.Length < 8)
            {
                throw new Exception("密钥长度不足8位。");
            }
            byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey);
            using (var des = DES.Create())
            {
                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream cStream = new CryptoStream(mStream, des.CreateDecryptor(rgbKey, Keys), CryptoStreamMode.Write))
                    {
                        cStream.Write(data, 0, data.Length);
                        cStream.FlushFinalBlock();
                        return mStream.ToArray();
                    }
                }
            }
        }

        /// <summary>
        /// 使用3DES流数据加密。
        /// <para>注意：数据会从<see cref="Stream.Position"/>开始</para>
        /// </summary>
        /// <param name="inStream"></param>
        /// <param name="outStream"></param>
        /// <param name="encryptKey">加密口令（长度为8）</param>
        /// <returns></returns>
        public static void StreamEncryptDES(Stream inStream, Stream outStream, string encryptKey)
        {
            if (encryptKey.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(encryptKey));
            }
            if (encryptKey.Length < 8)
            {
                throw new Exception("密钥长度不足8位。");
            }
            byte[] rgbKeys = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
            byte[] byteIn = new byte[1024 * 64];
            long readLen = 0;
            long totalLen = inStream.Length - inStream.Position;
            using (var des = DES.Create())
            {
                CryptoStream encStream = new CryptoStream(new WrapStream(outStream), des.CreateEncryptor(rgbKeys, Keys), CryptoStreamMode.Write);
                while (readLen < totalLen)
                {
                    int r = inStream.Read(byteIn, 0, byteIn.Length);
                    encStream.Write(byteIn, 0, r);
                    readLen += r;
                }
                encStream.Close();
            }
        }

        /// <summary>
        /// 使用3DES流数据解密
        /// <para>注意：数据会从<see cref="Stream.Position"/>开始</para>
        /// </summary>
        /// <param name="inStream"></param>
        /// <param name="outStream"></param>
        /// <param name="encryptKey">解密口令（长度为8）</param>
        public static void StreamDecryptDES(Stream inStream, Stream outStream, string encryptKey)
        {
            if (encryptKey.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(encryptKey));
            }
            if (encryptKey.Length < 8)
            {
                throw new Exception("密钥长度不足8位。");
            }
            byte[] rgbKeys = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
            byte[] byteIn = new byte[1024 * 64];
            long readLen = 0;
            long totalLen = inStream.Length - inStream.Position;
            using (DES des = DES.Create())
            {
                CryptoStream encStream = new CryptoStream(new WrapStream(outStream), des.CreateDecryptor(rgbKeys, Keys), CryptoStreamMode.Write);
                while (readLen < totalLen)
                {
                    int r = inStream.Read(byteIn, 0, byteIn.Length);
                    encStream.Write(byteIn, 0, r);
                    readLen = readLen + r;
                }
                encStream.Close();
            }
        }
    }
}