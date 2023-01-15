namespace ThingsGateway.Core;

/// <summary>
/// 系统角色菜单表种子数据
/// </summary>
public class SysRoleMenuSeedData : ISqlSugarEntitySeedData<SysRoleMenu>
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    [IgnoreUpdate]
    public IEnumerable<SysRoleMenu> HasData()
    {
        return new[]
        {
            // 数据面板【admin/252885263003721】
            new SysRoleMenu{ Id=252885263003000, RoleId=252885263003721, MenuId=252885263002100 },
            new SysRoleMenu{ Id=252885263003001, RoleId=252885263003721, MenuId=252885263002110 },
            new SysRoleMenu{ Id=252885263003002, RoleId=252885263003721, MenuId=252885263002111 },

            // 系统管理
            new SysRoleMenu{ Id=252885263003100, RoleId=252885263003721, MenuId=252885263055200 },
            // 账号管理
            new SysRoleMenu{ Id=252885263003101, RoleId=252885263003721, MenuId=252885263055210 },
            new SysRoleMenu{ Id=252885263003102, RoleId=252885263003721, MenuId=252885263055211 },
            new SysRoleMenu{ Id=252885263003103, RoleId=252885263003721, MenuId=252885263055212 },
            new SysRoleMenu{ Id=252885263003104, RoleId=252885263003721, MenuId=252885263055213 },
            new SysRoleMenu{ Id=252885263003105, RoleId=252885263003721, MenuId=252885263055214 },
            new SysRoleMenu{ Id=252885263003106, RoleId=252885263003721, MenuId=252885263055215 },
            new SysRoleMenu{ Id=252885263003107, RoleId=252885263003721, MenuId=252885263055216 },
            new SysRoleMenu{ Id=252885263003108, RoleId=252885263003721, MenuId=252885263055217 },
            new SysRoleMenu{ Id=252885263003109, RoleId=252885263003721, MenuId=252885263055218 },
            new SysRoleMenu{ Id=252885263003110, RoleId=252885263003721, MenuId=252885263055219 },
            new SysRoleMenu{ Id=252885263013111, RoleId=252885263003721, MenuId=252885263055220 },
            // 角色管理
            new SysRoleMenu{ Id=252885263003111, RoleId=252885263003721, MenuId=252885263055230 },
            new SysRoleMenu{ Id=252885263003112, RoleId=252885263003721, MenuId=252885263055231 },
            new SysRoleMenu{ Id=252885263003113, RoleId=252885263003721, MenuId=252885263055232 },
            new SysRoleMenu{ Id=252885263003114, RoleId=252885263003721, MenuId=252885263055233 },
            new SysRoleMenu{ Id=252885263003115, RoleId=252885263003721, MenuId=252885263055234 },
            new SysRoleMenu{ Id=252885263003116, RoleId=252885263003721, MenuId=252885263055235 },
            new SysRoleMenu{ Id=252885263003117, RoleId=252885263003721, MenuId=252885263055236 },
            new SysRoleMenu{ Id=252885263003118, RoleId=252885263003721, MenuId=252885263055237 },
            // 机构管理
            new SysRoleMenu{ Id=252885263003131, RoleId=252885263003721, MenuId=252885263055240 },
            new SysRoleMenu{ Id=252885263003132, RoleId=252885263003721, MenuId=252885263055241 },
            new SysRoleMenu{ Id=252885263003133, RoleId=252885263003721, MenuId=252885263055242 },
            new SysRoleMenu{ Id=252885263003134, RoleId=252885263003721, MenuId=252885263055243 },
            new SysRoleMenu{ Id=252885263003135, RoleId=252885263003721, MenuId=252885263055244 },
            // 职位管理
            new SysRoleMenu{ Id=252885263003141, RoleId=252885263003721, MenuId=252885263055250 },
            new SysRoleMenu{ Id=252885263003142, RoleId=252885263003721, MenuId=252885263055251 },
            new SysRoleMenu{ Id=252885263003143, RoleId=252885263003721, MenuId=252885263055252 },
            new SysRoleMenu{ Id=252885263003144, RoleId=252885263003721, MenuId=252885263055253 },
            new SysRoleMenu{ Id=252885263003145, RoleId=252885263003721, MenuId=252885263055254 },
            // 个人中心
            new SysRoleMenu{ Id=252885263003151, RoleId=252885263003721, MenuId=252885263055260 },
            new SysRoleMenu{ Id=252885263003152, RoleId=252885263003721, MenuId=252885263055261 },
            new SysRoleMenu{ Id=252885263003153, RoleId=252885263003721, MenuId=252885263055262 },
            new SysRoleMenu{ Id=252885263003154, RoleId=252885263003721, MenuId=252885263055263 },
            // 通知公告
            new SysRoleMenu{ Id=252885263003161, RoleId=252885263003721, MenuId=252885263055270 },
            new SysRoleMenu{ Id=252885263003162, RoleId=252885263003721, MenuId=252885263055271 },
            new SysRoleMenu{ Id=252885263003163, RoleId=252885263003721, MenuId=252885263055272 },
            new SysRoleMenu{ Id=252885263003164, RoleId=252885263003721, MenuId=252885263055273 },
            new SysRoleMenu{ Id=252885263003165, RoleId=252885263003721, MenuId=252885263055274 },
            new SysRoleMenu{ Id=252885263003166, RoleId=252885263003721, MenuId=252885263055275 },
            new SysRoleMenu{ Id=252885263003167, RoleId=252885263003721, MenuId=252885263055276 },
            // 三方账号
            new SysRoleMenu{ Id=252885263003171, RoleId=252885263003721, MenuId=252885263055280 },
            new SysRoleMenu{ Id=252885263003172, RoleId=252885263003721, MenuId=252885263055281 },
            new SysRoleMenu{ Id=252885263003173, RoleId=252885263003721, MenuId=252885263055282 },
            new SysRoleMenu{ Id=252885263003174, RoleId=252885263003721, MenuId=252885263055283 },

            //// 平台管理
            //new SysRoleMenu{ Id=252885263003200, RoleId=252885263003721, MenuId=252885263055300 },
            // 任务调度
            new SysRoleMenu{ Id=252885263003221, RoleId=252885263003721, MenuId=252885263055350 },
            new SysRoleMenu{ Id=252885263003222, RoleId=252885263003721, MenuId=252885263055351 },
            new SysRoleMenu{ Id=252885263003223, RoleId=252885263003721, MenuId=252885263055352 },
            new SysRoleMenu{ Id=252885263003224, RoleId=252885263003721, MenuId=252885263055353 },
            new SysRoleMenu{ Id=252885263003225, RoleId=252885263003721, MenuId=252885263055354 },
            // 系统监控
            new SysRoleMenu{ Id=252885263003231, RoleId=252885263003721, MenuId=252885263055360 },
            // 缓存管理
            new SysRoleMenu{ Id=252885263003241, RoleId=252885263003721, MenuId=252885263055370 },
            new SysRoleMenu{ Id=252885263003242, RoleId=252885263003721, MenuId=252885263055371 },
            new SysRoleMenu{ Id=252885263003243, RoleId=252885263003721, MenuId=252885263055372 },
            // 行政区域
            new SysRoleMenu{ Id=252885263003251, RoleId=252885263003721, MenuId=252885263055380 },
            new SysRoleMenu{ Id=252885263003252, RoleId=252885263003721, MenuId=252885263055381 },
            new SysRoleMenu{ Id=252885263003253, RoleId=252885263003721, MenuId=252885263055382 },
            new SysRoleMenu{ Id=252885263003254, RoleId=252885263003721, MenuId=252885263055383 },
            new SysRoleMenu{ Id=252885263003255, RoleId=252885263003721, MenuId=252885263055384 },
            new SysRoleMenu{ Id=252885263003256, RoleId=252885263003721, MenuId=252885263055385 },
            // 文件管理
            new SysRoleMenu{ Id=252885263003261, RoleId=252885263003721, MenuId=252885263055390 },
            new SysRoleMenu{ Id=252885263003262, RoleId=252885263003721, MenuId=252885263055391 },
            new SysRoleMenu{ Id=252885263003263, RoleId=252885263003721, MenuId=252885263055392 },
            new SysRoleMenu{ Id=252885263003264, RoleId=252885263003721, MenuId=252885263055393 },
            new SysRoleMenu{ Id=252885263003265, RoleId=252885263003721, MenuId=252885263055394 },

            //// 日志管理
            //new SysRoleMenu{ Id=252885263003300, RoleId=252885263003721, MenuId=252885263055500 },
            new SysRoleMenu{ Id=252885263003301, RoleId=252885263003721, MenuId=252885263055510 },
            new SysRoleMenu{ Id=252885263003302, RoleId=252885263003721, MenuId=252885263055511 },
            new SysRoleMenu{ Id=252885263003311, RoleId=252885263003721, MenuId=252885263055520 },
            new SysRoleMenu{ Id=252885263003312, RoleId=252885263003721, MenuId=252885263055521 },
            new SysRoleMenu{ Id=252885263003321, RoleId=252885263003721, MenuId=252885263055530 },
            new SysRoleMenu{ Id=252885263003322, RoleId=252885263003721, MenuId=252885263055531 },
            new SysRoleMenu{ Id=252885263003331, RoleId=252885263003721, MenuId=252885263055540 },
            new SysRoleMenu{ Id=252885263003332, RoleId=252885263003721, MenuId=252885263055541 },

            // 帮助文档
            new SysRoleMenu{ Id=252885263003500, RoleId=252885263003721, MenuId=252885263055700 },
            new SysRoleMenu{ Id=252885263003501, RoleId=252885263003721, MenuId=252885263055710 },
            new SysRoleMenu{ Id=252885263003502, RoleId=252885263003721, MenuId=252885263055711 },

            // 其他角色默认菜单
            // 数据面板【252885263003722】
            new SysRoleMenu{ Id=252885263004000, RoleId=252885263003722, MenuId=252885263002100 },
            new SysRoleMenu{ Id=252885263004001, RoleId=252885263003722, MenuId=252885263002110 },
            new SysRoleMenu{ Id=252885263004002, RoleId=252885263003722, MenuId=252885263002111 },

            // 机构管理
            new SysRoleMenu{ Id=252885263004101, RoleId=252885263003722, MenuId=252885263055241 },
            // 个人中心
            new SysRoleMenu{ Id=252885263004151, RoleId=252885263003722, MenuId=252885263055260 },
            new SysRoleMenu{ Id=252885263004152, RoleId=252885263003722, MenuId=252885263055261 },
            new SysRoleMenu{ Id=252885263004153, RoleId=252885263003722, MenuId=252885263055262 },
            new SysRoleMenu{ Id=252885263004154, RoleId=252885263003722, MenuId=252885263055263 },

            // 数据面板【252885263003723】
            new SysRoleMenu{ Id=252885263005000, RoleId=252885263003723, MenuId=252885263002100 },
            new SysRoleMenu{ Id=252885263005001, RoleId=252885263003723, MenuId=252885263002110 },
            new SysRoleMenu{ Id=252885263005002, RoleId=252885263003723, MenuId=252885263002111 },
            // 机构管理
            new SysRoleMenu{ Id=252885263005101, RoleId=252885263003723, MenuId=252885263055241 },
            // 个人中心
            new SysRoleMenu{ Id=252885263005151, RoleId=252885263003723, MenuId=252885263055260 },
            new SysRoleMenu{ Id=252885263005152, RoleId=252885263003723, MenuId=252885263055261 },
            new SysRoleMenu{ Id=252885263005153, RoleId=252885263003723, MenuId=252885263055262 },
            new SysRoleMenu{ Id=252885263005154, RoleId=252885263003723, MenuId=252885263055263 },

            // 数据面板【252885263003724】
            new SysRoleMenu{ Id=252885263006000, RoleId=252885263003724, MenuId=252885263002100 },
            new SysRoleMenu{ Id=252885263006001, RoleId=252885263003724, MenuId=252885263002110 },
            new SysRoleMenu{ Id=252885263006002, RoleId=252885263003724, MenuId=252885263002111 },
            // 机构管理
            new SysRoleMenu{ Id=252885263006101, RoleId=252885263003724, MenuId=252885263055241 },
            // 个人中心
            new SysRoleMenu{ Id=252885263006151, RoleId=252885263003724, MenuId=252885263055260 },
            new SysRoleMenu{ Id=252885263006152, RoleId=252885263003724, MenuId=252885263055261 },
            new SysRoleMenu{ Id=252885263006153, RoleId=252885263003724, MenuId=252885263055262 },
            new SysRoleMenu{ Id=252885263006154, RoleId=252885263003724, MenuId=252885263055263 },

            // 数据面板【252885263003725】
            new SysRoleMenu{ Id=252885263007000, RoleId=252885263003725, MenuId=252885263002100 },
            new SysRoleMenu{ Id=252885263007001, RoleId=252885263003725, MenuId=252885263002110 },
            new SysRoleMenu{ Id=252885263007002, RoleId=252885263003725, MenuId=252885263002111 },
              // 机构管理
            new SysRoleMenu{ Id=252885263007101, RoleId=252885263003725, MenuId=252885263055241 },
            // 个人中心
            new SysRoleMenu{ Id=252885263007151, RoleId=252885263003725, MenuId=252885263055260 },
            new SysRoleMenu{ Id=252885263007152, RoleId=252885263003725, MenuId=252885263055261 },
            new SysRoleMenu{ Id=252885263007153, RoleId=252885263003725, MenuId=252885263055262 },
            new SysRoleMenu{ Id=252885263007154, RoleId=252885263003725, MenuId=252885263055263 },
        };
    }
}