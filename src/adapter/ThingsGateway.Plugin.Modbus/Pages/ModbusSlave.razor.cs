﻿
//------------------------------------------------------------------------------
//  此代码版权声明为全文件覆盖，如有原作者特别声明，会在下方手动补充
//  此代码版权（除特别声明外的代码）归作者本人Diego所有
//  源代码使用协议遵循本仓库的开源协议及附加协议
//  Gitee源代码仓库：https://gitee.com/diego2098/ThingsGateway
//  Github源代码仓库：https://github.com/kimdiego2098/ThingsGateway
//  使用文档：https://diego2098.gitee.io/thingsgateway-docs/
//  QQ群：605534569
//------------------------------------------------------------------------------





using Microsoft.AspNetCore.Components;

using ThingsGateway.Foundation;

using TouchSocket.Core;

namespace ThingsGateway.Debug
{
    public partial class ModbusSlave : ComponentBase, IDisposable
    {
        private ThingsGateway.Foundation.Modbus.ModbusSlave _plc;

        private string LogPath;

        ~ModbusSlave()
        {
            this.SafeDispose();
        }

        private AdapterDebugComponent AdapterDebugComponent { get; set; }

        private ChannelData ChannelData { get; set; }

        public void Dispose()
        {
            ChannelData?.Channel?.SafeDispose();
            _plc?.SafeDispose();
            GC.SuppressFinalize(this);
        }

        private void OnChannelChanged(ChannelData channelData)
        {
            _plc = new ThingsGateway.Foundation.Modbus.ModbusSlave(channelData.Channel);
            LogPath = channelData.Id.GetDebugLogPath();
        }

        private async Task OnConnectClick()
        {
            if (ChannelData != null)
            {
                try
                {
                    await ChannelData.Channel.ConnectAsync(_plc.ConnectTimeout, default);
                }
                catch (Exception ex)
                {
                    ChannelData.Channel.Logger.Exception(ex);
                }
            }
        }

        private async Task OnEditClick(ChannelData channelData)
        {
            ChannelData = channelData;
            if (channelData != null)
            {
                _plc = new ThingsGateway.Foundation.Modbus.ModbusSlave(channelData.Channel);
                LogPath = channelData.Id.GetDebugLogPath();
            }
            else
            {
                _plc?.Dispose();
                _plc = null;
            }
            await InvokeAsync(StateHasChanged);
        }
    }
}