﻿//------------------------------------------------------------------------------
//  此代码版权声明为全文件覆盖，如有原作者特别声明，会在下方手动补充
//  此代码版权（除特别声明外的代码）归作者本人Diego所有
//  源代码使用协议遵循本仓库的开源协议及附加协议
//  Gitee源代码仓库：https://gitee.com/diego2098/ThingsGateway
//  Github源代码仓库：https://github.com/kimdiego2098/ThingsGateway
//  使用文档：https://diego2098.gitee.io/thingsgateway-docs/
//  QQ群：605534569
//------------------------------------------------------------------------------

using Furion.ConfigurableOptions;
using Furion.Logging.Extensions;

using Mapster;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System.Runtime.InteropServices;

using TouchSocket.Core;
using TouchSocket.Dmtp;
using TouchSocket.Dmtp.FileTransfer;
using TouchSocket.Dmtp.Rpc;
using TouchSocket.Rpc;
using TouchSocket.Sockets;

namespace ThingsGateway.Gateway.Application;

public class ManagementOptions : IConfigurableOptions
{
    public string RemoteUri { get; set; }
    public string ServerStandbyUri { get; set; }
    public int Port { get; set; }
    public string VerifyToken { get; set; }
    public int HeartbeatInterval { get; set; }
    public int MaxErrorCount { get; set; }
    public Redundancy Redundancy { get; set; }
}

public class Redundancy
{
    public bool Enable { get; set; }
    public bool IsPrimary { get; set; }

    public bool IsStartBusinessDevice { get; set; }
}

/// <summary>
/// TODO:网关管理服务
/// </summary>
public class ManagementWoker : BackgroundService
{
    protected IServiceScope _serviceScope;
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly ILogger _logger;

    /// <inheritdoc cref="ManagementWoker"/>
    public ManagementWoker(IServiceScopeFactory serviceScopeFactory, IHostApplicationLifetime appLifetime)
    {
        _serviceScope = serviceScopeFactory.CreateScope();
        _logger = _serviceScope.ServiceProvider.GetService<ILoggerFactory>().CreateLogger("网关管理服务");
        _appLifetime = appLifetime;
    }

    internal readonly EasyLock workerLock = new();

    #region worker服务

    private EasyLock _easyLock = new();

    internal volatile bool IsStart = false;

    /// <inheritdoc/>
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger?.LogInformation("网关管理服务启动");
        await _easyLock.WaitAsync();
        _appLifetime.ApplicationStarted.Register(() => { _easyLock.Release(); _easyLock = null; });
        await base.StartAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger?.LogInformation("网关管理服务停止");
        return base.StopAsync(cancellationToken);
    }

    internal ManagementOptions Options;
    internal GlobalData GlobalData;

    /// <summary>
    /// 启动锁
    /// </summary>
    internal EasyLock StartLock = new(true);

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _easyLock?.WaitAsync();
        GlobalData = _serviceScope.ServiceProvider.GetService<GlobalData>();
        Options = App.GetOptions<ManagementOptions>();
        if (Options.Redundancy.Enable)
        {
            var udpDmtp = GetUdpDmtp(Options);
            await udpDmtp.StartAsync();//启动

            if (Options.Redundancy.IsPrimary)
            {
                //初始化时，主站直接启动
                IsStart = true;
                StartLock.Release();
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    GatewayState? gatewayState = null;
                    await StartLock.WaitAsync();
                    var online = await udpDmtp.PingAsync(3000);
                    if (online)
                    {
                        var readErrorCount = 0;
                        while (readErrorCount < Options.MaxErrorCount)
                        {
                            try
                            {
                                gatewayState = await udpDmtp.GetDmtpRpcActor().InvokeTAsync<GatewayState>("GetGatewayStateAsync", InvokeOption.WaitInvoke, IsStart);
                                break;
                            }
                            catch
                            {
                                readErrorCount++;
                            }
                        }
                    }

                    if (gatewayState != null)
                    {
                        if (gatewayState.IsPrimary == Options.Redundancy.IsPrimary)
                        {
                            if (!IsStart)
                            {
                                _logger.LogInformation("主备站设置重复！");
                                IsStart = true;
                            }
                            await Task.Delay(1000);
                            continue;
                        }
                    }
                    if (gatewayState == null)
                    {
                        //无法获取状态，启动本机
                        if (!IsStart)
                        {
                            _logger.LogInformation("无法连接冗余站点，本机将切换到正常状态");
                            IsStart = true;
                        }
                    }
                    else if (gatewayState.IsPrimary)
                    {
                        //主站已经启动
                        if (gatewayState.IsStart)
                        {
                            if (IsStart)
                            {
                                _logger.LogInformation("主站已恢复，本机(从站)将切换到备用状态");
                                IsStart = false;
                            }
                        }
                        else
                        {
                            //等待主站切换到正常后，再停止从站
                        }
                    }
                    else
                    {
                        //从站已经启动
                        if (gatewayState.IsStart)
                        {
                            //等待从站切换到备用后，再启动主站
                        }
                        else
                        {
                            if (!IsStart)
                            {
                                _logger.LogInformation("本机(主站)将切换到正常状态");
                                IsStart = true;
                            }
                        }
                    }

                    //TODO:发布到从站数据
                    if (Options.Redundancy.IsPrimary)
                    {
                        try
                        {
                            if (online)
                                await udpDmtp.GetDmtpRpcActor().InvokeTAsync<GatewayState>("UpdateGatewayDataAsync", InvokeOption.WaitInvoke, GlobalData.CollectDevices);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "同步数据到从站时，发生错误");
                        }
                    }
                    await Task.Delay(1000, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                }
                catch (ObjectDisposedException)
                {
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "循环线程出错");
                }
                finally
                {
                    StartLock.Release();
                }
            }
        }
        else
        {
            //直接启动
            IsStart = true;
            //无冗余，直接启动采集服务
            _logger.LogInformation("不启用网关冗余站点");
            StartLock.Release();
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(60000, stoppingToken);
            }
            catch (TaskCanceledException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "循环线程出错");
            }
        }
    }

    #endregion worker服务

    #region

    private void LogOut(TouchSocket.Core.LogLevel logLevel, object source, string message, Exception exception)
    {
        _logger?.Log_Out(logLevel, source, message, exception);
    }

    private UdpDmtp GetUdpDmtp(ManagementOptions options)
    {
        var udpDmtp = new UdpDmtp();
        var config = new TouchSocketConfig()
               .SetRemoteIPHost(options.RemoteUri)
               .SetBindIPHost(options.Port)
               .SetDmtpOption(
            new DmtpOption() { VerifyToken = options.VerifyToken })
               .ConfigureContainer(a =>
               {
                   a.AddEasyLogger(LogOut);
                   a.AddRpcStore(store =>
                   {
                       store.RegisterServer<ReverseCallbackServer>();
                       //    store.Container.RegisterSingleton<IHostApplicationLifetime>(_appLifetime);
                       //    store.Container.RegisterSingleton(client);
                   });
               })
               .ConfigurePlugins(a =>
               {
                   a.UseDmtpFileTransfer();//必须添加文件传输插件
                                           //a.Add<FilePlugin>();
                   a.UseDmtpHeartbeat()//使用Dmtp心跳
                   .SetTick(TimeSpan.FromMilliseconds(options.HeartbeatInterval))
                   .SetMaxFailCount(options.MaxErrorCount);
                   a.UseDmtpRpc();
               });
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            config.UseUdpConnReset();
        }
        udpDmtp.Setup(config);
        return udpDmtp;
    }

    #endregion
}

internal class GatewayState
{
    /// <summary>
    /// 是否启动
    /// </summary>
    public bool IsStart { get; set; }

    /// <summary>
    /// 是否主站
    /// </summary>
    public bool IsPrimary { get; set; }
}