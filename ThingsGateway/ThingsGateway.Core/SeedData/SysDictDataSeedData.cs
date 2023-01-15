namespace ThingsGateway.Core;

/// <summary>
/// 系统字典值表种子数据
/// </summary>
public class SysDictDataSeedData : ISqlSugarEntitySeedData<SysDictData>
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    [IgnoreUpdate]
    public IEnumerable<SysDictData> HasData()
    {
        return new[]
        {
            new SysDictData{ Id=269037953100001, DictTypeId=269037954100001, Value="输入框", Code="Input", OrderNo=100, Remark="输入框", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=269037953100002, DictTypeId=269037954100001, Value="外键", Code="fk", OrderNo=100, Remark="外键", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=269037953100003, DictTypeId=269037954100001, Value="时间选择", Code="DatePicker", OrderNo=100, Remark="时间选择", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=269037953100004, DictTypeId=269037954100001, Value="选择器", Code="Select", OrderNo=100, Remark="选择器", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=269037953100005, DictTypeId=269037954100001, Value="数字输入框", Code="InputNumber", OrderNo=100, Remark="数字输入框", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=269037953100006, DictTypeId=269037954100001, Value="文本域", Code="InputTextArea", OrderNo=100, Remark="文本域", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=269037953100007, DictTypeId=269037954100001, Value="上传", Code="Upload", OrderNo=100, Remark="上传", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=269037953100008, DictTypeId=269037954100001, Value="树选择", Code="ApiTreeSelect", OrderNo=100, Remark="树选择", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=269037953100009, DictTypeId=269037954100001, Value="开关", Code="Switch", OrderNo=100, Remark="开关", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=269037953100010, DictTypeId=269037954100001, Value="常量选择器", Code="ConstSelector", OrderNo=100, Remark="常量选择器", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },

            new SysDictData{ Id=269037953110001, DictTypeId=269037954100002, Value="等于", Code="==", OrderNo=1, Remark="等于", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=269037953110002, DictTypeId=269037954100002, Value="模糊", Code="like", OrderNo=1, Remark="模糊", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=269037953110003, DictTypeId=269037954100002, Value="大于", Code=">", OrderNo=1, Remark="大于", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=269037953110004, DictTypeId=269037954100002, Value="小于", Code="<", OrderNo=1, Remark="小于", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=269037953110005, DictTypeId=269037954100002, Value="不等于", Code="!=", OrderNo=1, Remark="不等于", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=269037953110006, DictTypeId=269037954100002, Value="大于等于", Code=">=", OrderNo=1, Remark="大于等于", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=269037953110007, DictTypeId=269037954100002, Value="小于等于", Code="<=", OrderNo=1, Remark="小于等于", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=269037953110008, DictTypeId=269037954100002, Value="不为空", Code="isNotNull", OrderNo=1, Remark="不为空", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=269037953110009, DictTypeId=269037954100002, Value="时间范围", Code="~", OrderNo=1, Remark="时间范围", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },

            new SysDictData{ Id=269037953120001, DictTypeId=269037954100003, Value="long", Code="long", OrderNo=1, Remark="long", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=269037953120002, DictTypeId=269037954100003, Value="string", Code="string", OrderNo=1, Remark="string", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=269037953120003, DictTypeId=269037954100003, Value="DateTime", Code="DateTime", OrderNo=1, Remark="DateTime", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=269037953120004, DictTypeId=269037954100003, Value="bool", Code="bool", OrderNo=1, Remark="bool", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=269037953120005, DictTypeId=269037954100003, Value="int", Code="int", OrderNo=1, Remark="int", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=269037953120006, DictTypeId=269037954100003, Value="double", Code="double", OrderNo=1, Remark="double", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=269037953120007, DictTypeId=269037954100003, Value="float", Code="float", OrderNo=1, Remark="float", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=269037953120008, DictTypeId=269037954100003, Value="decimal", Code="decimal", OrderNo=1, Remark="decimal", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=269037953120009, DictTypeId=269037954100003, Value="Guid", Code="Guid", OrderNo=1, Remark="Guid", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=269037953120010, DictTypeId=269037954100003, Value="DateTimeOffset", Code="DateTimeOffset", OrderNo=1, Remark="DateTimeOffset", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },

            new SysDictData{ Id=269037953130001, DictTypeId=269037954100004, Value="下载压缩包", Code="1", OrderNo=1, Remark="下载压缩包", Status=StatusEnum.Disable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=269037953130002, DictTypeId=269037954100004, Value="生成到本项目", Code="2", OrderNo=1, Remark="生成到本项目", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },

            new SysDictData{ Id=269037953140001, DictTypeId=269037954100005, Value="EntityBaseId【基础实体Id】", Code="EntityBaseId", OrderNo=1, Remark="【基础实体Id】", Status=StatusEnum.Disable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=269037953140002, DictTypeId=269037954100005, Value="EntityBase【基础实体】", Code="EntityBase", OrderNo=1, Remark="【基础实体】", Status=StatusEnum.Disable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=269037953140003, DictTypeId=269037954100005, Value="EntityTenantId【租户实体Id】", Code="EntityTenantId", OrderNo=1, Remark="【租户实体Id】", Status=StatusEnum.Disable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=269037953140004, DictTypeId=269037954100005, Value="EntityTenant【租户实体】", Code="EntityTenant", OrderNo=1, Remark="【租户实体】", Status=StatusEnum.Disable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictData{ Id=269037953140005, DictTypeId=269037954100005, Value="EntityBaseData【业务实体】", Code="EntityBaseData", OrderNo=1, Remark="【业务实体】", Status=StatusEnum.Disable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
        };
    }
}