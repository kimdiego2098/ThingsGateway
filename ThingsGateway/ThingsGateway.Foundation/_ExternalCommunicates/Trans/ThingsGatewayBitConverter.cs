using System.Text;

namespace ThingsGateway.Foundation
{
    /// <summary>
    /// 将基数据类型转换为指定端的一个字节数组，
    /// 或将一个字节数组转换为指定端基数据类型。
    /// </summary>
    public class ThingsGatewayBitConverter : IThingsGatewayBitConverter
    {

        #region Static Fields

        /// <summary>
        /// 以大端 
        /// </summary>
        public static ThingsGatewayBitConverter BigEndian;
        /// <summary>
        /// 以小端 
        /// </summary>
        public static ThingsGatewayBitConverter LittleEndian;
        private static ThingsGatewayBitConverter m_default;

        private static EndianType m_defaultEndianType;
        /// <summary>
        /// 以默认小端，可通过<see cref="DefaultEndianType"/>重新指定默认端。
        /// </summary>
        public static ThingsGatewayBitConverter Default => m_default;
        /// <summary>
        /// 默认大小端切换。
        /// </summary>
        public static EndianType DefaultEndianType
        {
            get => m_defaultEndianType;
            set
            {
                m_defaultEndianType = value;
                switch (value)
                {
                    case EndianType.Little:
                        m_default = LittleEndian;
                        break;

                    case EndianType.Big:
                        m_default = BigEndian;
                        break;
                }
            }
        }
        static ThingsGatewayBitConverter()
        {
            BigEndian = new ThingsGatewayBitConverter(EndianType.Big, DataFormat.DCBA);
            LittleEndian = new ThingsGatewayBitConverter(EndianType.Little, DataFormat.DCBA);
            DefaultEndianType = EndianType.Little;
        }
        #endregion

        public virtual IThingsGatewayBitConverter CreateByDateFormat(DataFormat dataFormat)
        {
            ThingsGatewayBitConverter byteConverter = new ThingsGatewayBitConverter(EndianType, dataFormat)
            {
                IsStringReverseByteWord = IsStringReverseByteWord,
            };
            return byteConverter;
        }
        private readonly EndianType endianType;
        private DataFormat dataFormat;
        public DataFormat DataFormat
        {
            get
            {
                return dataFormat;
            }
            set
            {
                dataFormat = value;
            }
        }

        #region Public Constructors
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="endianType"></param>
        public ThingsGatewayBitConverter(EndianType endianType)
        {
            this.endianType = endianType;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="endianType"></param>
        public ThingsGatewayBitConverter(EndianType endianType, DataFormat dataFormat)
        {
            this.endianType = endianType; this.dataFormat = dataFormat;
        }

        #endregion Public Constructors

        #region Public Properties
        /// <summary>
        /// 指定大小端。
        /// </summary>
        public EndianType EndianType => endianType;

        public bool IsStringReverseByteWord { get; set; }

        #endregion Public Properties

        #region Public Methods






        #region bool

        public byte[] GetBytes(bool value)
        {
            return GetBytes(new bool[1]
{
      value
});
        }

        public byte[] GetBytes(bool[] values)
        {
            return values?.BoolArrayToByte();
        }

        public bool ToBoolean(byte[] buffer, int offset)
        {
            byte[] bytes = new byte[buffer.Length];
            Array.Copy(buffer, 0, bytes, 0, buffer.Length);
            if (!IsSameOfSet())
            {
                bytes = bytes.BytesReverseByWord();
            }
            return bytes.GetBoolByIndex(offset);

        }

        public bool[] ToBoolean(byte[] buffer, int offset, int length)
        {
            byte[] bytes = new byte[buffer.Length];
            Array.Copy(buffer, 0, bytes, 0, buffer.Length);
            if (!IsSameOfSet())
            {
                bytes = bytes.BytesReverseByWord();
            }
            bool[] flagArray = new bool[length];
            for (int index1 = 0; index1 < length; ++index1)
            {
                flagArray[index1] = bytes.GetBoolByIndex(index1 + offset);
            }

            return flagArray;
        }

        #endregion
        #region short

        public byte[] GetBytes(short value)
        {
            return GetBytes(new short[1]
{
      value
});

        }
        public byte[] GetBytes(short[] values)
        {
            if (values == null)
            {
                return null;
            }

            byte[] numArray = new byte[values.Length * 2];
            for (int offset = 0; offset < values.Length; ++offset)
            {
                byte[] bytes = BitConverter.GetBytes(values[offset]);
                if (!IsSameOfSet())
                {
                    Array.Reverse(bytes);
                }
                bytes.CopyTo(numArray, 2 * offset);
            }
            return numArray;

        }


        public short ToInt16(byte[] buffer, int offset)
        {
            byte[] bytes = new byte[2];
            Array.Copy(buffer, offset, bytes, 0, bytes.Length);
            if (!IsSameOfSet())
            {
                Array.Reverse(bytes);
            }
            return BitConverter.ToInt16(bytes, 0);
        }

        public short[] ToInt16(byte[] buffer, int offset, int length)
        {
            short[] numArray = new short[length];
            for (int index1 = 0; index1 < length; ++index1)
            {
                numArray[index1] = ToInt16(buffer, offset + (2 * index1));
            }

            return numArray;
        }

        public short[,] ToInt16(byte[] buffer, int offset, int row, int col)
        {
            return GenericHelper.CreateTwoArrayFromOneArray<short>(ToInt16(buffer, offset, row * col), row, col);
        }


        #endregion
        #region ushort
        public byte[] GetBytes(ushort[] values)
        {
            if (values == null)
            {
                return null;
            }
            byte[] numArray = new byte[values.Length * 2];
            for (int offset = 0; offset < values.Length; ++offset)
            {
                byte[] bytes = BitConverter.GetBytes(values[offset]);
                if (!IsSameOfSet())
                {
                    Array.Reverse(bytes);
                }
                bytes.CopyTo(numArray, 2 * offset);
            }
            return numArray;

        }

        public byte[] GetBytes(ushort value)
        {
            return GetBytes(new ushort[1]
{
      value
});
        }


        public ushort ToUInt16(byte[] buffer, int offset)
        {
            byte[] bytes = new byte[2];
            Array.Copy(buffer, offset, bytes, 0, 2);
            if (!IsSameOfSet())
            {
                Array.Reverse(bytes);
            }
            return BitConverter.ToUInt16(bytes, 0);
        }

        public ushort[] ToUInt16(byte[] buffer, int offset, int length)
        {
            ushort[] numArray = new ushort[length];
            for (int index1 = 0; index1 < length; ++index1)
            {
                numArray[index1] = ToUInt16(buffer, offset + (2 * index1));
            }

            return numArray;
        }

        public ushort[,] ToUInt16(byte[] buffer, int offset, int row, int col)
        {
            return GenericHelper.CreateTwoArrayFromOneArray<ushort>(ToUInt16(buffer, offset, row * col), row, col);
        }


        #endregion
        #region int

        public byte[] GetBytes(int value)
        {
            return GetBytes(new int[1]
{
      value
});
        }
        public byte[] GetBytes(int[] values)
        {
            if (values == null)
            {
                return null;
            }

            byte[] numArray = new byte[values.Length * 4];
            for (int offset = 0; offset < values.Length; ++offset)
            {
                ByteTransDataFormat4(BitConverter.GetBytes(values[offset])).CopyTo(numArray, 4 * offset);
            }

            return numArray;
        }


        public int ToInt32(byte[] buffer, int offset)
        {
            byte[] bytes = ByteTransDataFormat4(buffer, offset);

            return BitConverter.ToInt32(bytes, 0);
        }

        public int[] ToInt32(byte[] buffer, int offset, int length)
        {
            int[] numArray = new int[length];
            for (int index1 = 0; index1 < length; ++index1)
            {
                numArray[index1] = ToInt32(buffer, offset + (4 * index1));
            }

            return numArray;
        }

        public int[,] ToInt32(byte[] buffer, int offset, int row, int col)
        {
            return GenericHelper.CreateTwoArrayFromOneArray<int>(ToInt32(buffer, offset, row * col), row, col);
        }


        #endregion
        #region uint
        public byte[] GetBytes(uint value)
        {
            return GetBytes(new uint[1]
{
      value
});
        }
        public byte[] GetBytes(uint[] values)
        {
            if (values == null)
            {
                return null;
            }

            byte[] numArray = new byte[values.Length * 4];
            for (int offset = 0; offset < values.Length; ++offset)
            {
                ByteTransDataFormat4(BitConverter.GetBytes(values[offset])).CopyTo(numArray, 4 * offset);
            }

            return numArray;
        }


        public uint ToUInt32(byte[] buffer, int offset)
        {
            byte[] bytes = ByteTransDataFormat4(buffer, offset);

            return BitConverter.ToUInt32(bytes, 0);
        }

        public uint[] ToUInt32(byte[] buffer, int offset, int length)
        {
            uint[] numArray = new uint[length];
            for (int index1 = 0; index1 < length; ++index1)
            {
                numArray[index1] = ToUInt32(buffer, offset + (4 * index1));
            }

            return numArray;
        }

        public uint[,] ToUInt32(byte[] buffer, int offset, int row, int col)
        {
            return GenericHelper.CreateTwoArrayFromOneArray<uint>(ToUInt32(buffer, offset, row * col), row, col);
        }



        #endregion
        #region long
        public byte[] GetBytes(long value)
        {
            return GetBytes(new long[1]
{
      value
});
        }
        public byte[] GetBytes(long[] values)
        {
            if (values == null)
            {
                return null;
            }

            byte[] numArray = new byte[values.Length * 8];
            for (int offset = 0; offset < values.Length; ++offset)
            {
                ByteTransDataFormat8(BitConverter.GetBytes(values[offset])).CopyTo(numArray, 8 * offset);
            }

            return numArray;
        }


        public long ToInt64(byte[] buffer, int offset)
        {
            byte[] bytes = ByteTransDataFormat8(buffer, offset);
            return BitConverter.ToInt64(bytes, 0);
        }

        public long[] ToInt64(byte[] buffer, int offset, int length)
        {
            long[] numArray = new long[length];
            for (int index1 = 0; index1 < length; ++index1)
            {
                numArray[index1] = ToInt64(buffer, offset + (8 * index1));
            }

            return numArray;
        }

        public long[,] ToInt64(byte[] buffer, int offset, int row, int col)
        {
            return GenericHelper.CreateTwoArrayFromOneArray<long>(ToInt64(buffer, offset, row * col), row, col);
        }

        #endregion
        #region ulong

        public byte[] GetBytes(ulong value)
        {
            return GetBytes(new ulong[1]
{
      value
});
        }
        public byte[] GetBytes(ulong[] values)
        {
            if (values == null)
            {
                return null;
            }

            byte[] numArray = new byte[values.Length * 8];
            for (int offset = 0; offset < values.Length; ++offset)
            {
                ByteTransDataFormat8(BitConverter.GetBytes(values[offset])).CopyTo(numArray, 8 * offset);
            }

            return numArray;
        }



        public ulong ToUInt64(byte[] buffer, int offset)
        {
            byte[] bytes = ByteTransDataFormat8(buffer, offset);
            return BitConverter.ToUInt64(bytes, 0);
        }

        public ulong[] ToUInt64(byte[] buffer, int offset, int length)
        {
            ulong[] numArray = new ulong[length];
            for (int index1 = 0; index1 < length; ++index1)
            {
                numArray[index1] = ToUInt64(buffer, offset + (8 * index1));
            }

            return numArray;
        }

        public ulong[,] ToUInt64(byte[] buffer, int offset, int row, int col)
        {
            return GenericHelper.CreateTwoArrayFromOneArray<ulong>(ToUInt64(buffer, offset, row * col), row, col);
        }

        #endregion
        #region float
        public byte[] GetBytes(float value)
        {
            return GetBytes(new float[1]
{
      value
});
        }
        public byte[] GetBytes(float[] values)
        {
            if (values == null)
            {
                return null;
            }

            byte[] numArray = new byte[values.Length * 4];
            for (int offset = 0; offset < values.Length; ++offset)
            {
                ByteTransDataFormat4(BitConverter.GetBytes(values[offset])).CopyTo(numArray, 4 * offset);
            }

            return numArray;
        }


        public float ToSingle(byte[] buffer, int offset)
        {
            byte[] bytes = ByteTransDataFormat4(buffer, offset);

            return BitConverter.ToSingle(bytes, 0);
        }

        public float[] ToSingle(byte[] buffer, int offset, int length)
        {
            float[] numArray = new float[length];
            for (int index1 = 0; index1 < length; ++index1)
            {
                numArray[index1] = ToSingle(buffer, offset + (4 * index1));
            }

            return numArray;
        }

        public float[,] ToSingle(byte[] buffer, int offset, int row, int col)
        {
            return GenericHelper.CreateTwoArrayFromOneArray<float>(ToSingle(buffer, offset, row * col), row, col);
        }


        #endregion
        #region double
        public byte[] GetBytes(double value)
        {
            return GetBytes(new double[1]
{
      value
});
        }
        public byte[] GetBytes(double[] values)
        {
            if (values == null)
            {
                return null;
            }

            byte[] numArray = new byte[values.Length * 8];
            for (int offset = 0; offset < values.Length; ++offset)
            {
                ByteTransDataFormat8(BitConverter.GetBytes(values[offset])).CopyTo(numArray, 8 * offset);
            }

            return numArray;
        }





        /// <summary>
        ///  转换为指定端模式的double数据。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public double ToDouble(byte[] buffer, int offset)
        {
            byte[] bytes = ByteTransDataFormat8(buffer, offset);

            return BitConverter.ToDouble(bytes, 0);
        }

        public double[] ToDouble(byte[] buffer, int offset, int length)
        {
            double[] numArray = new double[length];
            for (int index1 = 0; index1 < length; ++index1)
            {
                numArray[index1] = ToDouble(buffer, offset + (8 * index1));
            }

            return numArray;
        }

        public double[,] ToDouble(byte[] buffer, int offset, int row, int col)
        {
            return GenericHelper.CreateTwoArrayFromOneArray<double>(ToDouble(buffer, offset, row * col), row, col);
        }

        #endregion
        #region char
        public byte[] GetBytes(char value)
        {
            return GetBytes(new char[1]
{
      value
});
        }
        public byte[] GetBytes(char[] values)
        {
            if (values == null)
            {
                return null;
            }

            byte[] numArray = new byte[values.Length * 2];
            for (int offset = 0; offset < values.Length; ++offset)
            {
                byte[] bytes = BitConverter.GetBytes(values[offset]);
                if (!IsSameOfSet())
                {
                    Array.Reverse(bytes);
                }
                bytes.CopyTo(numArray, 2 * offset);
            }
            return numArray;
        }

        public char ToChar(byte[] buffer, int offset)
        {
            byte[] bytes = new byte[2];
            Array.Copy(buffer, offset, bytes, 0, bytes.Length);
            if (!IsSameOfSet())
            {
                Array.Reverse(bytes);
            }
            return BitConverter.ToChar(bytes, 0);
        }

        public char[] ToChar(byte[] buffer, int offset, int length)
        {
            if (buffer == null)
            {
                return null;
            }
            char[] numArray = new char[length];
            for (int index1 = 0; index1 < length; ++index1)
            {
                numArray[index1] = ToChar(buffer, offset + (2 * index1));
            }

            return numArray;
        }
        #endregion

        #region char

        public byte ToByte(byte[] buffer, int offset)
        {
            byte[] bytes = new byte[1];
            Array.Copy(buffer, offset, bytes, 0, bytes.Length);
            return bytes[0];
        }

        public byte[] ToByte(byte[] buffer, int offset, int length)
        {
            byte[] bytes = new byte[length];
            Array.Copy(buffer, offset, bytes, 0, bytes.Length);
            return bytes;
        }
        #endregion
        public bool IsSameOfSet()
        {
            return !(BitConverter.IsLittleEndian ^ (endianType == EndianType.Little));
        }

        #region string
        public byte[] GetBytes(string value, Encoding encoding)
        {
            if (value == null)
            {
                return null;
            }

            byte[] bytes = encoding.GetBytes(value);
            return IsStringReverseByteWord ? bytes.BytesReverseByWord() : bytes;
        }

        public byte[] GetBytes(string value, int length, Encoding encoding)
        {
            if (value == null)
            {
                return null;
            }

            byte[] bytes = encoding.GetBytes(value);
            return IsStringReverseByteWord ? bytes.BytesReverseByWord().ArrayExpandToLength(length) : bytes.ArrayExpandToLength(length);

        }
        public byte[] GetBytes(string value, BCDFormat bCDFormat)
        {
            if (value == null)
            {
                return null;
            }

            byte[] bytes = DataHelper.GetBytesFromBCD(value, bCDFormat);
            return IsStringReverseByteWord ? bytes.BytesReverseByWord() : bytes;
        }
        public byte[] GetBytes(string value, int length, BCDFormat bCDFormat)
        {
            if (value == null)
            {
                return null;
            }

            byte[] bytes = DataHelper.GetBytesFromBCD(value, bCDFormat);
            return IsStringReverseByteWord ? bytes.BytesReverseByWord().ArrayExpandToLength(length) : bytes.ArrayExpandToLength(length);

        }
        public string ToBcdString(byte[] buffer, BCDFormat bCDFormat)
        {
            return ToBcdString(buffer, 0, buffer.Length, bCDFormat);
        }
        public string ToBcdString(byte[] buffer, int offset, int length, BCDFormat bCDFormat)
        {
            return IsStringReverseByteWord ? DataHelper.GetBCDValue(buffer.SelectMiddle<byte>(offset, length).BytesReverseByWord(), bCDFormat) : DataHelper.GetBCDValue(buffer.SelectMiddle<byte>(offset, length), bCDFormat);
        }
        public string ToString(byte[] buffer, Encoding encoding)
        {
            return ToString(buffer, 0, buffer.Length, encoding);
        }

        public string ToString(byte[] buffer, int offset, int length, Encoding encoding)
        {
            byte[] numArray = buffer.SelectMiddle<byte>(offset, length);
            return IsStringReverseByteWord ?
                encoding.GetString(numArray.BytesReverseByWord()).TrimEnd().Replace($"\0", "") :
                encoding.GetString(numArray).TrimEnd().Replace($"\0", "");
            ;

        }

        #endregion










        #endregion Public Methods

        #region Protected Methods

        /// <summary>反转多字节的数据信息</summary>
        /// <param name="value">数据字节</param>
        /// <param name="offset">起始索引，默认值为0</param>
        /// <returns>实际字节信息</returns>
        protected byte[] ByteTransDataFormat4(byte[] value, int offset = 0)
        {
            byte[] numArray = new byte[4];
            switch (DataFormat)
            {
                case DataFormat.ABCD:
                    numArray[0] = value[offset + 3];
                    numArray[1] = value[offset + 2];
                    numArray[2] = value[offset + 1];
                    numArray[3] = value[offset];
                    break;

                case DataFormat.BADC:
                    numArray[0] = value[offset + 2];
                    numArray[1] = value[offset + 3];
                    numArray[2] = value[offset];
                    numArray[3] = value[offset + 1];
                    break;

                case DataFormat.CDAB:
                    numArray[0] = value[offset + 1];
                    numArray[1] = value[offset];
                    numArray[2] = value[offset + 3];
                    numArray[3] = value[offset + 2];
                    break;

                case DataFormat.DCBA:
                    numArray[0] = value[offset];
                    numArray[1] = value[offset + 1];
                    numArray[2] = value[offset + 2];
                    numArray[3] = value[offset + 3];
                    break;
            }
            return numArray;
        }

        /// <summary>反转多字节的数据信息</summary>
        /// <param name="value">数据字节</param>
        /// <param name="offset">起始索引，默认值为0</param>
        /// <returns>实际字节信息</returns>
        protected byte[] ByteTransDataFormat8(byte[] value, int offset = 0)
        {
            byte[] numArray = new byte[8];
            switch (DataFormat)
            {
                case DataFormat.ABCD:
                    numArray[0] = value[offset + 7];
                    numArray[1] = value[offset + 6];
                    numArray[2] = value[offset + 5];
                    numArray[3] = value[offset + 4];
                    numArray[4] = value[offset + 3];
                    numArray[5] = value[offset + 2];
                    numArray[6] = value[offset + 1];
                    numArray[7] = value[offset];
                    break;

                case DataFormat.BADC:
                    numArray[0] = value[offset + 6];
                    numArray[1] = value[offset + 7];
                    numArray[2] = value[offset + 4];
                    numArray[3] = value[offset + 5];
                    numArray[4] = value[offset + 2];
                    numArray[5] = value[offset + 3];
                    numArray[6] = value[offset];
                    numArray[7] = value[offset + 1];
                    break;

                case DataFormat.CDAB:
                    numArray[0] = value[offset + 1];
                    numArray[1] = value[offset];
                    numArray[2] = value[offset + 3];
                    numArray[3] = value[offset + 2];
                    numArray[4] = value[offset + 5];
                    numArray[5] = value[offset + 4];
                    numArray[6] = value[offset + 7];
                    numArray[7] = value[offset + 6];
                    break;

                case DataFormat.DCBA:
                    numArray[0] = value[offset];
                    numArray[1] = value[offset + 1];
                    numArray[2] = value[offset + 2];
                    numArray[3] = value[offset + 3];
                    numArray[4] = value[offset + 4];
                    numArray[5] = value[offset + 5];
                    numArray[6] = value[offset + 6];
                    numArray[7] = value[offset + 7];
                    break;
            }
            return numArray;
        }

        #endregion Protected Methods

    }
}