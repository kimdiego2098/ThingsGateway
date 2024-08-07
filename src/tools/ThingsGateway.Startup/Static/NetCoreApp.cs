﻿//------------------------------------------------------------------------------
//  此代码版权声明为全文件覆盖，如有原作者特别声明，会在下方手动补充
//  此代码版权（除特别声明外的代码）归作者本人Diego所有
//  源代码使用协议遵循本仓库的开源协议及附加协议
//  Gitee源代码仓库：https://gitee.com/diego2098/ThingsGateway
//  Github源代码仓库：https://github.com/kimdiego2098/ThingsGateway
//  使用文档：https://kimdiego2098.github.io/
//  QQ群：605534569
//------------------------------------------------------------------------------

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Localization;

using System.Diagnostics;
using System.Reflection;

using ThingsGateway.Core;

namespace ThingsGateway;

/// <summary>
/// NetCoreApp静态类
/// </summary>
public class NetCoreApp
{
    /// <summary>
    /// 直接引用程序集
    /// </summary>
    public static readonly IEnumerable<Assembly> Assemblies;

    /// <summary>
    /// 直接引用程序集中的类型
    /// </summary>
    public static readonly IEnumerable<Type> EffectiveTypes;

    /// <summary>
    /// 直接引用程序集中的Route Razor类，不支持单文件
    /// </summary>
    public static readonly IEnumerable<Assembly> RazorAssemblies;

    private static IStringLocalizerFactory? stringLocalizerFactory;

    static NetCoreApp()
    {
        Assemblies = GetAssemblies().ToList();
        EffectiveTypes = Assemblies!.SelectMany(a =>
        a.GetTypes());
        RazorAssemblies = EffectiveTypes.Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass
    && u.IsDefined(typeof(Microsoft.AspNetCore.Components.RouteAttribute), true)).Select(a => a.Assembly).Distinct().ToList();
    }

    /// <summary>
    /// 当前缓存服务
    /// </summary>
    public static ICacheService CacheService { get; set; }

    /// <summary>
    /// 系统配置
    /// </summary>
    public static IConfiguration? Configuration { get; set; }

    /// <summary>
    /// 当前程序文件夹
    /// </summary>
    public static string? ContentRootPath { get; set; }

    /// <summary>
    /// 是否开发环境
    /// </summary>
    public static bool IsDevelopment { get; set; } = false;

    /// <summary>
    /// 系统根服务
    /// </summary>
    public static IServiceProvider? RootServices { get; set; }

    /// <summary>
    /// 本地化服务工厂
    /// </summary>
    public static IStringLocalizerFactory? StringLocalizerFactory

    {
        get
        {
            if ((stringLocalizerFactory == null))
            {
                stringLocalizerFactory = RootServices?.GetRequiredService<IStringLocalizerFactory>();
            }
            return stringLocalizerFactory;
        }
    }


    /// <summary>
    /// 系统 wwwroot 文件夹路径
    /// </summary>
    public static string? WebRootPath { get; set; }

    /// <summary>
    /// 根据类型创建本地化服务
    /// </summary>
    /// <param name="resourceSource"></param>
    /// <returns></returns>
    public static IStringLocalizer? CreateLocalizerByType(Type resourceSource)
    {
        return resourceSource.Assembly.IsDynamic ? null : StringLocalizerFactory?.Create(resourceSource);
    }

    /// <summary>
    /// 获取当前线程 Id
    /// </summary>
    /// <returns></returns>
    public static int GetThreadId()
    {
        return Environment.CurrentManagedThreadId;
    }

    /// <summary>
    /// 获取应用有效程序集
    /// </summary>
    /// <returns>IEnumerable</returns>
    private static IEnumerable<Assembly> GetAssemblies()
    {
        // 需排除的程序集后缀
        var excludeAssemblyNames = new string[] {
            };

        IEnumerable<Assembly> scanAssemblies;

        // 获取入口程序集
        var entryAssembly = Assembly.GetEntryAssembly();

        // 非独立发布/非单文件发布
        if (!string.IsNullOrWhiteSpace(entryAssembly?.Location))
        {
            var dependencyContext = DependencyContext.Default;

            // 读取项目程序集
            scanAssemblies = dependencyContext.RuntimeLibraries
               .Where(u =>
                      (u.Type == "project" && !excludeAssemblyNames.Any(j => u.Name.EndsWith(j))) ||
                      (u.Type == "package" && (u.Name.StartsWith(nameof(ThingsGateway)))))
               .Select(u => Reflect.GetAssembly(u.Name));

            return scanAssemblies;
        }
        else
        {
            if (entryAssembly != null)
            {

                IEnumerable<Assembly> fixedSingleFileAssemblies = new[] { entryAssembly };

                // 扫描实现 ISingleFilePublish 接口的类型
                var singleFilePublishType = entryAssembly.GetTypes()
                                                    .FirstOrDefault(u => u.IsClass && !u.IsInterface && !u.IsAbstract && typeof(ISingleFilePublish).IsAssignableFrom(u));
                if (singleFilePublishType != null)
                {
                    var singleFilePublish = Activator.CreateInstance(singleFilePublishType) as ISingleFilePublish;

                    // 加载用户自定义配置单文件所需程序集
                    var nativeAssemblies = singleFilePublish.IncludeAssemblies();
                    var loadAssemblies = singleFilePublish.IncludeAssemblyNames()
                                                    .Select(u => Reflect.GetAssembly(u));

                    fixedSingleFileAssemblies = fixedSingleFileAssemblies.Concat(nativeAssemblies)
                                                                .Concat(loadAssemblies);
                }

                // 通过 AppDomain.CurrentDomain 扫描，默认为延迟加载，正常只能扫描到入口程序集（启动层）
                scanAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                                        .Where(ass =>
                                                // 排除 System，Microsoft，netstandard 开头的程序集
                                                !ass.FullName.StartsWith(nameof(System))
                                                && !ass.FullName.StartsWith(nameof(Microsoft))
                                                && !ass.FullName.StartsWith("netstandard"))
                                        .Concat(fixedSingleFileAssemblies)
                                        .Distinct();
                return scanAssemblies;
            }
            else
            {
                //maui

                scanAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                        .Where(ass =>
                                // 排除 System，Microsoft，netstandard 开头的程序集
                                !ass.FullName.StartsWith(nameof(System))
                                && !ass.FullName.StartsWith(nameof(Microsoft))
                                && !ass.FullName.StartsWith("netstandard"));
                // 扫描实现 ISingleFilePublish 接口的类型
                IEnumerable<Assembly> fixedSingleFileAssemblies = new List<Assembly>();
                var singleFilePublishType = scanAssemblies.SelectMany(a=>a.GetTypes())
                                                    .FirstOrDefault(u => u.IsClass && !u.IsInterface && !u.IsAbstract && typeof(ISingleFilePublish).IsAssignableFrom(u));
                if (singleFilePublishType != null)
                {
                    var singleFilePublish = Activator.CreateInstance(singleFilePublishType) as ISingleFilePublish;

                    // 加载用户自定义配置单文件所需程序集
                    var nativeAssemblies = singleFilePublish.IncludeAssemblies();
                    var loadAssemblies = singleFilePublish.IncludeAssemblyNames()
                                                    .Select(u => Reflect.GetAssembly(u));

                    fixedSingleFileAssemblies = fixedSingleFileAssemblies.Concat(nativeAssemblies)
                                                                .Concat(loadAssemblies);
                }

                // 通过 AppDomain.CurrentDomain 扫描，默认为延迟加载，正常只能扫描到入口程序集（启动层）

                scanAssemblies= scanAssemblies.Concat(fixedSingleFileAssemblies)
                                        .Distinct().ToList();
                return scanAssemblies;

            }
        }
    }
}
