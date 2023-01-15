namespace ThingsGateway.Core;

/// <summary>
/// 自定义实体过滤器接口
/// </summary>
public interface IEntityFilter
{
    /// <summary>
    /// 实体过滤器
    /// </summary>
    /// <returns></returns>
    IEnumerable<TableFilterItem<object>> AddEntityFilter();
}