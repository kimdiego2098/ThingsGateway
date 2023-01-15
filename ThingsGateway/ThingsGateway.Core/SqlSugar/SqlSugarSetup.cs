namespace ThingsGateway.Core;
public static class SqlSugarSetup
{
    /// <summary>
    /// Sqlsugar 上下文初始化
    /// </summary>
    /// <param name="services"></param>
    public static void AddSqlSugar(this IServiceCollection services)
    {
        var dbOptions = App.GetOptions<DbConnectionOptions>();
        dbOptions.ConnectionConfigs.ForEach(config =>
        {
            SetDbConfig(config);
        });

        SqlSugarScope sqlSugar = new(dbOptions.ConnectionConfigs.Adapt<List<ConnectionConfig>>(), db =>
        {
            dbOptions.ConnectionConfigs.ForEach(config =>
            {
                var dbProvider = db.GetConnectionScope(config.ConfigId);
                SetDbAop(dbProvider, config);
                SetDbDiffLog(dbProvider, config);
            });
        });

        // 初始化数据库表结构及种子数据
        dbOptions.ConnectionConfigs.ForEach(config =>
        {
            InitDatabase(sqlSugar, config);
        });

        services.AddSingleton<ISqlSugarClient>(sqlSugar); // 单例注册
        services.AddScoped(typeof(SqlSugarRepository<>)); // 仓储注册
        services.AddUnitOfWork<SqlSugarUnitOfWork>(); // 事务与工作单元注册
    }

    /// <summary>
    /// 配置连接属性
    /// </summary>
    /// <param name="config"></param>
    public static void SetDbConfig(DbConnectionConfig config)
    {
        var configureExternalServices = new ConfigureExternalServices
        {
            EntityNameService = (type, entity) => // 处理表
            {
                if (config.EnableUnderLine && !entity.DbTableName.Contains('_'))
                    entity.DbTableName = UtilMethods.ToUnderLine(entity.DbTableName); // 驼峰转下划线
            },
            EntityService = (type, column) => // 处理列
            {
                if (new NullabilityInfoContext().Create(type).WriteState is NullabilityState.Nullable)
                    column.IsNullable = true;
                if (config.EnableUnderLine && !column.IsIgnore && !column.DbColumnName.Contains('_'))
                    column.DbColumnName = UtilMethods.ToUnderLine(column.DbColumnName); // 驼峰转下划线
            },
            DataInfoCacheService = new SqlSugarCache(),
        };
        config.ConfigureExternalServices = configureExternalServices;
        config.InitKeyType = InitKeyType.Attribute;
        config.IsAutoCloseConnection = true;
        config.MoreSettings = new ConnMoreSettings
        {
            IsAutoRemoveDataCache = true
        };
    }

    /// <summary>
    /// 配置Aop
    /// </summary>
    public static void SetDbAop(SqlSugarScopeProvider db, DbConnectionConfig config)
    {
        // 设置超时时间
        db.Ado.CommandTimeOut = 30;
        if (config.EnableConsoleOut)
        {
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                if (sql.Length > 300)
                {
                    sql = sql.Substring(0, 300);
                }
                if (sql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                    Console.ForegroundColor = ConsoleColor.Green;
                if (sql.StartsWith("UPDATE", StringComparison.OrdinalIgnoreCase) || sql.StartsWith("INSERT", StringComparison.OrdinalIgnoreCase))
                    Console.ForegroundColor = ConsoleColor.White;
                if (sql.StartsWith("DELETE", StringComparison.OrdinalIgnoreCase))
                    Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("【" + DateTime.Now + "——执行SQL】\r\n" + UtilMethods.GetSqlString(config.DbType, sql, pars) + "\r\n");
                App.PrintToMiniProfiler("SqlSugar", "Info", sql + "\r\n" + db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
            };
        }
        // 打印SQL语句
        db.Aop.OnError = (ex) =>
        {
            if (ex.Parametres == null) return;
            Console.ForegroundColor = ConsoleColor.Red;
            var pars = db.Utilities.SerializeObject(((SugarParameter[])ex.Parametres).ToDictionary(it => it.ParameterName, it => it.Value));
            Console.WriteLine("【" + DateTime.Now + "——错误SQL】" + Environment.NewLine + UtilMethods.GetSqlString(config.DbType, ex.Sql, (SugarParameter[])ex.Parametres) + Environment.NewLine);
            App.PrintToMiniProfiler("SqlSugar", "Error", $"{ex.Message}{Environment.NewLine}{ex.Sql}{pars}{Environment.NewLine}");
        };

        // 数据审计
        db.Aop.DataExecuting = (oldValue, entityInfo) =>
        {
            if (entityInfo.OperationType == DataFilterType.InsertByObject)
            {
                // 主键(long类型)且没有值的---赋值雪花Id
                if (entityInfo.EntityColumnInfo.IsPrimarykey && entityInfo.EntityColumnInfo.PropertyInfo.PropertyType == typeof(long))
                {
                    var id = entityInfo.EntityColumnInfo.PropertyInfo.GetValue(entityInfo.EntityValue);
                    if (id == null || (long)id <= 0)
                        entityInfo.SetValue(Yitter.IdGenerator.YitIdHelper.NextId());
                }
                if (entityInfo.PropertyName == "CreateTime")
                    entityInfo.SetValue(DateTime.Now);
                try
                {
                    if (App.User != null)
                    {
                        if (entityInfo.PropertyName == "TenantId")
                        {
                            var tenantId = ((dynamic)entityInfo.EntityValue).TenantId;
                            if (tenantId == null || tenantId == 0)
                                entityInfo.SetValue(App.User.FindFirst(ClaimConst.TenantId)?.Value);
                        }
                        if (entityInfo.PropertyName == "CreateUserId")
                            entityInfo.SetValue(App.User.FindFirst(ClaimConst.UserId)?.Value);
                        if (entityInfo.PropertyName == "CreateOrgId")
                            entityInfo.SetValue(App.User.FindFirst(ClaimConst.OrgId)?.Value);
                    }
                }
                catch { }
            }
            if (entityInfo.OperationType == DataFilterType.UpdateByObject)
            {
                if (entityInfo.PropertyName == "UpdateTime")
                    entityInfo.SetValue(DateTime.Now);
                if (entityInfo.PropertyName == "UpdateUserId")
                    entityInfo.SetValue(App.User?.FindFirst(ClaimConst.UserId)?.Value);
            }
        };

        // 超管时排除各种过滤器
        if (App.User?.FindFirst(ClaimConst.AccountType)?.Value == ((int)AccountTypeEnum.SuperAdmin).ToString())
            return;

        // 配置实体假删除过滤器
        SetDeletedEntityFilter(db);
        // 配置租户过滤器
        SetTenantEntityFilter(db);
        // 配置用户机构范围过滤器
        SetOrgEntityFilter(db);
        // 配置自定义过滤器
        SetCustomEntityFilter(db);
    }

    /// <summary>
    /// 开启库表差异化日志
    /// </summary>
    /// <param name="db"></param>
    /// <param name="config"></param>
    private static void SetDbDiffLog(SqlSugarScopeProvider db, DbConnectionConfig config)
    {
        if (!config.EnableDiffLog) return;

        db.Aop.OnDiffLogEvent = async u =>
        {
            var logDiff = new SysLogDiff
            {
                // 操作后记录（字段描述、列名、值、表名、表描述）
                AfterData = JsonConvert.SerializeObject(u.AfterData),
                // 操作前记录（字段描述、列名、值、表名、表描述）
                BeforeData = JsonConvert.SerializeObject(u.BeforeData),
                // 传进来的对象
                BusinessData = JsonConvert.SerializeObject(u.BusinessData),
                // 枚举（insert、update、delete）
                DiffType = u.DiffType.ToString(),
                Sql = UtilMethods.GetSqlString(config.DbType, u.Sql, u.Parameters),
                Parameters = JsonConvert.SerializeObject(u.Parameters),
                Duration = u.Time == null ? 0 : (long)u.Time.Value.TotalMilliseconds
            };
            await db.Insertable(logDiff).ExecuteCommandAsync();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(DateTime.Now + $"\r\n*****差异日志开始*****\r\n{Environment.NewLine}{JsonConvert.SerializeObject(logDiff)}{Environment.NewLine}*****差异日志结束*****\r\n");
        };
    }

    /// <summary>
    /// 初始化数据库
    /// </summary>
    /// <param name="db"></param>
    /// <param name="config"></param>
    public static void InitDatabase(SqlSugarScope db, DbConnectionConfig config)
    {
        if (!config.EnableInitDb) return;

        SqlSugarScopeProvider dbProvider = db.GetConnectionScope(config.ConfigId);

        // 创建数据库
        if (config.DbType != SqlSugar.DbType.Oracle)
            dbProvider.DbMaintenance.CreateDatabase();

        // 获取所有实体表-初始化表结构
        var entityTypes = App.EffectiveTypes.Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass && u.IsDefined(typeof(SugarTable), false));
        if (!entityTypes.Any()) return;
        foreach (var entityType in entityTypes)
        {
            var tAtt = entityType.GetCustomAttribute<TenantAttribute>();
            if (tAtt != null && tAtt.configId.ToString() != config.ConfigId) continue;
            if (tAtt == null && config.ConfigId != SqlSugarConst.ConfigId) continue;

            var splitTable = entityType.GetCustomAttribute<SplitTableAttribute>();
            if (splitTable == null)
                dbProvider.CodeFirst.InitTables(entityType);
            else
                dbProvider.CodeFirst.SplitTables().InitTables(entityType);
        }

        // 获取所有种子配置-初始化数据
        var seedDataTypes = App.EffectiveTypes.Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass
            && u.GetInterfaces().Any(i => i.HasImplementedRawGeneric(typeof(ISqlSugarEntitySeedData<>))));
        if (!seedDataTypes.Any()) return;
        foreach (var seedType in seedDataTypes)
        {
            var instance = Activator.CreateInstance(seedType);

            var hasDataMethod = seedType.GetMethod("HasData");
            var seedData = ((IEnumerable)hasDataMethod?.Invoke(instance, null))?.Cast<object>();
            if (seedData == null || seedData.Count() == 0) continue;

            var entityType = seedType.GetInterfaces().First().GetGenericArguments().First();
            var tAtt = entityType.GetCustomAttribute<TenantAttribute>();
            if (tAtt != null && tAtt.configId.ToString() != config.ConfigId) continue;
            if (tAtt == null && config.ConfigId != SqlSugarConst.ConfigId) continue;

            var entityInfo = dbProvider.EntityMaintenance.GetEntityInfo(entityType);
            if (entityInfo.Columns.Any(u => u.IsPrimarykey))
            {
                // 按主键进行批量增加和更新
                var storage = dbProvider.CopyNew().StorageableByObject(seedData.ToList()).ToStorage();
                storage.AsInsertable.ExecuteCommand();
                var ignoreUpdate = hasDataMethod.GetCustomAttribute<IgnoreUpdateAttribute>();
                if (ignoreUpdate == null) storage.AsUpdateable.ExecuteCommand();
            }
            else
            {
                // 无主键则只进行插入
                if (!dbProvider.Queryable(entityInfo.DbTableName, entityInfo.DbTableName).Any())
                    dbProvider.InsertableByObject(seedData.ToList()).ExecuteCommand();
            }
        }
    }

    /// <summary>
    /// 初始化租户业务数据库
    /// </summary>
    /// <param name="itenant"></param>
    /// <param name="config"></param>
    public static void InitTenantDatabase(ITenant itenant, DbConnectionConfig config)
    {
        SetDbConfig(config);

        itenant.AddConnection(config);
        var db = itenant.GetConnectionScope(config.ConfigId);
        db.DbMaintenance.CreateDatabase();

        // 获取所有实体表-初始化租户业务表
        var entityTypes = App.EffectiveTypes.Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass
      && u.IsDefined(typeof(SugarTable), false) && !u.IsDefined(typeof(SystemTableAttribute), false));
        if (!entityTypes.Any()) return;
        foreach (var entityType in entityTypes)
        {
            var splitTable = entityType.GetCustomAttribute<SplitTableAttribute>();
            if (splitTable == null)
                db.CodeFirst.InitTables(entityType);
            else
                db.CodeFirst.SplitTables().InitTables(entityType);
        }
    }

    /// <summary>
    /// 配置实体假删除过滤器
    /// </summary>
    private static void SetDeletedEntityFilter(SqlSugarScopeProvider db)
    {
        // 配置实体假删除缓存
        var cacheKey = $"db:{db.CurrentConnectionConfig.ConfigId}:IsDelete";
        var tableFilterItemList = db.DataCache.Get<List<TableFilterItem<object>>>(cacheKey);
        if (tableFilterItemList == null)
        {
            var tableFilterItems = new List<TableFilterItem<object>>();
            // 获取基类实体数据表
            var entityTypes = App.EffectiveTypes.Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass
                && (u.BaseType == typeof(EntityBase) || u.BaseType == typeof(EntityTenant) || u.BaseType == typeof(EntityBaseData)));
            if (!entityTypes.Any())
            {
                db.DataCache.Add(cacheKey, tableFilterItems);
                return;
            }

            foreach (var entityType in entityTypes)
            {
                // 排除非当前数据库实体
                var tAtt = entityType.GetCustomAttribute<TenantAttribute>();
                if ((tAtt != null && (string)db.CurrentConnectionConfig.ConfigId != tAtt.configId.ToString()) ||
                    (tAtt == null && (string)db.CurrentConnectionConfig.ConfigId != SqlSugarConst.ConfigId))
                    continue;

                var lambda = DynamicExpressionParser.ParseLambda(new[] {
                    Expression.Parameter(entityType, "u") }, typeof(bool), $"{nameof(EntityBase.IsDelete)} == @0", false);
                var tableFilterItem = new TableFilterItem<object>(entityType, lambda);
                tableFilterItems.Add(tableFilterItem);
                db.QueryFilter.Add(tableFilterItem);
            }
            db.DataCache.Add(cacheKey, tableFilterItems);
        }
        else
        {
            tableFilterItemList.ForEach(u =>
            {
                db.QueryFilter.Add(u);
            });
        }
    }

    /// <summary>
    /// 配置租户过滤器
    /// </summary>
    private static void SetTenantEntityFilter(SqlSugarScopeProvider db)
    {
        var tenantId = App.User?.FindFirst(ClaimConst.TenantId)?.Value;
        if (string.IsNullOrWhiteSpace(tenantId)) return;

        // 配置租户缓存
        var cacheKey = $"db:{db.CurrentConnectionConfig.ConfigId}:TenantId:{tenantId}";
        var tableFilterItemList = db.DataCache.Get<List<TableFilterItem<object>>>(cacheKey);
        if (tableFilterItemList == null)
        {
            // 获取租户实体数据表
            var entityTypes = App.EffectiveTypes.Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass
                && (u.BaseType == typeof(EntityTenant) || u.BaseType == typeof(EntityTenantId)));
            if (!entityTypes.Any()) return;

            var tableFilterItems = new List<TableFilterItem<object>>();
            foreach (var entityType in entityTypes)
            {
                // 获取库隔离租户业务实体
                var systemTableAtt = entityType.GetCustomAttribute<SystemTableAttribute>();
                if (systemTableAtt != null)
                {
                    // 排除非当前数据库实体
                    var tenantAtt = entityType.GetCustomAttribute<TenantAttribute>();
                    if ((tenantAtt != null && (string)db.CurrentConnectionConfig.ConfigId != tenantAtt.configId.ToString()) ||
                        (tenantAtt == null && (string)db.CurrentConnectionConfig.ConfigId != SqlSugarConst.ConfigId))
                        continue;
                }

                var lambda = DynamicExpressionParser.ParseLambda(new[] {
                    Expression.Parameter(entityType, "u") }, typeof(bool), $"{nameof(EntityTenant.TenantId)} == @0", long.Parse(tenantId));
                var tableFilterItem = new TableFilterItem<object>(entityType, lambda);
                tableFilterItems.Add(tableFilterItem);
                db.QueryFilter.Add(tableFilterItem);
            }
            db.DataCache.Add(cacheKey, tableFilterItems);
        }
        else
        {
            tableFilterItemList.ForEach(u =>
            {
                db.QueryFilter.Add(u);
            });
        }
    }

    /// <summary>
    /// 配置用户机构范围过滤器
    /// </summary>
    private static void SetOrgEntityFilter(SqlSugarScopeProvider db)
    {
        var userId = App.User?.FindFirst(ClaimConst.UserId)?.Value;
        if (string.IsNullOrWhiteSpace(userId)) return;

        // 配置用户机构范围缓存
        var cacheKey = $"db:{db.CurrentConnectionConfig.ConfigId}:UserId:{userId}";
        var tableFilterItemList = db.DataCache.Get<List<TableFilterItem<object>>>(cacheKey);
        if (tableFilterItemList == null)
        {
            var tableFilterItems = new List<TableFilterItem<object>>();
            // 获取用户所属机构
            var orgIds = App.GetService<SysCacheService>().Get<List<long>>(CacheConst.KeyOrgIdList + userId);
            if (orgIds == null || orgIds.Count == 0)
            {
                db.DataCache.Add(cacheKey, tableFilterItems);
                return;
            }

            // 获取业务实体数据表
            var entityTypes = App.EffectiveTypes.Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass
                && u.BaseType == typeof(EntityBaseData));
            if (!entityTypes.Any())
            {
                db.DataCache.Add(cacheKey, tableFilterItems);
                return;
            }

            foreach (var entityType in entityTypes)
            {
                // 排除非当前数据库实体
                var tAtt = entityType.GetCustomAttribute<TenantAttribute>();
                if ((tAtt != null && (string)db.CurrentConnectionConfig.ConfigId != tAtt.configId.ToString()) ||
                    (tAtt == null && (string)db.CurrentConnectionConfig.ConfigId != SqlSugarConst.ConfigId))
                    continue;

                var lambda = DynamicExpressionParser.ParseLambda(new[] {
                     Expression.Parameter(entityType, "u") }, typeof(bool), $"@0.Contains(u.{nameof(EntityBaseData.CreateOrgId)}??{default(long)})", orgIds);
                var tableFilterItem = new TableFilterItem<object>(entityType, lambda);
                tableFilterItems.Add(tableFilterItem);
                db.QueryFilter.Add(tableFilterItem);
            }
            db.DataCache.Add(cacheKey, tableFilterItems);
        }
        else
        {
            tableFilterItemList.ForEach(u =>
            {
                db.QueryFilter.Add(u);
            });
        }
    }

    /// <summary>
    /// 配置自定义过滤器
    /// </summary>
    private static void SetCustomEntityFilter(SqlSugarScopeProvider db)
    {
        // 配置用户机构范围缓存
        var cacheKey = $"db:{db.CurrentConnectionConfig.ConfigId}:Custom";
        var tableFilterItemList = db.DataCache.Get<List<TableFilterItem<object>>>(cacheKey);
        if (tableFilterItemList == null)
        {
            var tableFilterItems = new List<TableFilterItem<object>>();
            // 获取自定义实体过滤器
            var entityFilterTypes = App.EffectiveTypes.Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass
                && u.GetInterfaces().Any(i => i.HasImplementedRawGeneric(typeof(IEntityFilter))));
            if (!entityFilterTypes.Any())
            {
                db.DataCache.Add(cacheKey, tableFilterItems);
                return;
            }

            foreach (var entityFilter in entityFilterTypes)
            {
                var instance = Activator.CreateInstance(entityFilter);
                var entityFilterMethod = entityFilter.GetMethod("AddEntityFilter");
                var entityFilters = ((IList)entityFilterMethod?.Invoke(instance, null))?.Cast<object>();
                if (entityFilters == null) continue;

                foreach (var u in entityFilters)
                {
                    var tableFilterItem = (TableFilterItem<object>)u;
                    var entityType = tableFilterItem.GetType().GetProperty("type", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(tableFilterItem, null) as Type;
                    // 排除非当前数据库实体
                    var tAtt = entityType.GetCustomAttribute<TenantAttribute>();
                    if ((tAtt != null && (string)db.CurrentConnectionConfig.ConfigId != tAtt.configId.ToString()) ||
                        (tAtt == null && (string)db.CurrentConnectionConfig.ConfigId != SqlSugarConst.ConfigId))
                        return;

                    tableFilterItems.Add(tableFilterItem);
                    db.QueryFilter.Add(tableFilterItem);
                }
            }
            db.DataCache.Add(cacheKey, tableFilterItems);
        }
        else
        {
            tableFilterItemList.ForEach(u =>
            {
                db.QueryFilter.Add(u);
            });
        }
    }
}