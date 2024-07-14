﻿//------------------------------------------------------------------------------
//  此代码版权声明为全文件覆盖，如有原作者特别声明，会在下方手动补充
//  此代码版权（除特别声明外的代码）归作者本人Diego所有
//  源代码使用协议遵循本仓库的开源协议及附加协议
//  Gitee源代码仓库：https://gitee.com/diego2098/ThingsGateway
//  Github源代码仓库：https://github.com/kimdiego2098/ThingsGateway
//  使用文档：https://kimdiego2098.github.io/
//  QQ群：605534569
//------------------------------------------------------------------------------

using Newtonsoft.Json;

namespace ThingsGateway.Foundation.Json.Extension;

public static class JsonExtensions
{
    /// <summary>
    /// 默认Json规则
    /// </summary>
    public static JsonSerializerSettings Options = new JsonSerializerSettings
    {
        Formatting = Formatting.Indented,// 使用缩进格式化输出
        NullValueHandling = NullValueHandling.Ignore // 忽略空值属性
    };

    public static object FromJsonNetString(this string json, Type type, JsonSerializerSettings? jsonSerializerSettings = null)
    {
        return Newtonsoft.Json.JsonConvert.DeserializeObject(json, type, jsonSerializerSettings ?? Options);
    }

    public static T FromJsonNetString<T>(this string json, JsonSerializerSettings? jsonSerializerSettings = null)
    {
        return (T)FromJsonNetString(json, typeof(T), jsonSerializerSettings);
    }

    public static string ToJsonNetString(this object item, JsonSerializerSettings? jsonSerializerSettings = null)
    {
        return Newtonsoft.Json.JsonConvert.SerializeObject(item, jsonSerializerSettings ?? Options);
    }
}
