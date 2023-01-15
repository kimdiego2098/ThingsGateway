using DbType = SqlSugar.DbType;

namespace ThingsGateway.Core.Util;
/// <summary>
/// 代码生成帮助类
/// </summary>
public static class CodeGenUtil
{
    /// <summary>
    /// 转换大驼峰法命名
    /// </summary>
    /// <param name="columnName">字段名</param>
    /// <param name="dbColumnNames">EntityBase 实体属性名称</param>
    /// <returns></returns>
    public static string CamelColumnName(string columnName, string[] dbColumnNames)
    {
        if (columnName.Contains('_'))
        {
            var arrColName = columnName.Split('_');
            var sb = new StringBuilder();
            foreach (var col in arrColName)
            {
                sb.Append(col[..1].ToUpper() + col[1..].ToLower());
            }
            columnName = sb.ToString();
        }
        else
        {
            var propertyName = dbColumnNames.FirstOrDefault(c => c.ToLower() == columnName.ToLower());
            if (!string.IsNullOrEmpty(propertyName))
            {
                columnName = propertyName;
            }
            else
            {
                columnName = columnName[..1].ToUpper() + columnName[1..].ToLower();
            }
        }
        return columnName;
    }

    // 根据数据库类型来处理对应的数据字段类型
    public static string ConvertDataType(DbColumnInfo dbColumnInfo)
    {
        var dbType = App.GetOptions<DbConnectionOptions>().ConnectionConfigs[0].DbType;
        return dbType switch
        {
            DbType.Oracle => ConvertDataType_OracleSQL(string.IsNullOrEmpty(dbColumnInfo.OracleDataType) ? dbColumnInfo.DataType : dbColumnInfo.OracleDataType, dbColumnInfo.Length, dbColumnInfo.Scale),
            DbType.PostgreSQL => ConvertDataType_PostgreSQL(dbColumnInfo.DataType),
            _ => ConvertDataType_Default(dbColumnInfo.DataType),
        };
    }

    public static string ConvertDataType_OracleSQL(string dataType, int? length, int? scale)
    {
        switch (dataType.ToLower())
        {
            case "interval year to month":
                return "int";

            case "interval day to second":
                return "TimeSpan";

            case "smallint":
                return "Int16";

            case "int":
            case "integer":
                return "int";

            case "long":
                return "long";

            case "float":
                return "float";

            case "decimal":
                return "decimal";

            case "number":
                if (length != null)
                {
                    if (scale != null && scale > 0)
                    {
                        return "decimal";
                    }
                    else if ((scale != null && scale == 0) || scale == null)
                    {
                        if (length > 1 && length < 12)
                        {
                            return "int";
                        }
                        else if (length > 11)
                        {
                            return "long";
                        }
                    }
                    if (length == 1)
                    {
                        return "bool";
                    }
                }
                return "decimal";

            case "char":
            case "clob":
            case "nclob":
            case "nchar":
            case "nvarchar":
            case "varchar":
            case "nvarchar2":
            case "varchar2":
            case "rowid":
                return "string";

            case "timestamp":
            case "timestamp with time zone":
            case "timestamptz":
            case "timestamp without time zone":
            case "date":
            case "time":
            case "time with time zone":
            case "timetz":
            case "time without time zone":
                return "DateTime";

            case "bfile":
            case "blob":
            case "raw":
                return "byte[]";

            default:
                return "object";
        }
    }

    //PostgreSQL数据类型对应的字段类型
    public static string ConvertDataType_PostgreSQL(string dataType)
    {
        switch (dataType)
        {
            case "int2":
            case "smallint":
                return "Int16";

            case "int4":
            case "integer":
                return "int";

            case "int8":
            case "bigint":
                return "long";

            case "float4":
            case "real":
                return "float";

            case "float8":
            case "double precision":
                return "double";

            case "numeric":
            case "decimal":
            case "path":
            case "point":
            case "polygon":
            case "interval":
            case "lseg":
            case "macaddr":
            case "money":
                return "decimal";

            case "boolean":
            case "bool":
            case "box":
            case "bytea":
                return "bool";

            case "varchar":
            case "character varying":
            case "geometry":
            case "name":
            case "text":
            case "char":
            case "character":
            case "cidr":
            case "circle":
            case "tsquery":
            case "tsvector":
            case "txid_snapshot":
            case "xml":
            case "json":
                return "string";

            case "uuid":
                return "Guid";

            case "timestamp":
            case "timestamp with time zone":
            case "timestamptz":
            case "timestamp without time zone":
            case "date":
            case "time":
            case "time with time zone":
            case "timetz":
            case "time without time zone":
                return "DateTime";

            case "bit":
            case "bit varying":
                return "byte[]";

            case "varbit":
                return "byte";

            case "public.geometry":
            case "inet":
                return "object";

            default:
                return "object";
        }
    }

    public static string ConvertDataType_Default(string dataType)
    {
        return dataType switch
        {
            "text" or "varchar" or "char" or "nvarchar" or "nchar" or "timestamp" => "string",
            "int" => "int",
            "smallint" => "Int16",
            "tinyint" => "byte",
            "bigint" or "integer" => "long",
            "bit" => "bool",
            "money" or "smallmoney" or "numeric" or "decimal" => "decimal",
            "real" => "Single",
            "datetime" or "smalldatetime" => "DateTime",
            "float" => "double",
            "image" or "binary" or "varbinary" => "byte[]",
            "uniqueidentifier" => "Guid",
            _ => "object",
        };
    }

    /// <summary>
    /// 数据类型转显示类型
    /// </summary>
    /// <param name="dataType"></param>
    /// <returns></returns>
    public static string DataTypeToEff(string dataType)
    {
        if (string.IsNullOrEmpty(dataType)) return "";
        return dataType switch
        {
            "string" => "Input",
            "int" => "InputNumber",
            "long" => "Input",
            "float" => "Input",
            "double" => "Input",
            "decimal" => "Input",
            "bool" => "Switch",
            "Guid" => "Input",
            "DateTime" => "DatePicker",
            _ => "Input",
        };
    }

    // 是否通用字段
    public static bool IsCommonColumn(string columnName)
    {
        var columnList = new List<string>()
        {
            "CreateTime", "UpdateTime", "CreateUserId", "UpdateUserId", "IsDelete"
        };
        return columnList.Contains(columnName);
    }
}