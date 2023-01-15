namespace ThingsGateway.Core;

/// <summary>
/// 系统配置表种子数据
/// </summary>
public class SysConfigSeedData : ISqlSugarEntitySeedData<SysConfig>
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    [IgnoreUpdate]
    public IEnumerable<SysConfig> HasData()
    {
        return new[]
        {
            new SysConfig{ Id=252885263003800, Name="演示环境", Code="sys_demo", Value="False", SysFlag=YesNoEnum.Y, Remark="演示环境", OrderNo=1, GroupCode="Default", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=252885263003801, Name="默认密码", Code="sys_password", Value="123456", SysFlag=YesNoEnum.Y, Remark="默认密码", OrderNo=2, GroupCode="Default", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=252885263003811, Name="Mqtt用户名", Code="mqtt_username", Value="ThingsGateway", SysFlag=YesNoEnum.Y, Remark="Mqtt用户名", OrderNo=2, GroupCode="Default", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=252885263003812, Name="Mqtt密码", Code="mqtt_password", Value="ThingsGateway", SysFlag=YesNoEnum.Y, Remark="Mqtt密码", OrderNo=2, GroupCode="Default", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=252885263003802, Name="Token过期时间", Code="sys_token_expire", Value="10080", SysFlag=YesNoEnum.Y, Remark="Token过期时间", OrderNo=3, GroupCode="Default", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=252885263003803, Name="操作日志", Code="sys_oplog", Value="True", SysFlag=YesNoEnum.Y, Remark="开启操作日志", OrderNo=4, GroupCode="Default", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=252885263003804, Name="单点登录", Code="sys_single_login", Value="True", SysFlag=YesNoEnum.Y, Remark="开启单点登录", OrderNo=5, GroupCode="Default", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=252885263003805, Name="登录二次验证", Code="sys_second_ver", Value="False", SysFlag=YesNoEnum.Y, Remark="登录二次验证", OrderNo=6, GroupCode="Default", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=252885263003806, Name="开启图形验证码", Code="sys_captcha", Value="True", SysFlag=YesNoEnum.Y, Remark="开启图形验证码", OrderNo=7, GroupCode="Default", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=252885263003807, Name="开启水印", Code="sys_wartermark", Value="True", SysFlag=YesNoEnum.Y, Remark="开启水印", OrderNo=8, GroupCode="Default", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
         };
    }
}