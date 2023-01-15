namespace ThingsGateway.Core;

/// <summary>
/// 系统用户表种子数据
/// </summary>
public class SysUserSeedData : ISqlSugarEntitySeedData<SysUser>
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    [IgnoreUpdate]
    public IEnumerable<SysUser> HasData()
    {
        return new[]
        {
            new SysUser{ Id=252885263000000, Account="superadmin", Password="e10adc3949ba59abbe56e057f20f883e", NickName="超级管理员", RealName="超级管理员", Phone="18020030720", Birthday=DateTime.Parse("1986-06-28"), Sex=GenderEnum.Male, AccountType=AccountTypeEnum.SuperAdmin, Remark="超级管理员", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), TenantId=123456780000000 },
            new SysUser{ Id=252885263003721, Account="admin", Password="e10adc3949ba59abbe56e057f20f883e", NickName="系统管理员", RealName="系统管理员", Phone="18020030720", Birthday=DateTime.Parse("1986-06-28"), Sex=GenderEnum.Male, AccountType=AccountTypeEnum.Admin, Remark="系统管理员", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrgId=252885263003720, PosId=252885263003721, TenantId=123456780000000 },
            //new SysUser{ Id=252885263003722, Account="user1", Password="e10adc3949ba59abbe56e057f20f883e", NickName="部门主管", RealName="部门主管", Phone="18020030720", Birthday=DateTime.Parse("1986-06-28"), Sex=GenderEnum.Female, AccountType=AccountTypeEnum.User, Remark="部门主管", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrgId=252885263003721, PosId=252885263003727, TenantId=123456780000000 },
            //new SysUser{ Id=252885263003723, Account="user2", Password="e10adc3949ba59abbe56e057f20f883e", NickName="部门职员", RealName="部门职员", Phone="18020030720", Birthday=DateTime.Parse("1986-06-28"), Sex=GenderEnum.Female, AccountType=AccountTypeEnum.User, Remark="部门职员", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrgId=252885263003722, PosId=252885263003729, TenantId=123456780000000 },
            //new SysUser{ Id=252885263003724, Account="user3", Password="e10adc3949ba59abbe56e057f20f883e", NickName="普通用户", RealName="普通用户", Phone="18020030720", Birthday=DateTime.Parse("1986-06-28"), Sex=GenderEnum.Female, AccountType=AccountTypeEnum.User, Remark="普通用户", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrgId=252885263003723, PosId=252885263003729, TenantId=123456780000000 },
            //new SysUser{ Id=252885263003725, Account="user4", Password="e10adc3949ba59abbe56e057f20f883e", NickName="其他", RealName="其他", Phone="18020030720", Birthday=DateTime.Parse("1986-06-28"), Sex=GenderEnum.Female, AccountType=AccountTypeEnum.None, Remark="普通用户", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrgId=252885263003724, PosId=252885263003729, TenantId=123456780000000 },
        };
    }
}