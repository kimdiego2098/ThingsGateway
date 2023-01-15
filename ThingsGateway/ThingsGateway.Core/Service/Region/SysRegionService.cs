using AngleSharp;
using AngleSharp.Html.Dom;

namespace ThingsGateway.Core.Service;

/// <summary>
/// 系统行政区域服务
/// </summary>
[ApiDescriptionSettings(Order = 100)]
public class SysRegionService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysRegion> _sysRegionRep;

    public SysRegionService(SqlSugarRepository<SysRegion> sysRegionRep)
    {
        _sysRegionRep = sysRegionRep;
    }

    /// <summary>
    /// 获取行政区域列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<SqlSugarPagedList<SysRegion>> GetRegionPage([FromQuery] PageRegionInput input)
    {
        return await _sysRegionRep.AsQueryable()
            .WhereIF(input.Pid > 0, u => u.Pid == input.Pid || u.Id == input.Pid)
            .WhereIF(!string.IsNullOrWhiteSpace(input.Name), u => u.Name.Contains(input.Name))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Code), u => u.Code.Contains(input.Code))
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 获取行政区域列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<List<SysRegion>> GetRegionList([FromQuery] RegionInput input)
    {
        return await _sysRegionRep.GetListAsync(u => u.Pid == input.Id);
    }

    /// <summary>
    /// 增加行政区域
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<long> AddRegion(AddRegionInput input)
    {
        var isExist = await _sysRegionRep.IsAnyAsync(u => u.Name == input.Name && u.Code == input.Code);
        if (isExist)
            throw Oops.Oh(ErrorCodeEnum.R2002);

        var sysRegion = input.Adapt<SysRegion>();
        var newRegion = await _sysRegionRep.AsInsertable(sysRegion).ExecuteReturnEntityAsync();
        return newRegion.Id;
    }

    /// <summary>
    /// 更新行政区域
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task UpdateRegion(UpdateRegionInput input)
    {
        if (input.Pid != 0)
        {
            var pRegion = await _sysRegionRep.GetFirstAsync(u => u.Id == input.Pid);
            _ = pRegion ?? throw Oops.Oh(ErrorCodeEnum.D2000);
        }
        if (input.Id == input.Pid)
            throw Oops.Oh(ErrorCodeEnum.R2001);

        var sysRegion = await _sysRegionRep.GetFirstAsync(u => u.Id == input.Id);
        var isExist = await _sysRegionRep.IsAnyAsync(u => u.Name == input.Name && u.Code == input.Code && u.Id != sysRegion.Id);
        if (isExist)
            throw Oops.Oh(ErrorCodeEnum.R2002);

        //// 父Id不能为自己的子节点
        //var regionTreeList = await _sysRegionRep.AsQueryable().ToChildListAsync(u => u.Pid, input.Id, true);
        //var childIdList = regionTreeList.Select(u => u.Id).ToList();
        //if (childIdList.Contains(input.Pid))
        //    throw Oops.Oh(ErrorCodeEnum.R2001);

        await _sysRegionRep.AsUpdateable(input.Adapt<SysRegion>()).IgnoreColumns(true).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除行政区域
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task DeleteRegion(DeleteRegionInput input)
    {
        var regionTreeList = await _sysRegionRep.AsQueryable().ToChildListAsync(u => u.Pid, input.Id, true);
        var regionIdList = regionTreeList.Select(u => u.Id).ToList();
        await _sysRegionRep.DeleteAsync(u => regionIdList.Contains(u.Id));
    }

    /// <summary>
    /// 同步行政区域
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task SyncRegion()
    {
        await _sysRegionRep.DeleteAsync(u => u.Id > 0);

        // 国家统计局行政区域2021年
        var url = "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2021/index.html";
        var context = BrowsingContext.New(Configuration.Default.WithDefaultLoader());
        var dom = await context.OpenAsync(url);

        // 省级
        var itemList = dom.QuerySelectorAll("table.provincetable tr.provincetr td a");
        foreach (IHtmlAnchorElement item in itemList)
        {
            var region = await _sysRegionRep.InsertReturnEntityAsync(new SysRegion
            {
                Pid = 0,
                Name = item.TextContent,
                Remark = item.Href,
                Level = 1,
            });

            // 市级
            var dom1 = await context.OpenAsync(item.Href);
            var itemList1 = dom1.QuerySelectorAll("table.citytable tr.citytr td a");
            for (var i1 = 0; i1 < itemList1.Length; i1 += 2)
            {
                var item1 = (IHtmlAnchorElement)itemList1[i1 + 1];
                var region1 = await _sysRegionRep.InsertReturnEntityAsync(new SysRegion
                {
                    Pid = region.Id,
                    Name = item1.TextContent,
                    Code = itemList1[i1].TextContent,
                    Remark = item1.Href,
                    Level = 2,
                });

                // 区县级
                var dom2 = await context.OpenAsync(item1.Href);
                var itemList2 = dom2.QuerySelectorAll("table.countytable tr.countytr td a");
                for (var i2 = 0; i2 < itemList2.Length; i2 += 2)
                {
                    var item2 = (IHtmlAnchorElement)itemList2[i2 + 1];
                    var region2 = await _sysRegionRep.InsertReturnEntityAsync(new SysRegion
                    {
                        Pid = region1.Id,
                        Name = item2.TextContent,
                        Code = itemList2[i2].TextContent,
                        Remark = item2.Href,
                        Level = 3,
                    });

                    // 街道级
                    var dom3 = await context.OpenAsync(item2.Href);
                    var itemList3 = dom3.QuerySelectorAll("table.towntable tr.towntr td a");
                    for (var i3 = 0; i3 < itemList3.Length; i3 += 2)
                    {
                        var item3 = (IHtmlAnchorElement)itemList3[i3 + 1];
                        var region3 = await _sysRegionRep.InsertReturnEntityAsync(new SysRegion
                        {
                            Pid = region2.Id,
                            Name = item3.TextContent,
                            Code = itemList3[i3].TextContent,
                            Remark = item3.Href,
                            Level = 4,
                        });

                        // 村级
                        var dom4 = await context.OpenAsync(item3.Href);
                        var itemList4 = dom4.QuerySelectorAll("table.villagetable tr.villagetr td");
                        for (var i4 = 0; i4 < itemList4.Length; i4 += 3)
                        {
                            await _sysRegionRep.InsertAsync(new SysRegion
                            {
                                Pid = region3.Id,
                                Name = itemList4[i4 + 2].TextContent,
                                Code = itemList4[i4].TextContent,
                                CityCode = itemList4[i4 + 1].TextContent,
                                Level = 5,
                            });
                        }
                    }
                }
            }
        }
    }
}