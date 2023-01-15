namespace ThingsGateway.Foundation
{
    public interface IReadWriteDevice : IReadWrite
    {

        /// <summary>
        /// 超时时间
        /// </summary>
        ushort TimeOut { get; set; }

        /// <summary>
        /// 当前的数据变换
        /// </summary>
        IThingsGatewayBitConverter ThingsGatewayBitConverter { get; }
        DataFormat DataFormat { get; set; }

        /// <summary>
        /// 获取解析后的实际寄存器数量<br />
        /// </summary>
        /// <param name="address">读取的设备的地址信息</param>
        /// <param name="length">读取的数据长度信息</param>
        /// <param name="dataTypeLength">数据类型的字长度信息</param>
        ushort GetWordLength(string address, int length, int dataTypeLength);

        /// <summary>
        /// 一个字单位的数据表示的实际寄存器数量<br />
        /// </summary>
        ushort WordLength { get; set; }
    }
}