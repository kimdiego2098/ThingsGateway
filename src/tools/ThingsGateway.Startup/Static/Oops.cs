﻿//------------------------------------------------------------------------------
//  此代码版权声明为全文件覆盖，如有原作者特别声明，会在下方手动补充
//  此代码版权（除特别声明外的代码）归作者本人Diego所有
//  源代码使用协议遵循本仓库的开源协议及附加协议
//  Gitee源代码仓库：https://gitee.com/diego2098/ThingsGateway
//  Github源代码仓库：https://github.com/kimdiego2098/ThingsGateway
//  使用文档：https://kimdiego2098.github.io/
//  QQ群：605534569
//------------------------------------------------------------------------------

using NewLife.Extension;

namespace ThingsGateway;

/// <summary>
/// 抛异常静态类
/// </summary>
public class Oops
{
    /// <summary>
    /// 抛出业务异常信息
    /// </summary>
    /// <param name="errorMessage">异常消息</param>
    /// <returns>异常实例</returns>
    public static UserFriendlyException Bah(string errorMessage)
    {
        if (errorMessage.IsNullOrWhiteSpace())
        {
            return new UserFriendlyException("unkown error");
        }
        var friendlyException = new UserFriendlyException(errorMessage);
        return friendlyException;
    }
}
