namespace ThingsGateway.Core;

/// <summary>
/// 通用常量
/// </summary>
[Const("平台配置")]
public class CommonConst
{
    /// <summary>
    /// 演示环境开关
    /// </summary>
    public const string SysDemoEnv = "sys_demo";

    /// <summary>
    /// 默认密码
    /// </summary>
    public const string SysPassword = "sys_password";

    /// <summary>
    /// Mqtt密码
    /// </summary>
    public const string MqttPassword = "mqtt_password";
    /// <summary>
    /// Mqtt账号
    /// </summary>
    public const string MqttUserName = "mqtt_username";

    /// <summary>
    /// 登录二次验证
    /// </summary>
    public const string SysSecondVer = "sys_second_ver";
    /// <summary>
    /// 开启图形验证码
    /// </summary>
    public const string SysCaptcha = "sys_captcha";

    /// <summary>
    /// 开启水印
    /// </summary>
    public const string SysWartermark = "sys_wartermark";

    /// <summary>
    /// 开启操作日志
    /// </summary>
    public const string SysOpLog = "sys_oplog";

    /// <summary>
    /// Token过期时间
    /// </summary>
    public const string SysTokenExpire = "sys_token_expire";

    /// <summary>
    /// 单用户登录
    /// </summary>
    public const string SysSingleLogin = "sys_single_login";

    /// <summary>
    /// 系统管理员角色编码
    /// </summary>
    public const string SysAdminRole = "sys_admin";

    /// <summary>
    /// 开启全局脱敏处理（默认不开启）
    /// </summary>
    public static bool SysSensitiveDetection = false;
}