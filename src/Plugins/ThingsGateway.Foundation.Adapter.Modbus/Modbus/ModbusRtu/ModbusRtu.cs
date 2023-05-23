﻿#region copyright
//------------------------------------------------------------------------------
//  此代码版权声明为全文件覆盖，如有原作者特别声明，会在下方手动补充
//  此代码版权（除特别声明外的代码）归作者本人Diego所有
//  源代码使用协议遵循本仓库的开源协议及附加协议
//  Gitee源代码仓库：https://gitee.com/diego2098/ThingsGateway
//  Github源代码仓库：https://github.com/kimdiego2098/ThingsGateway
//  使用文档：https://diego2098.gitee.io/thingsgateway/
//  QQ群：605534569
//------------------------------------------------------------------------------
#endregion

using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

using ThingsGateway.Foundation.Serial;

using TouchSocket.Resources;

namespace ThingsGateway.Foundation.Adapter.Modbus
{
    public class ModbusRtu : ReadWriteDevicesSerialClientBase
    {
        public ModbusRtuDataHandleAdapter DataHandleAdapter = new();

        public ModbusRtu(SerialClient serialClient) : base(serialClient)
        {
            ThingsGatewayBitConverter = new ThingsGatewayBitConverter(EndianType.Big);
            RegisterByteLength = 2;
            waitingClient = SerialClient.GetTGWaitingClient(new());
        }
        private IWaitingClient<SerialClient> waitingClient;
        [Description("Crc校验")]
        public bool Crc16CheckEnable { get; set; }
        [Description("帧前时间")]
        public int FrameTime { get; set; }
        [Description("组包缓存时间")]
        public double CacheTimeout { get; set; } = 1;
        [Description("站号")]
        public byte Station { get; set; } = 1;
        public override async Task<OperResult<byte[]>> ReadAsync(string address, int length, CancellationToken token = default)
        {
            try
            {
                await OpenAsync();
                var commandResult = ModbusHelper.GetReadModbusCommand(address, length, Station);
                if (commandResult.IsSuccess)
                {
                    ResponsedData result = await SendThenReturnAsync(commandResult, token);
                    if (result.RequestInfo is MessageBase collectMessage)
                    {
                        return collectMessage;
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

        private async Task<ResponsedData> SendThenReturnAsync(OperResult<byte[]> commandResult, CancellationToken token)
        {
            try
            {
                var item = commandResult.Content;
                await SerialClient.EasyLock.LockAsync();
                await Task.Delay(FrameTime, token);

                var result = await waitingClient.SendThenResponseAsync(item, TimeOut, token);
                return result;

            }
            finally
            {
                SerialClient.EasyLock.UnLock();
            }
        }

        public override void SetDataAdapter()
        {
            DataHandleAdapter = new();
            DataHandleAdapter.Crc16CheckEnable = Crc16CheckEnable;
            DataHandleAdapter.CacheTimeout = TimeSpan.FromSeconds(CacheTimeout);
            SerialClient.SetDataHandlingAdapter(DataHandleAdapter);
        }
        public override async Task<OperResult> WriteAsync(string address, byte[] value, CancellationToken token = default)
        {
            try
            {
                await OpenAsync();
                var commandResult = ModbusHelper.GetWriteModbusCommand(address, value, Station);
                if (commandResult.IsSuccess)
                {
                    ResponsedData result = await SendThenReturnAsync(commandResult, token);
                    if (result.RequestInfo is MessageBase collectMessage)
                    {
                        return collectMessage;
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
            return new OperResult<byte[]>(TouchSocketStatus.UnknownError.GetDescription());
        }

        public override async Task<OperResult> WriteAsync(string address, bool[] value, CancellationToken token = default)
        {
            try
            {
                await OpenAsync();
                var commandResult = ModbusHelper.GetWriteBoolModbusCommand(address, value, Station);
                if (commandResult.IsSuccess)
                {
                    ResponsedData result = await SendThenReturnAsync(commandResult, token);
                    if (result.RequestInfo is MessageBase collectMessage)
                    {
                        return collectMessage;
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
            return new OperResult<byte[]>(TouchSocketStatus.UnknownError.GetDescription());
        }
    }
}
