using System.Text;
using System.Threading.Tasks;

namespace ThingsGateway.Foundation
{
    public interface IReadWrite
    {

        ILog Logger { get; }

        /// <summary>
        /// 直接发送字节组<br />
        /// </summary>
        OperResult<byte[]> Send(byte[] data, WaitingOptions waitingOptions = null);
        /// <summary>
        /// 直接发送字节组<br />
        /// </summary>
        Task<OperResult<byte[]>> SendAsync(byte[] data, WaitingOptions waitingOptions = null);

        /// <summary>
        /// 批量读取字节数组信息，需要指定地址和长度，返回原始的字节数组<br />
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="length">数据长度</param>
        /// <returns>带有成功标识的byte[]数组</returns>
        OperResult<byte[]> Read(string address, ushort length);

        /// <summary>
        /// 异步批量读取字节数组信息，需要指定地址和长度，返回原始的字节数组<br />
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="length">数据长度</param>
        /// <returns>带有成功标识的byte[]数组</returns>
        Task<OperResult<byte[]>> ReadAsync(string address, ushort length);

        /// <summary>
        /// 批量读取<see cref="T:System.Boolean" />数组信息，需要指定地址和长度，返回<see cref="T:System.Boolean" /> 数组<br />
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="length">数据长度</param>
        OperResult<bool[]> ReadBool(string address, ushort length);

        /// <summary>
        /// 读取单个的<see cref="T:System.Boolean" />数据信息<br />
        /// </summary>
        /// <param name="address">数据地址</param>
        OperResult<bool> ReadBool(string address);

        /// <summary>
        /// 异步批量读取<see cref="T:System.Boolean" />数组信息，需要指定地址和长度，返回<see cref="T:System.Boolean" /> 数组<br />
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="length">数据长度</param>
        Task<OperResult<bool[]>> ReadBoolAsync(string address, ushort length);

        /// <summary>
        /// 异步读取单个的<see cref="T:System.Boolean" />数据信息<br />
        /// </summary>
        /// <param name="address">数据地址</param>
        Task<OperResult<bool>> ReadBoolAsync(string address);


        /// <summary>
        /// 读取双浮点的数据<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带有成功标识的double数据</returns>
        OperResult<double> ReadDouble(string address);

        /// <summary>
        /// 读取双浮点数据的数组<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带有成功标识的double数组</returns>
        OperResult<double[]> ReadDouble(string address, ushort length);

        /// <summary>
        /// 异步读取双浮点的数据<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带有成功标识的double数据</returns>
        Task<OperResult<double>> ReadDoubleAsync(string address);

        /// <summary>
        /// 异步读取双浮点数据的数组<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带有成功标识的double数组</returns>
        Task<OperResult<double[]>> ReadDoubleAsync(string address, ushort length);

        /// <summary>
        /// 读取单浮点数据<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带有成功标识的float数据</returns>

        OperResult<float> ReadFloat(string address);

        /// <summary>
        /// 读取单浮点精度的数组<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带有成功标识的float数组</returns>

        OperResult<float[]> ReadFloat(string address, ushort length);

        /// <summary>
        /// 异步读取单浮点数据<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带有成功标识的float数据</returns>

        Task<OperResult<float>> ReadFloatAsync(string address);

        /// <summary>
        /// 异步读取单浮点精度的数组<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带有成功标识的float数组</returns>

        Task<OperResult<float[]>> ReadFloatAsync(string address, ushort length);

        /// <summary>
        /// 读取16位的有符号的整型数据<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带有成功标识的short数据</returns>

        OperResult<short> ReadInt16(string address);

        /// <summary>
        /// 读取16位的有符号整型数组<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">读取的数组长度</param>
        /// <returns>带有成功标识的short数组</returns>

        OperResult<short[]> ReadInt16(string address, ushort length);

        /// <summary>
        /// 异步读取16位的有符号的整型数据<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带有成功标识的short数据</returns>

        Task<OperResult<short>> ReadInt16Async(string address);

        /// <summary>
        /// 异步读取16位的有符号整型数组<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">读取的数组长度</param>
        /// <returns>带有成功标识的short数组</returns>

        Task<OperResult<short[]>> ReadInt16Async(string address, ushort length);

        /// <summary>
        /// 读取32位的有符号整型<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带有成功标识的int数据</returns>

        OperResult<int> ReadInt32(string address);

        /// <summary>
        /// 读取32位有符号整型数组<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带有成功标识的int数组</returns>

        OperResult<int[]> ReadInt32(string address, ushort length);

        /// <summary>
        /// 异步读取32位的有符号整型<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带有成功标识的int数据</returns>

        Task<OperResult<int>> ReadInt32Async(string address);

        /// <summary>
        /// 异步读取32位有符号整型数组<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带有成功标识的int数组</returns>

        Task<OperResult<int[]>> ReadInt32Async(string address, ushort length);

        /// <summary>
        /// 读取64位的有符号整型<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带有成功标识的long数据</returns>

        OperResult<long> ReadInt64(string address);

        /// <summary>
        /// 读取64位的有符号整型数组<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带有成功标识的long数组</returns>

        OperResult<long[]> ReadInt64(string address, ushort length);

        /// <summary>
        /// 异步读取64位的有符号整型<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带有成功标识的long数据</returns>

        Task<OperResult<long>> ReadInt64Async(string address);

        /// <summary>
        /// 异步读取64位的有符号整型数组<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带有成功标识的long数组</returns>

        Task<OperResult<long[]>> ReadInt64Async(string address, ushort length);

        /// <summary>
        /// 读取字符串数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数据长度</param>
        /// <returns>带有成功标识的string数据</returns>

        OperResult<string> ReadString(string address, ushort length);

        /// <summary>
        /// 使用指定的编码，读取字符串数据<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数据长度</param>
        /// <param name="encoding">指定的自定义的编码</param>
        /// <returns>带有成功标识的string数据</returns>

        OperResult<string> ReadString(string address, ushort length, Encoding encoding);

        /// <summary>
        /// 异步读取字符串数据
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数据长度</param>
        /// <returns>带有成功标识的string数据</returns>

        Task<OperResult<string>> ReadStringAsync(string address, ushort length);

        /// <summary>
        /// 异步使用指定的编码
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数据长度</param>
        /// <param name="encoding">指定的自定义的编码</param>
        /// <returns>带有成功标识的string数据</returns>

        Task<OperResult<string>> ReadStringAsync(
          string address,
          ushort length,
          Encoding encoding);


        /// <summary>
        /// 读取16位的无符号整型<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带有成功标识的ushort数据</returns>

        OperResult<ushort> ReadUInt16(string address);

        /// <summary>
        /// 读取16位的无符号整型数组<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">读取的数组长度</param>
        /// <returns>带有成功标识的ushort数组</returns>

        OperResult<ushort[]> ReadUInt16(string address, ushort length);

        /// <summary>
        /// 异步读取16位的无符号整型<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带有成功标识的ushort数据</returns>

        Task<OperResult<ushort>> ReadUInt16Async(string address);

        /// <summary>
        /// 异步读取16位的无符号整型数组<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">读取的数组长度</param>
        /// <returns>带有成功标识的ushort数组</returns>

        Task<OperResult<ushort[]>> ReadUInt16Async(string address, ushort length);

        /// <summary>
        /// 读取32位的无符号整型<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带有成功标识的uint数据</returns>

        OperResult<uint> ReadUInt32(string address);

        /// <summary>
        /// 读取32位的无符号整型数组<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带有成功标识的uint数组</returns>

        OperResult<uint[]> ReadUInt32(string address, ushort length);

        /// <summary>
        /// 异步读取32位的无符号整型<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带有成功标识的uint数据</returns>

        Task<OperResult<uint>> ReadUInt32Async(string address);

        /// <summary>
        /// 异步读取32位的无符号整型数组<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带有成功标识的uint数组</returns>

        Task<OperResult<uint[]>> ReadUInt32Async(string address, ushort length);

        /// <summary>
        /// 读取64位的无符号整型<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带有成功标识的ulong数据</returns>

        OperResult<ulong> ReadUInt64(string address);

        /// <summary>
        /// 读取64位的无符号整型的数组<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带成功标志的结果数据对象</returns>

        OperResult<ulong[]> ReadUInt64(string address, ushort length);

        /// <summary>
        /// 异步读取64位的无符号整型<br />
        /// Asynchronously read 64-bit unsigned integer
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>带有成功标识的ulong数据</returns>

        Task<OperResult<ulong>> ReadUInt64Async(string address);

        /// <summary>
        /// 异步读取64位的无符号整型的数组<br />
        /// Asynchronously read 64-bit unsigned integer array
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数组长度</param>
        /// <returns>带有成功标识的ulong数组</returns>

        Task<OperResult<ulong[]>> ReadUInt64Async(string address, ushort length);



        /// <summary>
        /// 写入原始的byte数组数据到指定的地址，返回是否写入成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>是否写入成功</returns>
        OperResult Write(string address, byte[] value);
        /// <summary>
        /// 写入原始的byte数据到指定的地址，返回是否写入成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>是否写入成功</returns>
        OperResult Write(string address, byte value);
        /// <summary>
        /// 批量写入<see cref="T:System.Boolean" />数组数据，返回是否成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>是否写入成功</returns>
        OperResult Write(string address, bool[] value);

        /// <summary>
        /// 写入单个的<see cref="T:System.Boolean" />数据，返回是否成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>是否写入成功</returns>
        OperResult Write(string address, bool value);

        /// <summary>
        /// 写入short数据，返回是否成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>

        OperResult Write(string address, short value);

        /// <summary>
        /// 写入short数组，返回是否成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="values">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>

        OperResult Write(string address, short[] values);

        /// <summary>
        /// 写入ushort数据，返回是否成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>

        OperResult Write(string address, ushort value);

        /// <summary>
        /// 写入ushort数组，返回是否成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="values">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>

        OperResult Write(string address, ushort[] values);

        /// <summary>
        /// 写入int数据，返回是否成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>

        OperResult Write(string address, int value);

        /// <summary>
        /// 写入int[]数组，返回是否成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="values">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>

        OperResult Write(string address, int[] values);

        /// <summary>
        /// 写入uint数据，返回是否成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>

        OperResult Write(string address, uint value);

        /// <summary>
        /// 写入uint[]数组，返回是否成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="values">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>

        OperResult Write(string address, uint[] values);

        /// <summary>
        /// 写入long数据，返回是否成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>

        OperResult Write(string address, long value);

        /// <summary>
        /// 写入long数组，返回是否成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="values">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>

        OperResult Write(string address, long[] values);

        /// <summary>
        /// 写入ulong数据，返回是否成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>

        OperResult Write(string address, ulong value);

        /// <summary>
        /// 写入ulong数组，返回是否成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="values">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>

        OperResult Write(string address, ulong[] values);

        /// <summary>
        /// 写入float数据，返回是否成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>

        OperResult Write(string address, float value);

        /// <summary>
        /// 写入float数组，返回是否成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="values">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>

        OperResult Write(string address, float[] values);

        /// <summary>
        /// 写入double数据，返回是否成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>

        OperResult Write(string address, double value);

        /// <summary>
        /// 写入double数组，返回是否成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="values">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>

        OperResult Write(string address, double[] values);

        /// <summary>
        /// 写入字符串信息
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>

        OperResult Write(string address, string value);

        /// <summary>
        /// 写入字符串信息，需要指定的编码信息<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <param name="encoding">指定的编码信息</param>
        /// <returns>带有成功标识的结果类对象</returns>

        OperResult Write(string address, string value, Encoding encoding);

        /// <summary>
        /// 写入指定长度的字符串信息
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <param name="length">字符串的长度</param>
        /// <returns>带有成功标识的结果类对象</returns>

        OperResult Write(string address, string value, int length);

        /// <summary>
        /// 写入指定长度的字符串信息
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <param name="length">字符串的长度</param>
        /// <param name="encoding">指定的编码信息</param>
        /// <returns>带有成功标识的结果类对象</returns>

        OperResult Write(string address, string value, int length, Encoding encoding);

        /// <summary>
        /// 异步写入原始的byte数组数据到指定的地址，返回是否写入成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>

        Task<OperResult> WriteAsync(string address, byte[] value);

        /// <summary>
        /// 异步批量写入<see cref="T:System.Boolean" />数组数据，返回是否成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>
        Task<OperResult> WriteAsync(string address, bool[] value);

        /// <summary>
        /// 异步批量写入<see cref="T:System.Boolean" />数组数据，返回是否成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>
        Task<OperResult> WriteAsync(string address, bool value);

        /// <summary>
        /// 异步写入short数据，返回是否成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>

        Task<OperResult> WriteAsync(string address, short value);

        /// <summary>
        /// 异步写入short数组，返回是否成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="values">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>

        Task<OperResult> WriteAsync(string address, short[] values);

        /// <summary>
        /// 异步写入ushort数据，返回是否成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>

        Task<OperResult> WriteAsync(string address, ushort value);

        /// <summary>
        /// 异步写入ushort数组，返回是否成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="values">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>

        Task<OperResult> WriteAsync(string address, ushort[] values);

        /// <summary>
        /// 异步写入int数据，返回是否成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>

        Task<OperResult> WriteAsync(string address, int value);

        /// <summary>
        /// 异步写入int[]数组，返回是否成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="values">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>

        Task<OperResult> WriteAsync(string address, int[] values);

        /// <summary>
        /// 异步写入uint数据，返回是否成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>

        Task<OperResult> WriteAsync(string address, uint value);

        /// <summary>
        /// 异步写入uint[]数组，返回是否成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="values">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>

        Task<OperResult> WriteAsync(string address, uint[] values);

        /// <summary>
        /// 异步写入long数据，返回是否成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>

        Task<OperResult> WriteAsync(string address, long value);

        /// <summary>
        /// 异步写入long数组，返回是否成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="values">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>

        Task<OperResult> WriteAsync(string address, long[] values);

        /// <summary>
        /// 异步写入ulong数据，返回是否成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>

        Task<OperResult> WriteAsync(string address, ulong value);

        /// <summary>
        /// 异步写入ulong数组，返回是否成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="values">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>

        Task<OperResult> WriteAsync(string address, ulong[] values);

        /// <summary>
        /// 异步写入float数据，返回是否成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>

        Task<OperResult> WriteAsync(string address, float value);

        /// <summary>
        /// 异步写入float数组，返回是否成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="values">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>

        Task<OperResult> WriteAsync(string address, float[] values);

        /// <summary>
        /// 异步写入double数据，返回是否成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>

        Task<OperResult> WriteAsync(string address, double value);

        /// <summary>
        /// 异步写入double数组，返回是否成功<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="values">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>

        Task<OperResult> WriteAsync(string address, double[] values);

        /// <summary>
        /// 异步写入字符串信息，编码为ASCII<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <returns>带有成功标识的结果类对象</returns>

        Task<OperResult> WriteAsync(string address, string value);

        /// <summary>
        /// 异步写入字符串信息，需要指定的编码信息<br />
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <param name="encoding">指定的编码信息</param>
        /// <returns>带有成功标识的结果类对象</returns>

        Task<OperResult> WriteAsync(
          string address,
          string value,
          Encoding encoding);

        /// <summary>
        /// 异步写入指定长度的字符串信息
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <param name="length">字符串的长度</param>
        /// <returns>带有成功标识的结果类对象</returns>
        Task<OperResult> WriteAsync(string address, string value, int length);

        /// <summary>
        /// 异步写入指定长度的字符串信息
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="value">写入值</param>
        /// <param name="length">字符串的长度</param>
        /// <param name="encoding">指定的编码信息</param>
        /// <returns>带有成功标识的结果类对象</returns>
        Task<OperResult> WriteAsync(
          string address,
          string value,
          int length,
          Encoding encoding);

    }
}