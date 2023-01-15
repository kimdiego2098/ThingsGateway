namespace ThingsGateway.Application.Core;

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
            new SysMenu{ Id=411311111111111, Pid=0, Title="采集配置", Path="/collect", Name="collect", Component="Layout", Redirect="/collect/driverPlugin", Icon="ele-Connection", Type=MenuTypeEnum.Dir, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=50 },

            new SysMenu{ Id=411311111311111, Pid=411311111111111, Title="驱动配置", Path="/collect/driverPlugin", Name="driverPlugin", Component="/collect/driverplugin/index", Icon="ele-SetUp", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=411311111321111, Pid=411311111311111, Title="查询", Permission="driverPlugin:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=411311111321112, Pid=411311111311111, Title="新增", Permission="driverPlugin:add", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=411311111321113, Pid=411311111311111, Title="编辑", Permission="driverPlugin:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=411311111321114, Pid=411311111311111, Title="删除", Permission="driverPlugin:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            new SysMenu{ Id=411311111411111, Pid=411311111111111, Title="设备配置", Path="/collect/device", Name="device", Component="/collect/device/index", Icon="ele-SetUp", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=411311111421111, Pid=411311111411111, Title="查询", Permission="device:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=411311111421112, Pid=411311111411111, Title="新增", Permission="device:add", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=411311111421113, Pid=411311111411111, Title="编辑", Permission="device:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=411311111421114, Pid=411311111411111, Title="删除", Permission="device:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=411311111421115, Pid=411311111411111, Title="启停", Permission="device:setStatus", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=411311111421116, Pid=411311111411111, Title="导出", Permission="device:export", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=411311111421117, Pid=411311111411111, Title="导入", Permission="device:import", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            new SysMenu{ Id=411311111511111, Pid=411311111111111, Title="变量配置", Path="/collect/variable", Name="variable", Component="/collect/variable/index", Icon="ele-SetUp", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=411311111521111, Pid=411311111511111, Title="查询", Permission="variable:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=411311111521112, Pid=411311111511111, Title="新增", Permission="variable:add", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=411311111521113, Pid=411311111511111, Title="编辑", Permission="variable:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=411311111521114, Pid=411311111511111, Title="删除", Permission="variable:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=411311111521115, Pid=411311111511111, Title="导出", Permission="variable:export", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=411311111521116, Pid=411311111511111, Title="导入", Permission="variable:import", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },



            new SysMenu{ Id=411211111111111, Pid=0, Title="采集状态", Path="/collectstatus", Name="collectstatus", Component="Layout", Redirect="/collectstatus/driverPluginStatus", Icon="ele-Operation", Type=MenuTypeEnum.Dir, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=50 },

            new SysMenu{ Id=411211111131111, Pid=411211111111111, Title="驱动状态", Path="/collectstatus/driverPluginStatus", Name="driverPluginStatus", Component="/collectstatus/driverpluginStatus/index", Icon="ele-Operation", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=411211111132111, Pid=411211111131111, Title="查询", Permission="driverPluginStatus:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            new SysMenu{ Id=411211111141111, Pid=411211111111111, Title="设备状态", Path="/collectstatus/deviceStatus", Name="deviceStatus", Component="/collectstatus/deviceStatus/index", Icon="ele-Operation", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=411211111142111, Pid=411211111141111, Title="查询", Permission="deviceStatus:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            new SysMenu{ Id=411211111151111, Pid=411211111111111, Title="变量状态", Path="/collectstatus/variableStatus", Name="variableStatus", Component="/collectstatus/variableStatus/index", Icon="ele-Operation", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=411211111152111, Pid=411211111151111, Title="查询", Permission="variableStatus:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },


            new SysMenu{ Id=411111111111113, Pid=0, Title="报警信息", Path="/alarm", Name="alarm", Component="Layout", Redirect="/alarm/realalarm", Icon="ele-Bell", Type=MenuTypeEnum.Dir, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=50 },

            new SysMenu{ Id=411111111131158, Pid=411111111111113, Title="实时报警", Path="/alarm/realalarm", Name="realalarm", Component="/alarm/realalarm/index", Icon="ele-Bell", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=411111111151158, Pid=411111111131158, Title="查询", Permission="realalarm:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            new SysMenu{ Id=411111111131258, Pid=411111111111113, Title="报警存储配置", Path="/alarm/alarmconfig", Name="alarmconfig", Component="/alarm/alarmconfig/index", Icon="ele-SetUp", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=411111111151258, Pid=411111111131258, Title="保存", Permission="alarmconfig:save", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            new SysMenu{ Id=411111111131358, Pid=411111111111113, Title="历史报警", Path="/alarm/hisalarm", Name="hisalarm", Component="/alarm/hisalarm/index", Icon="ele-Bell", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=411111111151358, Pid=411111111131358, Title="查询", Permission="hisalarm:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=411111111151360, Pid=411111111131358, Title="导出", Permission="hisalarm:export", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },



            new SysMenu{ Id=421111111111113, Pid=0, Title="历史数据", Path="/his", Name="his", Component="Layout", Redirect="/his/hisconfig", Icon="ele-Histogram", Type=MenuTypeEnum.Dir, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=50 },

            new SysMenu{ Id=421111111131258, Pid=421111111111113, Title="历史存储配置", Path="/his/hisconfig", Name="hisconfig", Component="/his/hisconfig/index", Icon="ele-SetUp", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=421111111151258, Pid=421111111131258, Title="保存", Permission="hisconfig:save", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            new SysMenu{ Id=421111111131358, Pid=421111111111113, Title="历史数据查询", Path="/his/hisvalue", Name="hisvalue", Component="/his/hisvalue/index", Icon="ele-Histogram", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=421111111151358, Pid=421111111131358, Title="查询", Permission="hisvalue:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=421111111151360, Pid=421111111131358, Title="导出", Permission="hisvalue:export", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },


            new SysMenu{ Id=440000000000000, Pid=0, Title="上传配置", Path="/upload", Name="upload", Component="Layout", Redirect="/upload/uploadPlugin", Icon="ele-Connection", Type=MenuTypeEnum.Dir, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=50 },

            new SysMenu{ Id=440000000000001, Pid=440000000000000, Title="插件配置", Path="/upload/uploadPlugin", Name="uploadPlugin", Component="/upload/uploadplugin/index", Icon="ele-SetUp", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=440000000000100, Pid=440000000000001, Title="查询", Permission="uploadPlugin:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=440000000000101, Pid=440000000000001, Title="新增", Permission="uploadPlugin:add", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=440000000000102, Pid=440000000000001, Title="编辑", Permission="uploadPlugin:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=440000000000103, Pid=440000000000001, Title="删除", Permission="uploadPlugin:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            new SysMenu{ Id=440000000002223, Pid=440000000000000, Title="上传设备配置", Path="/upload/uploaddevice", Name="uploaddevice", Component="/upload/uploaddevice/index", Icon="ele-SetUp", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=440000000003333, Pid=440000000002223, Title="查询", Permission="uploaddevice:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=440000000003334, Pid=440000000002223, Title="新增", Permission="uploaddevice:add", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=440000000003335, Pid=440000000002223, Title="编辑", Permission="uploaddevice:update", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=440000000003336, Pid=440000000002223, Title="删除", Permission="uploaddevice:delete", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=440000000003337, Pid=440000000002223, Title="启停", Permission="uploaddevice:setStatus", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=440000000003338, Pid=440000000002223, Title="导出", Permission="uploaddevice:export", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=440000000003339, Pid=440000000002223, Title="导入", Permission="uploaddevice:import", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },



            new SysMenu{ Id=441000000000000, Pid=0, Title="上传状态", Path="/uploadstatus", Name="uploadstatus", Component="Layout", Redirect="/uploadstatus/uploadPluginStatus", Icon="ele-Operation", Type=MenuTypeEnum.Dir, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=50 },

            new SysMenu{ Id=441000000000100, Pid=441000000000000, Title="插件状态", Path="/uploadstatus/uploadPluginStatus", Name="uploadPluginStatus", Component="/uploadstatus/uploadpluginStatus/index", Icon="ele-Operation", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=441000000000101, Pid=441000000000100, Title="查询", Permission="uploadPluginStatus:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },

            new SysMenu{ Id=442000000003356, Pid=441000000000000, Title="上传设备状态", Path="/uploadstatus/uploaddeviceStatus", Name="uploaddeviceStatus", Component="/uploadstatus/uploaddeviceStatus/index", Icon="ele-Operation", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=442000000003556, Pid=441000000000000, Title="查询", Permission="uploaddeviceStatus:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },


            new SysMenu{ Id=451000000000000, Pid=252885263055500, Title="网关日志", Path="/log/gatewaylog", Name="gatewaylog", Component="/system/log/gatewaylog/index", Icon="ele-Document", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=110 },
            new SysMenu{ Id=451500000000000, Pid=451000000000000, Title="查询", Permission="gatewaylog:page", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=451500000000001, Pid=451000000000000, Title="清空", Permission="gatewaylog:clear", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=451500000000002, Pid=451000000000000, Title="导出", Permission="gatewaylog:export", Type=MenuTypeEnum.Btn, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },


        };
    }
}