using System.Threading;

using ThingsGateway.Foundation.Core;
using ThingsGateway.Foundation.Extension;
using ThingsGateway.Foundation.Resources;
using ThingsGateway.Foundation.Sockets;

namespace ThingsGateway.Foundation.Adapter.Modbus
{
    public class ModbusTcp : ReadWriteDevicesTcpClientBase
    {
        public ModbusTcp(TcpClient tcpClient) : base(tcpClient)
        {
            ThingsGatewayBitConverter = new ThingsGatewayBitConverter(EndianType.Big);
        }
        public ModbusTcpDataHandleAdapter DataHandleAdapter = new();
        public override void SetDataAdapter()
        {
            if (DataHandleAdapter.Client != TcpClient)
                TcpClient.SetDataHandlingAdapter(DataHandleAdapter);
        }
        public bool IsCheckMessageId { get => DataHandleAdapter.IsCheckMessageId; set => DataHandleAdapter.IsCheckMessageId = value; }

        public byte Station { get; set; } = 1;

        public override OperResult<byte[]> Read(string address, ushort length)
        {
            try
            {
                Connect();
                var commandResult = ModbusHelper.GetReadModbusCommand(address, length, Station);
                if (commandResult.IsSuccess)
                {
                    var item = commandResult.Content;
                    var result = TcpClient.GetWaitingClient(new()).SendThenResponse(item, TimeOut, CancellationToken.None);
                    if (result.RequestInfo is CollectMessageBase collectMessage)
                    {
                        if (collectMessage.IsSuccess)
                            return OperResult.CreateSuccessResult(collectMessage.Content);
                        else
                            return OperResult.CreateFailedResult<byte[]>(collectMessage);
                    }
                }
                else
                {
                    return OperResult.CreateFailedResult<byte[]>(commandResult);
                }
            }
            catch (Exception ex)
            {
                return new OperResult<byte[]>(ex);
            }
            return new OperResult<byte[]>(TouchSocketStatus.UnknownError.GetDescription());

        }
        public override OperResult<bool[]> ReadBool(string address, ushort length)
        {
            try
            {
                Connect();
                var commandResult = ModbusHelper.GetReadModbusCommand(address, length, Station);
                if (commandResult.IsSuccess)
                {
                    var item = commandResult.Content;
                    var result = TcpClient.GetWaitingClient(new()).SendThenResponse(item, TimeOut, CancellationToken.None);
                    if (result.RequestInfo is CollectMessageBase collectMessage)
                    {
                        if (collectMessage.IsSuccess)
                            return OperResult.CreateSuccessResult(collectMessage.Content.ByteToBoolArray(length));
                        else
                            return OperResult.CreateFailedResult<bool[]>(collectMessage);

                    }
                }

                else
                {
                    return OperResult.CreateFailedResult<bool[]>(commandResult);
                }
            }
            catch (Exception ex)
            {
                return new OperResult<bool[]>(ex);
            }
            return new OperResult<bool[]>(TouchSocketStatus.UnknownError.GetDescription());

        }

        public override OperResult Write(string address, byte[] value)
        {
            try
            {
                Connect();
                var commandResult = ModbusHelper.GetWriteModbusCommand(address, value, Station);
                if (commandResult.IsSuccess)
                {
                    var result = TcpClient.GetWaitingClient(new()).SendThenResponse(commandResult.Content, TimeOut, CancellationToken.None);
                    return OperResult.CreateSuccessResult(result);
                }
                else
                {
                    return OperResult.CreateFailedResult<bool[]>(commandResult);
                }
            }
            catch (Exception ex)
            {
                return new OperResult<bool[]>(ex);
            }

        }

        public override OperResult Write(string address, bool[] value)
        {
            try
            {
                Connect();
                var commandResult = ModbusHelper.GetWriteBoolModbusCommand(address, value, Station);
                if (commandResult.IsSuccess)
                {
                    var result = TcpClient.GetWaitingClient(new()).SendThenResponse(commandResult.Content, TimeOut, CancellationToken.None);
                    return OperResult.CreateSuccessResult(result);
                }
                else
                {
                    return OperResult.CreateFailedResult<bool[]>(commandResult);
                }
            }
            catch (Exception ex)
            {
                return new OperResult<bool[]>(ex);
            }

        }

    }
}
