﻿//------------------------------------------------------------------------------
//  此代码版权声明为全文件覆盖，如有原作者特别声明，会在下方手动补充
//  此代码版权（除特别声明外的代码）归作者本人Diego所有
//  源代码使用协议遵循本仓库的开源协议及附加协议
//  Gitee源代码仓库：https://gitee.com/diego2098/ThingsGateway
//  Github源代码仓库：https://github.com/kimdiego2098/ThingsGateway
//  使用文档：https://kimdiego2098.github.io/
//  QQ群：605534569
//------------------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using ThingsGateway.Core;

namespace ThingsGateway.Razor;

[AppStartup(1000000)]
public class Startup : AppStartup
{
    public void ConfigureApp(IServiceCollection services)
    {
        services.AddBootstrapBlazor(
            //option => option.JSModuleVersion = Random.Shared.Next(10000).ToString()
            );

        services.AddOptions();
        // 配置
        services.TryAddSingleton<IConfigureOptions<WebsiteOptions>, Microsoft.Extensions.DependencyInjection.ConfigureOptions<WebsiteOptions>>();

        services.ConfigureIconThemeOptions(options => options.ThemeKey = "fa");
        services.AddSingleton<ICacheService, MemoryCacheService>();
    }

    public void UseService(IServiceProvider serviceProvider)
    {
    }
}
