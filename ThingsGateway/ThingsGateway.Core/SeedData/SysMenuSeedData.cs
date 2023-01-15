namespace ThingsGateway.Core;

/// <summary>
/// 系统菜单表种子数据
/// </summary>
public class SysMenuSeedData : ISqlSugarEntitySeedData<SysMenu>
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    [IgnoreUpdate]
    public IEnumerable<SysMenu> HasData()
    {
        return new[]
        {
            //new SysMenu{ Id=252885263002100, Pid=0, Title="工作台", Path="/dashboard", Name="dashboard", Component="Layout", Redirect="/dashboard/home", Icon="ele-HomeFilled", Type=MenuTypeEnum.Dir, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=0 },
            //new SysMenu{ Id=252885263002110, Pid=252885263002100, Title="工作台", Path="/dashboard/home", Name="home", Component="/home/index", IsAffix=true, Icon="ele-HomeFilled", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            //new SysMenu{ Id=252885263002111, Pid=252885263002100, Title="站内信", Path="/dashboard/notice", Name="notice", Component="/home/notice/index", Icon="ele-Bell", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=101 },

            new SysMenu{ Id=252885263055200, Pid=0, Title="系统管理", Path="/system", Name="system", Component="Layout", Redirect="/system/userCenter", Icon="ele-Setting", Type=MenuTypeEnum.Dir, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=1000 },

            new SysMenu{ Id=252885263055210, Pid=252885263055200, Title="账号管理", Path="/system/user", Name="sysUser", Component="/system/user/index", Icon="ele-User", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055211, Pid=252885263055210, Title="查询", Permission="sysUser:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055212, Pid=252885263055210, Title="编辑", Permission="sysUser:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055213, Pid=252885263055210, Title="增加", Permission="sysUser:add", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055214, Pid=252885263055210, Title="删除", Permission="sysUser:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055215, Pid=252885263055210, Title="详情", Permission="sysUser:detail", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055216, Pid=252885263055210, Title="授权角色", Permission="sysUser:grantRole", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055217, Pid=252885263055210, Title="重置密码", Permission="sysUser:resetPwd", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055218, Pid=252885263055210, Title="设置状态", Permission="sysUser:setStatus", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055219, Pid=252885263055210, Title="修改密码", Permission="sysUser:changePwd", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055220, Pid=252885263055210, Title="强制下线", Permission="sysUser:forceOffline", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            new SysMenu{ Id=252885263055230, Pid=252885263055200, Title="角色管理", Path="/system/role", Name="sysRole", Component="/system/role/index", Icon="ele-Help", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=110 },
            new SysMenu{ Id=252885263055231, Pid=252885263055230, Title="查询", Permission="sysRole:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055232, Pid=252885263055230, Title="编辑", Permission="sysRole:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055233, Pid=252885263055230, Title="增加", Permission="sysRole:add", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055234, Pid=252885263055230, Title="删除", Permission="sysRole:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055235, Pid=252885263055230, Title="授权菜单", Permission="sysRole:grantMenu", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055236, Pid=252885263055230, Title="授权数据", Permission="sysRole:grantData", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055237, Pid=252885263055230, Title="设置状态", Permission="sysRole:setStatus", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            new SysMenu{ Id=252885263055240, Pid=252885263055200, Title="机构管理", Path="/system/org", Name="sysOrg", Component="/system/org/index", Icon="ele-OfficeBuilding", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=120 },
            new SysMenu{ Id=252885263055241, Pid=252885263055240, Title="查询", Permission="sysOrg:list", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055242, Pid=252885263055240, Title="编辑", Permission="sysOrg:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055243, Pid=252885263055240, Title="增加", Permission="sysOrg:add", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055244, Pid=252885263055240, Title="删除", Permission="sysOrg:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            new SysMenu{ Id=252885263055250, Pid=252885263055200, Title="职位管理", Path="/system/pos", Name="sysPos", Component="/system/pos/index",Icon="ele-Mug", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=130 },
            new SysMenu{ Id=252885263055251, Pid=252885263055250, Title="查询", Permission="sysPos:list", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055252, Pid=252885263055250, Title="编辑", Permission="sysPos:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055253, Pid=252885263055250, Title="增加", Permission="sysPos:add", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055254, Pid=252885263055250, Title="删除", Permission="sysPos:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            new SysMenu{ Id=252885263055260, Pid=252885263055200, Title="个人中心", Path="/system/userCenter", Name="sysUserCenter", Component="/system/user/component/userCenter",Icon="ele-Medal", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=140 },
            new SysMenu{ Id=252885263055261, Pid=252885263055260, Title="修改密码", Permission="sysUser:changePwd", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055262, Pid=252885263055260, Title="更新信息", Permission="sysUser:updateBase", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055263, Pid=252885263055260, Title="电子签名", Permission="sysUser:signature", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            new SysMenu{ Id=252885263055270, Pid=252885263055200, Title="通知公告", Path="/system/notice", Name="sysNotice", Component="/system/notice/index",Icon="ele-Bell", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=150 },
            new SysMenu{ Id=252885263055271, Pid=252885263055270, Title="查询", Permission="sysNotice:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055272, Pid=252885263055270, Title="编辑", Permission="sysNotice:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055273, Pid=252885263055270, Title="增加", Permission="sysNotice:add", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055274, Pid=252885263055270, Title="删除", Permission="sysNotice:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055275, Pid=252885263055270, Title="发布", Permission="sysNotice:public", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055276, Pid=252885263055270, Title="撤回", Permission="sysNotice:cancel", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            new SysMenu{ Id=252885263055280, Pid=252885263055200, Title="三方账号", Path="/system/weChatUser", Name="weChatUser", Component="/system/weChatUser/index",Icon="ele-ChatDotRound", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=160 },
            new SysMenu{ Id=252885263055281, Pid=252885263055280, Title="查询", Permission="weChatUser:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055282, Pid=252885263055280, Title="编辑", Permission="weChatUser:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055283, Pid=252885263055280, Title="删除", Permission="weChatUser:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            new SysMenu{ Id=252885263055300, Pid=0, Title="平台管理", Path="/platform", Name="platform", Component="Layout", Redirect="/platform/server", Icon="ele-Menu", Type=MenuTypeEnum.Dir, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=1100 },

            new SysMenu{ Id=252885263055310, Pid=252885263055300, Title="租户管理", Path="/platform/tenant", Name="sysTenant", Component="/system/tenant/index", Icon="ele-School", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055311, Pid=252885263055310, Title="查询", Permission="sysTenant:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055312, Pid=252885263055310, Title="编辑", Permission="sysTenant:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055313, Pid=252885263055310, Title="增加", Permission="sysTenant:add", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055314, Pid=252885263055310, Title="删除", Permission="sysTenant:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055315, Pid=252885263055310, Title="授权菜单", Permission="sysTenant:grantMenu", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055316, Pid=252885263055310, Title="重置密码", Permission="sysTenant:resetPwd", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055317, Pid=252885263055310, Title="生成库", Permission="sysTenant:createDb", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            new SysMenu{ Id=252885263055320, Pid=252885263055300, Title="菜单管理", Path="/platform/menu", Name="sysMenu", Component="/system/menu/index", Icon="ele-Menu", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=110 },
            new SysMenu{ Id=252885263055321, Pid=252885263055320, Title="查询", Permission="sysMenu:list", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055322, Pid=252885263055320, Title="编辑", Permission="sysMenu:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055323, Pid=252885263055320, Title="增加", Permission="sysMenu:add", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055324, Pid=252885263055320, Title="删除", Permission="sysMenu:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            new SysMenu{ Id=252885263055330, Pid=252885263055300, Title="参数配置", Path="/platform/config", Name="sysConfig", Component="/system/config/index", Icon="ele-DocumentCopy", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=120 },
            new SysMenu{ Id=252885263055331, Pid=252885263055330, Title="查询", Permission="sysConfig:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055332, Pid=252885263055330, Title="编辑", Permission="sysConfig:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055333, Pid=252885263055330, Title="增加", Permission="sysConfig:add", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055334, Pid=252885263055330, Title="删除", Permission="sysConfig:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            new SysMenu{ Id=252885263055340, Pid=252885263055300, Title="字典管理", Path="/platform/dict", Name="sysDict", Component="/system/dict/index", Icon="ele-Collection", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=130 },
            new SysMenu{ Id=252885263055341, Pid=252885263055340, Title="查询", Permission="sysDictType:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055342, Pid=252885263055340, Title="编辑", Permission="sysDictType:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055343, Pid=252885263055340, Title="增加", Permission="sysDictType:add", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055344, Pid=252885263055340, Title="删除", Permission="sysDictType:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            new SysMenu{ Id=252885263055350, Pid=252885263055300, Title="任务调度", Path="/platform/job", Name="sysJob", Component="/system/job/index", Icon="ele-AlarmClock", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=140 },
            new SysMenu{ Id=252885263055351, Pid=252885263055350, Title="查询", Permission="sysJob:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055352, Pid=252885263055350, Title="编辑", Permission="sysJob:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055353, Pid=252885263055350, Title="增加", Permission="sysJob:add", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055354, Pid=252885263055350, Title="删除", Permission="sysJob:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            new SysMenu{ Id=252885263055360, Pid=252885263055300, Title="系统监控", Path="/platform/server", Name="sysServer", Component="/system/server/index", Icon="ele-Monitor", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=150 },

            new SysMenu{ Id=252885263055370, Pid=252885263055300, Title="缓存管理", Path="/platform/cache", Name="sysCache", Component="/system/cache/index", Icon="ele-Loading", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=160 },
            new SysMenu{ Id=252885263055371, Pid=252885263055370, Title="查询", Permission="sysCache:keyList", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055372, Pid=252885263055370, Title="删除", Permission="sysCache:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            new SysMenu{ Id=252885263055380, Pid=252885263055300, Title="行政区域", Path="/platform/region", Name="sysRegion", Component="/system/region/index", Icon="ele-LocationInformation", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=170 },
            new SysMenu{ Id=252885263055381, Pid=252885263055380, Title="查询", Permission="sysRegion:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055382, Pid=252885263055380, Title="编辑", Permission="sysRegion:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055383, Pid=252885263055380, Title="增加", Permission="sysRegion:add", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055384, Pid=252885263055380, Title="删除", Permission="sysRegion:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055385, Pid=252885263055380, Title="同步", Permission="sysRegion:sync", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            new SysMenu{ Id=252885263055390, Pid=252885263055300, Title="文件管理", Path="/platform/file", Name="sysFile", Component="/system/file/index", Icon="ele-Document", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=180 },
            new SysMenu{ Id=252885263055391, Pid=252885263055390, Title="查询", Permission="sysFile:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055392, Pid=252885263055390, Title="上传", Permission="sysFile:upload", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055393, Pid=252885263055390, Title="下载", Permission="sysFile:download", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055394, Pid=252885263055390, Title="删除", Permission="sysFile:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            new SysMenu{ Id=252885263055500, Pid=0, Title="日志管理", Path="/log", Name="log", Component="Layout", Redirect="/log/gatewaylog", Icon="ele-DocumentCopy", Type=MenuTypeEnum.Dir, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=1200 },
            new SysMenu{ Id=252885263055510, Pid=252885263055500, Title="访问日志", Path="/log/vislog", Name="sysVislog", Component="/system/log/vislog/index", Icon="ele-Document", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055511, Pid=252885263055510, Title="查询", Permission="sysVislog:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055512, Pid=252885263055510, Title="清空", Permission="sysVislog:clear", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055520, Pid=252885263055500, Title="操作日志", Path="/log/oplog", Name="sysOplog", Component="/system/log/oplog/index", Icon="ele-Document", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=110 },
            new SysMenu{ Id=252885263055521, Pid=252885263055520, Title="查询", Permission="sysOplog:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055522, Pid=252885263055520, Title="清空", Permission="sysOplog:clear", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055523, Pid=252885263055520, Title="导出", Permission="sysOplog:export", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055530, Pid=252885263055500, Title="异常日志", Path="/log/exlog", Name="sysExlog", Component="/system/log/exlog/index", Icon="ele-Document", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=120 },
            new SysMenu{ Id=252885263055531, Pid=252885263055530, Title="查询", Permission="sysExlog:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055532, Pid=252885263055530, Title="清空", Permission="sysExlog:clear", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055533, Pid=252885263055530, Title="导出", Permission="sysExlog:export", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055540, Pid=252885263055500, Title="差异日志", Path="/log/difflog", Name="sysDifflog", Component="/system/log/difflog/index", Icon="ele-Document", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=130 },
            new SysMenu{ Id=252885263055541, Pid=252885263055540, Title="查询", Permission="sysDifflog:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=252885263055542, Pid=252885263055540, Title="清空", Permission="sysDifflog:clear", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

        };
    }
}