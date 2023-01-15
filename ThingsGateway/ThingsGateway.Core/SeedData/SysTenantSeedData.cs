namespace ThingsGateway.Core;

/// <summary>
/// 系统租户表种子数据
/// </summary>
public class SysTenantSeedData : ISqlSugarEntitySeedData<SysTenant>
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    [IgnoreUpdate]
    public IEnumerable<SysTenant> HasData()
    {
        return new[]
        {
            new SysTenant{ Id=123456780000000, OrgId=252885263003720, UserId=252885263003721, Host=@"https://gitee.com/diego2098/ThingsGateway", TenantType=TenantTypeEnum.Id, DbType=SqlSugar.DbType.Sqlite, Connection="DataSource=./Default.db", ConfigId=SqlSugarConst.ConfigId, Remark="系统默认", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
        };
    }
}