



using System.Collections.Generic;
namespace ThingsGateway.Foundation.Extension
{

    public static class ByteExtensions
    {

        /// <summary>
        /// 获取byte数据类型的第offset位，是否为True<br />
        /// </summary>
        /// <param name="value">byte数值</param>
        /// <param name="offset">索引位置</param>
        /// <returns>结果</returns>
        public static bool BoolOnByteIndex(this byte value, int offset)
        {
            byte dataByBitIndex = DataHelper.GetDataByBitIndex(offset);
            return (value & dataByBitIndex) == dataByBitIndex;
        }

        /// <summary>
        /// 字节数组转16进制字符
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="splite"></param>
        /// <returns></returns>
        public static string ByBytesToHexString(this byte[] buffer, int offset, int length, string splite = default)
        {
            if (string.IsNullOrEmpty(splite))
            {
                return BitConverter.ToString(buffer, offset, length).Replace("-", string.Empty);
            }
            else
            {
                return BitConverter.ToString(buffer, offset, length).Replace("-", splite);
            }
        }

        /// <summary>
        /// 将byte数组按照双字节进行反转，如果为单数的情况，则自动补齐<br />
        /// </summary>
        /// <param name="inBytes">输入的字节信息</param>
        /// <returns>反转后的数据</returns>
        public static byte[] BytesReverseByWord(this byte[] inBytes)
        {
            if (inBytes == null)
                return null;
            if (inBytes.Length == 0)
                return new byte[0];
            byte[] lengthEven = inBytes.CopyArray<byte>().ArrayExpandToLengthEven();
            for (int index = 0; index < lengthEven.Length / 2; ++index)
            {
                byte num = lengthEven[index * 2];
                lengthEven[index * 2] = lengthEven[(index * 2) + 1];
                lengthEven[(index * 2) + 1] = num;
            }
            return lengthEven;
        }

        /// <summary>
        /// 从Byte数组中提取位数组，length代表位数<br />
        /// </summary>
        /// <param name="InBytes">原先的字节数组</param>
        /// <param name="length">想要转换的长度，如果超出自动会缩小到数组最大长度</param>
        /// <returns>转换后的bool数组</returns>
        public static bool[] ByteToBoolArray(this byte[] InBytes, int length)
        {
            if (InBytes == null)
                return null;
            if (length > InBytes.Length * 8)
                length = InBytes.Length * 8;
            bool[] boolArray = new bool[length];
            for (int index = 0; index < length; ++index)
                boolArray[index] = InBytes[index / 8].BoolOnByteIndex(index % 8);
            return boolArray;
        }

        /// <summary>
        /// 获取字节中的指定Bit的值
        /// </summary>
        /// <param name="this">字节</param>
        /// <param name="index">Bit的索引值(0-7)</param>
        /// <returns></returns>
        public static int GetBit(this byte @this, short index)
        {
            byte x = 1;
            switch (index)
            {
                case 0: { x = 0x01; } break;
                case 1: { x = 0x02; } break;
                case 2: { x = 0x04; } break;
                case 3: { x = 0x08; } break;
                case 4: { x = 0x10; } break;
                case 5: { x = 0x20; } break;
                case 6: { x = 0x40; } break;
                case 7: { x = 0x80; } break;
                default: { return 0; }
            }
            return (@this & x) == x ? 1 : 0;
        }

        /// <summary>
        /// 获取Byte数组的第 boolIndex 偏移的bool值，这个偏移值可以为 10，就是第 1 个字节的 第3位 <br />
        /// </summary>
        /// <param name="bytes">字节数组信息</param>
        /// <param name="boolIndex">指定字节的位偏移</param>
        /// <returns>bool值</returns>
        public static bool GetBoolByIndex(this byte[] bytes, int boolIndex)
        {
            return bytes[boolIndex / 8].BoolOnByteIndex(boolIndex % 8);
        }
        /// <summary>
        /// 索引第一个包含数组的索引位置，例如：在{0,1,2,3,1,2,3}中索引{2,3}，则返回3。
        /// <para>如果目标数组为null或长度为0，则直接返回offset的值</para>
        /// </summary>
        /// <param name="srcByteArray"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="subByteArray"></param>
        /// <returns></returns>
        public static int IndexOfFirst(this byte[] srcByteArray, int offset, int length, byte[] subByteArray)
        {
            if (length < subByteArray.Length)
            {
                return -1;
            }
            if (subByteArray == null || subByteArray.Length == 0)
            {
                return offset;
            }
            int hitLength = 0;
            for (int i = offset; i < length + offset; i++)
            {
                if (srcByteArray[i] == subByteArray[hitLength])
                {
                    hitLength++;
                }
                else
                {
                    hitLength = 0;
                }

                if (hitLength == subByteArray.Length)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// 索引包含数组。
        /// <para>
        /// 例如：在{0,1,2,3,1,2,3}中搜索{1,2}，则会返回list:[2,5]，均为最后索引的位置。
        /// </para>
        /// </summary>
        /// <param name="srcByteArray"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="subByteArray"></param>
        /// <returns></returns>
        public static List<int> IndexOfInclude(this byte[] srcByteArray, int offset, int length, byte[] subByteArray)
        {
            int subByteArrayLen = subByteArray.Length;
            List<int> indexes = new List<int>();
            if (length < subByteArrayLen)
            {
                return indexes;
            }
            int hitLength = 0;
            for (int i = offset; i < length; i++)
            {
                if (srcByteArray[i] == subByteArray[hitLength])
                {
                    hitLength++;
                }
                else
                {
                    hitLength = 0;
                }

                if (hitLength == subByteArray.Length)
                {
                    hitLength = 0;
                    indexes.Add(i);
                }
            }

            return indexes;
        }

        /// <summary>
        /// 设置字节中的指定Bit的值
        /// </summary>
        /// <param name="this">字节</param>
        /// <param name="index">Bit的索引值(0-7)</param>
        /// <param name="bitvalue">Bit值(0,1)</param>
        /// <returns></returns>
        public static byte SetBit(this byte @this, short index, int bitvalue)
        {
            var _byte = @this;
            if (bitvalue == 1)
            {
                switch (index)
                {
                    case 0: { return _byte |= 0x01; }
                    case 1: { return _byte |= 0x02; }
                    case 2: { return _byte |= 0x04; }
                    case 3: { return _byte |= 0x08; }
                    case 4: { return _byte |= 0x10; }
                    case 5: { return _byte |= 0x20; }
                    case 6: { return _byte |= 0x40; }
                    case 7: { return _byte |= 0x80; }
                    default: { return _byte; }
                }
            }
            else
            {
                switch (index)
                {
                    case 0: { return _byte &= 0xFE; }
                    case 1: { return _byte &= 0xFD; }
                    case 2: { return _byte &= 0xFB; }
                    case 3: { return _byte &= 0xF7; }
                    case 4: { return _byte &= 0xEF; }
                    case 5: { return _byte &= 0xDF; }
                    case 6: { return _byte &= 0xBF; }
                    case 7: { return _byte &= 0x7F; }
                    default: { return _byte; }
                }
            }
        }

        /// <summary>
        /// 转Base64。
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ToBase64(this byte[] data)
        {
            return Convert.ToBase64String(data);
        }

        /// <summary>
        /// 字节数组默认转16进制字符
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="splite"></param>
        /// <returns></returns>
        public static string ToHexString(this byte[] buffer, string splite = default)
        {
            if (string.IsNullOrEmpty(splite))
            {
                return BitConverter.ToString(buffer, 0, buffer.Length).Replace("-", string.Empty);
            }
            else
            {
                return BitConverter.ToString(buffer, 0, buffer.Length).Replace("-", splite);
            }
        }

        /// <summary>
        /// 字节数组转16进制字符
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="splite"></param>
        /// <returns></returns>
        public static string ToHexString(this byte[] buffer, int offset, int length, string splite = default)
        {
            if (string.IsNullOrEmpty(splite))
            {
                return BitConverter.ToString(buffer, offset, length).Replace("-", string.Empty);
            }
            else
            {
                return BitConverter.ToString(buffer, offset, length).Replace("-", splite);
            }
        }

    }
}