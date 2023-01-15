namespace ThingsGateway.Core;

/// <summary>
/// 系统字典类型表种子数据
/// </summary>
public class SysDictTypeSeedData : ISqlSugarEntitySeedData<SysDictType>
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    [IgnoreUpdate]
    public IEnumerable<SysDictType> HasData()
    {
        return new[]
        {
            new SysDictType{ Id=269037954100001, Name="代码生成控件类型", Code="code_gen_effect_type", OrderNo=100, Remark="代码生成控件类型", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictType{ Id=269037954100002, Name="代码生成查询类型", Code="code_gen_query_type", OrderNo=100, Remark="代码生成查询类型", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictType{ Id=269037954100003, Name="代码生成.NET类型", Code="code_gen_net_type", OrderNo=100, Remark="代码生成.NET类型", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictType{ Id=269037954100004, Name="代码生成方式", Code="code_gen_create_type", OrderNo=100, Remark="代码生成方式", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictType{ Id=269037954100005, Name="代码生成基类", Code="code_gen_base_class", OrderNo=100, Remark="代码生成基类", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
        };
    }
}