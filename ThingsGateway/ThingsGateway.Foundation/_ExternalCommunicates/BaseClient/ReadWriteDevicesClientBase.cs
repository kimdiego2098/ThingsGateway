using System.Text;
using System.Threading.Tasks;

namespace ThingsGateway.Foundation
{
    /// <summary>
    /// TCP读写设备
    /// </summary>
    public abstract class ReadWriteDevicesClientBase : IReadWriteDevice
    {
        public ushort ConnectTimeOut { get; set; } = 3000;
        public ILog Logger { get; protected set; }
        public IThingsGatewayBitConverter ThingsGatewayBitConverter { get; protected set; } = new ThingsGatewayBitConverter(EndianType.Big);
        public DataFormat DataFormat
        {
            get
            {
                return ThingsGatewayBitConverter.DataFormat;
            }
            set
            {
                ThingsGatewayBitConverter.DataFormat = value;
            }
        }
        public ushort TimeOut { get; set; } = 3000;
        public ushort WordLength { get; set; } = 1;
        public abstract void SetDataAdapter();
        public abstract void Connect();
        public abstract void Disconnect();

        public virtual ushort GetWordLength(string address, int length, int dataTypeLength)
        {
            if (WordLength != 0)
            {
                return (ushort)(WordLength * length * dataTypeLength);
            }

            int num = length * dataTypeLength * 2 / 4;
            return num == 0 ? (ushort)1 : (ushort)num;
        }

        public abstract OperResult<byte[]> Send(byte[] data, WaitingOptions waitingOptions = null);
        public virtual async Task<OperResult<byte[]>> SendAsync(byte[] data, WaitingOptions waitingOptions = null)
        {
            OperResult<byte[]> result = await Task.FromResult(Send(data, waitingOptions));
            return result;
        }
        public abstract OperResult<byte[]> Read(string address, ushort length);

        public virtual async Task<OperResult<byte[]>> ReadAsync(string address, ushort length)
        {
            OperResult<byte[]> result = await Task.FromResult(Read(address, length));
            return result;
        }

        public abstract OperResult<bool[]> ReadBool(string address, ushort length);

        public virtual OperResult<bool> ReadBool(string address)
        {
            return ByteTransformHelper.GetResultFromArray<bool>(ReadBool(address, 1));
        }

        public virtual async Task<OperResult<bool[]>> ReadBoolAsync(string address, ushort length)
        {
            OperResult<bool[]> result = await Task.FromResult(ReadBool(address, length));
            return result;
        }

        public virtual async Task<OperResult<bool>> ReadBoolAsync(string address)
        {
            return await Task.FromResult(ReadBool(address));
        }

        public virtual OperResult<double> ReadDouble(string address)
        {
            return ByteTransformHelper.GetResultFromArray<double>(ReadDouble(address, 1));
        }

        public virtual OperResult<double[]> ReadDouble(string address, ushort length)
        {
            IThingsGatewayBitConverter transformParameter = ByteConverterHelper.GetTransByAddress(ref address, ThingsGatewayBitConverter);
            return ByteTransformHelper.GetResultFromBytes<double[]>(Read(address, GetWordLength(address, length, 4)), m => transformParameter.ToDouble(m, 0, length));
        }

        public virtual async Task<OperResult<double>> ReadDoubleAsync(string address)
        {
            OperResult<double[]> result = await ReadDoubleAsync(address, 1);
            return ByteTransformHelper.GetResultFromArray<double>(result);
        }

        public virtual async Task<OperResult<double[]>> ReadDoubleAsync(string address, ushort length)
        {
            OperResult<byte[]> result = await ReadAsync(address, GetWordLength(address, length, 4));
            IThingsGatewayBitConverter transformParameter = ByteConverterHelper.GetTransByAddress(ref address, ThingsGatewayBitConverter);
            return ByteTransformHelper.GetResultFromBytes<double[]>(result, m => transformParameter.ToDouble(m, 0, length));
        }

        public virtual OperResult<float> ReadFloat(string address)
        {
            return ByteTransformHelper.GetResultFromArray<float>(ReadFloat(address, 1));
        }

        public virtual OperResult<float[]> ReadFloat(string address, ushort length)
        {
            IThingsGatewayBitConverter transformParameter = ByteConverterHelper.GetTransByAddress(ref address, ThingsGatewayBitConverter);
            return ByteTransformHelper.GetResultFromBytes<float[]>(Read(address, GetWordLength(address, length, 2)), m => transformParameter.ToSingle(m, 0, length));
        }

        public virtual async Task<OperResult<float>> ReadFloatAsync(string address)
        {
            OperResult<float[]> result = await ReadFloatAsync(address, 1);
            return ByteTransformHelper.GetResultFromArray<float>(result);
        }

        public virtual async Task<OperResult<float[]>> ReadFloatAsync(string address, ushort length)
        {
            OperResult<byte[]> result = await ReadAsync(address, GetWordLength(address, length, 2));
            IThingsGatewayBitConverter transformParameter = ByteConverterHelper.GetTransByAddress(ref address, ThingsGatewayBitConverter);
            return ByteTransformHelper.GetResultFromBytes<float[]>(result, m => transformParameter.ToSingle(m, 0, length));

        }

        public virtual OperResult<short> ReadInt16(string address)
        {
            return ByteTransformHelper.GetResultFromArray<short>(ReadInt16(address, 1));
        }

        public virtual OperResult<short[]> ReadInt16(string address, ushort length)
        {
            IThingsGatewayBitConverter transformParameter = ByteConverterHelper.GetTransByAddress(ref address, ThingsGatewayBitConverter);
            return ByteTransformHelper.GetResultFromBytes<short[]>(Read(address, GetWordLength(address, length, 1)), m => transformParameter.ToInt16(m, 0, length));
        }

        public virtual async Task<OperResult<short>> ReadInt16Async(string address)
        {
            OperResult<short[]> result = await ReadInt16Async(address, 1);
            return ByteTransformHelper.GetResultFromArray<short>(result);
        }

        public virtual async Task<OperResult<short[]>> ReadInt16Async(string address, ushort length)
        {
            OperResult<byte[]> result = await ReadAsync(address, GetWordLength(address, length, 1));
            IThingsGatewayBitConverter transformParameter = ByteConverterHelper.GetTransByAddress(ref address, ThingsGatewayBitConverter);
            return ByteTransformHelper.GetResultFromBytes<short[]>(result, m => transformParameter.ToInt16(m, 0, length));
        }

        public virtual OperResult<int> ReadInt32(string address)
        {
            return ByteTransformHelper.GetResultFromArray<int>(ReadInt32(address, 1));
        }

        public virtual OperResult<int[]> ReadInt32(string address, ushort length)
        {
            IThingsGatewayBitConverter transformParameter = ByteConverterHelper.GetTransByAddress(ref address, ThingsGatewayBitConverter);
            return ByteTransformHelper.GetResultFromBytes<int[]>(Read(address, GetWordLength(address, length, 2)), m => transformParameter.ToInt32(m, 0, length));
        }

        public virtual async Task<OperResult<int>> ReadInt32Async(string address)
        {
            OperResult<int[]> result = await ReadInt32Async(address, 1);
            return ByteTransformHelper.GetResultFromArray<int>(result);
        }

        public virtual async Task<OperResult<int[]>> ReadInt32Async(string address, ushort length)
        {
            OperResult<byte[]> result = await ReadAsync(address, GetWordLength(address, length, 2));
            IThingsGatewayBitConverter transformParameter = ByteConverterHelper.GetTransByAddress(ref address, ThingsGatewayBitConverter);
            return ByteTransformHelper.GetResultFromBytes<int[]>(result, m => transformParameter.ToInt32(m, 0, length));

        }

        public virtual OperResult<long> ReadInt64(string address)
        {
            return ByteTransformHelper.GetResultFromArray<long>(ReadInt64(address, 1));
        }

        public virtual OperResult<long[]> ReadInt64(string address, ushort length)
        {
            IThingsGatewayBitConverter transformParameter = ByteConverterHelper.GetTransByAddress(ref address, ThingsGatewayBitConverter);
            return ByteTransformHelper.GetResultFromBytes<long[]>(Read(address, GetWordLength(address, length, 4)), m => transformParameter.ToInt64(m, 0, length));

        }

        public virtual async Task<OperResult<long>> ReadInt64Async(string address)
        {
            OperResult<long[]> result = await ReadInt64Async(address, 1);
            return ByteTransformHelper.GetResultFromArray<long>(result);
        }

        public virtual async Task<OperResult<long[]>> ReadInt64Async(string address, ushort length)
        {
            OperResult<byte[]> result = await ReadAsync(address, GetWordLength(address, length, 4));
            IThingsGatewayBitConverter transformParameter = ByteConverterHelper.GetTransByAddress(ref address, ThingsGatewayBitConverter);
            return ByteTransformHelper.GetResultFromBytes<long[]>(result, m => transformParameter.ToInt64(m, 0, length));

        }

        public virtual OperResult<string> ReadString(string address, ushort length)
        {
            return ReadString(address, length, Encoding.ASCII);
        }

        public virtual OperResult<string> ReadString(string address, ushort length, Encoding encoding)
        {
            IThingsGatewayBitConverter transformParameter = ByteConverterHelper.GetTransByAddress(
                ref address, ThingsGatewayBitConverter, out Encoding encoding1, out ushort length1, out BCDFormat bCDFormat1);
            length = length1 == 0 ? length : length1;
            encoding = encoding1 == null ? encoding : encoding1;
            return ByteTransformHelper.GetResultFromBytes<string>(Read(address, length), m => transformParameter.ToString(m, 0, m.Length, encoding));

        }

        public virtual async Task<OperResult<string>> ReadStringAsync(string address, ushort length)
        {
            OperResult<string> result = await ReadStringAsync(address, length, Encoding.ASCII);
            return result;
        }

        public virtual async Task<OperResult<string>> ReadStringAsync(string address, ushort length, Encoding encoding)
        {
            return await Task.FromResult(ReadString(address, length, encoding));

        }

        public virtual OperResult<ushort> ReadUInt16(string address)
        {
            return ByteTransformHelper.GetResultFromArray<ushort>(ReadUInt16(address, 1));

        }

        public virtual OperResult<ushort[]> ReadUInt16(string address, ushort length)
        {
            IThingsGatewayBitConverter transformParameter = ByteConverterHelper.GetTransByAddress(ref address, ThingsGatewayBitConverter);
            return ByteTransformHelper.GetResultFromBytes<ushort[]>(Read(address, GetWordLength(address, length, 1)), m => transformParameter.ToUInt16(m, 0, length));

        }

        public virtual async Task<OperResult<ushort>> ReadUInt16Async(string address)
        {
            OperResult<ushort[]> result = await ReadUInt16Async(address, 1);
            return ByteTransformHelper.GetResultFromArray<ushort>(result);
        }

        public virtual async Task<OperResult<ushort[]>> ReadUInt16Async(string address, ushort length)
        {
            OperResult<byte[]> result = await ReadAsync(address, GetWordLength(address, length, 1));
            IThingsGatewayBitConverter transformParameter = ByteConverterHelper.GetTransByAddress(ref address, ThingsGatewayBitConverter);
            return ByteTransformHelper.GetResultFromBytes<ushort[]>(result, m => transformParameter.ToUInt16(m, 0, length));
        }

        public virtual OperResult<uint> ReadUInt32(string address)
        {
            return ByteTransformHelper.GetResultFromArray<uint>(ReadUInt32(address, 1));

        }

        public virtual OperResult<uint[]> ReadUInt32(string address, ushort length)
        {
            IThingsGatewayBitConverter transformParameter = ByteConverterHelper.GetTransByAddress(ref address, ThingsGatewayBitConverter);
            return ByteTransformHelper.GetResultFromBytes<uint[]>(Read(address, GetWordLength(address, length, 2)), m => transformParameter.ToUInt32(m, 0, length));

        }

        public virtual async Task<OperResult<uint>> ReadUInt32Async(string address)
        {
            OperResult<uint[]> result = await ReadUInt32Async(address, 1);
            return ByteTransformHelper.GetResultFromArray<uint>(result);
        }

        public virtual async Task<OperResult<uint[]>> ReadUInt32Async(string address, ushort length)
        {
            OperResult<byte[]> result = await ReadAsync(address, GetWordLength(address, length, 2));
            IThingsGatewayBitConverter transformParameter = ByteConverterHelper.GetTransByAddress(ref address, ThingsGatewayBitConverter);
            return ByteTransformHelper.GetResultFromBytes<uint[]>(result, m => transformParameter.ToUInt32(m, 0, length));

        }

        public virtual OperResult<ulong> ReadUInt64(string address)
        {
            return ByteTransformHelper.GetResultFromArray<ulong>(ReadUInt64(address, 1));

        }

        public virtual OperResult<ulong[]> ReadUInt64(string address, ushort length)
        {

            IThingsGatewayBitConverter transformParameter = ByteConverterHelper.GetTransByAddress(ref address, ThingsGatewayBitConverter);
            return ByteTransformHelper.GetResultFromBytes<ulong[]>(Read(address, GetWordLength(address, length, 4)), m => transformParameter.ToUInt64(m, 0, length));

        }

        public virtual async Task<OperResult<ulong>> ReadUInt64Async(string address)
        {
            OperResult<ulong[]> result = await ReadUInt64Async(address, 1);
            return ByteTransformHelper.GetResultFromArray<ulong>(result);
        }

        public virtual async Task<OperResult<ulong[]>> ReadUInt64Async(string address, ushort length)
        {
            OperResult<byte[]> result = await ReadAsync(address, GetWordLength(address, length, 4));
            IThingsGatewayBitConverter transformParameter = ByteConverterHelper.GetTransByAddress(ref address, ThingsGatewayBitConverter);
            return ByteTransformHelper.GetResultFromBytes<ulong[]>(result, m => transformParameter.ToUInt64(m, 0, length));

        }


        public abstract OperResult Write(string address, byte[] value);

        public abstract OperResult Write(string address, bool[] value);

        public virtual OperResult Write(string address, bool value)
        {
            return Write(address, new bool[1]
{
      value
});
        }

        public virtual OperResult Write(string address, short value)
        {
            return Write(address, new short[1]
{
      value
});
        }

        public virtual OperResult Write(string address, byte value)
        {
            return Write(address, new byte[1]
{
      value
});
        }

        public virtual OperResult Write(string address, short[] values)
        {
            IThingsGatewayBitConverter transformParameter = ByteConverterHelper.GetTransByAddress(ref address, ThingsGatewayBitConverter);
            return Write(address, transformParameter.GetBytes(values));
        }

        public virtual OperResult Write(string address, ushort value)
        {
            return Write(address, new ushort[1]
{
      value
});
        }

        public virtual OperResult Write(string address, ushort[] values)
        {
            IThingsGatewayBitConverter transformParameter = ByteConverterHelper.GetTransByAddress(ref address, ThingsGatewayBitConverter);
            return Write(address, transformParameter.GetBytes(values));
        }

        public virtual OperResult Write(string address, int value)
        {
            return Write(address, new int[1]
{
      value
});
        }

        public virtual OperResult Write(string address, int[] values)
        {
            IThingsGatewayBitConverter transformParameter = ByteConverterHelper.GetTransByAddress(ref address, ThingsGatewayBitConverter);
            return Write(address, transformParameter.GetBytes(values));

        }

        public virtual OperResult Write(string address, uint value)
        {
            return Write(address, new uint[1]
{
      value
});
        }

        public virtual OperResult Write(string address, uint[] values)
        {
            IThingsGatewayBitConverter transformParameter = ByteConverterHelper.GetTransByAddress(ref address, ThingsGatewayBitConverter);
            return Write(address, transformParameter.GetBytes(values));
        }

        public virtual OperResult Write(string address, long value)
        {
            return Write(address, new long[1]
{
      value
});
        }

        public virtual OperResult Write(string address, long[] values)
        {
            IThingsGatewayBitConverter transformParameter = ByteConverterHelper.GetTransByAddress(ref address, ThingsGatewayBitConverter);
            return Write(address, transformParameter.GetBytes(values));

        }

        public virtual OperResult Write(string address, ulong value)
        {
            return Write(address, new ulong[1]
{
      value
});
        }

        public virtual OperResult Write(string address, ulong[] values)
        {
            IThingsGatewayBitConverter transformParameter = ByteConverterHelper.GetTransByAddress(ref address, ThingsGatewayBitConverter);
            return Write(address, transformParameter.GetBytes(values));

        }

        public virtual OperResult Write(string address, float value)
        {
            return Write(address, new float[1]
{
      value
});
        }

        public virtual OperResult Write(string address, float[] values)
        {
            IThingsGatewayBitConverter transformParameter = ByteConverterHelper.GetTransByAddress(ref address, ThingsGatewayBitConverter);
            return Write(address, transformParameter.GetBytes(values));

        }

        public virtual OperResult Write(string address, double value)
        {
            return Write(address, new double[1]
{
      value
});
        }

        public virtual OperResult Write(string address, double[] values)
        {
            IThingsGatewayBitConverter transformParameter = ByteConverterHelper.GetTransByAddress(ref address, ThingsGatewayBitConverter);
            return Write(address, transformParameter.GetBytes(values));

        }

        public virtual OperResult Write(string address, string value)
        {
            return Write(address, value, Encoding.UTF8);

        }

        public virtual OperResult Write(string address, string value, Encoding encoding)
        {
            IThingsGatewayBitConverter transformParameter = ByteConverterHelper.GetTransByAddress(ref address, ThingsGatewayBitConverter);
            byte[] data = transformParameter.GetBytes(value, encoding);
            if (WordLength == 1)
            {
                data = GenericHelper.ArrayExpandToLengthEven<byte>(data);
            }

            return Write(address, data);
        }

        public virtual OperResult Write(string address, string value, int length)
        {
            return Write(address, value, length, Encoding.ASCII);

        }

        public virtual OperResult Write(string address, string value, int length, Encoding encoding)
        {
            IThingsGatewayBitConverter transformParameter = ByteConverterHelper.GetTransByAddress(ref address, ThingsGatewayBitConverter);
            byte[] data = transformParameter.GetBytes(value, encoding);
            if (WordLength == 1)
            {
                data = GenericHelper.ArrayExpandToLengthEven<byte>(data);
            }
            byte[] length1 = GenericHelper.ArrayExpandToLength<byte>(data, length);
            return Write(address, length1);
        }

        public virtual async Task<OperResult> WriteAsync(string address, byte[] value)
        {
            OperResult result = await Task.FromResult(Write(address, value));
            return result;
        }

        public virtual async Task<OperResult> WriteAsync(string address, bool[] value)
        {
            OperResult result = await Task.FromResult(Write(address, value));
            return result;
        }

        public virtual async Task<OperResult> WriteAsync(string address, bool value)
        {
            OperResult result = await WriteAsync(address, new bool[1]
            {
        value
            });
            return result;
        }

        public virtual async Task<OperResult> WriteAsync(string address, short value)
        {
            OperResult result = await WriteAsync(address, new short[1]
        {
        value
        });
            return result;
        }

        public virtual async Task<OperResult> WriteAsync(string address, short[] values)
        {
            IThingsGatewayBitConverter transformParameter = ByteConverterHelper.GetTransByAddress(ref address, ThingsGatewayBitConverter);
            OperResult result = await WriteAsync(address, transformParameter.GetBytes(values));
            return result;
        }

        public virtual async Task<OperResult> WriteAsync(string address, ushort value)
        {
            OperResult result = await WriteAsync(address, new ushort[1]
        {
        value
        });
            return result;
        }

        public virtual async Task<OperResult> WriteAsync(string address, ushort[] values)
        {
            IThingsGatewayBitConverter transformParameter = ByteConverterHelper.GetTransByAddress(ref address, ThingsGatewayBitConverter);
            OperResult result = await WriteAsync(address, transformParameter.GetBytes(values));
            return result;
        }

        public virtual async Task<OperResult> WriteAsync(string address, int value)
        {
            OperResult result = await WriteAsync(address, new int[1]
        {
        value
        });
            return result;
        }

        public virtual async Task<OperResult> WriteAsync(string address, int[] values)
        {
            IThingsGatewayBitConverter transformParameter = ByteConverterHelper.GetTransByAddress(ref address, ThingsGatewayBitConverter);
            OperResult result = await WriteAsync(address, transformParameter.GetBytes(values));
            return result;
        }

        public virtual async Task<OperResult> WriteAsync(string address, uint value)
        {
            OperResult result = await WriteAsync(address, new uint[1]
        {
        value
        });
            return result;
        }

        public virtual async Task<OperResult> WriteAsync(string address, uint[] values)
        {
            IThingsGatewayBitConverter transformParameter = ByteConverterHelper.GetTransByAddress(ref address, ThingsGatewayBitConverter);
            OperResult result = await WriteAsync(address, transformParameter.GetBytes(values));
            return result;
        }

        public virtual async Task<OperResult> WriteAsync(string address, long value)
        {
            OperResult result = await WriteAsync(address, new long[1]
        {
        value
        });
            return result;
        }

        public virtual async Task<OperResult> WriteAsync(string address, long[] values)
        {
            IThingsGatewayBitConverter transformParameter = ByteConverterHelper.GetTransByAddress(ref address, ThingsGatewayBitConverter);
            OperResult result = await WriteAsync(address, transformParameter.GetBytes(values));
            return result;
        }

        public virtual async Task<OperResult> WriteAsync(string address, ulong value)
        {
            OperResult result = await WriteAsync(address, new ulong[1]
    {
        value
    });
            return result;
        }

        public virtual async Task<OperResult> WriteAsync(string address, ulong[] values)
        {
            IThingsGatewayBitConverter transformParameter = ByteConverterHelper.GetTransByAddress(ref address, ThingsGatewayBitConverter);
            OperResult result = await WriteAsync(address, transformParameter.GetBytes(values));
            return result;
        }

        public virtual async Task<OperResult> WriteAsync(string address, float value)
        {
            OperResult result = await WriteAsync(address, new float[1]
{
        value
});
            return result;
        }

        public virtual async Task<OperResult> WriteAsync(string address, float[] values)
        {
            IThingsGatewayBitConverter transformParameter = ByteConverterHelper.GetTransByAddress(ref address, ThingsGatewayBitConverter);
            OperResult result = await WriteAsync(address, transformParameter.GetBytes(values));
            return result;
        }

        public virtual async Task<OperResult> WriteAsync(string address, double value)
        {
            OperResult result = await WriteAsync(address, new double[1]
{
        value
});
            return result;
        }

        public virtual async Task<OperResult> WriteAsync(string address, double[] values)
        {
            IThingsGatewayBitConverter transformParameter = ByteConverterHelper.GetTransByAddress(ref address, ThingsGatewayBitConverter);
            OperResult result = await WriteAsync(address, transformParameter.GetBytes(values));
            return result;
        }

        public virtual async Task<OperResult> WriteAsync(string address, string value)
        {
            OperResult result = await WriteAsync(address, value, Encoding.ASCII);
            return result;
        }

        public virtual async Task<OperResult> WriteAsync(string address, string value, Encoding encoding)
        {
            OperResult result = await WriteAsync(address, value, encoding);
            return result;
        }

        public virtual async Task<OperResult> WriteAsync(string address, string value, int length)
        {
            OperResult result = await WriteAsync(address, value, length, Encoding.ASCII);
            return result;
        }

        public virtual async Task<OperResult> WriteAsync(string address, string value, int length, Encoding encoding)
        {
            IThingsGatewayBitConverter transformParameter = ByteConverterHelper.GetTransByAddress(ref address, ThingsGatewayBitConverter);
            byte[] temp = transformParameter.GetBytes(value, encoding);
            if (WordLength == 1)
            {
                temp = GenericHelper.ArrayExpandToLengthEven<byte>(temp);
            }

            temp = GenericHelper.ArrayExpandToLength<byte>(temp, length);
            OperResult result = await WriteAsync(address, temp);
            temp = null;
            return result;
        }



    }
}
