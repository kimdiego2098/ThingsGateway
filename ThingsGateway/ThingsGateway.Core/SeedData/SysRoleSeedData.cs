namespace ThingsGateway.Core;

/// <summary>
/// 系统角色表种子数据
/// </summary>
public class SysRoleSeedData : ISqlSugarEntitySeedData<SysRole>
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    [IgnoreUpdate]
    public IEnumerable<SysRole> HasData()
    {
        return new[]
        {
            new SysRole{ Id=252885263003721, Name="系统管理员", DataScope=DataScopeEnum.All, Code="sys_useradmin", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="系统管理员", TenantId=123456780000000 },
            new SysRole{ Id=252885263103721, Name="管理员", DataScope=DataScopeEnum.All, Code="sys_admin", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="系统管理员", TenantId=123456780000000 },
            new SysRole{ Id=252885263003722, Name="本部门及以下数据", DataScope=DataScopeEnum.DeptChild, Code="sys_deptChild", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="本部门及以下数据", TenantId=123456780000000 },
            new SysRole{ Id=252885263003723, Name="本部门数据", DataScope=DataScopeEnum.Dept, Code="sys_dept", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="本部门数据", TenantId=123456780000000 },
            new SysRole{ Id=252885263003724, Name="仅本人数据", DataScope=DataScopeEnum.Self, Code="sys_self", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="仅本人数据", TenantId=123456780000000 },
            new SysRole{ Id=252885263003725, Name="自定义数据", DataScope=DataScopeEnum.Define, Code="sys_define", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="自定义数据", TenantId=123456780000000 },
        };
    }
}