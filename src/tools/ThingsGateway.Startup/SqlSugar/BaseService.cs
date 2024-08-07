﻿//------------------------------------------------------------------------------
//  此代码版权声明为全文件覆盖，如有原作者特别声明，会在下方手动补充
//  此代码版权（除特别声明外的代码）归作者本人Diego所有
//  源代码使用协议遵循本仓库的开源协议及附加协议
//  Gitee源代码仓库：https://gitee.com/diego2098/ThingsGateway
//  Github源代码仓库：https://github.com/kimdiego2098/ThingsGateway
//  使用文档：https://kimdiego2098.github.io/
//  QQ群：605534569
//------------------------------------------------------------------------------

using BootstrapBlazor.Components;

using Microsoft.Extensions.Localization;

using SqlSugar;

using ThingsGateway.Core;

namespace ThingsGateway;

public class BaseService<T> : IDataService<T>, IDisposable where T : class, new()
{
    public BaseService()
    {
        this.Localizer = NetCoreApp.CreateLocalizerByType(typeof(T))!;
    }

    public bool IsDisposed { get; private set; }
    protected IStringLocalizer Localizer { get; }

    public Task<bool> AddAsync(T model)
    {
        return SaveAsync(model, ItemChangedType.Add);
    }

    public Task<bool> DeleteAsync(IEnumerable<T> models)
    {
        if (models.FirstOrDefault() is IPrimaryIdEntity)
            return DeleteAsync(models.Select(a => ((IPrimaryIdEntity)a).Id));
        else
            return Task.FromResult(false);
    }

    public virtual async Task<bool> DeleteAsync(IEnumerable<long> models)
    {
        using var db = GetDB();
        return await db.Deleteable<T>().In(models.ToList()).ExecuteCommandHasChangeAsync();
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        IsDisposed = true;
        GC.SuppressFinalize(this);
    }

    public Task<QueryData<T>> QueryAsync(QueryPageOptions option)
    {
        return QueryAsync(option, null);
    }

    public async Task<QueryData<T>> QueryAsync(QueryPageOptions option, Func<ISugarQueryable<T>, ISugarQueryable<T>>? queryFunc = null)
    {
        var ret = new QueryData<T>()
        {
            IsSorted = option.SortOrder != SortOrder.Unset,
            IsFiltered = option.Filters.Any(),
            IsAdvanceSearch = option.AdvanceSearches.Any() || option.CustomerSearches.Any(),
            IsSearch = option.Searches.Any()
        };

        using var db = GetDB();
        var query = db.GetQuery<T>(option);
        if (queryFunc != null)
            query = queryFunc(query);
        if (option.IsPage)
        {
            RefAsync<int> totalCount = 0;

            var items = await query
                .ToPageListAsync(option.PageIndex, option.PageItems, totalCount);

            ret.TotalCount = totalCount;
            ret.Items = items;
        }
        else if (option.IsVirtualScroll)
        {
            RefAsync<int> totalCount = 0;

            var items = await query
                .ToPageListAsync(option.StartIndex, option.PageItems, totalCount);

            ret.TotalCount = totalCount;
            ret.Items = items;
        }
        else
        {
            var items = await query
                .ToListAsync();
            ret.TotalCount = items.Count;
            ret.Items = items;
        }
        return ret;
    }

    public virtual async Task<bool> SaveAsync(T model, ItemChangedType changedType)
    {
        using var db = GetDB();
        if (changedType == ItemChangedType.Add)
        {
            return (await db.Insertable(model).ExecuteCommandAsync()) > 0;
        }
        else
        {
            return (await db.Updateable(model).ExecuteCommandAsync()) > 0;
        }
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
    }

    protected SqlSugarClient GetDB()
    {
        return DbContext.Db.GetConnectionScopeWithAttr<T>().CopyNew();
    }
}
