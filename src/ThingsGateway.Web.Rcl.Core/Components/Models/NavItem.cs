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

public class NavItem : IDefaultItem<NavItem>
{
    public NavItem()
    {
    }

    public NavItem(string title)
    {
        Title = title;
    }

    public List<NavItem> Children { get; set; }
    public bool Divider { get; set; }
    public string Group { get; set; }
    public string Heading { get; set; }
    public string Href { get; set; }
    public string Icon { get; set; }
    public string Segment => Group ?? Title;
    public string State { get; set; }
    public string SubTitle { get; set; }
    public string Target { get; set; }
    public string Title { get; set; }

    public StringNumber Value { get; set; }
}