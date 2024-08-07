﻿
//------------------------------------------------------------------------------
//  此代码版权声明为全文件覆盖，如有原作者特别声明，会在下方手动补充
//  此代码版权（除特别声明外的代码）归作者本人Diego所有
//  源代码使用协议遵循本仓库的开源协议及附加协议
//  Gitee源代码仓库：https://gitee.com/diego2098/ThingsGateway
//  Github源代码仓库：https://github.com/kimdiego2098/ThingsGateway
//  使用文档：https://kimdiego2098.github.io/
//  QQ群：605534569
//------------------------------------------------------------------------------




#if NETFRAMEWORK || NETSTANDARD || NETCOREAPP3_1

namespace System.Diagnostics.CodeAnalysis;

/// <summary>执行方法后指定成员不为空（带条件）</summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
public sealed class MemberNotNullWhenAttribute : Attribute
{
    /// <summary>返回值</summary>
    public Boolean ReturnValue { get; }

    /// <summary>不为空的成员</summary>
    public String[] Members { get; }

    /// <summary>成员不为空</summary>
    /// <param name="returnValue"></param>
    /// <param name="member"></param>
    public MemberNotNullWhenAttribute(Boolean returnValue, String member)
    {
        ReturnValue = returnValue;
        Members = [member];
    }

    /// <summary>成员不为空</summary>
    /// <param name="returnValue"></param>
    /// <param name="members"></param>
    public MemberNotNullWhenAttribute(Boolean returnValue, params String[] members)
    {
        ReturnValue = returnValue;
        Members = members;
    }
}

#endif