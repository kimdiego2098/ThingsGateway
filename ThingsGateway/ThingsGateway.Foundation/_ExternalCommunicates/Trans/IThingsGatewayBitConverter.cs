using System.Text;

namespace ThingsGateway.Foundation
{
    public interface IThingsGatewayBitConverter
    {
        #region Public Properties
        DataFormat DataFormat { get; set; }
        EndianType EndianType { get; }
        /// <summary>
        /// 获取或设置在解析字符串的时候是否将字节按照字单位反转<br />
        /// </summary>
        bool IsStringReverseByteWord { get; set; }
        IThingsGatewayBitConverter CreateByDateFormat(DataFormat dataFormat);
        #endregion Public Properties

        #region GetBytes
        /// <summary>
        /// bool变量转化缓存数据，一般来说单bool只能转化为0x01 或是 0x00<br />
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        byte[] GetBytes(bool value);
        /// <summary>
        /// 将bool数组变量转化缓存数据，如果数组长度不满足8的倍数，则自动补0操作。<br />
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        byte[] GetBytes(bool[] values);

        /// <summary>
        /// short变量转化缓存数据，一个short数据可以转为2个字节的byte数组<br />
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        byte[] GetBytes(short value);
        /// <summary>
        /// short数组变量转化缓存数据，n个长度的short数组，可以转为2*n个长度的byte数组<br />
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        byte[] GetBytes(short[] values);
        /// <summary>
        /// ushort变量转化缓存数据，一个ushort数据可以转为2个字节的Byte数组<br />
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        byte[] GetBytes(ushort value);

        /// <summary>
        /// ushort数组变量转化缓存数据，n个长度的ushort数组，可以转为2*n个长度的byte数组<br />
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        byte[] GetBytes(ushort[] values);
        /// <summary>
        /// int变量转化缓存数据，一个int数据可以转为4个字节的byte数组<br />
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        byte[] GetBytes(int value);

        /// <summary>
        /// int数组变量转化缓存数据，n个长度的int数组，可以转为4*n个长度的byte数组<br />
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        byte[] GetBytes(int[] values);

        /// <summary>
        /// uint变量转化缓存数据，一个uint数据可以转为4个字节的byte数组<br />
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        byte[] GetBytes(uint value);

        /// <summary>
        /// uint数组变量转化缓存数据，n个长度的uint数组，可以转为4*n个长度的byte数组<br />
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        byte[] GetBytes(uint[] values);
        /// <summary>
        /// long变量转化缓存数据，一个long数据可以转为8个字节的byte数组<br />
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        byte[] GetBytes(long value);

        /// <summary>
        /// long数组变量转化缓存数据，n个长度的long数组，可以转为8*n个长度的byte数组<br />
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        byte[] GetBytes(long[] values);
        /// <summary>
        /// ulong变量转化缓存数据，一个ulong数据可以转为8个字节的byte数组<br />
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        byte[] GetBytes(ulong value);

        /// <summary>
        /// ulong数组变量转化缓存数据，n个长度的ulong数组，可以转为8*n个长度的byte数组<br />
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        byte[] GetBytes(ulong[] values);
        /// <summary>
        /// float变量转化缓存数据，一个float数据可以转为4个字节的byte数组<br />
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        byte[] GetBytes(float value);

        /// <summary>
        /// float数组变量转化缓存数据，n个长度的float数组，可以转为4*n个长度的byte数组<br />
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        byte[] GetBytes(float[] values);

        /// <summary>
        /// double变量转化缓存数据，一个double数据可以转为8个字节的byte数组<br />
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <returns>buffer数据</returns>
        byte[] GetBytes(double value);

        /// <summary>
        /// double数组变量转化缓存数据，n个长度的double数组，可以转为8*n个长度的byte数组<br />
        /// </summary>
        /// <param name="values">等待转化的数组</param>
        /// <returns>buffer数据</returns>
        byte[] GetBytes(double[] values);

        /// <summary>
        /// 使用指定的编码字符串转化缓存数据<br />
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <param name="encoding">字符串的编码方式</param>
        /// <returns>buffer数据</returns>
        byte[] GetBytes(string value, Encoding encoding);

        /// <summary>
        /// 使用指定的编码字符串转化缓存数据，指定转换之后的字节长度信息<br />
        /// </summary>
        /// <param name="value">等待转化的数据</param>
        /// <param name="length">转换之后的数据长度</param>
        /// <param name="encoding">字符串的编码方式</param>
        /// <returns>buffer数据</returns>
        byte[] GetBytes(string value, int length, Encoding encoding);
        byte[] GetBytes(char value);
        byte[] GetBytes(char[] values);
        #endregion

        /// <summary>
        /// 判断当前系统是否为设置的大小端
        /// </summary>
        /// <returns></returns>
        bool IsSameOfSet();


        #region ToValue
        byte ToByte(byte[] buffer, int offset);
        byte[] ToByte(byte[] buffer, int offset, int length);


        /// <summary>
        ///  转换为指定端模式的Char数据。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        char ToChar(byte[] buffer, int offset);

        /// <summary>
        ///  转换为指定端模式的Char数据。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        char[] ToChar(byte[] buffer, int offset, int length);
        /// <summary>
        /// 从缓存中提取出bool结果，需要传入想要提取的位索引，注意：是从0开始的位索引，10则表示 buffer[1] 的第二位。<br />
        /// </summary>
        /// <param name="buffer">等待提取的缓存数据</param>
        /// <param name="offset">位的索引，注意：是从0开始的位索引，10则表示 buffer[1] 的第二位。</param>
        bool ToBoolean(byte[] buffer, int offset);
        /// <summary>
        /// 从缓存中提取出bool数组结果，需要传入想要提取的位索引，注意：是从0开始的位索引，10则表示 buffer[1] 的第二位。长度为 bool 数量的长度，传入 10 则获取 10 个长度的 bool[] 数组。<br />
        /// </summary>
        /// <param name="buffer">等待提取的缓存数据</param>
        /// <param name="offset">位的索引，注意：是从0开始的位索引，10则表示 buffer[1] 的第二位。</param>
        /// <param name="length">读取的 bool 长度，按照位为单位，传入 10 则表示获取 10 个长度的 bool[] </param>
        bool[] ToBoolean(byte[] buffer, int offset, int length);




        /// <summary>
        /// 从缓存中提取double结果，需要指定起始的字节索引，按照字节为单位，一个double占用八个字节<br />
        /// </summary>
        /// <param name="buffer">缓存对象</param>
        /// <param name="offset">索引位置</param>
        /// <returns>double对象</returns>
        double ToDouble(byte[] buffer, int offset);
        /// <summary>
        /// 从缓存中提取double数组结果，需要指定起始的字节索引，按照字节为单位，然后指定提取的 double 数组的长度，如果传入 10 ，则表示提取 10 个连续的 double 数据，该数据共占用 80 字节。<br />
        /// </summary>
        /// <param name="buffer">缓存对象</param>
        /// <param name="offset">索引位置</param>
        /// <param name="length">读取的数组长度</param>
        /// <returns>double数组</returns>
        double[] ToDouble(byte[] buffer, int offset, int length);
        /// <summary>
        /// 从缓存中提取double二维数组结果，需要指定起始的字节索引，按照字节为单位，然后指定提取的 double 数组的行和列的长度，按照 double 为单位的个数。<br />
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="offset">索引位置</param>
        /// <param name="row">二维数组行</param>
        /// <param name="col">二维数组列</param>
        /// <returns>double二维数组对象</returns>
        double[,] ToDouble(byte[] buffer, int offset, int row, int col);

        /// <summary>
        /// 从缓存中提取short结果，需要指定起始的字节索引，按照字节为单位，一个short占用两个字节<br />
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="offset">索引位置</param>
        /// <returns>short对象</returns>
        short ToInt16(byte[] buffer, int offset);

        /// <summary>
        /// 从缓存中提取short数组结果，需要指定起始的字节索引，按照字节为单位，然后指定提取的 short 数组的长度，如果传入 10 ，则表示提取 10 个连续的 short 数据，该数据共占用 20 字节。<br />
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="offset">索引位置</param>
        /// <param name="length">读取的数组长度</param>
        /// <returns>short数组对象</returns>
        short[] ToInt16(byte[] buffer, int offset, int length);

        /// <summary>
        /// 从缓存中提取short二维数组结果，需要指定起始的字节索引，按照字节为单位，然后指定提取的 short 数组的行和列的长度，按照 short 为单位的个数。<br />
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="offset">索引位置</param>
        /// <param name="row">二维数组行</param>
        /// <param name="col">二维数组列</param>
        /// <returns>二维short数组</returns>
        short[,] ToInt16(byte[] buffer, int offset, int row, int col);

        /// <summary>
        /// 从缓存中提取int结果，需要指定起始的字节索引，按照字节为单位，一个int占用四个字节<br />
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="offset">索引位置</param>
        /// <returns>int对象</returns>
        int ToInt32(byte[] buffer, int offset);

        /// <summary>
        /// 从缓存中提取int数组结果，需要指定起始的字节索引，按照字节为单位，然后指定提取的 int 数组的长度，如果传入 10 ，则表示提取 10 个连续的 int 数据，该数据共占用 40 字节。<br />
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="offset">索引位置</param>
        /// <param name="length">读取的数组长度</param>
        /// <returns>int数组对象</returns>
        int[] ToInt32(byte[] buffer, int offset, int length);

        /// <summary>
        /// 从缓存中提取int二维数组结果，需要指定起始的字节索引，按照字节为单位，然后指定提取的 int 数组的行和列的长度，按照 int 为单位的个数。<br />
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="offset">索引位置</param>
        /// <param name="row">二维数组行</param>
        /// <param name="col">二维数组列</param>
        /// <returns>二维int数组</returns>
        int[,] ToInt32(byte[] buffer, int offset, int row, int col);

        /// <summary>
        /// 从缓存中提取long结果，需要指定起始的字节索引，按照字节为单位，一个long占用八个字节<br />
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="offset">索引位置</param>
        /// <returns>long对象</returns>
        long ToInt64(byte[] buffer, int offset);

        /// <summary>
        /// 从缓存中提取long数组结果，需要指定起始的字节索引，按照字节为单位，然后指定提取的 long 数组的长度，如果传入 10 ，则表示提取 10 个连续的 long 数据，该数据共占用 80 字节。<br />
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="offset">索引位置</param>
        /// <param name="length">读取的数组长度</param>
        /// <returns>long数组对象</returns>
        long[] ToInt64(byte[] buffer, int offset, int length);

        /// <summary>
        /// 从缓存中提取long二维数组结果，需要指定起始的字节索引，按照字节为单位，然后指定提取的 long 数组的行和列的长度，按照 long 为单位的个数。<br />
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="offset">索引位置</param>
        /// <param name="row">二维数组行</param>
        /// <param name="col">二维数组列</param>
        /// <returns>long二维数组对象</returns>
        long[,] ToInt64(byte[] buffer, int offset, int row, int col);

        /// <summary>
        /// 从缓存中提取float结果，需要指定起始的字节索引，按照字节为单位，一个float占用四个字节<b />
        /// </summary>
        /// <param name="buffer">缓存对象</param>
        /// <param name="offset">索引位置</param>
        /// <returns>float对象</returns>
        float ToSingle(byte[] buffer, int offset);

        /// <summary>
        /// 从缓存中提取float数组结果，需要指定起始的字节索引，按照字节为单位，然后指定提取的 float 数组的长度，如果传入 10 ，则表示提取 10 个连续的 float 数据，该数据共占用 40 字节。<br />
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="offset">索引位置</param>
        /// <param name="length">读取的数组长度</param>
        /// <returns>float数组</returns>
        float[] ToSingle(byte[] buffer, int offset, int length);

        /// <summary>
        /// 从缓存中提取float二维数组结果，需要指定起始的字节索引，按照字节为单位，然后指定提取的 float 数组的行和列的长度，按照 float 为单位的个数。<br />
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="offset">索引位置</param>
        /// <param name="row">二维数组行</param>
        /// <param name="col">二维数组列</param>
        /// <returns>float二维数组对象</returns>
        float[,] ToSingle(byte[] buffer, int offset, int row, int col);

        /// <summary>
        /// 从缓存中提取ushort结果，需要指定起始的字节索引，按照字节为单位，一个ushort占用两个字节<br />
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="offset">索引位置</param>
        /// <returns>ushort对象</returns>
        ushort ToUInt16(byte[] buffer, int offset);

        /// <summary>
        /// 从缓存中提取ushort数组结果，需要指定起始的字节索引，按照字节为单位，然后指定提取的 ushort 数组的长度，如果传入 10 ，则表示提取 10 个连续的 ushort 数据，该数据共占用 20 字节。<br />
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="offset">索引位置</param>
        /// <param name="length">读取的数组长度</param>
        /// <returns>ushort数组对象</returns>
        ushort[] ToUInt16(byte[] buffer, int offset, int length);

        /// <summary>
        /// 从缓存中提取ushort二维数组结果，需要指定起始的字节索引，按照字节为单位，然后指定提取的 ushort 数组的行和列的长度，按照 ushort 为单位的个数。<br />
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="offset">索引位置</param>
        /// <param name="row">二维数组行</param>
        /// <param name="col">二维数组列</param>
        /// <returns>二维ushort数组</returns>
        ushort[,] ToUInt16(byte[] buffer, int offset, int row, int col);

        /// <summary>
        /// 从缓存中提取uint结果，需要指定起始的字节索引，按照字节为单位，一个uint占用四个字节<br />
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="offset">索引位置</param>
        /// <returns>uint对象</returns>
        uint ToUInt32(byte[] buffer, int offset);

        /// <summary>
        /// 从缓存中提取uint数组结果，需要指定起始的字节索引，按照字节为单位，然后指定提取的 uint 数组的长度，如果传入 10 ，则表示提取 10 个连续的 uint 数据，该数据共占用 40 字节。<br />
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="offset">索引位置</param>
        /// <param name="length">读取的数组长度</param>
        /// <returns>uint数组对象</returns>
        uint[] ToUInt32(byte[] buffer, int offset, int length);

        /// <summary>
        /// 从缓存中提取uint二维数组结果，需要指定起始的字节索引，按照字节为单位，然后指定提取的 uint 数组的行和列的长度，按照 uint 为单位的个数。<br />
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="offset">索引位置</param>
        /// <param name="row">二维数组行</param>
        /// <param name="col">二维数组列</param>
        /// <returns>uint二维数组对象</returns>
        uint[,] ToUInt32(byte[] buffer, int offset, int row, int col);

        /// <summary>
        /// 从缓存中提取ulong结果，需要指定起始的字节索引，按照字节为单位，一个ulong占用八个字节<b />
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="offset">索引位置</param>
        /// <returns>ulong对象</returns>
        ulong ToUInt64(byte[] buffer, int offset);

        /// <summary>
        /// 从缓存中提取ulong数组结果，需要指定起始的字节索引，按照字节为单位，然后指定提取的 ulong 数组的长度，如果传入 10 ，则表示提取 10 个连续的 ulong 数据，该数据共占用 80 字节。<br />
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="offset">索引位置</param>
        /// <param name="length">读取的数组长度</param>
        /// <returns>ulong数组对象</returns>
        ulong[] ToUInt64(byte[] buffer, int offset, int length);

        /// <summary>
        /// 从缓存中提取ulong二维数组结果，需要指定起始的字节索引，按照字节为单位，然后指定提取的 ulong 数组的行和列的长度，按照 ulong 为单位的个数。<br />
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <param name="offset">索引位置</param>
        /// <param name="row">二维数组行</param>
        /// <param name="col">二维数组列</param>
        /// <returns>ulong二维数组对象</returns>
        ulong[,] ToUInt64(byte[] buffer, int offset, int row, int col);





        /// <summary>
        /// 从缓存中提取string结果，使用指定的编码将全部的缓存转为字符串<br />
        /// </summary>
        /// <param name="buffer">缓存对象</param>
        /// <param name="encoding">字符串的编码</param>
        /// <returns>string对象</returns>
        string ToString(byte[] buffer, Encoding encoding);

        /// <summary>
        /// 从缓存中的部分字节数组转化为string结果，使用指定的编码，指定起始的字节索引，字节长度信息。<br />
        /// </summary>
        /// <param name="buffer">缓存对象</param>
        /// <param name="offset">索引位置</param>
        /// <param name="length">byte数组长度</param>
        /// <param name="encoding">字符串的编码</param>
        /// <returns>string对象</returns>
        string ToString(byte[] buffer, int offset, int length, Encoding encoding);
        /// <summary>
        /// 从缓存中提取bcdstring结果，使用指定的编码将全部的缓存转为字符串<br />
        /// </summary>
        /// <param name="buffer">缓存对象</param>
        /// <param name="encoding">字符串的编码</param>
        /// <returns>string对象</returns>
        string ToBcdString(byte[] buffer, BCDFormat bCDFormat);

        /// <summary>
        /// 从缓存中的部分字节数组转化为bcdstring结果，使用指定的编码，指定起始的字节索引，字节长度信息。<br />
        /// </summary>
        /// <param name="buffer">缓存对象</param>
        /// <param name="offset">索引位置</param>
        /// <param name="length">byte数组长度</param>
        /// <param name="encoding">字符串的编码</param>
        /// <returns>string对象</returns>
        string ToBcdString(byte[] buffer, int offset, int length, BCDFormat bCDFormat);
        public byte[] GetBytes(string value, BCDFormat bCDFormat);
        public byte[] GetBytes(string value, int length, BCDFormat bCDFormat);


        #endregion
    }
}