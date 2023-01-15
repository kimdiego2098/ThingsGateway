using OnceMi.AspNetCore.OSS;

namespace ThingsGateway.Core;

/// <summary>
/// 对象存储配置选项
/// </summary>
public sealed class OSSProviderOptions : OSSOptions, IConfigurableOptions
{
    /// <summary>
    /// 是否启用OSS存储
    /// </summary>
    public bool IsEnable { get; set; }

    /// <summary>
    /// 自定义桶名称 不能直接使用Provider来替代桶名称
    /// 例：阿里云 1.只能包括小写字母，数字，短横线（-）2.必须以小写字母或者数字开头 3.长度必须在3-63字节之间
    /// </summary>
    public string Bucket { get; set; }
}