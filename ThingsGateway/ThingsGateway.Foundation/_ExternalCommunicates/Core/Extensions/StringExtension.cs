using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace ThingsGateway.Foundation.Extension
{
    public static class StringExtension
    {
        /// <summary>
        /// 将字符串数组转换成字符串
        /// </summary>
        /// <param name="strArray">需要转换的字符串</param>
        /// <returns>合并完成的字符串</returns>
        public static string ArrayToString(this string[] strArray, string spitStr = "")
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < strArray.Length; i++)
            {
                if (i > 0)
                {
                    //分割符可根据需要自行修改
                    str.Append(spitStr);
                }
                str.Append(strArray[i]);
            }
            return str.ToString();
        }

        /// <summary>
        /// 从Base64转到数组。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] ByBase64ToBytes(this string value)
        {
            return Convert.FromBase64String(value);
        }

        /// <summary>
        /// 将16进制的字符转换为数组。
        /// </summary>
        /// <param name="hexString"></param>
        /// <param name="splite"></param>
        /// <returns></returns>
        public static byte[] ByHexStringToBytes(this string hexString, string splite = default)
        {
            if (!string.IsNullOrEmpty(splite))
            {
                hexString = hexString.Replace(splite, string.Empty);
            }

            if ((hexString.Length % 2) != 0)
            {
                hexString += " ";
            }
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return returnBytes;
        }

        /// <summary>
        /// 将16进制的字符转换为int32。
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static int ByHexStringToInt32(this string hexString)
        {
            if (string.IsNullOrEmpty(hexString))
            {
                return default;
            }
            return int.Parse(hexString, System.Globalization.NumberStyles.HexNumber);
        }

        /// <summary>
        /// 按格式填充
        /// </summary>
        /// <param name="str"></param>
        /// <param name="ps"></param>
        /// <returns></returns>
        public static string Format(this string str, params object[] ps)
        {
            if (ps == null || ps.Length == 0)
            {
                return str;
            }
            try
            {
                return string.Format(str, ps);
            }
            catch
            {
                return str;
            }
        }

        /// <summary>
        /// 当不为null，且不为空。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool HasValue(this string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// 判断字符串compare 在 input字符串中出现的次数
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <param name="compare">用于比较的字符串</param>
        /// <returns>字符串compare 在 input字符串中出现的次数</returns>
        public static int HitStringCount(this string input, string compare)
        {
            int index = input.IndexOf(compare);
            if (index != -1)
            {
                return 1 + HitStringCount(input.Substring(index + compare.Length), compare);
            }
            else
            {
                return 0;
            }
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// IsNullOrWhiteSpace
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }
        /// <summary>
        /// 将字符转换为对应的基础类型类型。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="destinationType">目标类型必须为基础类型</param>
        /// <returns></returns>
        public static object ParseToType(this string value, Type destinationType)
        {
            object returnValue;
            if ((value == null) || destinationType.IsInstanceOfType(value))
            {
                return value;
            }
            string str = value;
            if ((str != null) && (str.Length == 0))
            {
                return null;
            }
            TypeConverter converter = TypeDescriptor.GetConverter(destinationType);
            bool flag = converter.CanConvertFrom(value.GetType());
            if (!flag)
            {
                converter = TypeDescriptor.GetConverter(value.GetType());
            }
            if (!flag && !converter.CanConvertTo(destinationType))
            {
                throw new InvalidOperationException("无法转换成类型：" + value.ToString() + "==>" + destinationType);
            }
            try
            {
                returnValue = flag ? converter.ConvertFrom(null, null, value) : converter.ConvertTo(null, null, value, destinationType);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(" 类型转换出错：" + value.ToString() + "==>" + destinationType, e);
            }
            return returnValue;
        }

        /// <summary>
        /// 按字符串分割
        /// </summary>
        /// <param name="str"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static string[] Split(this string str, string pattern)
        {
            return Regex.Split(str, pattern);
        }

        /// <summary>
        /// 根据英文小数点进行切割字符串，去除空白的字符<br />
        /// </summary>
        /// <param name="str">字符串本身</param>
        public static string[] SplitDot(this string str)
        {
            return str.Split(new char[1]
{
      '.'
}, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// 只按第一个匹配项分割
        /// </summary>
        /// <param name="str"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public static string[] SplitFirst(this string str, char split)
        {
            List<string> s = new List<string>();
            int index = str.IndexOf(split);
            if (index > 0)
            {
                s.Add(str.Substring(0, index).Trim());
                s.Add(str.Substring(index + 1, str.Length - index - 1).Trim());
            }

            return s.ToArray();
        }

        /// <summary>
        /// 只按最后一个匹配项分割
        /// </summary>
        /// <param name="str"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public static string[] SplitLast(this string str, char split)
        {
            List<string> s = new List<string>();
            int index = str.LastIndexOf(split);
            if (index > 0)
            {
                s.Add(str.Substring(0, index).Trim());
                s.Add(str.Substring(index + 1, str.Length - index - 1).Trim());
            }

            return s.ToArray();
        }

        /// <summary>
        /// 转换为SHA1。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] ToSha1(this string value, Encoding encoding)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                return sha1.ComputeHash(encoding.GetBytes(value));
            }
        }

        /// <summary>
        /// 转换为UTF-8数据，效果等于<see cref="Encoding.UTF8"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] ToUTF8Bytes(this string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        ///<summary>
        ///  将字符串格式化成指定的数据类型
        ///</summary>
        ///<param name="str"></param>
        ///<param name="type"></param>
        /// <param name="value"></param>
        /// <param name="splits"></param>
        ///   <returns></returns>
        public static bool TryParseToType(string str, Type type, out object value, char[] splits = default)
        {
            if (string.IsNullOrEmpty(str))
            {
                value = default;
                return true;
            }

            if (type == null)
            {
                value = str;
                return true;
            }
            if (type.IsArray)
            {
                Type elementType = type.GetElementType();
                string[] strs;
                if (splits == null)
                {
                    strs = str.Split(new char[] { ' ', ';', '-', '/' });
                }
                else
                {
                    strs = str.Split(splits);
                }

                Array array = Array.CreateInstance(elementType, strs.Length);
                for (int i = 0, c = strs.Length; i < c; ++i)
                {
                    object o;
                    if (ConvertSimpleType(strs[i], elementType, out o))
                    {
                        array.SetValue(o, i);
                    }
                    else
                    {
                        value = default;
                        return false;
                    }
                }
                value = array;
                return true;
            }
            return ConvertSimpleType(str, type, out value);
        }

        private static bool ConvertSimpleType(string value, Type destinationType, out object returnValue)
        {
            if ((value == null) || destinationType.IsInstanceOfType(value))
            {
                returnValue = value;
                return true;
            }

            if (string.IsNullOrEmpty(value))
            {
                returnValue = default;
                return true;
            }
            TypeConverter converter = TypeDescriptor.GetConverter(destinationType);
            bool flag = converter.CanConvertFrom(value.GetType());
            if (!flag)
            {
                converter = TypeDescriptor.GetConverter(value.GetType());
            }
            if (!flag && !converter.CanConvertTo(destinationType))
            {
                returnValue = default;
                return false;
            }
            try
            {
                returnValue = flag ? converter.ConvertFrom(null, null, value) : converter.ConvertTo(null, null, value, destinationType);
            }
            catch
            {
                returnValue = default;
                return false;
            }
            return true;
        }

    }
}