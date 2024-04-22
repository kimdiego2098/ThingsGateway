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




using System.Text;

namespace ThingsGateway.Foundation;

[PluginOption(Singleton = true)]
public class DtuPlugin : PluginBase, ITcpReceivingPlugin
{
    private IDtu DtuService;

    public DtuPlugin(IDtu dtuService)
    {
        this.DtuService = dtuService;
    }

    public async Task OnTcpReceiving(ITcpClientBase client, ByteBlockEventArgs e)
    {
        if (client is ISocketClient socket)
        {
            var bytes = e.ByteBlock.ToArray();
            if (!socket.Id.StartsWith("ID="))
            {
                var id = $"ID={Encoding.UTF8.GetString(bytes)}";
                client.Logger.Info(DefaultResource.Localizer["DtuConnected", id]);
                socket.ResetId(id);
                e.Handled = true;
            }
            if (DtuService.HeartbeatHexString == bytes.ToHexString())
            {
                //回应心跳包
                socket.DefaultSend(bytes);
                e.Handled = true;
                if (socket.Logger.LogLevel <= LogLevel.Trace)
                    socket.Logger?.Trace($"{socket.ToString()}- Send:{bytes.ToHexString(' ')}");
            }
        }
        await e.InvokeNext();//如果本插件无法处理当前数据，请将数据转至下一个插件。
    }
}