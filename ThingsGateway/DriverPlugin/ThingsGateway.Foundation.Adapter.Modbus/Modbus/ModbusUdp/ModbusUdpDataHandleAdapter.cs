using ThingsGateway.Foundation.Extension;

namespace ThingsGateway.Foundation.Adapter.Modbus
{
    public class ModbusUdpDataHandleAdapter : ReadWriteDevicesUdpDataHandleAdapter<ModbusTcpMessage>
    {
        private readonly EasyIncrementCount easyIncrementCount = new EasyIncrementCount(ushort.MaxValue);

        public EasyIncrementCount MessageId => easyIncrementCount;
        public bool IsCheckMessageId
        {
            get
            {
                return Request?.IsCheckMessageId ?? false;
            }
            set
            {
                Request.IsCheckMessageId = value;
            }
        }

        public override byte[] PackCommand(byte[] command)
        {
            return ModbusHelper.AddModbusTcpHead(command, (ushort)easyIncrementCount.GetCurrentValue());
        }

        protected override OperResult<byte[]> UnpackResponse(
                  byte[] send,
          byte[] response)
        {
            return ModbusHelper.GetModbusData(send.RemoveBegin(6), response.RemoveBegin(6));
        }

        protected override ModbusTcpMessage GetInstance()
        {
            return new ModbusTcpMessage();
        }


    }
}
