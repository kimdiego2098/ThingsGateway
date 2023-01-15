namespace ThingsGateway.Core;

/// <summary>
/// 分页泛型集合
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public class SqlSugarPagedList<TEntity>
    where TEntity : new()
{
    /// <summary>
    /// 页码
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// 页容量
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 总条数
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// 总页数
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// 当前页集合
    /// </summary>
    public IEnumerable<TEntity> Items { get; set; }

    /// <summary>
    /// 是否有上一页
    /// </summary>
    public bool HasPrevPage { get; set; }

    /// <summary>
    /// 是否有下一页
    /// </summary>
    public bool HasNextPage { get; set; }
}

/// <summary>
/// 分页拓展类
/// </summary>
public static class SqlSugarPagedExtensions
{
    /// <summary>
    /// 分页拓展
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public static SqlSugarPagedList<TEntity> ToPagedList<TEntity>(this ISugarQueryable<TEntity> entity, int pageIndex, int pageSize)
        where TEntity : new()
    {
        var total = 0;
        var items = entity.ToPageList(pageIndex, pageSize, ref total);
        var totalPages = (int)Math.Ceiling(total / (double)pageSize);
        return new SqlSugarPagedList<TEntity>
        {
            Page = pageIndex,
            PageSize = pageSize,
            Items = items,
            Total = total,
            TotalPages = totalPages,
            HasNextPage = pageIndex < totalPages,
            HasPrevPage = pageIndex - 1 > 0
        };
    }

    /// <summary>
    /// 分页拓展
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public static async Task<SqlSugarPagedList<TEntity>> ToPagedListAsync<TEntity>(this ISugarQueryable<TEntity> entity, int pageIndex, int pageSize)
        where TEntity : new()
    {
        RefAsync<int> total = 0;
        var items = await entity.ToPageListAsync(pageIndex, pageSize, total);
        var totalPages = (int)Math.Ceiling(total / (double)pageSize);
        return new SqlSugarPagedList<TEntity>
        {
            Page = pageIndex,
            PageSize = pageSize,
            Items = items,
            Total = total,
            TotalPages = totalPages,
            HasNextPage = pageIndex < totalPages,
            HasPrevPage = pageIndex - 1 > 0
        };
    }

    /// <summary>
    /// 分页拓展
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public static Task<SqlSugarPagedList<TEntity>> ToPagedListAsync<TEntity>(this IEnumerable<TEntity> entity, int pageIndex, int pageSize)
        where TEntity : new()
    {

        int _PageIndex = pageIndex == 0 ? 1 : pageIndex;
        int _PageSize = pageSize == 0 ? 20 : pageSize;
        var num = entity.Count();
        var pageConut = (double)num / _PageSize;
        int PageConut = (int)Math.Ceiling(pageConut);
        List<TEntity> list = new List<TEntity>();
        if (PageConut >= _PageIndex)
        {
            list = entity.Skip((_PageIndex - 1) * _PageSize).Take(_PageSize).ToList();
        }
        return Task.FromResult(new SqlSugarPagedList<TEntity>
        {
            Page = pageIndex,
            PageSize = pageSize,
            Items = list,
            Total = entity?.Count() ?? 0,
            TotalPages = PageConut,
            HasNextPage = pageIndex < PageConut,
            HasPrevPage = pageIndex - 1 > 0
        });
    }

}