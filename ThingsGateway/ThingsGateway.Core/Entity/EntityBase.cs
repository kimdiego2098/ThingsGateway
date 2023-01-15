namespace ThingsGateway.Core;

/// <summary>
/// 框架实体基类Id
/// </summary>
public abstract class EntityBaseId
{
    /// <summary>
    /// 雪花Id
    /// </summary>
    [SugarColumn(ColumnDescription = "Id", IsPrimaryKey = true, IsIdentity = false)]
    public virtual long Id { get; set; }
}

/// <summary>
/// 框架实体基类
/// </summary>
public abstract class EntityBase : EntityBaseId
{
    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间", IsOnlyIgnoreUpdate = true)]
    public virtual DateTime? CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(ColumnDescription = "更新时间", IsOnlyIgnoreInsert = true)]
    public virtual DateTime? UpdateTime { get; set; }

    /// <summary>
    /// 创建者Id
    /// </summary>
    [SugarColumn(ColumnDescription = "创建者Id", IsOnlyIgnoreUpdate = true)]
    public virtual long? CreateUserId { get; set; }

    /// <summary>
    /// 修改者Id
    /// </summary>
    [SugarColumn(ColumnDescription = "修改者Id", IsOnlyIgnoreInsert = true)]
    public virtual long? UpdateUserId { get; set; }

    /// <summary>
    /// 软删除
    /// </summary>
    [SugarColumn(ColumnDescription = "软删除")]
    public virtual bool IsDelete { get; set; } = false;
}

/// <summary>
/// 业务数据实体基类(数据权限)
/// </summary>
public abstract class EntityBaseData : EntityBase
{
    /// <summary>
    /// 创建者部门Id
    /// </summary>
    [SugarColumn(ColumnDescription = "创建者部门Id")]
    public virtual long? CreateOrgId { get; set; }
}

/// <summary>
/// 租户基类实体
/// </summary>
public abstract class EntityTenant : EntityBase
{
    /// <summary>
    /// 租户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "租户Id")]
    public virtual long? TenantId { get; set; }
}

/// <summary>
/// 租户基类实体Id
/// </summary>
public abstract class EntityTenantId : EntityBaseId
{
    /// <summary>
    /// 租户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "租户Id")]
    public virtual long? TenantId { get; set; }
}