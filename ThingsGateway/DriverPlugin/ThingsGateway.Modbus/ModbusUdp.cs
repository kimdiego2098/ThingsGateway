using Microsoft.Extensions.Logging;

using System.Net;

using ThingsGateway.Application.Core;
using ThingsGateway.Foundation;
using ThingsGateway.Foundation.Sockets;

namespace ThingsGateway.Modbus
{
    public class ModbusUdp : DriverBase
    {

        private ThingsGateway.Foundation.Adapter.Modbus.ModbusUdp? _plc;

        public ModbusUdp(ILogger logger) : base(logger)
        {
        }

        public override IThingsGatewayBitConverter ThingsGatewayBitConverter { get => _plc?.ThingsGatewayBitConverter; }

        [DeviceProperty("连接超时时间", "")] public ushort ConnectTimeOut { get; set; } = 3000;
        [DeviceProperty("默认解析顺序", "")] public DataFormat DataFormat { get; set; }
        [DeviceProperty("IP", "")] public string IP { get; set; } = "127.0.0.1";
        [DeviceProperty("最大打包长度", "")] public ushort MaxPack { get; set; } = 100;
        [DeviceProperty("端口", "")] public int Port { get; set; } = 502;
        [DeviceProperty("默认站号", "")] public byte Station { get; set; } = 1;
        [DeviceProperty("读写超时时间", "")] public ushort TimeOut { get; set; } = 3000;

        public override void AfterStop()
        {
        }

        public override void BeforStart()
        {
        }
        public override void Dispose()
        {

        }
        public override void Init(Device device, object client = null)
        {
            base.Init(device, client);
            if (client == null)
            {
                config.SetRemoteIPHost(new IPHost(IPAddress.Parse(IP), Port))
                    .SetBufferLength(1024);
                client = config.BuildWithUdpSession<UdpSession>();
            }
            //载入配置
            _plc = new((UdpSession)client);
            _plc.DataFormat = DataFormat;
            _plc.ConnectTimeOut = ConnectTimeOut;
            _plc.Station = Station;
            _plc.TimeOut = TimeOut;
        }

        public override bool IsSupportAddressRequest()
        {
            return true;
        }

        public override OperResult<List<DeviceVariableSourceRead>> LoadSourceRead(List<DeviceVariable> deviceVariables)
        {
            return deviceVariables.LoadSourceRead(_logger, ThingsGatewayBitConverter, MaxPack);
        }
        protected override async Task<OperResult<byte[]>> ReadAsync(string address, ushort length)
        {
            return await _plc.ReadAsync(address, length);
        }


        public override async Task<OperResult> WriteValueByNameAsync(DeviceVariable deviceVariable, string value)
        {
            return await _plc.WriteAsync(deviceVariable.DataType, deviceVariable.VariableAddress, value);
        }



    }
}
